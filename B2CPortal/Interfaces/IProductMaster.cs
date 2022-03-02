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
        Task<IEnumerable<ProductMaster>> GetNewArrivalProducts();

        Task<ProductMaster> GetProductById(long Id, int id);
        Task<IEnumerable<ProductMaster>> GetProductByName(string name);

        Task<ProductMaster> GetDataForWishList(int id, int cityid);
        Task<BrandCategoryVM> GetSidebar();
        Task<IEnumerable<ProductsVM>> GetProductListbySidebar(SideBarVM[] filterList, string search, int nextPage, int prevPage, int cityid); //int[] filterList
        Task<IEnumerable<ProductsVM>> GetProductByIdWithRating(long Id);
        List<ProductsVM> GetProductRating(string Id);
        Task<List<ProductsVM>> SearchProducts(string name);

        Task<IEnumerable<AndroidViewModel>> AndriodProductList(SideBarVM[] filterList, string search, int nextPage, int prevPage, int cityid); //int[] filterList
        Task<BrandCategoryVM> AndroidBrandCatagory();

        //=============android===============
        Task<ProductMaster> AndroidGetProductById(int id);
        Task<AndroidProductDetails> AndroidGetProductByIdWithRating(int Id);
       // Task<AndroidProductDetails> AndroidProductCommentAndRating(AndroidRequestCommentAndRating model);


    }
}
