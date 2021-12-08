using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Base.PaymentMethod
{
    public class Address
    {
        public object city { get; set; }
        public object country { get; set; }
        public object line1 { get; set; }
        public object line2 { get; set; }
        public object postal_code { get; set; }
        public object state { get; set; }
    }

    public class BillingDetails
    {
        public Address address { get; set; }
        public object email { get; set; }
        public object name { get; set; }
        public object phone { get; set; }
    }

    public class FraudDetails
    {
    }

    public class Metadata
    {
    }

    public class Checks
    {
        public object address_line1_check { get; set; }
        public object address_postal_code_check { get; set; }
        public string cvc_check { get; set; }
    }

    public class Card
    {
        public string brand { get; set; }
        public Checks checks { get; set; }
        public string country { get; set; }
        public int exp_month { get; set; }
        public int exp_year { get; set; }
        public string fingerprint { get; set; }
        public string funding { get; set; }
        public object installments { get; set; }
        public string last4 { get; set; }
        public string network { get; set; }
        public object three_d_secure { get; set; }
        public object wallet { get; set; }
    }

    public class PaymentMethodDetails
    {
        public Card card { get; set; }
        public string type { get; set; }
    }

    public class Refunds
    {
        public string @object { get; set; }
        public List<object> data { get; set; }
        public bool has_more { get; set; }
        public string url { get; set; }
    }

    public class ResponseViewModel
    {
        public string id { get; set; }
        public string @object { get; set; }
        public int amount { get; set; }
        public int amount_captured { get; set; }
        public int amount_refunded { get; set; }
        public object application { get; set; }
        public object application_fee { get; set; }
        public object application_fee_amount { get; set; }
        public string balance_transaction { get; set; }
        public BillingDetails billing_details { get; set; }
        public object calculated_statement_descriptor { get; set; }
        public bool captured { get; set; }
        public int created { get; set; }
        public string currency { get; set; }
        public object customer { get; set; }
        public string description { get; set; }
        public bool disputed { get; set; }
        public object failure_code { get; set; }
        public string failure_message { get; set; }
        public FraudDetails fraud_details { get; set; }
        public object invoice { get; set; }
        public bool livemode { get; set; }
        public Metadata metadata { get; set; }
        public object on_behalf_of { get; set; }
        public object order { get; set; }
        public object outcome { get; set; }
        public bool paid { get; set; }
        public object payment_intent { get; set; }
        public string payment_method { get; set; }
        public PaymentMethodDetails payment_method_details { get; set; }
        public object receipt_email { get; set; }
        public object receipt_number { get; set; }
        public string receipt_url { get; set; }
        public bool refunded { get; set; }
        public Refunds refunds { get; set; }
        public object review { get; set; }
        public object shipping { get; set; }
        public object source_transfer { get; set; }
        public object statement_descriptor { get; set; }
        public object statement_descriptor_suffix { get; set; }
        public string status { get; set; }
        public object transfer_data { get; set; }
        public object transfer_group { get; set; }
    }


}
