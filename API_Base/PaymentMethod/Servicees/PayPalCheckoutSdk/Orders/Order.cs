// This class was generated on Tue, 21 May 2019 11:25:19 PDT by version 0.1.0-dev+8fcb5f of Braintree SDK Generator
// Order.cs
// @version 0.1.0-dev+8fcb5f
// @type object
// DO NOT EDIT
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace PayPalCheckoutSdk.Orders
{
    /// <summary>
    /// The order details.
    /// </summary>
    [DataContract]
    public class Order
    {
        /// <summary>
	    /// Required default constructor
		/// </summary>
        public Order() {}

        /// <summary>
        /// The intent to either capture payment immediately or authorize a payment for an order after order creation.
        /// </summary>
        [DataMember(Name="intent", EmitDefaultValue = false)]
        public string CheckoutPaymentIntent;

        /// <summary>
        /// The date and time, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6). Seconds are required while fractional seconds are optional.<blockquote><strong>Note:</strong> The regular expression provides guidance but does not reject all invalid dates.</blockquote>
        /// </summary>
        [DataMember(Name="create_time", EmitDefaultValue = false)]
        public string CreateTime;

        /// <summary>
        /// The date and time, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6). Seconds are required while fractional seconds are optional.<blockquote><strong>Note:</strong> The regular expression provides guidance but does not reject all invalid dates.</blockquote>
        /// </summary>
        [DataMember(Name="expiration_time", EmitDefaultValue = false)]
        public string ExpirationTime;

        /// <summary>
        /// The ID of the order.
        /// </summary>
        [DataMember(Name="id", EmitDefaultValue = false)]
        public string Id;

        /// <summary>
        /// An array of request-related [HATEOAS links](/docs/api/reference/api-responses/#hateoas-links). To complete payer approval, use the `approve` link with the `GET` method.
        /// </summary>
        [DataMember(Name="links", EmitDefaultValue = false)]
        public List<LinkDescription> Links;

        /// <summary>
        /// The customer who approves and pays for the order. The customer is also known as the payer.
        /// </summary>
        [DataMember(Name="payer", EmitDefaultValue = false)]
        public Payer Payer;

        /// <summary>
        /// An array of purchase units. Each purchase unit establishes a contract between a customer and merchant. Each purchase unit represents either a full or partial order that the customer intends to purchase from the merchant.
        /// </summary>
        [DataMember(Name="purchase_units", EmitDefaultValue = false)]
        public List<PurchaseUnit> PurchaseUnits;

        /// <summary>
        /// The order status.
        /// </summary>
        [DataMember(Name="status", EmitDefaultValue = false)]
        public string Status;

        /// <summary>
        /// The date and time, in [Internet date and time format](https://tools.ietf.org/html/rfc3339#section-5.6). Seconds are required while fractional seconds are optional.<blockquote><strong>Note:</strong> The regular expression provides guidance but does not reject all invalid dates.</blockquote>
        /// </summary>
        [DataMember(Name="update_time", EmitDefaultValue = false)]
        public string UpdateTime;
    }
}

