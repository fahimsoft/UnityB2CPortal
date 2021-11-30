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
        public async Task<IEnumerable<ProductsVM>> GetProductListbySidebar(SideBarVM[] filterList, string search, int nextPage, int prevPage) //int[] filterList
        {
            try
            {
                var totalProduct = 0;

                if (filterList == null)
                {
                    filterList = new SideBarVM[] { };
                }

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
                    if (search.Length > 0)
                    {
                        obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true
                        && (cat.Contains(x.FK_ProductCategory)
                        && brand.Contains(x.FK_ProductBrand))
                        && x.Name.Contains(search))
                            .Include(x => x.ProductPrices)
                            .OrderByDescending(x => x.Id)
                            .AsNoTracking()
                            .Skip(prevPage).Take(nextPage)
                            .ToListAsync();

                        totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true
                            && (cat.Contains(x.FK_ProductCategory)
                            && brand.Contains(x.FK_ProductBrand))
                            && x.Name.Contains(search)
                        );
                    }
                    else
                    {
                        obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true
                        && (cat.Contains(x.FK_ProductCategory)
                        && brand.Contains(x.FK_ProductBrand)))
                            .Include(x => x.ProductPrices)
                            .OrderByDescending(x => x.Id)
                            .AsNoTracking()
                            .Skip(prevPage).Take(nextPage)
                            .ToListAsync();

                        totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true
                            && (cat.Contains(x.FK_ProductCategory)
                            && brand.Contains(x.FK_ProductBrand))
                        );
                    }
                }
                else if (cat.Length > 0)
                {
                    if (search.Length > 0)
                    {
                        obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true &&
                        cat.Contains(x.FK_ProductCategory)
                        && x.Name.Contains(search))

                            .Include(x => x.ProductPrices)
                            .OrderByDescending(x => x.Id)
                            .AsNoTracking()
                            .Skip(prevPage).Take(nextPage)
                            .ToListAsync();

                        totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true
                            && cat.Contains(x.FK_ProductCategory)
                            && x.Name.Contains(search)
                        );
                    }
                    else
                    {
                        obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true &&
                        cat.Contains(x.FK_ProductCategory))

                            .Include(x => x.ProductPrices)
                            .OrderByDescending(x => x.Id)
                            .AsNoTracking()
                            .Skip(prevPage).Take(nextPage)
                            .ToListAsync();
                        //wrong count
                        totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true &&
                        cat.Contains(x.FK_ProductCategory));
                    }
                }
                else if (brand.Length > 0)
                {
                    if (search.Length > 0)
                    {
                        obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true &&
                        brand.Contains(x.FK_ProductBrand)
                        && x.Name.Contains(search))

                            .Include(x => x.ProductPrices)
                            .OrderByDescending(x => x.Id)
                            .AsNoTracking()
                            .Skip(prevPage).Take(nextPage)
                            .ToListAsync();

                        totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true &&
                        brand.Contains(x.FK_ProductBrand)
                        && x.Name.Contains(search));
                    }
                    else
                    {
                        obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true &&
                    brand.Contains(x.FK_ProductBrand))

                        .Include(x => x.ProductPrices)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .Skip(prevPage).Take(nextPage)
                        .ToListAsync();

                        totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true &&
                    brand.Contains(x.FK_ProductBrand));
                    }
                }
                else if (search.Length > 0)
                {
                    obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true
                    && x.Name.Contains(search))

                        .Include(x => x.ProductPrices)
                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .Skip(prevPage).Take(nextPage)
                        .ToListAsync();

                    totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true &&
                    x.Name.Contains(search));
                }
                else
                {
                    obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true)
                        .Include(x => x.ProductPrices)

                        .OrderByDescending(x => x.Id)
                        .AsNoTracking()
                        .Skip(prevPage).Take(nextPage)
                        .ToListAsync();

                    totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true);
                }

                List<ProductsVM> productsVM = new List<ProductsVM>();



                foreach (var item in obj)
                {
                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = item.ProductPrices.Select(x => x.Price).FirstOrDefault(),
                        Discount = item.ProductPrices.Select(x => x.Discount).FirstOrDefault(),
                        MasterImageUrl = item.MasterImageUrl,
                        ImageUrl = item.ProductDetails.Select(x => x.ImageUrl).FirstOrDefault(),
                        ShortDescription = item.ShortDescription,
                        LongDescription = item.LongDescription,
                        totalProduct = totalProduct

                    };
                    productsVM.Add(producVMList);
                }


                return productsVM;


            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<List<ProductsVM>> SearchProducts(string name)
        {
            try
            {

                var obj = from PM in _dxcontext.ProductMasters
                          join PP in _dxcontext.ProductPrices on PM.Id equals PP.FK_ProductMaster
                          join PD in _dxcontext.ProductDetails on PM.Id equals PD.FK_ProductMaster
                          where PM.Name.Contains(name)
                          select new { PM.Id, PM.Name, PM.ShortDescription, PM.LongDescription, PM.MasterImageUrl, PP.Price, PD.ImageUrl };
                var obj2 = await obj. ToListAsync().ConfigureAwait(false);
                var dd = await obj.Select(x => new ProductsVM()

                {

                    Id = x.Id,

                    Name = x.Name,

                    Price = x.Price,

                    MasterImageUrl = x.MasterImageUrl,

                    ShortDescription = x.ShortDescription,

                    LongDescription = x.LongDescription

                }).ToListAsync();

                return dd;
                //var obj = await _dxcontext.ProductMasters.Where(x => x.Name.Contains(name)).Include(x => x.ProductPrices).AsNoTracking().ToListAsync();//  GetAll();
                //return obj;                                                                                                         // return obj;
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