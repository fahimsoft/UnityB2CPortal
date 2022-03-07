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
    
    public partial class ProductPrice
    {
        public int Id { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int FK_City { get; set; }
        public int FK_ProductMaster { get; set; }
        public Nullable<decimal> Tax { get; set; }
    
        public virtual ProductMaster ProductMaster { get; set; }
    }
}
