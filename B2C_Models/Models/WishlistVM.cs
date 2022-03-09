using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2C_Models.Models
{
    public class WishlistVM
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

        public string Name { get; set; }
        public string MasterImageUrl { get; set; }

        public Nullable<decimal> Price { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> DiscountedPrice { get; set; }
public decimal DiscountAmount { get; set; }


        //  public List<WishlistVM> wishlistVMs { get; set; }

        public decimal CartSubTotal { get;  set; }
        public decimal CartSubTotalDiscount { get;  set; }

        public decimal ShipingAndHostring { get;  set; }
        public decimal OrderTotal { get;  set; }
        public decimal VatTax { get;  set; }
        public decimal Tax { get;  set; }
        public decimal ActualPrice { get; set; }
        public virtual customer customer { get; set; }
        public virtual ProductMaster ProductMaster { get; set; }
        public decimal TaxAmount { get; set; }
    }
}
