﻿using API_Base.Common;
using API_Base.PaymentMethod;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using Stripe;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;

namespace B2CPortal.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrders _orders = null;
        //private readonly IOrderDetail _ordersDetail = null;
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly PaymentMethodFacade _PaymentMethodFacade = null;
        public PaymentController(IOrders order, IProductMaster productMaster, ICart cart)
        {
            _orders = order;
            _IProductMaster = productMaster;
            _cart = cart;
            _PaymentMethodFacade = new PaymentMethodFacade();
        }

        public ActionResult Stripe()
        {
            ViewBag.Amount = Session["ordertotal"];
            if (ViewBag.Amount <= 0 || ViewBag.Amount == null)
            {
                return RedirectToAction("Checkout", "Orders");
            }
            return View();


        }
        [HttpPost]
        public async Task<ActionResult> StripeAsync(string stripeToken, Payment payment)
        {
            try
            {
                ViewBag.error = "";
                if (string.IsNullOrEmpty(Session["ordertotal"]?.ToString()) || string.IsNullOrEmpty(Session["ordermasterId"]?.ToString()) || HttpContext.Session["UserId"] == null)
                {
                    return RedirectToAction("Checkout", "Orders");
                }
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                var paymentmodel = new PaymentVM
                {
                    Name = payment.Name,
                    Email = payment.Email,
                    Phone = payment.Phone,
                    Description = string.IsNullOrEmpty(payment.Description) ? "this payment from stripe" : payment.Description,
                    StripeToken = stripeToken,
                    Amount = (Session["ordertotal"] == null ? 1 : Convert.ToDecimal(Session["ordertotal"]) * 100) < 50 ? 100 : Convert.ToDecimal(Session["ordertotal"]) * 100,

                };
                dynamic result = _PaymentMethodFacade.CreateStripePayment(paymentmodel);
                //ResponseViewModel resmodel  =  HelperFunctions.ResponseHandler(result);
                Charge chargeobj =  (Charge)result;
                if (result != null && chargeobj.Paid == true)
                {
                    var ordervm = new OrderVM
                    {
                        Id = Convert.ToInt32(Session["ordermasterId"]?.ToString()),
                        Currency =currency,
                        ConversionRate = conversionvalue,
                        PaymentMode = PaymentType.Stripe.ToString(),
                        Status = OrderStatus.Confirmed.ToString(),
                        TotalPrice = Convert.ToDecimal(Session["ordertotal"]),
                        PaymentStatus = true,
                    };
                    var ordermodel = await _orders.UpdateOrderMAster(ordervm);
                    ordervm = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, ordervm);
                    ordervm.OrderNo = HelperFunctions.GenrateOrderNumber(ordervm.Id.ToString());
                    // ------------Remove from cart------------
                    var customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    var removeCart = await _cart.DisableCart(customerId, cookie);
                    return RedirectToAction("PaymentStatus", ordervm);
                }
                else
                {
                    TempData["error"] = chargeobj.FailureMessage;
                    return RedirectToAction("Stripe");
                }   
            }
            catch (StripeException ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Stripe");
            }

        }
      [HttpGet]
        public async Task<ActionResult> PaymentStatus(OrderVM model =  null)
        {
            string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                string conrate =  HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate);
            if (!string.IsNullOrEmpty(conrate))
            {
                decimal conversionvalue = Convert.ToDecimal(conrate);

                if (model == null || model.TotalPrice == null)
                {
                    string orderid = HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId);
                    OrderMaster ordermodel = await _orders.GetOrderMasterById(Convert.ToInt32(orderid));

                    model = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, model);
                    model.DiscountAmount =  model.OrderDetails.Sum(x => x.DiscountedPrice);
                    model.SubTotalPrice =  model.OrderDetails.Sum(x => x.Price);

                    model.Id = ordermodel.Id;
                    model.Status = OrderStatus.Confirmed.ToString();
                    model.TotalPrice = model.TotalPrice;
                    model.PaymentStatus = true;

                    var ordermodelresponse = await _orders.UpdateOrderMAster(model);
                    // orderVM =  (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, ordervm);
                    // ------------Remove from cart------------
                    var customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    var removeCart = await _cart.DisableCart(customerId, cookie);
                }
                model.OrderNo = HelperFunctions.GenrateOrderNumber(model.Id.ToString());
                return View(model);
            }
            else
            {
                return RedirectToAction("Checkout","Orders");
            }
        }
        [HttpGet]
        public ActionResult PaymentStatusCOD(OrderVM orderVM)
        {
            orderVM = (OrderVM)Session["orderdata"];
            if (orderVM ==  null)
            {
                return RedirectToAction("Index","Orders");
            }
            return View(orderVM);
        }
        [HttpGet]
        public ActionResult DownloadPDFOrder()
        {
           var  orderVM = (OrderVM)Session["orderdata"];
            return Json(new { data = orderVM, msg = "", success = true },JsonRequestBehavior.AllowGet);
        }
        #region Paypal paymemnt method in PaypalPaymentMethodController
        public ActionResult Paypal()
        {
            ViewBag.Amount = Session["ordertotal"];
            if (ViewBag.Amount <= 0 || ViewBag.Amount == null)
            {
                return RedirectToAction("Checkout", "Orders");
            }
            return View();

        }
        [HttpPost]
        public ActionResult PaypalAsync(Payment payment)
        {

            ViewBag.error = "";
            if (string.IsNullOrEmpty(Session["ordertotal"]?.ToString()) || string.IsNullOrEmpty(Session["ordermasterId"]?.ToString()) || HttpContext.Session["UserId"] == null)
            {
                return RedirectToAction("Checkout", "Orders");
            }
            string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
            decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

            var paymentmodel = new PaymentVM
            {
                Name = payment.Name,
                Email = payment.Email,
                Phone = payment.Phone,
                Description = string.IsNullOrEmpty(payment.Description) ? "this payment from stripe" : payment.Description,
                Amount = (Session["ordertotal"] == null ? 1 : Convert.ToDecimal(Session["ordertotal"]) * 100) < 50 ? 100 : Convert.ToDecimal(Session["ordertotal"]) * 100,

            };
            dynamic result = _PaymentMethodFacade.CreatePaypalPayment(paymentmodel);

            return View();

        }
        #endregion
    }
    public class Payment
    {
        public decimal Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }

}