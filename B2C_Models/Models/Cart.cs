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
    
    public partial class Cart
    {
        public int Id { get; set; }
        public int FK_ProductMaster { get; set; }
        public Nullable<int> FK_Customer { get; set; }
        public string Guid { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<bool> IsWishlist { get; set; }
        public Nullable<int> TotalQuantity { get; set; }
        public Nullable<decimal> TotalPrice { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Currency { get; set; }
        public Nullable<decimal> ConversionRate { get; set; }
        public Nullable<int> FK_CityId { get; set; }
    
        public virtual customer customer { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public virtual City City { get; set; }
    }
}
