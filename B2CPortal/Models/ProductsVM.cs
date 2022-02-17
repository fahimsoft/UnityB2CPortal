using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class ProductsVM
    {

        public int Id { get; set; }
        public int totalProduct { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public  string ImageUrl { get; set; }
        public string MasterImageUrl { get; set; }
        public List<string> ImageUrl2 { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public decimal? DiscountedAmount { get; set; }

        public string UOM { get; set; }
        public SingleProduct product { get; set; }
        public decimal? TotalRating { get; set; }
        public decimal? AvgRating { get; set; }
        public int TotalRatingCount { get; set; }
public int? UnitInNumeric { get; set; }


    }
    public class ProductsCommentVM
    {
        public int? Id { get; set; }
        public int TotalRatingCount { get; set; }
        public decimal? AvgRating { get; set; }



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