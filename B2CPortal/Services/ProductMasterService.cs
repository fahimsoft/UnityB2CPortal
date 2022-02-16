using API_Base.Base;
using API_Base.Common;
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
                var obj = await _dxcontext.ProductMasters.Where(x => x.IsFeatured && x.IsActive == true)
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
        public async Task<IEnumerable<ProductMaster>> GetNewArrivalProducts()
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false;
                var obj = await _dxcontext.ProductMasters.Where(x => x.IsActive == true && x.IsNewArrival == true)
                    .Include(x => x.ProductPrices)
                    .AsNoTracking()
                    .OrderByDescending(a => a.Id)
                    .ToListAsync();

                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public async Task<ProductMaster> GetProductById(long Id, int cityid)
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
                obj.ProductPrices = obj.ProductPrices.Where(y => y.FK_City == cityid).ToList();
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
                    .Where(x => x.IsActive == true)
                    .Include(x => x.ProductPrices.Where(i => i.IsActive == true))
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
                    .Where(x => x.Name.Contains(name)).OrderByDescending(a => a.Id).AsNoTracking().ToListAsync();//  GetAll();
                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public async Task<ProductMaster> GetDataForWishList(int id, int cityid)
        {
            try
            {
                var obj = await _dxcontext.ProductMasters
                    .Include(x => x.ProductPrices)
                    .Include(x => x.ProductDetails)
                    .Include(x => x.ProductPackSize)
                    .AsNoTracking()
                    .Where(x => x.Id == id).FirstOrDefaultAsync();
                obj.ProductPrices = obj.ProductPrices.Where(x => x.FK_City == cityid).ToList();
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
        public async Task<List<ProductsVM>> SearchProducts(string name)
        {
            try
            {

                var obj = from PM in _dxcontext.ProductMasters
                          join PP in _dxcontext.ProductPrices on PM.Id equals PP.FK_ProductMaster
                          join PD in _dxcontext.ProductDetails on PM.Id equals PD.FK_ProductMaster
                          where PM.Name.Contains(name)
                          select new { PM.Id, PM.Name, PM.ShortDescription, PM.LongDescription, PM.MasterImageUrl, PP.Price, PD.ImageUrl };
                var obj2 = await obj.ToListAsync().ConfigureAwait(false);
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
        public async Task<IEnumerable<ProductsVM>> GetProductByIdWithRating(long Id) //updated 30 Nov 5.43 pm
        {
            try
            {
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                var productRating = _dxcontext.Database.SqlQuery<ProductsVM>("exec GetProductRating " + Id + "").ToList<ProductsVM>();

                var obj = await _dxcontext.ProductMasters
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductDetails)
                .Include(x => x.ProductPackSize)
                .AsNoTracking().Where(x => x.Id == Id).ToListAsync();

                List<ProductsVM> productsVM = new List<ProductsVM>();
                foreach (var item in obj)
                {
                    var discount = item.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    var discountedprice = Math.Round(Convert.ToDecimal((price * (1 - (discount / 100))) / conversionvalue), 2);
                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = Math.Round(Convert.ToDecimal(price) / conversionvalue, 2),
                        DiscountedAmount = discountedprice,
                        Discount = discount,
                        MasterImageUrl = item.MasterImageUrl,
                        ImageUrl = item.ProductDetails.Select(x => x.ImageUrl).FirstOrDefault(),
                        ShortDescription = item.ShortDescription,
                        LongDescription = item.LongDescription,
                        UOM = item.ProductPackSize.UOM,
                        TotalRating = productRating.Select(x => x.TotalRating).FirstOrDefault(),
                        AvgRating = productRating.Select(x => x.AvgRating).FirstOrDefault(),
                        ImageUrl2 = item.ProductDetails.Select(x => x.ImageUrl).ToList(),

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
        public async Task<IEnumerable<ProductsVM>> GetProductListbySidebar(SideBarVM[] filterList, string search, int nextPage, int prevPage, int cityid) //int[] filterList
        {
            try
            {
                var totalProduct = 0;
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                if (filterList == null)
                {
                    filterList = new SideBarVM[] { };
                }

                int[] cat = new int[filterList.Count(x => x.Name == "Category")];
                int[] brand = new int[filterList.Count(x => x.Name == "Brand")];
                int[] packSize = new int[filterList.Count(x => x.Name == "PackSize")];
                int catI = 0;
                int brandI = 0;
                int packSizeI = 0;

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
                        case "PackSize":
                            packSize[packSizeI] = item.ID;
                            packSizeI++;
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                    .Include(x => x.ProductPackSize)
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
                    .Include(x => x.ProductPackSize)
                    .OrderByDescending(x => x.Id)
                    .AsNoTracking()
                    .Skip(prevPage).Take(nextPage)
                    .ToListAsync();



                    totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true);
                }



                List<ProductsVM> productsVM = new List<ProductsVM>();
                var ids = obj.Select(x => x.Id).ToArray();
                var objRating = _dxcontext.CommentAndRatings
                .GroupBy(w => new
                {
                    Id = w.FK_ProductMaster,
                })
                .Select(s => new ProductsCommentVM()
                {
                    Id = s.Key.Id,
                    AvgRating = s.Sum(x => (decimal)x.Rate) / s.Count(),
                    TotalRatingCount = s.Count()
                }).AsNoTracking().ToList();
                foreach (var item in obj)
                {
                    var discount = item.ProductPrices.Where(c => c.FK_City == cityid).Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Where(c => c.FK_City == cityid).Select(x => x.Price).FirstOrDefault();
                    var discountedprice = Math.Round(Convert.ToDecimal((price * (1 - (discount / 100))) / conversionvalue), 2);
                    //var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);


                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = item.Name == null ? "" : item.Name,
                        Price = Math.Round(Convert.ToDecimal(price) / conversionvalue, 2),
                        Discount = discount,
                        DiscountedAmount = discountedprice,
                        MasterImageUrl = item.MasterImageUrl,
                        ImageUrl = item.ProductDetails.Select(x => x.ImageUrl).FirstOrDefault(),
                        ShortDescription = item.ShortDescription == null ? "" : item.ShortDescription,
                        LongDescription = item.LongDescription == null ? "" : item.LongDescription,
                        totalProduct = totalProduct,
                        UOM = item.ProductPackSize.UOM == null ? "" : item.ProductPackSize.UOM,
                        UnitInNumeric = item.ProductPackSize.UnitInNumeric,
                        TotalRating = objRating.FirstOrDefault(x => x.Id == item.Id) == null ? 0 : objRating.FirstOrDefault(x => x.Id == item.Id).TotalRatingCount,
                        AvgRating = objRating.FirstOrDefault(x => x.Id == item.Id) == null ? 0 : objRating.FirstOrDefault(x => x.Id == item.Id).AvgRating
                    };
                    productsVM.Add(producVMList);
                }

                if (packSize.Length > 1)
                {
                    productsVM.Where(x => x.UnitInNumeric >= packSize[0] && x.UnitInNumeric <= packSize[1]).ToList();
                }
                return productsVM;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public List<ProductsVM> GetProductRating(string id)
        {
            var productRating = _dxcontext.Database.SqlQuery<ProductsVM>("exec GetProductRating " + id + "").ToList<ProductsVM>();
            return productRating;
        }
        //=====================================android===================================
        public async Task<IEnumerable<AndroidViewModel>> AndriodProductList(SideBarVM[] filterList, string search, int nextPage, int prevPage, int cityid) //int[] filterList
        {
            try
            {
                var totalProduct = 0;
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                if (filterList == null)
                {
                    filterList = new SideBarVM[] { };
                }

                int[] cat = new int[filterList.Count(x => x.Name == "Category")];
                int[] brand = new int[filterList.Count(x => x.Name == "Brand")];
                int[] packSize = new int[filterList.Count(x => x.Name == "PackSize")];
                int catI = 0;
                int brandI = 0;
                int packSizeI = 0;

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
                        case "PackSize":
                            packSize[packSizeI] = item.ID;
                            packSizeI++;
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                        .Include(x => x.ProductPackSize)
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
                    .Include(x => x.ProductPackSize)
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
                    .Include(x => x.ProductPackSize)
                    .OrderByDescending(x => x.Id)
                    .AsNoTracking()
                    //.Skip(prevPage).Take(nextPage)
                    .ToListAsync();



                    totalProduct = _dxcontext.ProductMasters.Count(x => x.IsActive == true);
                }



                List<AndroidViewModel> productsVM = new List<AndroidViewModel>();
                var ids = obj.Select(x => x.Id).ToArray();
                var objRating = _dxcontext.CommentAndRatings
                .GroupBy(w => new
                {
                    Id = w.FK_ProductMaster,
                })
                .Select(s => new ProductsCommentVM()
                {
                    Id = s.Key.Id,
                    AvgRating = s.Sum(x => (decimal)x.Rate) / s.Count(),
                    TotalRatingCount = s.Count()
                }).AsNoTracking().ToList();
                foreach (var item in obj)
                {
                    var discount = item.ProductPrices.Where(c => c.FK_City == cityid).Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Where(c => c.FK_City == cityid).Select(x => x.Price).FirstOrDefault();
                    var discountedprice = Math.Round(Convert.ToDecimal((price * (1 - (discount / 100))) / conversionvalue), 2);
                    //var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);


                    var producVMList = new AndroidViewModel
                    {
                        Id = item.Id,
                        Name = item.Name == null ? "" : item.Name,
                        Price = Math.Round(Convert.ToDecimal(price) / conversionvalue, 2),
                        Discount = discount,
                        //DiscountedAmount = discountedprice,
                        MasterImageUrl = item.MasterImageUrl,
                        ImageUrl = item.ProductDetails.Select(x => x.ImageUrl).FirstOrDefault(),
                        ShortDescription = item.ShortDescription == null ? "" : item.ShortDescription,
                        LongDescription = item.LongDescription == null ? "" : item.LongDescription,
                        //totalProduct = totalProduct,
                        UOM = item.ProductPackSize.UOM == null ? "" : item.ProductPackSize.UOM,
                        UnitInNumeric = item.ProductPackSize.UnitInNumeric,
                        TotalRating = objRating.FirstOrDefault(x => x.Id == item.Id) == null ? 0 : objRating.FirstOrDefault(x => x.Id == item.Id).TotalRatingCount,
                        AvgRating = objRating.FirstOrDefault(x => x.Id == item.Id) == null ? 0 : objRating.FirstOrDefault(x => x.Id == item.Id).AvgRating,
                        IsFeatured = item.IsFeatured,
                        IsNewArrival = item.IsNewArrival
                    };
                    productsVM.Add(producVMList);
                }

                if (packSize.Length > 1)
                {
                    productsVM.Where(x => x.UnitInNumeric >= packSize[0] && x.UnitInNumeric <= packSize[1]).ToList();
                }
                return productsVM;
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        public async Task<BrandCategoryVM> AndroidBrandCatagory()
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
    }
}