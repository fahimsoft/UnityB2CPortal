using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Base.PaymentMethod
{
    public class PaymentVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
        public string StripeToken { get; set; }
        public string Description { get; set; }
        public long Amount { get; set; }
        public string Phone { get; set; }
    }
}
