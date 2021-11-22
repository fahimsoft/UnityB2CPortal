using API_Base.Base;
using API_Base.Common;
using B2C_Models.Models;
using B2CPortal.Interfaces;
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
        public AccountController(IAccount account, IProductMaster productMaster, ICart cart)
        {
            _account = account;
            _IProductMaster = productMaster;
            _cart = cart;
        }

        #endregion


 
        [HttpGet]
        public  ActionResult Login()
        {
            try
            {
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
        public async Task<JsonResult> Login(customer customer)
        {
            try
            {
                var res = await _account.SelectByIdPassword(customer);
                if ( res != null)
                {
                    //string Uri = Request.Url.AbsoluteUri;
                    //string url = HttpContext.Current.Request.Url.AbsoluteUri;
                    string cookie = string.Empty;
                    Session["UserAccount"] = res;
                    Session["UserId"] = res.Id;
                    Session["UserName"] = res.FirstName;
                    if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
                    {
                        cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                        List<Cart> cartlsit = await _cart.GetCartProducts(cookie, res.Id) as List<Cart>;
                        List<Cart> wishlist = await _cart.GetWishListProducts(cookie, res.Id) as List<Cart>;
                        cartlsit.ForEach(x =>
                        {
                            if (x.FK_Customer == null || string.IsNullOrEmpty(x.Guid))
                            {
                                x.FK_Customer = res.Id;
                                x.Guid = cookie;
                                _cart.UpdateCart(x);
                            }
                        });
                        wishlist.ForEach(x =>
                        {
                            if (x.FK_Customer == null || string.IsNullOrEmpty(x.Guid))
                            {
                                x.FK_Customer = res.Id;
                                x.Guid = cookie;
                                _cart.UpdateToCart(x);
                            }
                        });
                       var duplicatelist = cartlsit.GroupBy(x => x).Where(y => y.Count() > 1).Select(z => z).ToList();

                    }

                    return SuccessResponse("true");

                    
                    //return Json(new { data = res, msg = "Login Successfull", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return BadResponse("false");
                    //return Json(new { data = "", msg = "Login Failed", success = false, statuscode = 400, count = 0 }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return BadResponse(ex);
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
                    var User =  await _account.SelectById(obj);
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
            catch(Exception ex)
            {
                return BadResponse(ex);
            }
            
           
        }
        // Add Update Customer
        public async Task<JsonResult> AddUpdate(customer customer)
        {
            try
            {
                var res = await _account.CreateCustomer(customer);
                return SuccessResponse("true");

            }
            catch (Exception ex)
            {
                return BadResponse(ex);
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
                if(res == null)
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
    }
}