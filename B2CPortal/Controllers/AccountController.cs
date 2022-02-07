using API_Base.Base;
using API_Base.Common;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace B2CPortal.Controllers
{
    public class AccountController : BaseController
    {
        #region Interface instance
        private readonly IAccount _account = null;
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly ICity _ICity = null;

        public AccountController(IAccount account, IProductMaster productMaster, ICart cart, ICity city)
        {
            _account = account;
            _IProductMaster = productMaster;
            _cart = cart;
            _ICity = city;
        }
        #endregion
        [HttpGet]
        public async Task<ActionResult> Login(string CODE = null, string email = null)
        {
            try
            {
                if (CODE != null && email != null)
                {
                    var obj = await _account.verification(email);
                }
                if (Session["UserId"] != null)
                {
                    return RedirectToAction("Index", "Home");
                }
                customer customer = new customer();
                return View(customer);
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        // Login json response
        [HttpPost]
        public async Task<ActionResult> Login(customer customer)
        {
            try
            {
                string Current = "/Home/Index";
                var res = await _account.SelectByIdPassword(customer);
                if (res != null)
                {
                    //get location from cookie
                    string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                    if (string.IsNullOrEmpty(cookiecity))
                    {
                        HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                    }
                    cookiecity =  string.IsNullOrEmpty(cookiecity)? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                    City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                    var genral = new GenralClass();
                    string cookie = string.Empty;
                    Session["UserAccount"] = res;
                    Session["UserId"] = res.Id;
                    Session["UserName"] = res.FirstName;
                    Session["email"] = res.EmailId;
                    if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
                    {
                        cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                        List<Cart> cartlsit = await _cart.GetCartProducts(cookie, res.Id, citymodel) as List<Cart>;
                        List<Cart> wishlist = await _cart.GetWishListProducts(cookie, res.Id) as List<Cart>;
                        cartlsit.ForEach(x =>
                        {
                            if (x.FK_Customer == null || string.IsNullOrEmpty(x.Guid))
                            {
                                x.FK_Customer = res.Id;
                                x.Guid = cookie;
                                x.FK_CityId = citymodel.Id;
                                _cart.UpdateCart(x);
                            }
                        });
                        wishlist.ForEach(x =>
                        {
                            if (x.FK_Customer == null || string.IsNullOrEmpty(x.Guid))
                            {
                                x.FK_Customer = res.Id;
                                x.Guid = cookie;
                                x.FK_CityId = citymodel.Id;
                                _cart.UpdateToCart(x);
                            }
                        });
                        var duplicatelist = cartlsit.GroupBy(x => x).Where(y => y.Count() > 1).Select(z => z).ToList();



                    }
                    if (TempData["returnurl"] != null)
                    {
                        Current = TempData["returnurl"].ToString();
                    }
                    return Json(new { data = Current, msg = "Login Successfull", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "", msg = "Incorrect Email and Password ", success = false, statuscode = 400, count = 0 }, JsonRequestBehavior.AllowGet);
                }



            }
            catch (Exception ex)
            {
                return Json(new { data = "", msg = ex.Message, success = false, statuscode = 400, count = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> MyAccount()
        {
            try
            {
                customer customer = new customer();
                var obj = 0;
                if (Session["UserId"] != null)
                {
                    obj = Convert.ToInt32(HttpContext.Session["UserId"]);
                }
                if (obj > 0)
                {
                    var User = await _account.SelectById(obj);
                    customer.Id = User.Id; // Result.id;
                    customer.FirstName = User.FirstName;
                    customer.LastName = User.LastName;
                    customer.PhoneNo = User.PhoneNo;
                    customer.Gender = User.Gender;
                    customer.DateOfBirth = User.DateOfBirth;
                    customer.EmailId = User.EmailId;
                    customer.Password = User.Password;
                    customer.Address = User.Address;
                    customer.Country = User.Country;
                    customer.City = User.City;

                }
                return View("MyAccount", customer);
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }


        }
        // Add Update Customer
        public async Task<JsonResult> AddUpdate(customer customer)
        {
            try
            {
                var obj = 0;
                if (Session["UserId"] != null)
                {
                    obj = Convert.ToInt32(HttpContext.Session["UserId"]);
                }
                if (obj > 0)
                {
                    try
                    {
                        string cookie = string.Empty;
                        if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
                        {
                            cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                        }
                        else
                        {
                            cookie = Guid.NewGuid().ToString();
                            HelperFunctions.SetCookie(HelperFunctions.cartguid, cookie, 1);
                        }
                        customer.Guid = cookie;
                        var res = await _account.CreateCustomer(customer);
                        return Json(new { data = res, msg = "Updated!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { data = ex, msg = "Failed!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var result = HelperFunctions.fBrowserIsMobile();
                    if (result == false)
                    {
                        customer.RegisteredFrom = "Web";
                        customer.IsWebUser = true;
                        customer.IsAppUser = false;
                    }
                    else
                    {
                        customer.RegisteredFrom = "App";
                        customer.IsAppUser = true;
                        customer.IsWebUser = false;
                    }
                    string cookie = string.Empty;
                    if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
                    {
                        cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    }
                    else
                    {
                        cookie = Guid.NewGuid().ToString();
                        HelperFunctions.SetCookie(HelperFunctions.cartguid, cookie, 1);
                    }
                    try
                    {
                        customer.Guid = cookie;
                        var res = await _account.CreateCustomer(customer);
                        var name = customer.FirstName;
                        string To = res.EmailId;
                        var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                        var random = new Random();
                        var resultToken = new string(
                        Enumerable.Repeat(allChar, 8)
                        .Select(token => token[random.Next(token.Length)]).ToArray());





                        string authToken = resultToken.ToString();



                        //Create URL with above token
                        var lnkHref = " <a href='" + Url.Action("Login", "Account", new { email = To, code = authToken }, "http") + "'>Account Verification</a>";



                        //HTML Template for Send email
                        string subject = "Account Verification";
                        string body = "<b>Please find the Account Verification Link. </b><br/>" + lnkHref;



                        //Call send email methods.
                        bool IsSendEmail = HelperFunctions.EmailSend(To, subject, body, true);
                        // return Json(new { data = IsSendEmail, msg = "Check Your Inbox!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);




                        //bool IsSendEmail = HelperFunctions.EmailSend(customer.EmailId, "Confirm your account", htmlString, true);
                        if (IsSendEmail)
                        {
                            return Json(new { data = "", msg = "Check Your Inbox!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            return Json(new { data = "", msg = "Registeration Failed", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
                            //return BadResponse("Failed");
                        }
                    }
                    catch
                    {

                    }   
                    return Json(new { data = "", msg = "Registeration Successful!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                    //if (IsSendEmail)
                    //{
                    //}
                    //else
                    //{
                    //    return Json(new { data = "", msg = "Registeration Failed", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
                    //}
                }

            }
            catch (Exception ex)
            {
                return Json(new { data = "", msg = ex.Message, success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> AddUpdateBilling(customer customer)
        {
            try
            {
                var res = await _account.CreateCustomerBilling(customer);
                return SuccessResponse("true");

            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }

        public async Task<JsonResult> uniqueEmailCheck(string email)
        {
            try
            {
                var res = await _account.uniqueEmailCheck(email);
                if (res == null)
                {
                    return Json(new { data = "", msg = "Email Available", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { data = "", msg = "Email already exist", success = false, statuscode = 400, count = 0 }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("index", "Home");
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            customer customer = new customer();
            return View(customer);
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string EmailId)
        {
            if (ModelState.IsValid)
            {
                var res = await _account.uniqueEmailCheck(EmailId);
                if (res != null)
                {
                    string To = EmailId;
                    var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var random = new Random();
                    var resultToken = new string(
                    Enumerable.Repeat(allChar, 8)
                    .Select(token => token[random.Next(token.Length)]).ToArray());



                    string authToken = resultToken.ToString();

                    //Create URL with above token
                    var lnkHref = " <a href='" + Url.Action("ResetPassword", "Account", new { email = EmailId, code = authToken }, "http") + "'>Reset Password</a>";
                    //var lnkHref = "<a href='"+='" + EmailId + @"'&code='" + authToken + @" >Reset Password </a>";



                    //HTML Template for Send email
                    string subject = "Your changed password";
                    string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;



                    //Call send email methods.
                    bool IsSendEmail = HelperFunctions.EmailSend(EmailId, subject, body, true);
                    return Json(new { data = IsSendEmail, msg = "Check Your Inbox!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    return Json(new { data = "", msg = "You are Not Registered!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);



                }
            }
            else
            {
                return Json(new { data = "", msg = "You are Not Registered!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);



            }



        }
        public ActionResult ResetPassword(string code, string email)
        {
            customer model = new customer();
            var ReturnToken = code;
            model.EmailId = email;
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword(customer customer)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resetResponse = await _account.ResetPassword(customer);
                    if (resetResponse != null)
                    {
                        return Json(new { data = resetResponse, msg = "Password Has Been Reset!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);



                    }
                    else
                    {
                        return Json(new { data = resetResponse, msg = "Password Reset Failed!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);



                    }



                }
                else
                {
                    return Json(new { data = "", msg = "Password Reset Failed!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);



                }
            }
            catch (Exception ex)
            {
                return Json(new { data = ex, msg = "Password Reset Failed!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);



            }



        }
        // Recaptcha
        [HttpPost]
        public JsonResult Recaptcha(string res)
        {
            // CaptchaResponse response = HelperFunctions.ValidateCaptcha(Request["g-recaptcha-response"]);
            try
            {
                CaptchaResponse response = HelperFunctions.ValidateCaptcha(res);
                if (response.Success)
                {
                    return Json(new { data = response, msg = "", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = response, msg = "Fill out Recaptcha!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(new { data = "", msg = ex.Message, success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
            }
        }
        /// Is Verified check
        [HttpPost]
        public async Task<ActionResult> IsVerified(string EmailId)
        {
            if (ModelState.IsValid)
            {
                var res = await _account.uniqueEmailCheck(EmailId);
                if (res != null)
                {
                    string To = EmailId;
                    var allChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                    var random = new Random();
                    var resultToken = new string(
                    Enumerable.Repeat(allChar, 8)
                    .Select(token => token[random.Next(token.Length)]).ToArray());

                    string authToken = resultToken.ToString();

                    //Create URL with above token
                    var lnkHref = " <a href='" + Url.Action("Login", "Account", new { email = EmailId, code = authToken }, "http") + "'>Account Verification</a>";

                    //HTML Template for Send email
                    string subject = "Account Verification";
                    string body = "<b>Please find the Account Verification Link. </b><br/>" + lnkHref;

                    //Call send email methods.
                    bool IsSendEmail = HelperFunctions.EmailSend(EmailId, subject, body, true);
                    return Json(new { data = IsSendEmail, msg = "Check Your Inbox!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "", msg = "You are Not Registered!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(new { data = "", msg = "You are Not Registered!", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult ThankYou()
        {
            try
            {
                return PartialView();
            }
            catch (Exception ex)
            {



                return BadResponse(ex);
            }
        }
    }
}