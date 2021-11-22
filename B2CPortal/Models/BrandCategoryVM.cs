using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class BrandCategoryVM
    {
        public List<ProductBrand> Brand { get; set; }
        public List<ProductCategory> Category { get; set; }

    }
}