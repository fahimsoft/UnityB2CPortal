using System;
using System.Collections.Generic;
using System.Linq;
using B2C_Models.Models;
using System.Text;
using System.Threading.Tasks;
using B2CPortal.Models;
using B2CPortal.Controllers;

namespace B2CPortal.Interfaces
{
  public interface IProductMaster
    {
        //Task<IEnumerable<ProductsVM>> GetProduct();

        Task<IEnumerable<ProductMaster>> GetProduct();

        Task<IEnumerable<ProductMaster>> GetFeaturedProduct();

        Task<ProductMaster> GetProductById(long Id);
        Task<IEnumerable<ProductMaster>> GetProductByName(string name);

        Task<ProductMaster> GetDataForWishList(int id);

        //Task<ProductMaster> AddProduct(ProductMaster Product);
        //Task<bool> DeleteProduct(long Id);

        Task<BrandCategoryVM> GetSidebar();
        Task<IEnumerable<ProductMaster>> GetProductListbySidebar(SideBarVM[] filterList); //int[] filterList

        //Task<ProductMaster> AddProduct(ProductMaster Product);
        //Task<bool> DeleteProduct(long Id);

    }
}
