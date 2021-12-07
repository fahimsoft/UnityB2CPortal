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
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> StripeAsync(string stripeToken, Payment payment)
        {
            try
            {
                if (string.IsNullOrEmpty(Session["ordertotal"]?.ToString()) || string.IsNullOrEmpty(Session["ordermasterId"]?.ToString()))
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
                if (result != null && ((Charge)result).Amount > 0)
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
                    var model = new PaymentViewModel();
                    model.TotalPrice = Convert.ToDecimal(Session["ordertotal"]);
                    return View("PaymentStatus", model);
                }
                else
                {
                    return RedirectToAction("Checkout", "Orders");
                }
                #region stripe comment code

                //var customeroptions = new CustomerCreateOptions
                //{
                //    Email = payment.Email,
                //    Name = payment.Name,
                //    Phone = payment.Phone,
                //    Description = "Stripe payment",
                //    Source = stripeToken
                //};
                //var customerservice = new CustomerService();
                // var customer = customerservice.Create(customeroptions);
                //payment.Description = "this payment from stripe";
                //var options = new ChargeCreateOptions
                //{
                //    Amount = long.Parse(Session["ordertotal"].ToString()) * 100 ,
                //    Currency = "usd",
                //    Description = payment.Description,
                //  //  Source = stripeToken,
                //    Customer = customer.Id,
                //};
                //var service = new ChargeService();
                //var charge = service.Create(options);
                #endregion


            }
            catch (Exception ex)
            {
                throw ex;
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