using API_Base.Base;
using API_Base.Common;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace B2CPortal.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        public ProductController(IProductMaster productMaster, ICart cart)
        {
            _IProductMaster = productMaster;
            _cart = cart;
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
        [ActionName("GetProductwithPaggination")] //updated
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

            GetCountryByIP(Request);
            Session["currency"] = HelperFunctions.GetCookie(HelperFunctions.pricesymbol);
            if (Session["currency"].ToString().ToLower() == "pkr")
            {
                Session["ConversionRate"] = "1";
            }
            else
            if (Session["currency"].ToString().ToLower() != "pkr" && Session["ConversionRate"] == null)
            {
                var conversionrate = HelperFunctions.GetConvertedCurrencyAmount("USD", "PKR");
                Session["ConversionRate"] = conversionrate;
            }
            decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
            string cartguid = string.Empty;
            List<CartViewModel> cartViewModels = new List<CartViewModel>();
            int userid = Convert.ToInt32(HttpContext.Session["UserId"]);
            string cookie = string.Empty;
            decimal totalprice = 0;
            if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
            {
                cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
            }
            var cartproducts = await _cart.GetCartProducts(cookie, userid);
            foreach (var item in cartproducts)
            {
                var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                string name = productmasetr.Name;
                string MasterImageUrl = productmasetr.MasterImageUrl;
                var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var packsize = productmasetr.ProductPackSize.UOM.ToString();// Select(x => x.).FirstOrDefault();
                var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity/ conversionvalue) - discountedprice), 2);
                var cartobj = new CartViewModel
                {
                    Price = Math.Round(Convert.ToDecimal(price / conversionvalue), 2),
                    Id = item.Id,
                    Packsize = packsize,
                    Quantity = (int)item.Quantity,
                    Name = name,
                    MasterImageUrl = MasterImageUrl,
                    Discount = discount,
                    TotalPrice = discountedprice,//item.TotalPrice == null ? 0 : Math.Round(Convert.ToDecimal(item.TotalPrice / conversionvalue), 2),
                    FK_ProductMaster = item.FK_ProductMaster ,
                };
                cartViewModels.Add(cartobj);
                totalprice += discountedprice;
            }
            var totalquentity = cartproducts.Sum(x => x.Quantity);
            //var totalprice = cartproducts.Sum(x => x.TotalPrice);

            return SuccessResponse(new
            {
                cartproducts = cartViewModels,
                cartproductscount = totalquentity,
                totalprice = totalprice
            }) ;

        }
        public async Task<ActionResult> GetCartList()
        {
            decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
            List<CartViewModel> cartViewModels = new List<CartViewModel>();
            int userid = Convert.ToInt32(HttpContext.Session["UserId"]);
            string cookie = string.Empty;
            if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
            {
                cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
            }
            var cartproducts = await _cart.GetCartProducts(cookie, userid);
            foreach (var item in cartproducts)
            {
                var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                string name = productmasetr.Name;
                string MasterImageUrl = productmasetr.MasterImageUrl;
                var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var actualprice = Math.Round(((decimal)(price * item.Quantity) / conversionvalue), 2);
                var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);
                var cartobj = new CartViewModel
                {
                    ActualPrice = actualprice,//Math.Round(((decimal)(price * item.Quantity) / conversionvalue), 2),
                    Price = Math.Round(Convert.ToDecimal(price / conversionvalue), 2),
                    Id = item.Id,
                    Quantity = (int)item.Quantity,
                    Name = name,
                    MasterImageUrl = MasterImageUrl,
                    Discount = discount,
                    TotalPrice = discountedprice,
                    DiscountAmount = totalDiscountAmount,
                    ShipingAndHostring = 0,
                    VatTax = 0
                };
                cartViewModels.Add(cartobj);
                cartViewModels.Select(c => { c.CartSubTotal += actualprice; return c; }).ToList();
                cartViewModels.Select(c => { c.CartSubTotalDiscount += totalDiscountAmount; return c; }).ToList();
                cartViewModels.Select(c => { c.OrderTotal += discountedprice; return c; }).ToList();
                //cartViewModels.Select(c => { c.CartSubTotal += Math.Round((decimal)(price * item.Quantity) / conversionvalue, 2); return c; }).ToList();
                //cartViewModels.Select(c => { c.CartSubTotalDiscount += Math.Round(((decimal)(price * item.Quantity) - (decimal)item.TotalPrice) / conversionvalue, 2); return c; }).ToList();
                //cartViewModels.Select(c => { c.OrderTotal += Math.Round((decimal)item.TotalPrice / conversionvalue, 2); return c; }).ToList();
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
                decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
                int userid = Convert.ToInt32(HttpContext.Session["UserId"]);
                string cookie = string.Empty;
                string msg = string.Empty;
                if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.cartguid)) && HelperFunctions.GetCookie(HelperFunctions.cartguid) != "undefined")
                {
                    cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                }
                else
                {
                    cookie = Guid.NewGuid().ToString();
                    HelperFunctions.SetCookie(HelperFunctions.cartguid, cookie, 1);
                }
                var productobj = await _IProductMaster.GetProductById(proid);
                var discount = productobj.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                var price = productobj.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var discountedprice = Math.Round(Convert.ToDecimal(price * (1 - (discount / 100))) / conversionvalue,2);
                var cart = new Cart();
                cart.Quantity = quentity;
                cart.Guid = cookie;
                cart.IsWishlist = false;
                cart.IsActive = true;
                cart.TotalPrice = discountedprice;
                cart.TotalQuantity = quentity;
                cart.FK_ProductMaster = proid;
                cart.Currency = Session["currency"].ToString().ToLower();
                cart.ConversionRate = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);

                if (userid > 0)
                {
                    cart.FK_Customer = userid;
                }
                var obj = await _cart.CreateCart(cart);

                var cartproducts = await _cart.GetCartProducts(cookie, userid);
                var totalquentity = cartproducts.Sum(x => x.Quantity);
                msg = obj == null ? "You Can't Add to Cart more then 10 times" : "AdD to Cart Successfully !";
                return Json(new { data = obj, msg = msg, cartproductscount = totalquentity, success = obj == null ? false : true }, JsonRequestBehavior.AllowGet);

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
            msg = cartproducts == true ? "Delete Cart Product Successfully !" : "Error: Cart Not Deleted !";
            return Json(new { data = "", msg = msg, success = cartproducts }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public async Task<JsonResult> UpdateCartList(List<int> cartquentites, List<int> cartids)
        {
            string msg = string.Empty;
            bool updateresult = false;
            decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
            for (int i = 0; i < cartids.Count(); i++)
            {
                var cartproducts = await _cart.GetCartById(cartids[i]);
                if (cartproducts != null)
                {
                    cartproducts.Quantity = cartquentites[i];
                    var productmasetr = await _IProductMaster.GetProductById(cartproducts.FK_ProductMaster);
                    var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    cartproducts.TotalPrice = Math.Round(Convert.ToDecimal((price * cartproducts.Quantity) * (1 - (discount / 100))) / conversionvalue,2);
                    updateresult = await _cart.UpdateCart(cartproducts);
                }
            }
            msg = updateresult == true ? "Carts Updated Successfully !" : "Error: Carts Not Updated!";
            return Json(new { data = "", msg = msg, success = false }, JsonRequestBehavior.AllowGet);

        }
        #endregion
        [HttpGet]
        [ActionName("GetProductbyIdWithRating")]
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
        public async Task<JsonResult> GetProductListbySidebar(SideBarVM[] filterList, string search = "", int nextPage = 10, int prevPage = 0) //int[] filterList
        {
            try
            {
                var filter = await _IProductMaster.GetProductListbySidebar(filterList, search, nextPage, prevPage);
                return SuccessResponse(filter);
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }
        [HttpGet]
        [ActionName("GetFeaturedProduct")]
        public async Task<JsonResult> GetFeaturedProduct()
        {
            try
            {
                decimal conversionvalue = Session["ConversionRate"] == null ? 1 : Convert.ToDecimal(Session["ConversionRate"]);
                var obj = await _IProductMaster.GetFeaturedProduct();
                List<ProductsVM> productsVM = new List<ProductsVM>();
                foreach (var item in obj)
                {
                    string MasterImageUrl = item.MasterImageUrl;
                    var discount = item.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Select(x => x.Price).FirstOrDefault();

                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = Math.Round(Convert.ToDecimal(price / conversionvalue), 2),
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
        public ActionResult PorductList()
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
                var obj = await _IProductMaster.GetProductById(Id);
                var Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString();
                var ConversionRate = Convert.ToDecimal(string.IsNullOrEmpty(Session["ConversionRate"]?.ToString()) ? "1" : Session["ConversionRate"]?.ToString());
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

            return Json(productmasetr.Take(8), JsonRequestBehavior.AllowGet);
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
                string msg = string.Empty;
                bool updateresult = false;
                for (int i = 0; i < wishlistids.Count(); i++)
                {
                    var wishlistProducts = await _cart.GetWishlistById(wishlistids[i]);
                    if (wishlistProducts != null)
                    {
                        wishlistProducts.Quantity = wishlistquentites[i];
                        wishlistProducts.TotalQuantity = wishlistquentites[i]; var productmasetr = await _IProductMaster.GetProductById(wishlistProducts.FK_ProductMaster);
                        var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                        var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault(); wishlistProducts.Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString();
                        var usdRate = HelperFunctions.GetConvertedCurrencyAmount(HelperFunctions.from, HelperFunctions.to);
                        if (wishlistProducts.Currency == "PKR")
                        {
                            wishlistProducts.TotalPrice = (price * (1 - (discount / 100))) * wishlistProducts.Quantity;
                        }
                        else
                        {
                            var totalUsd = (price * (1 - (discount / 100))) * wishlistProducts.Quantity;
                            wishlistProducts.TotalPrice = Math.Round((decimal)(totalUsd / Convert.ToDecimal(usdRate)), 2);
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
        public async Task<JsonResult> GetDataForWishList(int id, string guid)
        {
            try
            {
                var usdRate = HelperFunctions.GetConvertedCurrencyAmount(HelperFunctions.from, HelperFunctions.to);
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
                Cart cart = new Cart(); var res = await _IProductMaster.GetDataForWishList(id);
                var Price = res.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var Discount = res.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                var DiscountedPrice = Price * (1 - (Discount / 100)); var customerId = 0;
                if (Session["UserId"] != null)
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                }
                cart.Guid = cookie;
                if (customerId > 0)
                {
                    cart.FK_Customer = customerId;
                }
                cart.FK_ProductMaster = res.Id;
                cart.IsWishlist = true;
                cart.IsActive = true;
                cart.Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString();
                cart.ConversionRate = Convert.ToDecimal(string.IsNullOrEmpty(Session["ConversionRate"]?.ToString()) ? "1" : Session["ConversionRate"]?.ToString());
                if (cart.Currency == "PKR")
                {
                    cart.TotalPrice = DiscountedPrice;
                }
                else
                {
                    cart.TotalPrice = DiscountedPrice / Convert.ToDecimal(usdRate);
                }
                var response = await _cart.CreateWishList(cart);
                return Json(new { data = response, msg = "Added To WishList", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);
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
                var customerId = 0;
                if (Session["UserId"] != null)
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                }
                List<WishlistVM> wishlistVMs = new List<WishlistVM>();
                HttpCookie cookie = HttpContext.Request.Cookies.Get("cartguid");
                if (cookie != null || customerId > 0)
                {

                    Cart cart = new Cart();
                    var ProductIds = await _cart.GetWishListProducts(cookie.Value, customerId);
                    foreach (var item in ProductIds)
                    {
                        var productmaster = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                        var Name = productmaster.Name;
                        var mainImg = productmaster.MasterImageUrl;
                        //var priceobj = productmaster.ProductPrices.Select(x => x.)
                        var price = productmaster.ProductPrices.Select(x => x.Price).FirstOrDefault();
                        var discount = productmaster.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                        var DiscountedPrice = price * (1 - (discount / 100));
                        var Total = item.TotalPrice;
                        var Quantity = item.TotalQuantity;
                        var CartId = item.Id;
                        var productId = item.FK_ProductMaster;

                         cart.Currency = string.IsNullOrEmpty(Session["currency"]?.ToString()) ? "PKR" : Session["currency"]?.ToString();
                        cart.ConversionRate = Convert.ToDecimal(string.IsNullOrEmpty(Session["ConversionRate"]?.ToString()) ? "1" : Session["ConversionRate"]?.ToString());
                        if (cart.Currency == "PKR")
                        {
                            var wishlistVM = new WishlistVM
                            {
                                Id = CartId,
                                FK_ProductMaster = productId,
                                Name = Name,
                                MasterImageUrl = mainImg,
                                Price = price,
                                Discount = discount,
                                DiscountedPrice = DiscountedPrice,
                                ActualPrice = (decimal)(price * Quantity),
                                TotalPrice = (DiscountedPrice * Quantity),
                                DiscountAmount = ((decimal)(price * item.Quantity) - (item.TotalPrice == null ? 0 : (decimal)item.TotalPrice)),
                                //DiscountAmount = ((decimal)(price * item.Quantity) - (item.TotalPrice == null ? 0 : (decimal)item.TotalPrice)),
                                ShipingAndHostring = 0,
                                VatTax = 0,
                                TotalQuantity = Quantity,
                            };
                            wishlistVMs.Add(wishlistVM);
                            wishlistVMs.Select(c => { c.CartSubTotal += (decimal)(price * item.Quantity); return c; }).ToList();
                            wishlistVMs.Select(c => { c.CartSubTotalDiscount += ((decimal)(price * item.Quantity) - (decimal)item.TotalPrice); return c; }).ToList();
                            wishlistVMs.Select(c => { c.OrderTotal += (decimal)item.TotalPrice; return c; }).ToList();

                        }
                        else
                        {
                            var usdRate = HelperFunctions.GetConvertedCurrencyAmount(HelperFunctions.from, HelperFunctions.to);
                            var wishlistVM = new WishlistVM
                            {
                                Id = CartId,
                                FK_ProductMaster = productId,
                                Name = Name,
                                MasterImageUrl = mainImg,
                                Price = Math.Round((decimal)price / Convert.ToDecimal(usdRate), 2),
                                Discount = discount,
                                DiscountedPrice = DiscountedPrice / Convert.ToDecimal(usdRate),
                                ActualPrice = Math.Round((decimal)((price / Convert.ToDecimal(usdRate)) * Quantity), 2),
                                TotalPrice = Math.Round((decimal)((DiscountedPrice / Convert.ToDecimal(usdRate)) * Quantity), 2),
                                DiscountAmount = Math.Round(((decimal)((price / Convert.ToDecimal(usdRate)) * item.Quantity) - (item.TotalPrice == null ? 0 : (decimal)(item.TotalPrice))), 2),
                                ShipingAndHostring = 0,
                                VatTax = 0,
                                TotalQuantity = Quantity,
                            };
                            wishlistVMs.Add(wishlistVM);
                            wishlistVMs.Select(c => { c.CartSubTotal += Math.Round((decimal)((price / Convert.ToDecimal(usdRate)) * item.Quantity), 2); return c; }).ToList();
                            wishlistVMs.Select(c => { c.CartSubTotalDiscount += Math.Round(((decimal)((price / Convert.ToDecimal(usdRate)) * item.Quantity) - (decimal)(item.TotalPrice)), 2); return c; }).ToList();
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
                var usdRate = HelperFunctions.GetConvertedCurrencyAmount(HelperFunctions.from, HelperFunctions.to);
                if (Session["UserId"] != null)
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
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
                            var productData = await _IProductMaster.GetProductById(productId); if (cartData.Currency == "PKR")
                            {
                                if (productData != null)
                                {
                                    var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                    var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                    DiscountedPrice = (decimal)(price * (1 - (discount / 100)));
                                    //Total = (decimal)(DiscountedPrice * Quantity);
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
                                    DiscountedPrice = (decimal)(price * (1 - (discount / 100)));
                                    //Total = (decimal)(DiscountedPrice * Quantity);
                                }
                                cartData.TotalPrice = Math.Round(DiscountedPrice / Convert.ToDecimal(usdRate), 2);
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
        //[HttpPost]
        //[ActionName("GetProductListbySidebar")]
        //public async Task<JsonResult> GetProductListbySidebar(SideBarVM[] filterList, string search = "", int nextPage = 10, int prevPage = 0) //int[] filterList
        //{
        //    try
        //    {
        //        var filter = await _IProductMaster.GetProductListbySidebar(filterList, search, nextPage, prevPage);
        //        return SuccessResponse(filter);
        //    }
        //    catch (Exception Ex)
        //    {
        //        throw Ex;
        //    }
        //}
        public static string GetCountryByIP(HttpRequestBase request)
        {
            string pricesymbolvalue = "PKR";
            if (!string.IsNullOrEmpty(HelperFunctions.GetCookie(HelperFunctions.pricesymbol)) && HelperFunctions.GetCookie(HelperFunctions.pricesymbol) != "undefined")
            {
                pricesymbolvalue = HelperFunctions.GetCookie(HelperFunctions.pricesymbol);
            }
            else
            {
                IpInfo ipInfo = new IpInfo();
                string info = new WebClient().DownloadString("http://ip-api.com/json/" + request.ServerVariables["REMOTE_ADDR"]);
                JavaScriptSerializer jsonObject = new JavaScriptSerializer();
                ipInfo = jsonObject.Deserialize<IpInfo>(info);
                if (ipInfo.Country == null || (ipInfo.Country?.ToLower() == "pakistan" || ipInfo.Country?.ToLower() == "pk"))
                {
                    pricesymbolvalue = "PKR";
                    HelperFunctions.SetCookie(HelperFunctions.pricesymbol, pricesymbolvalue, 1);
                }
                else
                {
                    pricesymbolvalue = "$";

                    HelperFunctions.SetCookie(HelperFunctions.pricesymbol, pricesymbolvalue, 1);
                }
            }
            return pricesymbolvalue;
        }
      

    }
    public class SideBarVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class IpInfo
    {
        //country
        public string Country { get; set; }
    }

}
