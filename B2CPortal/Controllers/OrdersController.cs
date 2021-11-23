using API_Base.Base;
using API_Base.Common;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class OrdersController : BaseController
    {
        #region Interface instance
        private readonly IOrders _orders = null;
        private readonly IOrderDetail _ordersDetail = null;
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        public OrdersController(IOrders orders, IProductMaster productMaster, ICart cart, IOrderDetail orderDetail)
        {
            _IProductMaster = productMaster;
            _cart = cart;
            _orders = orders;
            _ordersDetail = orderDetail;
        }
        public ActionResult Index()
        {
            if (Convert.ToInt32(HttpContext.Session["UserId"]) <= 0)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
        public async Task<JsonResult> GetOrderDetailsById(int id)
        {
            List<OrderVM> orderdetailslist = new List<OrderVM>();
           var detailslist =  await _ordersDetail.GetOrderDetailsById(id);
            foreach (var item in detailslist)
            {
                var productmasetr = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                string name = productmasetr.Name;
                string MasterImageUrl = productmasetr.MasterImageUrl;
                var price = productmasetr.ProductPrices.Select(x => x.Price).FirstOrDefault();
                var discount = productmasetr.ProductPrices.Select(x => x.Discount).FirstOrDefault();

                var detailsobj = new OrderVM
                {
                    Name = name,
                    Price = price,
                    Discount = item.Discount, 
                    SubTotalPrice = (price * item.Quantity),
                    DiscountAmount = ((price * item.Quantity) - item.Price),
                    Quantity = item.Quantity,
                    TotalPrice = Convert.ToInt32(item.Price),
                    MasterImageUrl = MasterImageUrl,
                    Date = item.CreatedOn.ToString(),
                    FK_ProductMaster = item.FK_ProductMaster
                };
                orderdetailslist.Add(detailsobj);
            }
            return Json(new { data = orderdetailslist },JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetOrderList()
        {
        
            List<OrderVM> list = new List<OrderVM>();
            var orderlist = await _orders.GetOrderList();

            foreach (var item in orderlist)
            {
                OrderVM dd = (OrderVM)HelperFunctions.CopyPropertiesTo(item, new OrderVM());
                var result = HelperFunctions.GenrateOrderNumber(dd.Id.ToString());
                dd.OrderNo = result;
                

                list.Add((OrderVM)dd);
            }
           return PartialView("_OrderListPartialView", list);
        }
        #endregion
        [HttpGet]
        [ActionName("Checkout")]
        public async Task<ActionResult> Checkout()
        {
            try
            {
                OrderVM orderVM = new OrderVM();
                List<OrderVM> orderVMs = new List<OrderVM>();
                var OrderTotal = 0;
                var totalDiscount = 0;
                var customerId = 0;
                var subTotal = 0;
                if (Session["UserId"] != null)
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    if (customerId > 0)
                    {
                        // customer data for billing and shipment
                        var customer = await _orders.GetCustomerById(customerId);
                        orderVM.FK_Customer = customer.Id;
                        orderVM.FirstName = customer.FirstName;
                        orderVM.LastName = customer.LastName;
                        orderVM.EmailId = customer.EmailId;
                        orderVM.PhoneNo = customer.PhoneNo;
                        orderVM.Country = customer.Country;
                        orderVM.City = customer.City;
                        orderVM.Address = customer.Address;



                        var cartlist = await _cart.GetCartProducts("", customerId);
                        if (cartlist != null)
                        {
                            foreach (var item in cartlist)
                            {
                                OrderTotal = (int)(OrderTotal + item.TotalPrice);
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);



                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var DiscountedPrice = price * (1 - (discount / 100));



                                var ActualPrice = (decimal)(price * item.Quantity);
                                subTotal = (int)(subTotal + ActualPrice);
                                totalDiscount = (int)(totalDiscount + discount);
                                var Order = new OrderVM
                                {
                                    Name = productData.Name,
                                    Quantity = item.Quantity,
                                    TotalPrice = (int)item.TotalPrice



                                };
                                orderVMs.Add(Order);
                                orderVM.CartSubTotalDiscount += ((decimal)(price * item.Quantity) - (decimal)item.TotalPrice);



                            }
                            orderVM.orderVMs = orderVMs;
                            orderVM.CartSubTotal = subTotal;



                            //orderVM.CartSubTotalDiscount = totalDiscount;
                            orderVM.OrderTotal = OrderTotal;



                        }



                        return View(orderVM);
                    }
                    else
                    {
                        string ReturnUrl = Convert.ToString(Request.QueryString["url"]);
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    string CurrentURL = Request.Url.AbsoluteUri;
                    TempData["returnurl"] = CurrentURL;



                    return RedirectToAction("Login", "Account");
                }



            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }



        }
        [HttpPost]
        [ActionName("AddBillingDetails")]
        public async Task<ActionResult> AddBillingDetails(OrderVM Billing)
        {
            try
            {
                OrderVM orderVM = new OrderVM();
                List<OrderVM> orderVMs = new List<OrderVM>();
                var OrderTotal = 0;
                var subTotal = 0;
                var customerId = 0;
                var tQuantity = 0;
                if (Session["UserId"] != null)
                {
                    customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    if (customerId > 0)
                    {
                        // Billing Details Add
                        Billing.FK_Customer = customerId;
                        var cartlist = await _cart.GetCartProducts("", customerId);
                        if (cartlist != null)
                        {
                            foreach (var item in cartlist)
                            {
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var DiscountedPrice = price * (1 - (discount / 100));
                                var ActualPrice = (decimal)(price * item.Quantity);
                                OrderTotal = (int)(OrderTotal + item.TotalPrice);
                                subTotal = (int)(subTotal + ActualPrice);
                                tQuantity = (int)(tQuantity + item.Quantity);
                                var Order = new OrderVM
                                {
                                    Name = productData.Name,
                                    Quantity = item.Quantity,
                                    TotalPrice = (int)item.TotalPrice
                                };
                                orderVMs.Add(Order);
                            }
                            Billing.orderVMs = orderVMs;
                            Billing.CartSubTotal = subTotal;
                            Billing.OrderTotal = OrderTotal;
                            Billing.TotalQuantity = tQuantity;

                        }
                        // Insert order Master
                        var res = await _orders.CreateOrder(Billing);



                        // Insert order Detail
                        var ordermasterId = res.Id;



                        if (cartlist != null)
                        {
                            foreach (var item in cartlist)
                            {
                                var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
                                var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
                                var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                                var DiscountedPrice = price * (1 - (discount / 100));
                                var ActualPrice = (decimal)(price * item.Quantity);
                                OrderTotal = (int)(OrderTotal + item.TotalPrice);
                                subTotal = (int)(subTotal + ActualPrice);
                                tQuantity = (int)(tQuantity + item.Quantity);



                                var Order = new OrderVM
                                {
                                    FK_OrderMaster = ordermasterId,
                                    FK_ProductMaster = item.FK_ProductMaster,
                                    Quantity = item.Quantity,
                                    TotalPrice = (int)item.TotalPrice,
                                    Discount = discount
                                };
                                var response = await _ordersDetail.CreateOrderDetail(Order);
                            }
                        }



                        // Remove from cart
                        HttpCookie cookie = HttpContext.Request.Cookies.Get("cartguid");
                        var removeCart = await _cart.DisableCart(customerId, cookie.Value);



                        return Json(new { data = res, msg = "Order Successfull !", success = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                else
                {
                    return Json(new { data = "", msg = "Something bad happened", success = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }
        // GET: Orders
        //[HttpGet]
        //[ActionName("Checkout")]
        //public async Task<ActionResult> Checkout()
        //{
        //    try
        //    {
        //        OrderVM orderVM = new OrderVM();
        //        List<OrderVM> orderVMs = new List<OrderVM>();
        //        var OrderTotal = 0;
        //        var totalDiscount = 0;
        //        var customerId = 0;
        //        var subTotal = 0;
        //        if (Session["UserId"] != null)
        //        {
        //            customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
        //            if (customerId > 0)
        //            {
        //                // customer data for billing and shipment
        //                var customer = await _orders.GetCustomerById(customerId);
        //                orderVM.FK_Customer = customer.Id;
        //                orderVM.FirstName = customer.FirstName;
        //                orderVM.LastName = customer.LastName;
        //                orderVM.EmailId = customer.EmailId;
        //                orderVM.PhoneNo = customer.PhoneNo;
        //                orderVM.Country = customer.Country;
        //                orderVM.City = customer.City;
        //                orderVM.Address = customer.Address;

        //                var cartlist = await _cart.GetCartProducts("", customerId);
        //                if (cartlist != null)
        //                {
        //                    foreach (var item in cartlist)
        //                    {
        //                        OrderTotal = (int)(OrderTotal + item.TotalPrice);
        //                        var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);

        //                        var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
        //                        var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
        //                        var DiscountedPrice = price * (1 - (discount / 100));

        //                        var ActualPrice = (decimal)(price * item.Quantity);
        //                        subTotal = (int)(subTotal + ActualPrice);
        //                        totalDiscount = (int)(totalDiscount + discount);
        //                        var Order = new OrderVM
        //                        {
        //                            Name = productData.Name,
        //                            Quantity = item.Quantity,
        //                            TotalPrice = (int)item.TotalPrice

        //                        };
        //                        orderVMs.Add(Order);

        //                    }
        //                    orderVM.orderVMs = orderVMs;
        //                    orderVM.CartSubTotal = subTotal;
        //                    orderVM.CartSubTotalDiscount = totalDiscount;
        //                    orderVM.OrderTotal = OrderTotal;

        //                }

        //                return View(orderVM);
        //            }
        //            else
        //            {
        //                return RedirectToAction("Login", "Account");
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("Login", "Account");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return BadResponse(ex);
        //    }

        //}

        //[HttpPost]
        //[ActionName("AddBillingDetails")]
        //public async Task<ActionResult> AddBillingDetails(OrderVM Billing)
        //{
        //    try
        //    {
        //        OrderVM orderVM = new OrderVM();
        //        List<OrderVM> orderVMs = new List<OrderVM>();
        //        var OrderTotal = 0;
        //        var subTotal = 0;
        //        var customerId = 0;
        //        var tQuantity = 0;
        //        if (Session["UserId"] != null)
        //        {
        //            customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
        //            if (customerId > 0)
        //            {
        //                // Billing Details Add
        //                Billing.FK_Customer = customerId;
        //                var cartlist = await _cart.GetCartProducts("", customerId);
        //                if (cartlist != null)
        //                {
        //                    foreach (var item in cartlist)
        //                    {
        //                        var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
        //                        var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
        //                        var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
        //                        var DiscountedPrice = price * (1 - (discount / 100));
        //                        var ActualPrice = (decimal)(price * item.Quantity);
        //                        OrderTotal = (int)(OrderTotal + item.TotalPrice);
        //                        subTotal = (int)(subTotal + ActualPrice);
        //                        tQuantity = (int)(tQuantity + item.Quantity);

        //                        var Order = new OrderVM
        //                        {
        //                            Name = productData.Name,
        //                            Quantity = item.Quantity,
        //                            TotalPrice = (int)item.TotalPrice
        //                        };
        //                        orderVMs.Add(Order);

        //                    }
        //                    Billing.orderVMs = orderVMs;
        //                    Billing.CartSubTotal = subTotal;
        //                    Billing.OrderTotal = OrderTotal;
        //                    Billing.TotalQuantity = tQuantity;
        //                    Billing.OrderNo = HelperFunctions.GenerateRandomNo().ToString();
        //                    Billing.Status = OrderStatus.Pending.ToString();
        //                }
        //                // Insert order Master
        //                var res = await _orders.CreateOrder(Billing);

        //                // Insert order Detail
        //                var ordermasterId = res.Id;

        //                if (cartlist != null)
        //                {
        //                    foreach (var item in cartlist)
        //                    {
        //                        var productData = await _IProductMaster.GetProductById(item.FK_ProductMaster);
        //                        var price = productData.ProductPrices.Select(x => x.Price).FirstOrDefault();
        //                        var discount = productData.ProductPrices.Select(x => x.Discount).FirstOrDefault();
        //                        var DiscountedPrice = price * (1 - (discount / 100));
        //                        var ActualPrice = (decimal)(price * item.Quantity);
        //                        OrderTotal = (int)(OrderTotal + item.TotalPrice);
        //                        subTotal = (int)(subTotal + ActualPrice);
        //                        tQuantity = (int)(tQuantity + item.Quantity);

        //                        var Order = new OrderVM
        //                        {
        //                            FK_OrderMaster = ordermasterId,
        //                            FK_ProductMaster = item.FK_ProductMaster,
        //                            Quantity = item.Quantity,
        //                            TotalPrice = (int)item.TotalPrice,
        //                            Discount = discount
        //                        };
        //                        var response = await _ordersDetail.CreateOrderDetail(Order);

        //                    }
        //                }

        //                return Json(new { data = res, msg = "Billing Details Added Successfully !", success = true }, JsonRequestBehavior.AllowGet);
        //            }
        //            else
        //            {
        //                return RedirectToAction("Login", "Account");
        //            }
        //        }
        //        else
        //        {
        //            return Json(new { data = "", msg = "Something bad happened", success = false }, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadResponse(ex);
        //    }
        //}
    }
}