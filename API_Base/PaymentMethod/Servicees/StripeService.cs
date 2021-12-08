using API_Base.PaymentMethod.Interfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Base.PaymentMethod.Servicees
{
    public class StripeService : IStripe
    {
        public dynamic CreatePayment(PaymentVM payment)
        {
            try
            {
                StripeConfiguration.ApiKey = "sk_test_51K02zhHN8ZNk6QpY0XRnHeR44USPCKbNMY18HocZVMQ7BB0u2NA7xhzJum8kc1wZTJKiUWEVbOp6gwPA26eHF4Hh008jjfgFGZ";
                var customeroptions = new CustomerCreateOptions
                {
                    Email = payment.Email,
                    Name = payment.Name,
                    Phone = payment.Phone,
                    Description = payment.Description,
                    Source = payment.StripeToken
                };
                var customerservice = new CustomerService();
                var customer = customerservice.Create(customeroptions);
                payment.Description = "this payment from stripe";
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt64(payment.Amount),
                    Currency = "usd",
                    Description = payment.Description,
                    //  Source = stripeToken,
                    Customer = customer.Id,
                };
                var service = new ChargeService();
                dynamic charge = service.Create(options);
                return charge;
            }
            catch (StripeException ex)
            {
                throw ex;
            }
        }
    }
}
