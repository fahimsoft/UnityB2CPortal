using API_Base.Base;
using B2C_Models.Models;
using B2CPortal.Controllers;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Services
{
    public class ProductMasterService : DALBase<ProductsVM>, IProductMaster
    {
        public async Task<IEnumerable<ProductMaster>> GetProduct()
        {
            try
            {
                //_dxcontext.Configuration.LazyLoadingEnabled = false;
                var obj = await _dxcontext.ProductMasters//.Where(x => x.IsFeatured && x.IsActive == true)
                    .Include(x => x.ProductPrices)
                   .Include(x => x.ProductDetails)
                   .AsNoTracking()
                    .OrderByDescending(a => a.Id)
                    .ToListAsync();//  GetAll();
                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<IEnumerable<ProductMaster>> GetFeaturedProduct()
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false;
                var obj = await _dxcontext.ProductMasters.Where(x=>x.IsFeatured && x.IsActive == true)
                    .Include(x => x.ProductPrices)
                    //.Include(x => x.ProductDetails)
                    .AsNoTracking()
                    .OrderByDescending(a => a.Id)
                    .ToListAsync();//  GetAll();


                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<ProductMaster> GetProductById(long Id)
        {
            try
            {
                var obj = await _dxcontext.ProductMasters
                    .Include(x => x.ProductPrices)
                    .Include(x => x.ProductDetails)
                    .Include(x => x.ProductPackSize)
                    //.Where(x=>x.Id==Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == Id);
                    //.OrderByDescending(a=>a.Id);//  GetAll();
                return obj;


            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public async Task<ProductMaster> GetProductByIdPrice(long Id)
        {
            try
            {
                    
                var obj = await _dxcontext.ProductMasters
                    .Where(x=>x.IsActive == true)
                    .Include(x => x.ProductPrices.Where(i=>i.IsActive == true))
                    .AsNoTracking().Where(x => x.Id == Id)
                    .FirstOrDefaultAsync();
                //obj.ProductPrices.FirstOrDefault(x => x.IsActive == true);
                return obj;


            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<IEnumerable<ProductMaster>> GetProductByName(string name)
        {
            try
            {
                var obj = await _dxcontext.ProductMasters
                    .Include(x => x.ProductPrices)
                    .Include(x => x.ProductDetails)
                    .Include(x => x.ProductPackSize)
                    .Where(x=>x.Name.Contains(name)).OrderByDescending(a => a.Id).AsNoTracking().ToListAsync();//  GetAll();
                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<ProductMaster> GetDataForWishList(int id)
        {
            try
            {
                var obj = await _dxcontext.ProductMasters
                    .Include(x => x.ProductPrices)
                    .Include(x => x.ProductDetails)
                    .Include(x => x.ProductPackSize)
                    .AsNoTracking()
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<BrandCategoryVM> GetSidebar()
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false;
                var obj1 = await _dxcontext.ProductBrands.Where(x => x.IsActive == true)//.Include(x => x.ProductCategory)
                                                                               .OrderByDescending(a => a.Id)
                                                                               .AsNoTracking()
                                                                               .ToListAsync();

                var obj2 = await _dxcontext.ProductCategories.Where(x => x.IsActive == true)//.Include(x => x.ProductCategory)
                                                                               .OrderByDescending(a => a.Id)
                                                                               .AsNoTracking()
                                                                               .ToListAsync();

                var cb = new BrandCategoryVM();

                cb.Brand = obj1;
                cb.Category = obj2;

                return cb;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<IEnumerable<ProductMaster>> GetProductListbySidebar(SideBarVM[] filterList) //int[] filterList
        {
            try
            {
                int[] cat = new int[filterList.Count(x => x.Name == "Category")];
                int[] brand = new int[filterList.Count(x => x.Name == "Brand")];
                int catI = 0;
                int brandI = 0;
                foreach (var item in filterList)
                {
                    switch (item.Name)
                    {
                        case "Category":
                            cat[catI] = item.ID;
                            catI++;
                            break;
                        case "Brand":
                            brand[brandI] = item.ID;
                            brandI++;
                            break;
                        default:
                            break;
                    }
                }
                var obj = new List<ProductMaster>();
                if (cat.Length > 0 && brand.Length > 0)
                {
                    obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true
                    && (cat.Contains(x.FK_ProductCategory) 
                    && brand.Contains(x.FK_ProductBrand)))
                        //.Include(x => x.ProductCategory)
                        //.Include(x => x.ProductBrand)
                        .OrderByDescending(x=>x.Id)
                        .AsNoTracking()
                        .ToListAsync();
                }
                else if (cat.Length > 0)
                {
                    obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true && 
                    cat.Contains(x.FK_ProductCategory))
                        //.Include(x => x.ProductCategory)
                        //.Include(x => x.ProductBrand)
                        .Include(x =>x.ProductPrices)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .ToListAsync();
                }
                else if (brand.Length > 0)
                {
                    obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true && 
                    brand.Contains(x.FK_ProductBrand))
                        //.Include(x => x.ProductCategory)
                        //.Include(x => x.ProductBrand)
                        .Include(x => x.ProductPrices)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .ToListAsync();
                }
                else
                {
                    obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true)
                        //.Include(x => x.ProductCategory)
                        //.Include(x => x.ProductBrand)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .ToListAsync();

                }

                return obj;

                //var obj = await _dxcontext.ProductMasters.Where(x=>x.IsActive).Include(x => x.ProductPrices).Include(x => x.ProductDetails).Include(x => x.ProductPackSize).Where(x => x.Id == brandId).OrderByDescending(a => a.Id).ToListAsync();//  GetAll();
                //var obj = await _dxcontext.ProductMasters.Where(x=>x.IsActive).Include(x => x.ProductPrices).Include(x => x.ProductDetails).Include(x => x.ProductPackSize).OrderByDescending(a => a.Id).ToListAsync();//  GetAll();


            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<IEnumerable<ProductMaster>> SearchProducts(string name)
        {
            try
            {
                var obj = await _dxcontext.ProductMasters
                    .Where(x => x.Name.Contains(name)).OrderByDescending(a => a.Id).AsNoTracking().ToListAsync();//  GetAll();
                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        //public async Task<ProductMaster> AddUser(ProductMaster obj)
        //{
        //    try
        //    {
        //        //Current = await _dxcontext.Users.FirstOrDefaultAsync(o => o.ID == obj.Id);
        //        //if (Current == null)
        //        //{
        //        //    New();

        //        //}
        //        //else
        //        //{
        //        //    PrimaryKeyValue = Current.Id;

        //        //}
        //        ////Current.Name = Current.Name;
        //        ////Current.PhoneNo = Current.PhoneNo;

        //        //Save();

        //        return Current;

        //    }
        //    catch (Exception Ex)
        //    {

        //        throw Ex;
        //    }
        //}

        //public async Task<bool> DeleteUser(long Id)
        //{
        //    try
        //    {
        //        Current = await _dxcontext.ProductMasters.FirstOrDefaultAsync(o => o.Id == Id);
        //        if (Current != null)
        //            PrimaryKeyValue = Current.Id;
        //        Delete();

        //        return true;
        //    }
        //    catch (Exception Ex)
        //    {

        //        throw Ex;
        //    }
        //}



    }
}