using API_Base.Base;
using API_Base.Common;
using API_Base.PaymentMethod;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using B2CPortal.Services;
using B2CPortal.Services.EmailTemplates;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IShippingDetails _IShippingDetails = null;
        private readonly ICart _cart = null;
        private readonly PaymentMethodFacade _paymentMethodFacade = null;

        public OrdersController(IShippingDetails shippingDetails ,PaymentMethodFacade paymentMethodFacade, IOrders orders, IProductMaster productMaster, ICart cart, IOrderDetail orderDetail)
        {
            _IShippingDetails = shippingDetails;
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
            decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
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

                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
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
                        //dd.Price = dd.Price;
                        //dd.SubTotalPrice = dd.SubTotalPrice;
                        //dd.DiscountAmount = dd.DiscountAmount;
                        //dd.TotalPrice = dd.TotalPrice;

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
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

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

                        var cartguid = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                        var cartlist = await _cart.GetCartProducts(cartguid, customerId);
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
                                var ActualPrice = ((decimal)(price * item.Quantity) / conversionvalue) ;
                                subTotal = (int)(subTotal + ActualPrice);
                                totalDiscount = (int)(totalDiscount + discount);
                                var Order = new OrderVM
                                {
                                    Name = productData.Name,
                                    Price = Math.Round((decimal)(price /conversionvalue),2),
                                    Quantity = item.Quantity,
                                    Discount = discount,
                                    SubTotalPrice = Math.Round(ActualPrice,2),
                                };
                                orderVMs.Add(Order);

                                orderVM.CartSubTotalDiscount += totalDiscountAmount;//((decimal)(price * item.Quantity) - (decimal)(item.TotalPrice == null ? 0 : item.TotalPrice));
                                OrderTotal += discountedprice;//Convert.ToDecimal(item.TotalPrice == null ? 0: item.TotalPrice);
                            }
                            orderVM.orderVMs = orderVMs;
                            orderVM.CartSubTotal = Math.Round(subTotal / conversionvalue, 2);
                            orderVM.CartSubTotalDiscount = orderVM.CartSubTotalDiscount;
                            orderVM.OrderTotal = OrderTotal;
                            orderVM.Currency = currency;

                            HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount, OrderTotal.ToString(), true);

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
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                if (Session["UserId"] != null  && currency.ToLower() == "pkr" &&
                   ( Billing.paymenttype == PaymentType.Stripe ||
                    Billing.paymenttype == PaymentType.Paypal))
                {
                    return Json(new { data = "", msg = $"You can not pay with {Billing.paymenttype}", success = false }, JsonRequestBehavior.AllowGet);
                }
                if (Session["UserId"] != null &&  
                    (Billing.paymenttype == PaymentType.Stripe
                    || Billing.paymenttype == PaymentType.COD
                   || Billing.paymenttype == PaymentType.Paypal)
                    )
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    //------------existing order remove (manage)-----------------
                    OrderMaster Omaster = await _orders.ExestingOrder(customerId);
                    if (Omaster != null)
                    {
                        var result = await _orders.DeleteOrderMAster(Omaster.Id);
                    }
                    if (customerId > 0)
                    {
                       var shippingmodel = new ShippingDetail();
                        // Billing Details Add
                        Billing.FK_Customer = customerId;
                        string cartguid = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                        var cartlist = await _cart.GetCartProducts(cartguid, customerId);
                        if (cartlist != null && cartlist.Count() > 0)
                        {
                            //for shipping address table..
                            if (Billing.shippingdetails != null)
                            {
                                shippingmodel.FirstName = Billing.shippingdetails.FirstName;
                                shippingmodel.LastName = Billing.shippingdetails.LastName;
                                shippingmodel.City = Billing.shippingdetails.City;
                                shippingmodel.Country = Billing.shippingdetails.Country;
                                shippingmodel.EmailId = Billing.shippingdetails.EmailId;
                                shippingmodel.PhoneNo = Billing.shippingdetails.PhoneNo;
                                shippingmodel.Address = Billing.shippingdetails.Address;
                                shippingmodel.Address = Billing.shippingdetails.Address;
                                shippingmodel.IsActive = true;
                                shippingmodel = await _IShippingDetails.CreateShippingDetail(shippingmodel);
                            }

                            foreach (var item in cartlist)
                            {
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                                var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);
                                var ActualPrice = ((decimal)(price * item.Quantity) / conversionvalue);
                                OrderTotal = (OrderTotal + Convert.ToDecimal(discountedprice));
                                subTotal = (subTotal + ActualPrice);
                                tQuantity = (int)(tQuantity + item.Quantity);
                                var Order = new OrderVM
                                {
                                    Name = productData.Name,
                                    Quantity = item.Quantity,
                                    Discount = discount,
                                    Price = price,
                                    CartSubTotalDiscount = totalDiscountAmount,
                                    // TotalPrice = Math.Round(Convert.ToDecimal(item.TotalPrice) / conversionvalue, 2),
                                    //no need of conversion already converted into cart
                                    SubTotalPrice = Math.Round(ActualPrice, 2),

                                };
                                orderVMs.Add(Order);
                            }
                            Billing.orderVMs = orderVMs;
                            Billing.PaymentStatus = false;
                            Billing.CartSubTotal = Math.Round(subTotal / conversionvalue, 2);
                            Billing.OrderTotal = Math.Round(OrderTotal, 2);

                            Billing.TotalQuantity = tQuantity;
                            Billing.Currency = currency;
                            Billing.ConversionRate = conversionvalue;
                            Billing.PaymentMode = Billing.paymenttype.ToString();
                            Billing.Status = OrderStatus.InProcess.ToString();
                            Billing.Country = Billing.Country;
                            Billing.City = Billing.City;
                            Billing.ShippingAddress = Billing.ShippingAddress;
                            Billing.BillingAddress = Billing.BillingAddress;
                            Billing.FK_ShippingDetails = shippingmodel.Id;
                            Billing.IsShipping = Billing.shippingdetails == null ? false : true;
                            Billing.OrderDescription = string.IsNullOrEmpty(Billing.OrderDescription) ? "order has been genrated successfully" : Billing.OrderDescription;
                            var orderresult = Billing.TotalQuantity <= 0 ? null : await _orders.CreateOrder(Billing);
                            // Insert order Master

                            if (orderresult != null)
                            {
                                // Insert order Detail
                                var ordermasterId = orderresult.Id;
                                var orderNo = orderresult.OrderNo;
                                if (cartlist != null)
                                {
                                    foreach (var item in cartlist)
                                    {
                                        var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                        var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                        var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                        var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                                        var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);
                                        var ActualPrice = (decimal)(price * item.Quantity);

                                        OrderTotal = (int)(OrderTotal + item.TotalPrice);
                                        subTotal = (int)(subTotal + ActualPrice);
                                        tQuantity = (int)(tQuantity + item.Quantity);
                                        var Order = new OrderVM
                                        {
                                            FK_OrderMaster = ordermasterId,
                                            FK_ProductMaster = item.FK_ProductMaster,
                                            SubTotalPrice = discountedprice,
                                            DiscountAmount = totalDiscountAmount,
                                            Price = price,
                                            Discount = discount,
                                            Quantity = item.Quantity,
                                            FK_Customer = customerId,
                                            ConversionRate = conversionvalue,
                                            Currency = currency,

                                        };
                                        var response = await _ordersDetail.CreateOrderDetail(Order);
                                    }
                                }

                                // Sending Mail
                                try
                                {
                                    //if (Billing.paymenttype != PaymentType.Stripe)
                                    //{

                                        string recepit = string.Empty;
                                        var name = Session["UserName"].ToString();
                                        var email = Session["email"].ToString();
                                        string htmlString = @"<html>
                           <body>
                           <img src=" + "~/Content/Asset/img/img.PNG" + @">
                           <h1 style=" + "text-align:center;" + @">Thanks for Your Order!</h1>
                            <p>Dear " + name + @",</p>
                            <p>Hello, " + name + @"! Thanks for Your Order!</p>
                            <p>Order No: " + ordermasterId + @"</p>
                            <p>Total Amount: " + orderresult.TotalPrice + @"</p>
                            <p>Thanks,</p>
                            <p>Unity Foods LTD!</p>
                            </body>
                            </html>";
                                        //Fetching Email Body Text from EmailTemplate File.  
                                        string MailText = Templates.OrderEmail(name, orderresult.OrderDescription, orderresult.PhoneNo, orderresult.EmailId,
                                           orderresult.CreatedOn.ToString(), orderresult.ShippingAddress, orderresult.BillingAddress, orderresult.PaymentMode,
                                           orderresult.Status, orderresult.TotalQuantity.ToString(), currency,
                                          orderresult.TotalPrice.ToString(), HelperFunctions.GenrateOrderNumber(ordermasterId.ToString()),
                                          Billing.orderVMs.Sum(x => x.CartSubTotalDiscount).ToString(), Billing.orderVMs.Sum(x => x.SubTotalPrice).ToString(),
                                         recepit);
                                        bool IsSendEmail = HelperFunctions.EmailSend(email, "Thanks for Your Order!", MailText, true);

                                    //}
                                    // MailText = MailText.Replace("[name]", name);
                                    // MailText = MailText.Replace("[orderdescription]", orderresult.OrderDescription);
                                    // MailText = MailText.Replace("[phoneno]", orderresult.PhoneNo);
                                    // MailText = MailText.Replace("[email]", orderresult.EmailId);
                                    // MailText = MailText.Replace("[orderno]", HelperFunctions.GenrateOrderNumber(ordermasterId.ToString()));
                                    // MailText = MailText.Replace("[orderdate]", orderresult.CreatedOn.ToString());
                                    // MailText = MailText.Replace("[shippingaddress]", orderresult.ShippingAddress);
                                    // MailText = MailText.Replace("[billingaddress]", orderresult.BillingAddress.ToString());
                                    // MailText = MailText.Replace("[paymentmode]", orderresult.PaymentMode.ToString());
                                    // MailText = MailText.Replace("[paymentstatus]", orderresult.Status.ToString());
                                    // MailText = MailText.Replace("[quentity]", orderresult.TotalQuantity.ToString());
                                    //// MailText = MailText.Replace("[ordertotalamount]", Billing.orderVMs.Sum(x => x.Price).ToString());
                                    // MailText = MailText.Replace("[totaldiscount]", Billing.orderVMs.Sum(x => x.CartSubTotalDiscount).ToString());
                                    // MailText = MailText.Replace("[subtotal]", Billing.orderVMs.Sum(x => x.SubTotalPrice).ToString());
                                    // MailText = MailText.Replace("[ShippingandHanding]", "0.0");
                                    // MailText = MailText.Replace("[Vat]", "0.0");
                                    // MailText = MailText.Replace("[Ordertotal]",  currency + " "+ orderresult.TotalPrice.ToString());



                                }
                                catch
                                {


                                }

                                HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId, ordermasterId.ToString(), true);
                                HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount, Billing.OrderTotal.ToString(), true);

                                if (Billing.paymenttype == PaymentType.Stripe)
                                {
                                    string url = Url.Action("Stripe", "Payment");
                                    return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                                }
                                else if (Billing.paymenttype == PaymentType.Paypal)
                                {
                                    string url = Url.Action("Paypal", "PaypalPaymentMethod");
                                    return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                                }
                                else if (Billing.paymenttype == PaymentType.COD)
                                {
                                    //-------------remvoe from cart-------------
                                    var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                                    var removeCart = await _cart.DisableCart(customerId, cookie);
                                    //var result = HelperFunctions.GenrateOrderNumber(ordermasterId.ToString());
                                    //Billing.OrderNo = result;
                                    //Session["orderdata"] = Billing;
                                    string url = Url.Action("PaymentStatus", "Payment");
                                    return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    return Json(new { data = "", msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new { data = "", msg = "Please Re-Genrate Your Order.", success = false }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new { data = "", msg = "Your Cart is Empty.", success = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                   string msg =  Session["UserId"] != null ? string.Format("Sorry ! Currently {0} Not Supported", Billing.paymenttype.ToString()) : "Something bad happened";
                    return Json(new { data = "", msg = msg, success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { data = "", msg = ex.Message, success = false }, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //[ActionName("UpdateOrder")]
        //public async Task<ActionResult> UpdateOrder(OrderVM Billing)
        //{
        //    try
        //    {
        //        OrderVM orderVM = new OrderVM();
        //        List<OrderVM> orderVMs = new List<OrderVM>();
        //        decimal OrderTotal = 0;
        //        decimal subTotal = 0;
        //        var customerId = 0;
        //        var tQuantity = 0;
        //        string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
        //        decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

        //        if (Session["UserId"] != null)
        //        {
        //            customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
        //            if (customerId > 0)
        //            {
        //                var ordermodel = await _orders.GetOrderMasterById(Billing.Id);
        //                var ordervm = new OrderVM
        //                {
        //                    Id = Billing.Id,
        //                    Currency = ordermodel.Currency,
        //                    ConversionRate = (decimal)ordermodel.ConversionRate,
        //                    PaymentMode = Billing.paymenttype.ToString(),
        //                    Status = OrderStatus.InProcess.ToString(),
        //                    TotalPrice = ordermodel.TotalPrice,
        //                    PaymentStatus = false,
        //                };
        //                var orderresponse = await _orders.UpdateOrderMAster(ordervm);
        //                // Billing Details Add=============================================
        //                foreach (var item in ordermodel.OrderDetails)
        //                {
        //                    var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
        //                    var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
        //                    var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
        //                    var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
        //                    var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);
        //                    var ActualPrice = (decimal)(price * item.Quantity);

        //                    OrderTotal += Convert.ToDecimal(discountedprice);
        //                    subTotal = (subTotal + ActualPrice);
        //                    tQuantity = (int)(tQuantity + item.Quantity);
        //                    var Order = new OrderVM
        //                    {
        //                        Name = productData.Name,
        //                        Quantity = item.Quantity,
        //                        Discount = discount,
        //                        Price = price,
        //                        CartSubTotalDiscount = totalDiscountAmount,
        //                        // TotalPrice = Math.Round(Convert.ToDecimal(item.TotalPrice) / conversionvalue, 2),
        //                        //no need of conversion already converted into cart
        //                        SubTotalPrice = Math.Round(Convert.ToDecimal(item.TotalPrice), 2),

        //                    };
        //                    orderVMs.Add(Order);
        //                }
        //                Billing.orderVMs = orderVMs;
        //                Billing.PaymentStatus = false;
        //                Billing.CartSubTotal = Math.Round(subTotal / conversionvalue, 2);
        //                Billing.OrderTotal = Math.Round(OrderTotal, 2);

        //                Billing.TotalQuantity = tQuantity;
        //                Billing.Currency = currency;
        //                Billing.ConversionRate = conversionvalue;
        //                Billing.PaymentMode = Billing.paymenttype.ToString();
        //                Billing.Status = OrderStatus.InProcess.ToString();
        //                Billing.Country = Billing.Country;
        //                Billing.City = Billing.City;
        //                Billing.ShippingAddress = Billing.ShippingAddress;
        //                Billing.OrderDescription = "order has been genrated successfully";

        //                HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId, ordervm.Id.ToString(), true);
        //                HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount, orderVM.OrderTotal.ToString(), true);

        //                if (Billing.paymenttype == PaymentType.Stripe)
        //                {
        //                    string url = Url.Action("Stripe", "Payment");
        //                    return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
        //                }
        //                if (Billing.paymenttype == PaymentType.Paypal)
        //                {
        //                    string url = Url.Action("Paypal", "PaypalPaymentMethod");
        //                    return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
        //                }
        //                else if (Billing.paymenttype == PaymentType.COD)
        //                {
        //                    var result = HelperFunctions.GenrateOrderNumber(ordervm.Id.ToString());
        //                    Billing.OrderNo = result;
        //                    Session["orderdata"] = Billing;
        //                    string url = Url.Action("PaymentStatusCOD", "Payment");
        //                    return Json(new { data = url, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    return Json(new { data = "", msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction("Login", "Account");
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { data = "", msg = "Something bad happened", success = false }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadResponse(ex);
        //    }
        //}

        [HttpGet]
        [ActionName("ManagePayment")]
        public async Task<ActionResult> ManagePayment(int id)
        {
            try
            {
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate)); OrderVM orderVM = new OrderVM();
                List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();
                var customerId = 0;
                decimal ordertoal = 0;
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

                        var cartguid = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                        //var cartlist = await _cart.GetCartProducts(cartguid, customerId);
                        var ordermodel = await _orders.GetOrderMasterById(id);
                        if (ordermodel != null)
                        {
                            orderVM = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, orderVM);
                            foreach (var item in ordermodel.OrderDetails)
                            {
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                                var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);
                                var Orderdetails = new OrderDetailsViewModel
                                {
                                    Id = item.Id,
                                    Name = productData.Name,
                                    Quantity = item.Quantity,
                                    Price = price,
                                    Discount = discount,
                                    SubTotalPrice = discountedprice,
                                    DiscountAmount = totalDiscountAmount,
                                    Currency = currency,
                                    ConversionRate = conversionvalue,
                                };
                                orderDetailsVM.Add(Orderdetails);
                                //update order details
                                var detailsresult = await _ordersDetail.UpdateOrderDetails(Orderdetails);

                                orderVM.DiscountAmount += totalDiscountAmount;
                                ordertoal += discountedprice;
                            }
                            orderVM.TotalPrice = ordertoal;
                            orderVM.Currency = currency;
                            orderVM.ConversionRate = conversionvalue;
                            orderVM.Id = id;
                            var orderresult = await _orders.UpdateOrderMAster(orderVM);
                            HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount, orderVM.TotalPrice.ToString(), true);
                            orderVM.OrderDetailsViewModels = orderDetailsVM;
                            return View(orderVM);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }

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

    }
}