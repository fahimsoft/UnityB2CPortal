using API_Base.Base;
using API_Base.Common;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using B2CPortal.Services.EmailTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class AndriodDataController : Controller
    {
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly ICity _ICity = null;
        private readonly IAccount _account = null;
        private readonly IEmailSubscription _emailSubscription = null;
        private readonly IShippingDetails _IShippingDetails = null;
        private readonly IOrders _orders = null;
        private readonly IOrderDetail _ordersDetail = null;

        public AndriodDataController(IProductMaster productMaster,
            ICart cart,
            ICity city,
            IAccount account,
            IEmailSubscription emailSubscription,
            IShippingDetails shippingDetails,
             IOrders orders,
              IOrderDetail orderDetail
            )
        {
            _IProductMaster = productMaster;
            _cart = cart;
            _ICity = city;
            _account = account;
            _emailSubscription = emailSubscription;
            _IShippingDetails = shippingDetails;
            _orders = orders;
            _ordersDetail = orderDetail;

        }
        // GET: AndriodData
        [HttpPost]
        public async Task<ActionResult> AndroidProductDataById(string productid , string userid , string guid = "")
        {
            if (!string.IsNullOrEmpty(productid))
            {
            var productdetails =  await  _IProductMaster.AndroidGetProductByIdWithRating(Convert.ToInt32(productid));
                return Json(new
                {
                    status = 200,
                    sucess = 1,
                    message = ResultStatus.success,
                    data = productdetails
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    status = 400,
                    sucess = 0,
                    message = ResultStatus.failed,
                    data = new { }
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AndroidHomePageData(string guid = "", string userid = "")
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
                SideBarVM[] filterList = null;
                string search = string.Empty;
                int nextPage = 0;
                int prevPage = 0;
                var filter = await _IProductMaster.AndriodProductList(filterList, search, nextPage, prevPage, citymodel.Id);
                BrandCategoryVM model = await _IProductMaster.AndroidBrandCatagory();
                return Json(new
                {
                    status = 200,
                    sucess = 1,
                    message = ResultStatus.success.ToString(),
                    data = new
                    {
                        Brand = model.Brand,
                        Catagory = model.Category,
                        ProductList = filter
                    }
                });
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    status = 500,
                    sucess = 0,
                    message = Ex.Message,
                    data = new
                    {
                        Brand = new List<BrandCategoryVM>(),
                        Catagory = new List<BrandCategoryVM>(),
                        ProductList = new List<AndroidViewModel>()
                    }

                });
            }
        }
      
        #region Account management
        public ActionResult AndroidNewGuid()
        {
            return Json(new { guid = Guid.NewGuid().ToString() }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> AndroidLogin(string email, string password, string guid, string city = "Karachi")
        {
            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    AndroidAuthenticationVM model = new AndroidAuthenticationVM();
                    var resultmodel = await _account.AndroidLoginWithEmailPassword(email, password);
                    if (resultmodel != null && resultmodel.Id > 0)
                    {
                        model = (AndroidAuthenticationVM)HelperFunctions.CopyPropertiesTo(resultmodel, new AndroidAuthenticationVM());
                        if (string.IsNullOrEmpty(model.Guid))
                        {
                            model.Guid = guid;
                        }
                        City citymodel = await _ICity.GetCityByIdOrName(0, city);
                        string cookie = string.Empty;
                        if (!string.IsNullOrEmpty(guid) && guid != "undefined")
                        {
                            cookie = guid;
                            List<Cart> cartlsit = await _cart.GetCartProducts(cookie, resultmodel.Id, citymodel) as List<Cart>;
                            List<Cart> wishlist = await _cart.GetWishListProducts(cookie, resultmodel.Id) as List<Cart>;
                            cartlsit.ForEach(async x =>
                            {
                                if (x.FK_Customer == null || string.IsNullOrEmpty(x.Guid))
                                {
                                    x.FK_Customer = resultmodel.Id;
                                    x.Guid = cookie;
                                    x.FK_CityId = citymodel.Id;
                                    _ = await _cart.UpdateCart(x);
                                }
                            });
                            wishlist.ForEach(x =>
                            {
                                if (x.FK_Customer == null || string.IsNullOrEmpty(x.Guid))
                                {
                                    x.FK_Customer = resultmodel.Id;
                                    x.Guid = cookie;
                                    x.FK_CityId = citymodel.Id;
                                    _cart.UpdateToCart(x);
                                }
                            });
                        }
                        return Json(new
                        {
                            status = 200,
                            sucess = 1,
                            message = ResultStatus.success,
                            data = model,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 404,
                            sucess = 0,
                            message = ResultStatus.unauthorized.ToString(),
                            data = model
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 404,
                        sucess = 0,
                        message = ResultStatus.unauthorized.ToString(),
                        data = new { }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 404,
                    sucess = 0,
                    message = ResultStatus.Error.ToString(),
                    data = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<ActionResult> AndroidRegister(AndroidAuthenticationVM model)
        {
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.EmailId) && !string.IsNullOrEmpty(model.Password))
                {
                    customer customermodel = (customer)HelperFunctions.CopyPropertiesTo(model, new customer());
                    var resultmodel = await _account.AndroidCreateCustomer(customermodel);
                    if (resultmodel != null)
                    {
                        model.Id = resultmodel.Id;
                        return Json(new
                        {
                            status = 200,
                            sucess = 1,
                            message = ResultStatus.RegisterSuccess,
                            data = model,
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 500,
                            sucess = 0,
                            message = ResultStatus.AlreadyExist,
                            data = model
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 500,
                        sucess = 0,
                        message = ResultStatus.EmptyFillData,
                        data = model
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 404,
                    sucess = 0,
                    message = ResultStatus.Error.ToString(),
                    data = new { ex.Message }
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<ActionResult> AndroidUpdateAccount(AndroidAuthenticationVM model)
        {
            try
            {
                if (model != null && !string.IsNullOrEmpty(model.EmailId) && model.Id > 0)
                {
                    customer customermodel = (customer)HelperFunctions.CopyPropertiesTo(model, new customer());
                    var resultmodel = await _account.AndroidUpdateCustomer(customermodel);
                    if (resultmodel != null)
                    {
                        return Json(new
                        {
                            status = 200,
                            sucess = 1,
                            message = ResultStatus.Update,
                            data = new { }
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            status = 500,
                            sucess = 0,
                            message = ResultStatus.unauthorized,
                            data = model
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 500,
                        sucess = 0,
                        message = ResultStatus.EmptyFillData,
                        data = model
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 404,
                    sucess = 0,
                    message = ResultStatus.Error.ToString(),
                    data = new { ex.Message }
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<ActionResult> AndroidEmailSubscription(string emailid)
        {
            try
            {
                if (!string.IsNullOrEmpty(emailid))
                {
                    var model = new EmailSubscription
                    {
                        IsActive = true,
                        SubEmail = emailid,
                        CreatedOn = DateTime.Now

                    };
                    var dd = await _emailSubscription.CreateEmailSubscription(model);
                    return Json(new
                    {
                        status = 200,
                        sucess = 1,
                        message = ResultStatus.Insert,
                        data = new { }
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = 200,
                        sucess = 1,
                        message = ResultStatus.EmptyFillData,
                        data = new { }
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 200,
                    sucess = 1,
                    message = ResultStatus.Error,
                    data = new { ex.Message }
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region Orders & Checkout Management
        [HttpPost]
        public async Task<ActionResult> AndroidCheckout(AndroidCheckoutVM model)
        {
            //insert cart data
            try
            {
                List<CartDataResponseList> reslist = new List<CartDataResponseList>();
                City citymodel = await _ICity.GetCityByIdOrName(0, model.Usercity);
                if (model.cartData.Count() > 0 && !string.IsNullOrEmpty(model.userid) && !string.IsNullOrEmpty(model.guid))
                {
                    int custoemrid = string.IsNullOrEmpty(model.userid) ? 0 : Convert.ToInt32(model.userid);
                    string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                    decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                    string cookieid = model.guid;
                    decimal TotalPrice = 0;
                    decimal subTotal = 0;
                    decimal ActualPrice = 0;
                    decimal RemainingDiscountPrice = 0;
                    var customerId = 0;
                    var tQuantity = 0;

                    foreach (var item in model.cartData)
                    {
                        var productobj = await _IProductMaster.GetProductById(item.ProductId, citymodel.Id);
                        var discount = productobj.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                        var price = productobj.ProductPrices.Select(x => x.Price).FirstOrDefault();
                        if (item.ProductPrice != price || item.ProductDiscount != discount)
                        {
                            reslist.Add(new CartDataResponseList
                            {
                                ProductId = item.ProductId,
                                ProductPrice = (decimal)price,
                                ProductDiscount = (decimal)discount,
                                Name = productobj.Name
                            });
                        }
                    }
                    if (reslist.Count() > 0)
                    {
                        return Json(new
                        {
                            status = 300,
                            sucess = 0,
                            message = ResultStatus.PriceChanged,
                            data = reslist
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (reslist.Count() < 0)
                        {
                            foreach (var item in model.cartData)
                            {
                                var productobj = await _IProductMaster.GetProductById(item.ProductId, citymodel.Id);
                                var discount = productobj.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var price = productobj.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discountedprice = Math.Round(Convert.ToDecimal(price * (1 - (discount / 100))) / conversionvalue, 2);
                                var cart = new Cart();
                                cart.Quantity = item.ProductQuantity;
                                cart.Guid = cookieid;
                                cart.IsWishlist = false;
                                cart.IsActive = false;
                                cart.TotalPrice = discountedprice;
                                cart.TotalQuantity = item.ProductQuantity;
                                cart.FK_ProductMaster = item.ProductId;
                                cart.Currency = currency;
                                cart.ConversionRate = conversionvalue;
                                cart.FK_CityId = citymodel.Id;
                                cart.FK_Customer = custoemrid;
                                var obj = await _cart.CreateCart(cart);
                            }

                            if (custoemrid > 0)
                            {
                                var shippingmodel = new ShippingDetail();
                                // model Details Add
                                model.FK_Customer = custoemrid;
                                var cartlist = await _cart.GetCartProducts(model.guid, custoemrid, citymodel);
                                if (cartlist != null && cartlist.Count() > 0)
                                {
                                    //for shipping address table..
                                    if (model.shippingdetails != null)
                                    {
                                        shippingmodel.FirstName = model.shippingdetails.FirstName;
                                        shippingmodel.LastName = model.shippingdetails.LastName;
                                        shippingmodel.City = model.shippingdetails.City;
                                        shippingmodel.Country = model.shippingdetails.Country;
                                        shippingmodel.EmailId = model.shippingdetails.EmailId;
                                        shippingmodel.PhoneNo = model.shippingdetails.PhoneNo;
                                        shippingmodel.Address = model.shippingdetails.Address;
                                        shippingmodel.IsActive = true;
                                        shippingmodel = await _IShippingDetails.CreateShippingDetail(shippingmodel);
                                    }

                                    foreach (var item in cartlist)
                                    {
                                        var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster, citymodel.Id);
                                        var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                        var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                        var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                                        RemainingDiscountPrice += Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);
                                        ActualPrice += ((decimal)(price * item.Quantity) / conversionvalue);
                                        TotalPrice = (TotalPrice + Convert.ToDecimal(discountedprice));
                                        subTotal = (subTotal + ActualPrice);
                                        tQuantity = (int)(tQuantity + item.Quantity);
                                    }
                                    model.PaymentStatus = false;
                                    model.TotalPrice = Math.Round(TotalPrice, 2);
                                    model.TotalQuantity = tQuantity;
                                    model.Currency = currency;
                                    model.ConversionRate = conversionvalue;
                                    model.PaymentMode = model.PaymentMode;
                                    model.Status = OrderStatus.InProcess.ToString();
                                    model.Country = model.Country;
                                    model.CityName = model.CityName;
                                    model.PhoneNo = model.PhoneNo;
                                    model.EmailId = model.EmailId;
                                    model.ShippingAddress = model.ShippingAddress;
                                    model.BillingAddress = model.BillingAddress;
                                    model.FK_ShippingDetails = shippingmodel.Id;
                                    model.IsShipping = model.shippingdetails != null;
                                    model.FK_CityId = citymodel.Id;
                                    model.OrderDescription = string.IsNullOrEmpty(model.OrderDescription) ? "order has been genrated successfully" : model.OrderDescription;
                                    OrderMaster orderresult = null;// model.TotalQuantity <= 0 ? null : await _orders.AndroidCreateOrder(model);
                                                                   // Insert order Master
                                    if (model.TotalQuantity > 0)
                                    {
                                        var reqordermodel = (OrderMaster)HelperFunctions.CopyPropertiesTo(model, new OrderMaster());
                                        orderresult = await _orders.AndroidCreateOrder(reqordermodel);
                                    }
                                    if (orderresult != null)
                                    {
                                        // Insert order Detail
                                        var ordermasterId = orderresult.Id;
                                        var orderNo = orderresult.OrderNo;
                                        if (cartlist != null)
                                        {
                                            foreach (var item in cartlist)
                                            {
                                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster, citymodel.Id);
                                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                                var discountedprice = Math.Round(Convert.ToDecimal((price * item.Quantity) * (1 - (discount / 100))) / conversionvalue, 2);
                                                var totalDiscountAmount = Math.Round(((decimal)(price * item.Quantity / conversionvalue) - discountedprice), 2);
                                                var ActualPricedetails = (decimal)(price * item.Quantity);
                                                subTotal = (int)(subTotal + ActualPricedetails);
                                                tQuantity = (int)(tQuantity + item.Quantity);
                                                var Order = new OrderVM
                                                {
                                                    FK_OrderMaster = ordermasterId,
                                                    FK_ProductMaster = item.FK_ProductMaster,
                                                    SubTotalPrice = discountedprice,
                                                    DiscountAmount = totalDiscountAmount,
                                                    Price = price,
                                                    Discount = discount,
                                                    Quantity = item.Quantity,
                                                    FK_Customer = customerId,
                                                    ConversionRate = conversionvalue,
                                                    Currency = currency,

                                                };
                                                var response = await _ordersDetail.CreateOrderDetail(Order);
                                            }
                                        }
                                        // Sending Mail
                                        try
                                        {
                                            string recepit = string.Empty;
                                            string pth = Server.MapPath("~/Services/EmailTemplates/OrderEmail.html");
                                            string MailText = Templates.OrderEmail(pth, model.username, orderresult.OrderDescription, orderresult.PhoneNo, orderresult.EmailId,
                                                   orderresult.CreatedOn.ToString(), orderresult.ShippingAddress, "", orderresult.PaymentMode,
                                                   orderresult.Status, orderresult.TotalQuantity.ToString(), currency,
                                                  orderresult.TotalPrice.ToString(), HelperFunctions.GenrateOrderNumber(ordermasterId.ToString()),
                                                  RemainingDiscountPrice.ToString(), ActualPrice.ToString(),
                                                 recepit);
                                            bool IsSendEmail = HelperFunctions.EmailSend(model.useremail, "Thanks for Your Order!", MailText, true);
                                        }
                                        catch (Exception ex)
                                        {
                                            //throw;
                                        }

                                        if (model.PaymentMode == PaymentType.Stripe.ToString())
                                        {
                                            return Json(new
                                            {
                                                status = 200,
                                                sucess = 1,
                                                message = ResultStatus.Insert,
                                                data = model
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                        else if (model.PaymentMode == PaymentType.Paypal.ToString())
                                        {
                                            return Json(new
                                            {
                                                status = 200,
                                                sucess = 1,
                                                message = ResultStatus.Insert,
                                                data = model
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                        else if (model.PaymentMode == PaymentType.COD.ToString())
                                        {
                                            return Json(new
                                            {
                                                status = 200,
                                                sucess = 1,
                                                message = ResultStatus.Insert,
                                                data = model
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json(new
                                            {
                                                status = 400,
                                                sucess = 0,
                                                message = ResultStatus.failed,
                                                data = model
                                            }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                    else
                                    {
                                        return Json(new
                                        {
                                            status = 400,
                                            sucess = 0,
                                            message = ResultStatus.failed,
                                            data = model
                                        }, JsonRequestBehavior.AllowGet);
                                        // return Json(new { data = "", msg = "Please Re-Genrate Your Order.", success = false }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    return Json(new
                                    {
                                        status = 400,
                                        sucess = 0,
                                        message = ResultStatus.EmptyFillData,
                                        data = model
                                    }, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json(new
                                {
                                    status = 400,
                                    sucess = 0,
                                    message = ResultStatus.unauthorized,
                                    data = model
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(new
                            {
                                status = 400,
                                sucess = 0,
                                message = ResultStatus.EmptyFillData,
                                data = model
                            }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    return Json(new
                    {
                        status = 400,
                        sucess = 0,
                        message = ResultStatus.EmptyFillData,
                        data = model
                    }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 400,
                    sucess = 0,
                    message = ResultStatus.Error,
                    data = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<ActionResult> AndroidGetOrdersList(string userid, string guid = "")
        {
            try
            {
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                List<AndroidOrderListVM> list = new List<AndroidOrderListVM>();
                if (!string.IsNullOrEmpty(userid) && Convert.ToInt32(userid) > 0)
                {
                    var orderlist = await _orders.AndroidGetOrderList(Convert.ToInt32(userid));
                    foreach (var item in orderlist)
                    {
                        List<AndroidOrderDetailsListVM> odlist = new List<AndroidOrderDetailsListVM>();
                        AndroidOrderListVM omodel = (AndroidOrderListVM)HelperFunctions.CopyPropertiesTo(item, new AndroidOrderListVM());
                        var result = HelperFunctions.GenrateOrderNumber(omodel.Id.ToString());
                        omodel.OrderNo = result;
                        omodel.City = item?.City1?.Name;
                        var orderdetailslist =  await _ordersDetail.GetOrderDetailsById(item.Id);
                        foreach (var oditem in orderdetailslist)
                        {
                            AndroidOrderDetailsListVM odmodel = (AndroidOrderDetailsListVM)HelperFunctions.CopyPropertiesTo(oditem, new AndroidOrderDetailsListVM());
                            var pmodel = await _IProductMaster.AndroidGetProductById(oditem.FK_ProductMaster);
                            odmodel.ImageURL = pmodel.MasterImageUrl;
                            odmodel.Name = pmodel.Name;
                            odmodel.CreatedOnDate = oditem.CreatedOn.ToString();
                            odlist.Add(odmodel);
                        }
                        omodel.AndroidOrderDetailsListVMs = odlist;
                        list.Add((AndroidOrderListVM)omodel);
                    }
                    return Json(new
                    {
                        status = 200,
                        sucess = 1,
                        message = ResultStatus.success,
                        data = list
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = 404,
                        sucess = 0,
                        message = ResultStatus.UserNotExist,
                        data = list
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = 500,
                    sucess = 0,
                    message = ex.Message,
                    data = new { }
                }, JsonRequestBehavior.AllowGet); ;
            }
        }
        //[HttpPost]
        //public async Task<JsonResult> AndroidGetOrderDetailsById(string orderid, string userid, string usercity, string guid = "")
        //{
        //    try
        //    {
        //        List<AndroidOrderDetailsVM> orderdetailslist = new List<AndroidOrderDetailsVM>();
        //        if (!string.IsNullOrEmpty(userid) && !string.IsNullOrEmpty(orderid))
        //        {
        //            City citymodel = await _ICity.GetCityByIdOrName(0, usercity);
        //            decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
        //            var detailslist = await _ordersDetail.AndroidGetOrderDetailsById(Convert.ToInt32(orderid));
        //            foreach (var item in detailslist)
        //            {
        //                var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster, citymodel.Id);
        //                string name = productmasetr.Name;
        //                string MasterImageUrl = productmasetr.MasterImageUrl;
        //                var detailsobj = new AndroidOrderDetailsVM
        //                {
        //                    Name = name,
        //                    Price = item.Price,
        //                    Discount = item.Discount,
        //                    SubTotalPrice = item.Price * item.Quantity,
        //                    DiscountedPrice = item.DiscountedPrice,
        //                    Quantity = item.Quantity,
        //                    TotalPrice = item.TotalPrice,
        //                    MasterImageUrl = MasterImageUrl,
        //                    Date = item.CreatedOn.ToString(),
        //                    FK_ProductMaster = item.FK_ProductMaster
        //                };
        //                orderdetailslist.Add(detailsobj);
        //            }
        //            return Json(new
        //            {
        //                status = 200,
        //                sucess = 1,
        //                message = ResultStatus.success,
        //                data = orderdetailslist
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //            return Json(new
        //            {
        //                status = 404,
        //                sucess = 0,
        //                message = ResultStatus.unauthorized,
        //                data = orderdetailslist
        //            }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            status = 404,
        //            sucess = 0,
        //            message = ResultStatus.Error,
        //            data = ex.Message
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        #endregion
    }
}