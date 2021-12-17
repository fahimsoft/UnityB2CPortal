using API_Base.PaymentMethod.Interfaces;
using API_Base.PaymentMethod.Servicees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Base.PaymentMethod
{
   public class PaymentMethodFacade
    {
        private IStripe _stripe;
        private IHBL _hbl;
        private IEasyPaisa _easypaisa;
        private IPaypal _paypal;

        public PaymentMethodFacade()
        {
            _stripe = new StripeService();
            _easypaisa = new  EasyPaisaService();
            _hbl = new  HBLService();
            _paypal = new  PaypalService();
        }
        public dynamic CreateStripePayment(PaymentVM paymentViewModel)
        {
          return  _stripe.CreatePayment(paymentViewModel);
        }
        public dynamic CreatePaypalPayment(PaymentVM paymentViewModel)
        {
          return  _paypal.CreatePayment(paymentViewModel);
        }



    }
}
