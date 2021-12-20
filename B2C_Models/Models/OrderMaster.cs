//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace B2C_Models.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderMaster()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
            this.OrderDetailLogs = new HashSet<OrderDetailLog>();
            this.OrderTransections = new HashSet<OrderTransection>();
        }
    
        public int Id { get; set; }
        public string OrderDescription { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public int FK_Customer { get; set; }
        public string OrderNo { get; set; }
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public string Gender { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string CNIC { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> ConversionRate { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<bool> PaymentStatus { get; set; }
        public Nullable<bool> IsPriceChanged { get; set; }
    
        public virtual customer customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailLog> OrderDetailLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderTransection> OrderTransections { get; set; }
    }
}
