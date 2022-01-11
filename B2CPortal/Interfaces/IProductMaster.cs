using System;
using System.Collections.Generic;
using System.Linq;
using B2C_Models.Models;
using System.Text;
using System.Threading.Tasks;
using B2CPortal.Models;
using B2CPortal.Controllers;
using API_Base.Base;

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
        Task<BrandCategoryVM> GetSidebar();


        Task<IEnumerable<ProductsVM>> GetProductListbySidebar(SideBarVM[] filterList, string search, int nextPage, int prevPage); //int[] filterList


        Task<IEnumerable<ProductsVM>> GetProductByIdWithRating(long Id);
        List<ProductsVM> GetProductRating(string Id);
        Task<List<ProductsVM>> SearchProducts(string name);


    }
}
