﻿using API_Base.Base;
using API_Base.Common;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly ICity _ICity = null;
        public ProductController(IProductMaster productMaster, ICart cart, ICity city)
        {
            _IProductMaster = productMaster;
            _cart = cart;
            _ICity = city;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [ActionName("GetProduct")]
        public async Task<JsonResult> GetProduct()
        {
            try
            {


                var obj = await _IProductMaster.GetProduct();
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        [HttpGet]
        [ActionName("GetProductwithPaggination")]
        public async Task<JsonResult> GetProductwithPaggination(int nextPage, int prevPage)
        {
            try
            {
                List<ProductsVM> productsVM = new List<ProductsVM>();

                var obj = await _IProductMaster.GetProduct();
                var totalProduct = obj.Count();
                var fillterProductList = obj.OrderByDescending(x => x.CreatedOn).Skip(prevPage).Take(nextPage).ToList();

                foreach (var item in fillterProductList)
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
                return SuccessResponse(productsVM);

            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        #region cart handling
        public async Task<JsonResult> GetCartCount()
        {
            try
            {
                //get default city for price calculation
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);
                int customerid = string.IsNullOrEmpty(userid) ? 0 : Convert.ToInt32(userid);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);


                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                List<CartViewModel> cartViewModels = new List<CartViewModel>();

                string cartguid = string.Empty;
                decimal totalprice = 0;
                cartguid = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                if (string.IsNullOrEmpty(cartguid) || cartguid == "undefined")
                {
                    cartguid = Guid.NewGuid().ToString();
                    HelperFunctions.SetCookie(HelperFunctions.cartguid, cartguid, 1);
                }
                var cartproducts = await _cart.GetCartProducts(cartguid, customerid, citymodel);
                foreach (var item in cartproducts)
                {
                    var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster, citymodel.Id);

                    //productmasetr.ProductPrices.Where(x=> x.FK_City)

                    string name = productmasetr.Name;
                    string MasterImageUrl = productmasetr.MasterImageUrl;
                    var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    var packsize = productmasetr.ProductPackSize.UOM.ToString();
                    var tax = productmasetr.ProductPrices.Select(x => x.Tax).FirstOrDefault();


                    //(D.Price / D.Tax) AS BasePrice
                    //decimal BasePrice = Math.Round((decimal)((price / tax)));
                    //(D.Price - (D.Price / D.Tax)) tax
                    // TaxAmount = Math.Round((decimal)((price - (price / tax))));
                    //(D.Price * (D.Discount / 100)) AS DiscountAmount
                    //decimal DiscountAmount = HelperFunctions.DiscountedAmount(price,discount);
                    //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                    decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                    var cartobj = new CartViewModel
                    {
                        Price = Math.Round(Convert.ToDecimal(price / conversionvalue)),
                        Id = item.Id,
                        Packsize = packsize,
                        Quantity = (int)item.Quantity,
                        Name = name,
                        MasterImageUrl = MasterImageUrl,
                        Discount = discount,
                        TotalPrice = (decimal)(DiscountedPrice * item.Quantity),//item.TotalPrice == null ? 0 : Math.Round(Convert.ToDecimal(item.TotalPrice / conversionvalue)),
                        FK_ProductMaster = item.FK_ProductMaster,
                    };
                    cartViewModels.Add(cartobj);
                    totalprice += (decimal)(DiscountedPrice * item.Quantity);
                }
                var totalquentity = cartproducts.Sum(x => x.Quantity);
                //var totalprice = cartproducts.Sum(x => x.TotalPrice);

                return SuccessResponse(new
                {
                    cartproducts = cartViewModels,
                    cartproductscount = totalquentity,
                    totalprice = totalprice
                });

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<ActionResult> GetCartList()
        {
            //get default city for price calculation
            string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
            if (string.IsNullOrEmpty(cookiecity))
            {
                HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
            }
            cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
            string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

            City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);
            string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
            decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
            List<CartViewModel> cartViewModels = new List<CartViewModel>();
            int customerid = string.IsNullOrEmpty(userid) ? 0 : Convert.ToInt32(userid);
            string cookie = string.Empty;
            decimal TaxAmount = 0;
            if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
            {
                cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
            }
            var cartproducts = await _cart.GetCartProducts(cookie, customerid, citymodel);
            foreach (var item in cartproducts)
            {
                var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster, citymodel.Id);
                string name = productmasetr.Name;
                string MasterImageUrl = productmasetr.MasterImageUrl;
                var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var tax = productmasetr.ProductPrices.Select(x => x.Tax).FirstOrDefault();
                var packsize = productmasetr.ProductPackSize.UOM.ToString();

                //(D.Price / D.Tax) AS BasePrice
                decimal BasePrice = Math.Round((decimal)((price / tax)));
                //(D.Price - (D.Price / D.Tax)) tax
                TaxAmount = Math.Round((decimal)((price - (price / tax))));
                //(D.Price * (D.Discount / 100)) AS DiscountAmount
                decimal DiscountAmount = HelperFunctions.DiscountAmount(price,discount);
                //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                var actualprice = Math.Round(((decimal)(price * item.Quantity) / conversionvalue));
               // TaxAmount = Math.Round((decimal)(((price * item.Quantity) / tax) * (tax - 1)));
                //var priceExcludingTAX = Math.Round((decimal)((price / tax) * item.Quantity));

                var cartobj = new CartViewModel
                {
                    ActualPrice = actualprice,//Math.Round(((decimal)(price * item.Quantity) / conversionvalue)),
                    Price = Math.Round(Convert.ToDecimal(price / conversionvalue)),
                    Id = item.Id,
                    FK_ProductMaster = item.FK_ProductMaster,
                    Packsize = packsize,
                    Quantity = (int)item.Quantity,
                    Name = name,
                    MasterImageUrl = MasterImageUrl,
                    Discount = discount,
                    TotalPrice = ((decimal)(DiscountedPrice * item.Quantity)),
                    DiscountAmount = (decimal)(DiscountAmount *  item.Quantity),
                    ShipingAndHostring = 0,
                    VatTax = (decimal)tax,
                    Tax = (decimal)(tax - 1) * 100,
                    TaxAmount = TaxAmount * item.Quantity,
                    CartSubTotal = (decimal)(BasePrice * item.Quantity)
                };
                cartViewModels.Add(cartobj);
                // cartViewModels.Select(c => { c.CartSubTotal += actualprice; return c; }).ToList();
                cartViewModels.Select(c => { c.CartSubTotalDiscount += (decimal)(DiscountAmount * item.Quantity); return c; }).ToList();
                cartViewModels.Select(c => { c.OrderTotal += (decimal)(DiscountedPrice * item.Quantity); return c; }).ToList();
                //cartViewModels.Select(c => { c.CartSubTotal += Math.Round((decimal)(price * item.Quantity) / conversionvalue); return c; }).ToList();
                //cartViewModels.Select(c => { c.CartSubTotalDiscount += Math.Round(((decimal)(price * item.Quantity) - (decimal)item.TotalPrice) / conversionvalue); return c; }).ToList();
                //cartViewModels.Select(c => { c.OrderTotal += Math.Round((decimal)item.TotalPrice / conversionvalue); return c; }).ToList();
            }

            //return View(cartViewModels);
            return PartialView("_CartListPartialView", cartViewModels);
        }
        [HttpGet]
        public ActionResult AddToCart()
        {
            //List<CartViewModel> cartViewModels = new List<CartViewModel>();
            //int userid = Convert.ToInt32(HttpContext.Session["UserId"]);
            //string cookie = string.Empty;
            //if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
            //{
            //    cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
            //}
            //// HttpCookie cookie = HttpContext.Request.Cookies.Get("cartguid");
            //// cartguid = cookie == null || cookie.Value == null || cookie.Value == "undefined" ? "" : cookie.Value.ToString();
            //var cartproducts = await _cart.GetCartProducts(cookie, userid);
            //foreach (var item in cartproducts)
            //{
            //    var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster);
            //    string name = productmasetr.Name;
            //    string MasterImageUrl = productmasetr.MasterImageUrl;
            //    var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
            //    var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
            //    var cartobj = new CartViewModel
            //    {
            //       // CartSubTotal = (decimal)(price * item.Quantity),
            //        //OrderTotal = HelperFunctions.TotalPrice((decimal)(price * (1 - (discount / 100)))),
            //        // DiscountedPrice = price * (1 - (discount / 100)),
            //        ActualPrice = (decimal)(price * item.Quantity),
            //        Price =  price,
            //        //Price = decimal.Parse(String.Format("{0:0.00}", price * (1 - (discount / 100)))),
            //        Id = item.Id,
            //        Quantity = (int)item.Quantity,
            //        Name = name,
            //        MasterImageUrl = MasterImageUrl,
            //        Discount = discount,
            //        TotalPrice = item.TotalPrice == null ? 0 : (decimal)item.TotalPrice,
            //        DiscountAmount = ((decimal)(price * item.Quantity) - (item.TotalPrice == null ? 0 : (decimal)item.TotalPrice)),
            //        ShipingAndHostring = 0,
            //        VatTax = 0

            //    };
            //    cartViewModels.Add(cartobj);
            //    cartViewModels.Select(c => { c.CartSubTotal += (decimal) (price * item.Quantity); return c; }).ToList();
            //    cartViewModels.Select(c => { c.CartSubTotalDiscount += ((decimal)(price * item.Quantity) - (decimal)item.TotalPrice); return c; }).ToList();
            //    cartViewModels.Select(c => { c.OrderTotal += (decimal)item.TotalPrice; return c; }).ToList();
            //}

            return View();
        }
        [HttpPost]
        [ActionName("AddToCart")]
        public async Task<JsonResult> AddToCart(int proid, int quentity = 1)
        {
            try
            {
                if (quentity > 0)
                {
                    //get location from cookie
                    string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                    string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);
                    int custoemrid = string.IsNullOrEmpty(userid) ? 0 : Convert.ToInt32(userid);

                    if (string.IsNullOrEmpty(cookiecity))
                    {
                        HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                    }
                    cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                    City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                    string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                    decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                    string cookieid = string.Empty;
                    string msg = string.Empty;
                    cookieid = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    if (string.IsNullOrEmpty(cookieid) || cookieid == "undefined")
                    {
                        cookieid = Guid.NewGuid().ToString();
                        HelperFunctions.SetCookie(HelperFunctions.cartguid, cookieid, 1);
                    }
                    var productobj = await _IProductMaster.GetProductById(proid, citymodel.Id);
                    var discount = productobj.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = productobj.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    var tax = productobj.ProductPrices.Select(x => x.Tax).FirstOrDefault();
                    //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                    decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);


                    var cart = new Cart();
                    cart.Quantity = quentity;
                    cart.Guid = cookieid;
                    cart.IsWishlist = false;
                    cart.IsActive = true;
                    cart.TotalPrice = DiscountedPrice;
                    cart.TotalQuantity = quentity;
                    cart.FK_ProductMaster = proid;
                    cart.Currency = currency;// Session["currency"].ToString().ToLower();
                    cart.ConversionRate = conversionvalue;
                    cart.FK_CityId = citymodel.Id;


                    if (custoemrid > 0)
                    {
                        cart.FK_Customer = custoemrid;
                    }
                    var obj = await _cart.CreateCart(cart);
                    var cartproducts = await _cart.GetCartProducts(cookieid, custoemrid, citymodel);
                    cartproducts.ToList().ForEach(x =>
                    {
                        x.FK_CityId = citymodel.Id;
                        _cart.UpdateCart(x);
                    });
                    var totalquentity = cartproducts.Sum(x => x.Quantity);
                    msg = obj == null ? "You Can't Add to Cart more then 10 times" : "Add to Cart Successfully !";
                    return Json(new { data = obj, msg = msg, cartproductscount = totalquentity, success = obj == null ? false : true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { data = "", msg = "Not Insert 0 Quentity in Cart", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }

        [HttpPost]
        public async Task<JsonResult> DeleteCart(long id)
        {
            string msg = string.Empty;
            var cartproducts = await _cart.DeleteCart(id);
            msg = cartproducts == true ? "Removed from Cart !" : "Error: Cart Not Removed!";
            return Json(new { data = "", msg = msg, success = cartproducts }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public async Task<JsonResult> UpdateCartList(List<int> cartquentites, List<int> cartids)
        {
            //get location from cookie
            string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
            if (string.IsNullOrEmpty(cookiecity))
            {
                HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
            }
            cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
            City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

            string msg = string.Empty;
            bool updateresult = false;
            string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
            decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
            for (int i = 0; i < cartids.Count(); i++)
            {
                var cartproducts = await _cart.GetCartById(cartids[i]);
                if (cartproducts != null)
                {
                    cartproducts.Quantity = cartquentites[i];
                    var productmasetr = await _IProductMaster.GetProductById(cartproducts.FK_ProductMaster, citymodel.Id);
                    var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    var tax = productmasetr.ProductPrices.Select(x => x.Tax).FirstOrDefault();
                    //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                    decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);

                    cartproducts.TotalPrice = Math.Round(Convert.ToDecimal(DiscountedPrice * cartproducts.Quantity) / conversionvalue);
                    updateresult = await _cart.UpdateCart(cartproducts);
                }
            }
            msg = updateresult == true ? "Carts Updated Successfully !" : "Error: Carts Not Updated!";
            return Json(new { data = "", msg = msg, success = false }, JsonRequestBehavior.AllowGet);

        }
        #endregion
        [HttpGet]
        [ActionName("GetProductbyIdWithRating")]
        ///  [OutputCache(CacheProfile = "SetCache", VaryByParam = "Id")]
        public async Task<JsonResult> GetProductbyIdWithRating(long Id)
        {
            try
            {
                var obj = await _IProductMaster.GetProductByIdWithRating(Id);
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {



                return BadResponse(Ex);
            }
        } //Updated 1-Dec
        [HttpPost]
        [ActionName("GetProductListbySidebar")]
        // [OutputCache(CacheProfile = "SetCache", VaryByParam = "*")]
        public async Task<JsonResult> GetProductListbySidebar(SideBarVM[] filterList, string search = "", int nextPage = 10, int prevPage = 0) //int[] filterList
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);
                var filter = await _IProductMaster.GetProductListbySidebar(filterList, search, nextPage, prevPage, citymodel.Id);
                return SuccessResponse(filter);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        [HttpGet]
        [ActionName("GetFeaturedProduct")]
        // [OutputCache(CacheProfile = "SetCache")]
        public async Task<JsonResult> GetFeaturedProduct()
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                var obj = await _IProductMaster.GetFeaturedProduct();
                List<ProductsVM> productsVM = new List<ProductsVM>();
                foreach (var item in obj)
                {
                    string MasterImageUrl = item.MasterImageUrl;
                    var discount = item.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Price).FirstOrDefault();
                    var UOM = item.ProductPackSize.UOM;
                    var tax = item.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Tax).FirstOrDefault();
                    //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                    decimal DiscountedPrice  =  HelperFunctions.DiscountedPrice(price, discount, tax);
                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = String.Format("{0} {1}", item.Name,UOM),
                        Price = Math.Round(Convert.ToDecimal(price / conversionvalue)),
                        DiscountedAmount = DiscountedPrice,
                        Discount = discount,
                        MasterImageUrl = item.MasterImageUrl,
                        ImageUrl = MasterImageUrl,
                        ShortDescription = item.ShortDescription,
                        LongDescription = item.LongDescription,

                    };
                    productsVM.Add(producVMList);
                }

                return SuccessResponse(productsVM);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        [HttpGet]
        [ActionName("LoadNewArrivalProducts")]
        // [OutputCache(CacheProfile = "SetCache")]
        public async Task<JsonResult> LoadNewArrivalProducts()
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                var obj = await _IProductMaster.GetNewArrivalProducts();
                List<ProductsVM> productsVM = new List<ProductsVM>();
                foreach (var item in obj)
                {
                    string MasterImageUrl = item.MasterImageUrl;
                    var discount = item.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Price).FirstOrDefault();
                    var tax = item.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Tax).FirstOrDefault();
                    var UOM = item.ProductPackSize.UOM;

                    //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                    decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = String.Format( "{0} {1}",item.Name ,UOM) ,
                        Price = Math.Round(Convert.ToDecimal(price / conversionvalue)),
                        DiscountedAmount = DiscountedPrice,
                        Discount = discount,
                        MasterImageUrl = item.MasterImageUrl,
                        ImageUrl = MasterImageUrl,
                        ShortDescription = item.ShortDescription,
                        LongDescription = item.LongDescription,

                    };
                    productsVM.Add(producVMList);
                }

                return SuccessResponse(productsVM);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        public ActionResult ProductList()
        {
            string CurrentURL = Request.Url.AbsoluteUri;
            TempData["returnurl"] = CurrentURL;
            return View();
        }
        [HttpGet]
        [ActionName("GetProductbyId")]
        public async Task<JsonResult> GetProductbyId(long Id)
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                var obj = await _IProductMaster.GetProductById(Id, citymodel.Id);
                //string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                //decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                List<ProductsVM> productsVM = new List<ProductsVM>();
                var producVMList = new ProductsVM
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    Price = obj.ProductPrices.Select(x => x.Price).FirstOrDefault(),
                    Discount = obj.ProductPrices.Select(x => x.Discount).FirstOrDefault(),
                    MasterImageUrl = obj.MasterImageUrl,
                    ImageUrl = obj.ProductDetails.Select(x => x.ImageUrl).FirstOrDefault(),
                    ShortDescription = obj.ShortDescription,
                    LongDescription = obj.LongDescription,

                };
                productsVM.Add(producVMList);

                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {



                return BadResponse(Ex);
            }
        }
        [HttpPost]
        public async Task<JsonResult> SearchProductList(string Prefix)
        {
            var productmasetr = await _IProductMaster.SearchProducts(Prefix);

            return Json(productmasetr.DistinctBy(x => x.Id).Take(8), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [ActionName("GetProductbyName")]
        public async Task<JsonResult> GetProductbyName(string productName, int nextPage, int prevPage) //updated
        {
            try
            {

                //var obj = await _IProductMaster.GetProductById(Id);
                // var obj = await _IProductMaster.GetProductByName(productName);
                //return SuccessResponse(obj);
                //  if (prevPage== 0)
                //  obj.OrderByDescending(x => x.CreatedOn).Skip(0).Take(10).ToList();
                // else
                //   obj.OrderByDescending(x => x.CreatedOn).Skip(nextPage).Take(prevPage).ToList();

                // return SuccessResponse(obj);


                List<ProductsVM> productsVM = new List<ProductsVM>();
                var obj = await _IProductMaster.GetProductByName(productName);
                var totalProduct = obj.Count();


                if (prevPage == 0)
                    obj.OrderByDescending(x => x.CreatedOn).Skip(0).Take(10).ToList();
                else
                    obj.OrderByDescending(x => x.CreatedOn).Skip(nextPage).Take(prevPage).ToList();

                // var fillterProductList = obj.OrderByDescending(x => x.CreatedOn).Skip(prevPage).Take(nextPage).ToList();

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

                return SuccessResponse(productsVM);

            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateWishList(List<int> wishlistids, List<int> wishlistquentites)
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                string msg = string.Empty;
                bool updateresult = false;
                for (int i = 0; i < wishlistids.Count(); i++)
                {
                    var wishlistProducts = await _cart.GetWishlistById(wishlistids[i]);
                    if (wishlistProducts != null)
                    {
                        wishlistProducts.Quantity = wishlistquentites[i];
                        wishlistProducts.TotalQuantity = wishlistquentites[i]; var productmasetr = await _IProductMaster.GetProductById(wishlistProducts.FK_ProductMaster, citymodel.Id);
                        var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                        var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault(); wishlistProducts.Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString();
                        var tax = productmasetr.ProductPrices.Select(x => x.Tax).FirstOrDefault();

                        //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                        decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                        decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                        if (wishlistProducts.Currency == "PKR")
                        {
                            wishlistProducts.TotalPrice = DiscountedPrice * wishlistProducts.Quantity;
                        }
                        else
                        {
                            var totalUsd = DiscountedPrice * wishlistProducts.Quantity;
                            wishlistProducts.TotalPrice = Math.Round((decimal)(totalUsd / Convert.ToDecimal(conversionvalue)));
                        }
                        updateresult = await _cart.UpdateWishlistQuantity(wishlistProducts);
                    }
                }
                msg = updateresult == true ? "Wishlist Updated Successfully !" : "Error: Wishlist Not Updated!";
                return Json(new { data = "", msg = msg, success = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        [HttpPost]
        public async Task<JsonResult> AddWishlistProduct(int id, int quentity = 1)
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);
                string msg = string.Empty;
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                string cookie = string.Empty;
                if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
                {
                    cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                }
                else
                {
                    cookie = Guid.NewGuid().ToString();
                    HelperFunctions.SetCookie(HelperFunctions.cartguid, cookie, 1);
                }
                Cart cart = new Cart(); var res = await _IProductMaster.GetDataForWishList(id, citymodel.Id);
                var price = res.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var discount = res.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                var tax = res.ProductPrices.Select(x => x.Tax).FirstOrDefault();
                //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                var customerId = 0;
                if (!string.IsNullOrEmpty(userid))
                {
                    customerId = Convert.ToInt32(userid);
                }
                cart.Guid = cookie;
                if (customerId > 0)
                {
                    cart.FK_Customer = customerId;
                }
                cart.FK_ProductMaster = res.Id;
                cart.IsWishlist = true;
                cart.IsActive = true;
                cart.Quantity = quentity;
                cart.TotalQuantity = quentity;
                cart.Currency = currency;
                cart.ConversionRate = conversionvalue;
                cart.FK_CityId = citymodel.Id;

                if (cart.Currency.ToLower() == "pkr")
                {
                    cart.TotalPrice = DiscountedPrice;
                }
                else
                {
                    cart.TotalPrice = Math.Round(DiscountedPrice / Convert.ToDecimal(conversionvalue));
                }
                var obj = await _cart.CreateWishList(cart);

                var cartproducts = await _cart.GetWishListProducts(cookie, customerId);
                var totalquentity = cartproducts.Sum(x => x.Quantity);
                msg = obj == null ? "You Can't Add to WishList more then 10 times" : "Added To WishList!";
                return Json(new { data = obj, msg = msg, cartproductscount = totalquentity, success = obj == null ? false : true }, JsonRequestBehavior.AllowGet);

                // return Json(new { data = response, msg = "Added To WishList", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        public async Task<ActionResult> WishlistTable()
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);
                decimal TaxAmount = 0;

                var customerId = 0;
                if (!string.IsNullOrEmpty(userid))
                {
                    customerId = Convert.ToInt32(userid);
                }
                List<WishlistVM> wishlistVMs = new List<WishlistVM>();
                HttpCookie cookie = HttpContext.Request.Cookies.Get("cartguid");
                if (cookie != null || customerId > 0)
                {
                    string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                    decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                    Cart cart = new Cart();
                    var ProductIds = await _cart.GetWishListProducts(cookie.Value, customerId);
                    foreach (var item in ProductIds)
                    {
                        var productmaster = await _IProductMaster.GetProductById(item.FK_ProductMaster, citymodel.Id);
                        var Name = productmaster.Name;
                        var mainImg = productmaster.MasterImageUrl;
                        //var priceobj = productmaster.ProductPrices.Select(x => x.)
                        var price = productmaster.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Price).FirstOrDefault();
                        var discount = productmaster.ProductPrices.Where(x => x.FK_City == citymodel.Id).Select(x => x.Discount).FirstOrDefault();
                        var tax = productmaster.ProductPrices.Select(x => x.Tax).FirstOrDefault();

                        //(D.Price / D.Tax) AS BasePrice
                        decimal BasePrice = Math.Round((decimal)((price / tax)));
                        //(D.Price - (D.Price / D.Tax)) tax
                        TaxAmount = Math.Round((decimal)((price - (price / tax))));
                        //(D.Price * (D.Discount / 100)) AS DiscountAmount
                        decimal DiscountAmount =  HelperFunctions.DiscountAmount(price,discount);
                        //(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                        decimal DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                        //TaxAmount = Math.Round((decimal)(((price * item.Quantity) / tax) * (tax - 1)));
                        //var priceExcludingTAX = Math.Round((decimal)((price / tax) * item.Quantity));

                        var Total = item.TotalPrice;
                        var Quantity = item.TotalQuantity;
                        var CartId = item.Id;
                        var productId = item.FK_ProductMaster;

                        cart.Currency = currency;
                        cart.ConversionRate = conversionvalue;
                        if (cart.Currency.ToLower() == "pkr")
                        {
                            var wishlistVM = new WishlistVM
                            {
                                Id = CartId,
                                FK_ProductMaster = productId,
                                Name = Name,
                                MasterImageUrl = mainImg,
                                Price = price,
                                Discount = discount,
                                DiscountedPrice = DiscountedPrice * item.Quantity,
                                ActualPrice = Math.Round((decimal)(price * Quantity)),
                                TotalPrice = Math.Round((decimal)(DiscountedPrice * Quantity)),
                                DiscountAmount = Math.Round(((decimal)(price * item.Quantity) - (item.TotalPrice == null ? 0 : (decimal)item.TotalPrice))),
                                //DiscountAmount = ((decimal)(price * item.Quantity) - (item.TotalPrice == null ? 0 : (decimal)item.TotalPrice)),
                                ShipingAndHostring = 0,
                                TotalQuantity = Quantity,
                                VatTax = (decimal)tax,
                                Tax = (decimal)(tax - 1) * 100,
                                TaxAmount = TaxAmount,
                                CartSubTotal = (decimal)(BasePrice * item.Quantity)

                            };
                            wishlistVMs.Add(wishlistVM);
                            //wishlistVMs.Select(c => { c.CartSubTotal += (decimal)(price * item.Quantity); return c; }).ToList();
                            wishlistVMs.Select(c => { c.CartSubTotalDiscount += ((decimal)(price * item.Quantity) - (decimal)item.TotalPrice); return c; }).ToList();
                            wishlistVMs.Select(c => { c.OrderTotal += (decimal)item.TotalPrice; return c; }).ToList();

                        }
                        else
                        {
                            // var usdRate = HelperFunctions.GetConvertedCurrencyAmount(HelperFunctions.from, HelperFunctions.to);
                            var wishlistVM = new WishlistVM
                            {
                                Id = CartId,
                                FK_ProductMaster = productId,
                                Name = Name,
                                MasterImageUrl = mainImg,
                                Price = Math.Round((decimal)price / Convert.ToDecimal(conversionvalue)),
                                Discount = discount,
                                DiscountedPrice = Math.Round(DiscountedPrice / Convert.ToDecimal(conversionvalue)),
                                ActualPrice = Math.Round((decimal)((price / Convert.ToDecimal(conversionvalue)) * Quantity)),
                                TotalPrice = Math.Round((decimal)((DiscountedPrice / Convert.ToDecimal(conversionvalue)) * Quantity)),
                                DiscountAmount = Math.Round(((decimal)((price / Convert.ToDecimal(conversionvalue)) * item.Quantity) - (item.TotalPrice == null ? 0 : (decimal)(item.TotalPrice)))),
                                ShipingAndHostring = 0,
                                VatTax = (decimal)(tax - 1) * 100,
                                TotalQuantity = Quantity,
                                Tax = (decimal)tax,
                                TaxAmount = TaxAmount

                            };
                            wishlistVMs.Add(wishlistVM);
                            wishlistVMs.Select(c => { c.CartSubTotal += Math.Round((decimal)((price / Convert.ToDecimal(conversionvalue)) * item.Quantity)); return c; }).ToList();
                            wishlistVMs.Select(c => { c.CartSubTotalDiscount += Math.Round(((decimal)((price / Convert.ToDecimal(conversionvalue)) * item.Quantity) - (decimal)(item.TotalPrice))); return c; }).ToList();
                            wishlistVMs.Select(c => { c.OrderTotal += (decimal)(item.TotalPrice); return c; }).ToList();

                        }
                    }
                }
                return PartialView(wishlistVMs);
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        public ActionResult Wishlist()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateToCart(WishlistVM obj)
        {
            try
            {
                var cartId = obj.Id;
                var productId = obj.FK_ProductMaster;
                var customerId = 0;
                decimal DiscountedPrice = 0;
                var Quantity = obj.Quantity;
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                //var usdRate = HelperFunctions.GetConvertedCurrencyAmount(HelperFunctions.from, HelperFunctions.to);
                if (!string.IsNullOrEmpty(userid))
                {
                    customerId = Convert.ToInt32(userid);
                }
                HttpCookie cookie = HttpContext.Request.Cookies.Get("cartguid"); if (cookie != null || customerId > 0)
                {
                    var cartData = await _cart.GetCartData(cookie.Value, customerId, productId);
                    if (cartData != null)
                    {
                        try
                        {
                            if (customerId > 0)
                                cartData.FK_Customer = customerId;
                            cartData.Guid = cookie.Value;
                            //get location from cookie
                            string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                            if (string.IsNullOrEmpty(cookiecity))
                            {
                                HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                            }
                            cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                            City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);

                            var productData = await _IProductMaster.GetProductById(productId, citymodel.Id); if (cartData.Currency == "PKR")
                            {
                                if (productData != null)
                                {
                                    var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                    var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                    var tax = productData.ProductPrices.Select(x => x.Tax).FirstOrDefault();

                                    ////(D.Price / D.Tax) AS BasePrice
                                    //decimal BasePrice = Math.Round((decimal)((price / tax)));
                                    ////(D.Price - (D.Price / D.Tax)) tax
                                    //TaxAmount = Math.Round((decimal)((price - (price / tax))));
                                    ////(D.Price * (D.Discount / 100)) AS DiscountAmount
                                    //decimal DiscountAmount = HelperFunctions.DiscountedAmount(price,discount);
                                    ////(D.Price / D.Tax) + (D.Price - (D.Price / D.Tax)) + (D.Price * (D.Discount / 100)) AS DiscountedPrice
                                     DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax);
                                }
                                cartData.TotalPrice = DiscountedPrice;
                                cartData.Quantity = (cartData.Quantity + Quantity);
                                cartData.TotalQuantity = (cartData.TotalQuantity + Quantity); var updated = await _cart.UpdateToCart(cartData);
                                var res = await _cart.UpdateWishList(cartId);
                                return Json(new { data = updated, msg = "Cart Updated", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                if (productData != null)
                                {
                                    var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                    var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                    var tax = productData.ProductPrices.Select(x => x.Tax).FirstOrDefault();

                                    DiscountedPrice = HelperFunctions.DiscountedPrice(price, discount, tax); 
                                }
                                cartData.TotalPrice = Math.Round(DiscountedPrice / Convert.ToDecimal(conversionvalue));
                                cartData.Quantity = (cartData.Quantity + Quantity);
                                cartData.TotalQuantity = (cartData.TotalQuantity + Quantity); var updated = await _cart.UpdateToCart(cartData);
                                var res = await _cart.UpdateWishList(cartId);
                                return Json(new { data = updated, msg = "Cart Updated", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                    {
                        var cart = new Cart();
                        cart.Quantity = obj.Quantity;
                        cart.TotalQuantity = obj.TotalQuantity;
                        cart.TotalPrice = obj.TotalPrice;
                        cart.IsWishlist = false;
                        cart.IsActive = true;
                        cart.FK_ProductMaster = obj.FK_ProductMaster;
                        if (customerId > 0)
                        {
                            cart.FK_Customer = customerId;
                        }
                        if (cookie != null)
                        {
                            cart.Guid = cookie.Value;
                        }
                        else
                        {
                            cart.Guid = Guid.NewGuid().ToString();
                        }
                        var res = await _cart.UpdateCartFromWishList(cart);
                        return Json(new { data = res, msg = "Added To Cart", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { data = "", msg = "Something went wrong", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteFromCart(int Id)
        {
            try
            {
                var response = await _cart.DeleteFromCart(Id);
                return Json(new { data = response, msg = "Deleted", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json(new { data = ex, msg = "Failed", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);



            }



        }
        [HttpGet]
        [ActionName("GetSidebar")]
        public async Task<JsonResult> GetSidebar()
        {
            try
            {
                var obj = await _IProductMaster.GetSidebar();
                return SuccessResponse(obj);


            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
    }
}
