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
    
    public partial class ProductMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductMaster()
        {
            this.ProductDetails = new HashSet<ProductDetail>();
            this.ProductPrices = new HashSet<ProductPrice>();
            this.CommentAndRatings = new HashSet<CommentAndRating>();
            this.Carts = new HashSet<Cart>();
            this.OrderDetails = new HashSet<OrderDetail>();
            this.OrderDetailLogs = new HashSet<OrderDetailLog>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public int FK_ProductCategory { get; set; }
        public int FK_ProductBrand { get; set; }
        public int FK_ProductVeriant { get; set; }
        public int FK_ProductPackSize { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string MasterImageUrl { get; set; }
        public bool IsFeatured { get; set; }
    
        public virtual ProductBrand ProductBrand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductDetail> ProductDetails { get; set; }
        public virtual ProductPackSize ProductPackSize { get; set; }
        public virtual ProductVariant ProductVariant { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductPrice> ProductPrices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CommentAndRating> CommentAndRatings { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetailLog> OrderDetailLogs { get; set; }
    }
}
