﻿using API_Base.Base;
using API_Base.Common;
using API_Base.PaymentMethod;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using B2CPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class OrdersController : BaseController
    {
        #region Interface instance
        private readonly IOrders _orders = null;
        private readonly IOrderDetail _ordersDetail = null;
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly PaymentMethodFacade _paymentMethodFacade = null;

        public OrdersController(PaymentMethodFacade paymentMethodFacade, IOrders orders, IProductMaster productMaster, ICart cart, IOrderDetail orderDetail)
        {
            _IProductMaster = productMaster;
            _paymentMethodFacade = new PaymentMethodFacade();
            _cart = cart;
            _orders = orders;
            _ordersDetail = orderDetail;
        }
        public ActionResult Index()
        {
            if (Convert.ToInt32(HttpContext.Session["UserId"]) <= 0)
            {
                string CurrentURL = Request.Url.AbsoluteUri;
                TempData["returnurl"] = CurrentURL;
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public async Task<JsonResult> GetOrderDetailsById(int id)
        {
            List<OrderVM> orderdetailslist = new List<OrderVM>();
            decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
            var detailslist = await _ordersDetail.GetOrderDetailsById(id);
            foreach (var item in detailslist)
            {
                var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                string name = productmasetr.Name;
                string MasterImageUrl = productmasetr.MasterImageUrl;
                var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                var actualprice = Math.Round(((decimal)(price * item.Quantity) / conversionvalue), 2);
                var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);

                var detailsobj = new OrderVM
                {
                    Name = name,
                    Price = Math.Round(Convert.ToDecimal(price / conversionvalue), 2),
                    Discount = item.Discount,
                    SubTotalPrice = actualprice,//Math.Round(Convert.ToDecimal((price * item.Quantity) / conversionvalue), 2) ,
                    DiscountAmount = totalDiscountAmount,//Math.Round(Convert.ToDecimal(((price * item.Quantity) - item.Price) / conversionvalue), 2),
                    Quantity = item.Quantity,
                    TotalPrice = discountedprice,//Math.Round(Convert.ToDecimal(item.Price / conversionvalue), 2), 
                    MasterImageUrl = MasterImageUrl,
                    Date = item.CreatedOn.ToString(),
                    FK_ProductMaster = item.FK_ProductMaster
                };
                orderdetailslist.Add(detailsobj);
            }
            return Json(new { data = orderdetailslist }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetOrderList()
        {

            try
            {
                decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
                List<OrderVM> list = new List<OrderVM>();
                if (Convert.ToInt32(HttpContext.Session["UserId"]) > 0)
                {
                    int userid = Convert.ToInt32(HttpContext.Session["UserId"]);
                    var orderlist = await _orders.GetOrderList(userid);

                    foreach (var item in orderlist)
                    {
                        OrderVM dd = (OrderVM)HelperFunctions.CopyPropertiesTo(item, new OrderVM());
                        var result = HelperFunctions.GenrateOrderNumber(dd.Id.ToString());
                        dd.OrderNo = result;
                        dd.Price = dd.Price;
                        dd.SubTotalPrice = dd.SubTotalPrice;
                        dd.DiscountAmount = dd.DiscountAmount;
                        dd.TotalPrice = dd.TotalPrice;

                        list.Add((OrderVM)dd);
                    }
                    return PartialView("_OrderListPartialView", list);
                }
                else
                {
                    string CurrentURL = Request.Url.AbsoluteUri;
                    TempData["returnurl"] = CurrentURL;
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        [HttpGet]
        [ActionName("Checkout")]
        public async Task<ActionResult> Checkout()
        {
            try
            {
                decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
                OrderVM orderVM = new OrderVM();
                List<OrderVM> orderVMs = new List<OrderVM>();
                decimal OrderTotal = 0;
                var totalDiscount = 0;
                var customerId = 0;
                var subTotal = 0;
                if (Session["UserId"] != null)
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    if (customerId > 0)
                    {
                        // customer data for billing and shipment
                        var customer = await _orders.GetCustomerById(customerId);
                        orderVM.FK_Customer = customer.Id;
                        orderVM.FirstName = customer.FirstName;
                        orderVM.LastName = customer.LastName;
                        orderVM.EmailId = customer.EmailId;
                        orderVM.PhoneNo = customer.PhoneNo;
                        orderVM.Country = customer.Country;
                        orderVM.City = customer.City;
                        orderVM.Address = customer.Address;

                        var cartlist = await _cart.GetCartProducts("", customerId);
                        if (cartlist != null)
                        {
                            foreach (var item in cartlist)
                            {
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                                var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);

                                //var DiscountedPrice = price * (1 - (discount / 100));
                                var ActualPrice = (decimal)(price * item.Quantity);
                                subTotal = (int)(subTotal + ActualPrice);
                                totalDiscount = (int)(totalDiscount + discount);
                                var Order = new OrderVM
                                {
                                    Name = productData.Name,
                                    Quantity = item.Quantity,
                                    TotalPrice = discountedprice //(int?)(item.TotalPrice == null ? 0 : item.TotalPrice)
                                };
                                orderVMs.Add(Order);

                                orderVM.CartSubTotalDiscount += totalDiscountAmount;//((decimal)(price * item.Quantity) - (decimal)(item.TotalPrice == null ? 0 : item.TotalPrice));
                                OrderTotal += discountedprice;//Convert.ToDecimal(item.TotalPrice == null ? 0: item.TotalPrice);
                            }
                            orderVM.orderVMs = orderVMs;
                            orderVM.CartSubTotal = Math.Round(subTotal / conversionvalue, 2);
                            orderVM.CartSubTotalDiscount = orderVM.CartSubTotalDiscount;
                            orderVM.OrderTotal = OrderTotal;
                            Session["ordertotal"] = OrderTotal;
                        }
                        return View(orderVM);
                    }
                    else
                    {
                        string ReturnUrl = Convert.ToString(Request.QueryString["url"]);
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    string CurrentURL = Request.Url.AbsoluteUri;
                    TempData["returnurl"] = CurrentURL;





                    return RedirectToAction("Login", "Account");
                }





            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        [HttpPost]
        [ActionName("AddBillingDetails")]
        public async Task<ActionResult> AddBillingDetails(OrderVM Billing)
        { 
            try
            {
                OrderVM orderVM = new OrderVM();
                List<OrderVM> orderVMs = new List<OrderVM>();
                decimal OrderTotal = 0;
                decimal subTotal = 0;
                var customerId = 0;
                var tQuantity = 0;
                decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
                if (Session["UserId"] != null)
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    if (customerId > 0)
                    {
                        // Billing Details Add
                        Billing.FK_Customer = customerId;
                        var cartlist = await _cart.GetCartProducts("", customerId);
                        if (cartlist != null)
                        {
                            foreach (var item in cartlist)
                            {
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var DiscountedPrice = price * (1 - (discount / 100));
                                var ActualPrice = (decimal)(price * item.Quantity);
                                OrderTotal = (decimal)(OrderTotal + item.TotalPrice);
                                subTotal = (subTotal + ActualPrice);
                                tQuantity = (int)(tQuantity + item.Quantity);
                                var Order = new OrderVM
                                {
                                    Name = productData.Name,
                                    Quantity = item.Quantity,
                                    // TotalPrice = Math.Round(Convert.ToDecimal(item.TotalPrice) / conversionvalue, 2),
                                    //no need of conversion already converted into cart
                                    TotalPrice = Math.Round(Convert.ToDecimal(item.TotalPrice), 2),

                                };
                                orderVMs.Add(Order);
                            }
                            Billing.orderVMs = orderVMs;
                            Billing.CartSubTotal = Math.Round(subTotal / conversionvalue, 2);
                            //no need of conversion already converted
                            // Billing.OrderTotal = Math.Round(OrderTotal / conversionvalue, 2);
                            Billing.OrderTotal = Math.Round(OrderTotal, 2);

                            Billing.TotalQuantity = tQuantity;
                            Billing.Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString();
                            Billing.ConversionRate = conversionvalue;
                            Billing.PaymentMode = Billing.paymenttype.ToString();
                            Billing.Status = OrderStatus.InProcess.ToString();
                            Billing.Country = Billing.Country;
                            Billing.City = Billing.City;
                            Billing.ShippingAddress = Billing.ShippingAddress;
                        }
                        // Insert order Master
                        var res = await _orders.CreateOrder(Billing);
                        // Insert order Detail
                        var ordermasterId = res.Id;
                        var orderNo = res.OrderNo;
                        if (cartlist != null)
                        {
                            foreach (var item in cartlist)
                            {
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var DiscountedPrice = price * (1 - (discount / 100));
                                var ActualPrice = (decimal)(price * item.Quantity);
                                OrderTotal = (int)(OrderTotal + item.TotalPrice);
                                subTotal = (int)(subTotal + ActualPrice);
                                tQuantity = (int)(tQuantity + item.Quantity);
                                var Order = new OrderVM
                                {
                                    FK_OrderMaster = ordermasterId,
                                    FK_ProductMaster = item.FK_ProductMaster,
                                    Quantity = item.Quantity,
                                    TotalPrice = item.TotalPrice,
                                    Discount = discount
                                };
                                var response = await _ordersDetail.CreateOrderDetail(Order);
                            }
                        }

                        // Sending Mail
                        try
                        {
                            var name = Session["UserName"].ToString();
                            var email = Session["email"].ToString();
                            string htmlString = @"<html>
                           <body>
                           <img src=" + "~/Content/Asset/img/img.PNG" + @">
                           <h1 style=" + "text-align:center;" + @">Thanks for Your Order!</h1>
                            <p>Dear " + name + @",</p>
                            <p>Hello, " + name + @"! Thanks for Your Order!</p>
                            <p>Order No: " + ordermasterId + @"</p>
                            <p>Total Amount: " + res.TotalPrice + @"</p>
                            <p>Thanks,</p>
                            <p>Unity Foods LTD!</p>
                            </body>
                            </html>";
                            bool IsSendEmail = HelperFunctions.EmailSend(email, "Thanks for Your Order!", htmlString, true);
                            if (IsSendEmail)
                            {
                                // return SuccessResponse("true");
                                //return Json(new { data = IsSendEmail, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                //return BadResponse("Failed");
                                //return Json(new { data = IsSendEmail, msg = "Order Successfull !", success = false }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch 
                        {

                           
                        }

                        // Remove from cart
                        HttpCookie cookie = HttpContext.Request.Cookies.Get("cartguid");
                        var removeCart = await _cart.DisableCart(customerId, cookie.Value);

                        if (Billing.paymenttype == PaymentType.Stripe)
                        {
                            Session["ordermasterId"] = ordermasterId;
                            string url = Url.Action("Stripe", "Payment");
                            return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                        }
                        else if (Billing.paymenttype == PaymentType.COD)
                        {
                           // var model = new OrderVM();
                           //var ordermaster = await _orders.GetOrderMasterById(ordermasterId);
                           // var paymentobj =  HelperFunctions.CopyPropertiesTo(ordermaster, model);
                           // model = (OrderVM)paymentobj;
                            Session["ordermasterId"] = ordermasterId;
                            Session["ordertotal"] = orderVM.OrderTotal;
                            //model.TotalPrice = Convert.ToDecimal(Session["ordertotal"]);
                            //var customer = await _orders.GetCustomerById(customerId);
                            //model.FirstName = customer.FirstName;
                            //model.LastName = customer.LastName;
                            //model.EmailId = customer.EmailId;
                            //model.PhoneNo = customer.PhoneNo;
                            //model.Country = customer.Country;
                            //model.City = customer.City;
                            //model.Address = customer.Address;
                            var result = HelperFunctions.GenrateOrderNumber(ordermasterId.ToString());
                            Billing.OrderNo = result;
                            Session["orderdata"] = Billing;
                            string url = Url.Action("PaymentStatusCOD", "Payment");
                            return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { data = "", msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    return Json(new { data = "", msg = "Something bad happened", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
    }
}