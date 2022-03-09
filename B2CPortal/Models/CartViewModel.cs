using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class CartViewModel
    {

        public int Id { get; set; }
        public int FK_ProductMaster { get; set; }
        public int FK_Customer { get; set; }
        public string Guid { get; set; }
        public int Quantity { get; set; }
        public bool IsWishlist { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ActualPrice { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime ModifiedOn { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string ImageUrl { get; set; }
        public string MasterImageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Packsize { get; set; }
        public decimal? DiscountedPrice { get; internal set; }
        public decimal CartSubTotal { get; internal set; }
        public decimal CartSubTotalDiscount { get; internal set; }

        public decimal ShipingAndHostring { get; internal set; }
        public decimal OrderTotal { get; internal set; }
        public decimal VatTax { get; internal set; }
        public decimal Tax { get; internal set; }
        public decimal? TaxAmount { get; internal set; }
    }
   
}