using B2CPortal.Models;
using Stripe;
using System;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class PaymentController : Controller
    {
        public ActionResult Stripe()
        {
            ViewBag.Amount = Session["ordertotal"];
            return View();
        }
        [HttpPost]
        public ActionResult Stripe(string stripeToken, Payment payment)
        {
            StripeConfiguration.ApiKey = "sk_test_51K02zhHN8ZNk6QpY0XRnHeR44USPCKbNMY18HocZVMQ7BB0u2NA7xhzJum8kc1wZTJKiUWEVbOp6gwPA26eHF4Hh008jjfgFGZ";
            try
            {
                if (string.IsNullOrEmpty(Session["ordertotal"]?.ToString()))
                {
                    return RedirectToAction("Checkout","Orders");
                }
                var customeroptions = new CustomerCreateOptions
                {
                    Email = payment.Email,
                    Name = payment.Name,
                    Phone = payment.Phone,
                    Description = "Stripe payment",
                    Source = stripeToken
                };
                var customerservice = new CustomerService();
                 var customer = customerservice.Create(customeroptions);
                payment.Description = "this payment from stripe";
                var options = new ChargeCreateOptions
                {
                    Amount = long.Parse(Session["ordertotal"].ToString()) * 100 ,
                    Currency = "usd",
                    Description = payment.Description,
                  //  Source = stripeToken,
                    Customer = customer.Id,
                };
                var service = new ChargeService();
                var charge = service.Create(options);
                var model = new PaymentViewModel();
                model.TotalPrice = (int?)options.Amount;
                return View("PaymentStatus", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ActionResult PaymentStatus(PaymentViewModel paymentViewModel)
        {
            return View(paymentViewModel);
        }
    }
    public class Payment
    {
        public long Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
    }

}