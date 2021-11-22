using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class ProductsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public List< string> ImageUrl { get; set; }
        public string MasterImageUrl { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public SingleProduct  product { get; set; }

    }
    public class SingleProduct
    {
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }
    public List<string> ImageUrl { get; set; }
    public string MasterImageUrl { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
}
}