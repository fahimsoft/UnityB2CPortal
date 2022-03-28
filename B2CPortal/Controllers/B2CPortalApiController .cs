using API_Base.Base;
using API_Base.Common;
using API_Base.PaymentMethod;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class B2CPortalApiController : BaseController
    {
        private readonly IOrders _orders = null;
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly IOrderTransection _orderTransection = null;
        private readonly IOrderDetail _ordersDetail = null;
        private readonly IAccount _account = null;
        private readonly PaymentMethodFacade _PaymentMethodFacade = null;
        private readonly ICity _ICity = null;

        public B2CPortalApiController(IOrders order, 
            IProductMaster productMaster, 
            ICart cart, 
            IOrderTransection orderTransection, 
            IOrderDetail orderDetail, 
            IAccount account, ICity city)
        {
            _orders = order;
            _IProductMaster = productMaster;
            _cart = cart;
            _orderTransection = orderTransection;
            _PaymentMethodFacade = new PaymentMethodFacade();
            _ordersDetail = orderDetail;
            _account = account;
            _ICity = city;


        }
        // GET: Country
        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var obj = await _IProductMaster.GetProduct();
            try
            {
                List<ProductsVM> productsVM = new List<ProductsVM>();
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                var totalProduct = obj.Count();
                var productmasetr = obj.OrderByDescending(x => x.CreatedOn).ToList();

                foreach (var item in productmasetr)
                {
                    var productRating = _IProductMaster.GetProductRating(item.Id.ToString());// _dxcontext.Database.SqlQuery<ProductsVM>("exec GetProductRating " + Id + "").ToList<ProductsVM>();
                    var discount = item.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    var tax = item.ProductPrices.Select(x => x.Tax).FirstOrDefault();

                    //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                    decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Discount = item.ProductPrices.Select(x => x.Discount).FirstOrDefault(),
                        ImageUrl = item.ProductDetails.Select(x => x.ImageUrl).FirstOrDefault(),
                        MasterImageUrl = item.MasterImageUrl,
                        ShortDescription = item.ShortDescription,
                        LongDescription = item.LongDescription,
                        totalProduct = totalProduct,
                        Price = Math.Round(Convert.ToDecimal(price) / conversionvalue, 2),
                        DiscountedAmount = DiscountedPrice,
                        UOM = item.ProductPackSize?.UOM,
                        TotalRating = productRating.Select(x => x.TotalRating).FirstOrDefault(),
                        AvgRating = productRating.Select(x => x.AvgRating).FirstOrDefault(),
                        ImageUrl2 = item.ProductDetails.Select(x => x.ImageUrl).ToList(),

                    };
                    productsVM.Add(producVMList);
                }
                //return SuccessResponse(productsVM);
                return Json(new { data = productsVM }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetProductById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var obj = await _IProductMaster.GetProductByIdWithRating(Convert.ToInt64(id));
                return Json(new { success = true, data = obj }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, msg = "Try Again" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> StripePayment(PaymentVMRequest payment)
        {
            try
            {
                ViewBag.error = "";
                //string OrderTotalAmount = HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount);
                //string ordermasterId = HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId);

                if (string.IsNullOrEmpty(payment.Amount) || string.IsNullOrEmpty(payment.OrderId) || payment.UserId == null)
                {
                    return Json(new { success = false, msg = "please enter all data correctly !" }, JsonRequestBehavior.AllowGet);
                }
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                var paymentmodel = new PaymentVM
                {
                    Name = payment.Name,
                    Email = payment.Email,
                    Phone = payment.Phone,
                    Description = string.IsNullOrEmpty(payment.Description) ? "this payment from stripe" : payment.Description,
                    StripeToken = payment.stripeToken,
                    Amount = Convert.ToDecimal(payment.Amount) * 100 < 50 ? 100 : Convert.ToDecimal(payment.Amount) * 100,
                    StripeAmount = Convert.ToDecimal(payment.Amount),
                };
                dynamic result = _PaymentMethodFacade.CreateStripePayment(paymentmodel);
                //ResponseViewModel resmodel  =  HelperFunctions.ResponseHandler(result);
                Charge chargeobj = (Charge)result;
                if (result != null && chargeobj.Paid == true)
                {
                    var ordervm = new OrderVM
                    {
                        Id = Convert.ToInt32(payment.OrderId),
                        Currency = currency,
                        ConversionRate = conversionvalue,
                        PaymentMode = PaymentType.Stripe.ToString(),
                        Status = OrderStatus.Confirmed.ToString(),
                        TotalPrice = Convert.ToDecimal(payment.Amount),
                        PaymentStatus = true,
                    };
                    var ordermodel = await _orders.UpdateOrderMAster(ordervm);
                    ordervm = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, ordervm);
                    ordervm.OrderNo = HelperFunctions.GenrateOrderNumber(ordervm.Id.ToString());
                    ordervm.DiscountAmount = ordervm.OrderDetails.Sum(x => x.DiscountedPrice);
                    ordervm.SubTotalPrice = ordervm.OrderDetails.Sum(x => x.Price);
                    string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                    // ------------Remove from cart------------
                    //var customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    //var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    var removeCart = await _cart.DisableCart(Convert.ToInt32(payment.UserId), "");
                    //-------------------add order transection record--------------
                    OrderTransection mod3el = new OrderTransection();
                    mod3el.FullName = payment.Name;
                    mod3el.EmailId = payment.Email;
                    mod3el.PhoneNo = payment.Phone;
                    mod3el.StripePaymentID = chargeobj.Id;
                    mod3el.IsActive = true;
                    mod3el.Status = chargeobj.Status.ToLower();
                    mod3el.FK_Customer = Convert.ToInt32(userid);
                    mod3el.FK_OrderMAster = Convert.ToInt32(HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId));
                    var dd = await _orderTransection.CreateOrderTransection(mod3el);
                    return Json(new { success = true, data = payment, msg = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["error"] = chargeobj.FailureMessage;
                    return Json(new { success = false, msg = chargeobj.FailureMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (StripeException ex)
            {
                TempData["error"] = ex.Message;
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        //--------------------------------------------order--------------------------------------
       [HttpGet]
        public async Task<ActionResult> GetOrderListData(string customerid)
        {
            try
            {
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                List<OrderVM> list = new List<OrderVM>();
                if (Convert.ToInt32(customerid) > 0)
                {
                    var orderlist = await _orders.GetOrderList(Convert.ToInt32(customerid));
                    foreach (var item in orderlist)
                    {
                        OrderVM dd = (OrderVM)HelperFunctions.CopyPropertiesTo(item, new OrderVM());
                        var result = HelperFunctions.GenrateOrderNumber(dd.Id.ToString());
                        dd.OrderNo = result;
                        list.Add((OrderVM)dd);
                    }
                    return Json(new { success = true, data = list, msg = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, data = list, msg = "Please Re-Login for Order History" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false,  msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
       [HttpGet]
        public async Task<JsonResult> GetOrderDetailsByIdData(string orderid)
        {
            if (!string.IsNullOrEmpty(orderid))
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                List<OrderVM> orderdetailslist = new List<OrderVM>();
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                var detailslist = await _ordersDetail.GetOrderDetailsById(Convert.ToInt32(orderid));
                foreach (var item in detailslist)
                {
                    var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster, citymodel.Id);
                    string name = productmasetr.Name;
                    string MasterImageUrl = productmasetr.MasterImageUrl;
                    var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    var tax = productmasetr.ProductPrices.Select(x => x.Tax).FirstOrDefault();
                    var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var actualprice = Math.Round(((decimal)(price * item.Quantity) / conversionvalue), 2);

                    //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                    decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                    var totalDiscountAmount = Math.Round(((decimal)((price  - DiscountedPrice) * item.Quantity) / conversionvalue), 2);
                  
                    var detailsobj = new OrderVM
                    {
                        Name = name,
                        Price = Math.Round(Convert.ToDecimal(price / conversionvalue), 2),
                        Discount = item.Discount,
                        SubTotalPrice = actualprice,//Math.Round(Convert.ToDecimal((price * item.Quantity) / conversionvalue), 2) ,
                        DiscountAmount = totalDiscountAmount,//Math.Round(Convert.ToDecimal(((price * item.Quantity) - item.Price) / conversionvalue), 2),
                        Quantity = item.Quantity,
                        TotalPrice = DiscountedPrice * item.Quantity,//Math.Round(Convert.ToDecimal(item.Price / conversionvalue), 2), 
                        MasterImageUrl = MasterImageUrl,
                        Date = item.CreatedOn.ToString(),
                        FK_ProductMaster = item.FK_ProductMaster
                    };
                    orderdetailslist.Add(detailsobj);
                }
                return Json(new { success = true, data = orderdetailslist }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, msg = "Please Re-Login for Order Details History" }, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------account operation---------------------------------------
       [HttpGet]
        public async Task<ActionResult> LoginB2C(string EmailId, string password)
        {
            try
            {
                //string Current = "/Home/Index";
                if (!string.IsNullOrEmpty(EmailId) && !string.IsNullOrEmpty(password))
                {
                    var res = await _account.SelectByIdPassword(
                        new customer
                        {
                            EmailId = EmailId,
                            Password = password
                        });
                    if (res != null)
                    {
                        var genral = new GenralClass();
                        string cookie = string.Empty;
                        Dictionary<string, string> accountdata = new Dictionary<string, string>()
                        {
                            { "userid", res.Id.ToString() },
                            { "UserName", res.FirstName},
                            {"email",res.EmailId }

                        };
                        if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
                        {
                            //get location from cookie
                            string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                            if (string.IsNullOrEmpty(cookiecity))
                            {
                                HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                            }
                            cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                            City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                            cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                            List<Cart> cartlsit = await _cart.GetCartProducts(cookie, res.Id,citymodel) as List<Cart>;
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
                                    x.FK_CityId = citymodel.Id;
                                    _cart.UpdateToCart(x);
                                }
                            });
                            var duplicatelist = cartlsit.GroupBy(x => x).Where(y => y.Count() > 1).Select(z => z).ToList();
                        }
                        return Json(new { data = accountdata  ,msg = "Login Successfull", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { data = "", msg = "Incorrect Email and Password ", success = false, statuscode = 400, count = 0 }, JsonRequestBehavior.AllowGet);
                    }
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
    }
    public class PaymentVMRequest
    {
        public string Id { get; set; }
        public string Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string PaymentMode { get; set; }
        public string StripeToken { get; set; }
        public string OrderId { get; set; }
        public string stripeToken { get; set; }
        public string currency { get; set; }
        public string conversionrate { get; set; }
    }
}