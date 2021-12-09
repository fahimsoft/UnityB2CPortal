using API_Base.Common;
using API_Base.PaymentMethod;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using Stripe;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

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
                var ConversionRate = "";
                ConversionRate = string.IsNullOrEmpty(Session["ConversionRate"]?.ToString()) ? "1" : Session["ConversionRate"]?.ToString();

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
                        Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString(),
                        ConversionRate = decimal.Parse(ConversionRate),
                        PaymentMode = PaymentType.Stripe.ToString(),
                        Status = OrderStatus.Confirmed.ToString(),
                        TotalPrice = Convert.ToDecimal(Session["ordertotal"]),
                        PaymentStatus = true,
                    };
                    var dd = await _orders.UpdateOrderMAster(ordervm);
                    // ------------Remove from cart------------
                    var customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    var removeCart = await _cart.DisableCart(customerId, cookie);

                    return View("PaymentStatus", paymentmodel);
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
        public ActionResult PaymentStatus(PaymentVM paymentViewModel)
        {
            return View(paymentViewModel);
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