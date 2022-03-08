using API_Base.Common;
using API_Base.PaymentMethod;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using Stripe;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using B2CPortal.Services.EmailTemplates;

namespace B2CPortal.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrders _orders = null;
        //private readonly IOrderDetail _ordersDetail = null;
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly IOrderTransection _orderTransection = null;
        private readonly PaymentMethodFacade _PaymentMethodFacade = null;

        public PaymentController(IOrders order, IProductMaster productMaster, ICart cart, IOrderTransection orderTransection)
        {
            _orders = order;
            _IProductMaster = productMaster;
            _cart = cart;
            _orderTransection = orderTransection;
            _PaymentMethodFacade = new PaymentMethodFacade();
        }
        public ActionResult Stripe()
        {
            try
            {
                ViewBag.Amount = HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount);
                if (string.IsNullOrEmpty(ViewBag.Amount))
                {
                    return RedirectToAction("Checkout", "Orders");
                }
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public async Task<ActionResult> StripeAsync(string stripeToken, Payment payment)
        {
            try
            {
                ViewBag.error = "";
               string OrderTotalAmount = HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount);
               string ordermasterId = HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId);
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                if (string.IsNullOrEmpty(OrderTotalAmount) || string.IsNullOrEmpty(ordermasterId) || string.IsNullOrEmpty(userid))
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
                    Amount = Convert.ToDecimal(OrderTotalAmount) * 100 < 50 ? 100 : Convert.ToDecimal(OrderTotalAmount) * 100,
                    StripeAmount = Convert.ToDecimal(OrderTotalAmount),
                };
                dynamic result = _PaymentMethodFacade.CreateStripePayment(paymentmodel);
                //ResponseViewModel resmodel  =  HelperFunctions.ResponseHandler(result);
                Charge chargeobj =  (Charge)result;
                if (result != null && chargeobj.Paid == true)
                {
     

                    var ordervm = new OrderVM
                    {
                        Id = Convert.ToInt32(ordermasterId),
                        Currency =currency,
                        ConversionRate = conversionvalue,
                        PaymentMode = PaymentType.Stripe.ToString(),
                        Status = OrderStatus.Confirmed.ToString(),
                        TotalPrice = Convert.ToDecimal(OrderTotalAmount),
                        PaymentStatus = true,
                    };
                    var ordermodel = await _orders.UpdateOrderMAster(ordervm);
                    ordervm = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, ordervm);
                    ordervm.OrderNo = HelperFunctions.GenrateOrderNumber(ordervm.Id.ToString());
                    ordervm.DiscountAmount = ordervm.OrderDetails.Sum(x => x.DiscountedPrice);
                    ordervm.SubTotalPrice = ordervm.OrderDetails.Sum(x => x.Price);
                    // ------------Remove from cart------------
                    var customerId = Convert.ToInt32(userid);
                    var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    var removeCart = await _cart.DisableCart(customerId, cookie);
                    //-------------------add order transection record--------------
                    OrderTransection mod3el = new OrderTransection();
                    mod3el.FullName = payment.Name;
                    mod3el.EmailId = payment.Email;
                    mod3el.PhoneNo = payment.Phone;
                    mod3el.StripePaymentID = chargeobj.Id;
                    mod3el.IsActive = true;
                    mod3el.Status = chargeobj.Status.ToLower();
                    mod3el.FK_Customer = customerId;
                    mod3el.FK_OrderMAster = Convert.ToInt32(HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId));
                    var dd = await _orderTransection.CreateOrderTransection(mod3el);
                    //---------------order email------------------
                    //string recepit = string.Empty;
                    //var name = Session["UserName"].ToString();
                    //var email = Session["email"].ToString();
                    ////Fetching Email Body Text from EmailTemplate File.  
                    //string MailText = Templates.OrderEmail(name, ordermodel.OrderDescription, ordermodel.PhoneNo, ordermodel.EmailId,
                    //   ordermodel.CreatedOn.ToString(), ordermodel.ShippingAddress, ordermodel.BillingAddress, ordermodel.PaymentMode,
                    //   ordermodel.Status, ordermodel.TotalQuantity.ToString(), currency,
                    //  ordermodel.TotalPrice.ToString(), HelperFunctions.GenrateOrderNumber(ordermasterId.ToString()),
                    //  ordermodel.OrderDetails.Sum(x => x.DiscountedPrice).ToString(), ordermodel.OrderDetails.Sum(x => x.TotalPrice).ToString(),
                    // chargeobj.ReceiptUrl);
                    //bool IsSendEmail = HelperFunctions.EmailSend(email, "Thanks for Your Order!", MailText, true);

                    return RedirectToAction("PaymentStatus");
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
            try
            {
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                string conrate = HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate);
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                if (!string.IsNullOrEmpty(conrate))
                {
                   /// decimal conversionvalue = Convert.ToDecimal(conrate);

                    if (model == null || model.TotalPrice == null)
                    {
                        string orderid = HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId);
                        OrderMaster ordermodel = await _orders.GetOrderMasterById(Convert.ToInt32(orderid));

                        model = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, model);
                        model.DiscountAmount = model.OrderDetails.Sum(x => x.DiscountedPrice);
                        model.SubTotalPrice = model.OrderDetails.Sum(x => x.Price) ;

                        model.Id = ordermodel.Id;
                        model.Status = OrderStatus.Confirmed.ToString();
                        model.TotalPrice = model.TotalPrice;
                        model.PaymentStatus = true;

                        var ordermodelresponse = await _orders.UpdateOrderMAster(model);
                        // orderVM =  (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, ordervm);
                        // ------------Remove from cart------------
                        var customerId = Convert.ToInt32(userid);
                        var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                        var removeCart = await _cart.DisableCart(customerId, cookie);
                    }
                    model.OrderNo = HelperFunctions.GenrateOrderNumber(model.Id.ToString());
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Checkout", "Orders");
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult PaymentStatusCOD(OrderVM orderVM)
        {
            try
            {
                orderVM = (OrderVM)Session["orderdata"];
                if (orderVM == null)
                {
                    return RedirectToAction("Index", "Orders");
                }
                return View(orderVM);
            }
            catch (Exception)
            {

                throw;
            }
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
            if (ViewBag.Amount <= 0 || ViewBag.Amount == null)
            {
                return RedirectToAction("Checkout", "Orders");
            }
            return View();

        }
        [HttpPost]
        public ActionResult PaypalAsync(Payment payment)
        {

            string OrderTotalAmount = HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount);
            string ordermasterId = HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId);
            string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);


            ViewBag.error = "";
            if (string.IsNullOrEmpty(OrderTotalAmount) || string.IsNullOrEmpty(ordermasterId) || string.IsNullOrEmpty(userid))
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
                Amount = Convert.ToDecimal(OrderTotalAmount),
            };
            dynamic result = _PaymentMethodFacade.CreatePaypalPayment(paymentmodel);

            return View();

        }
        #endregion
    }
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
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