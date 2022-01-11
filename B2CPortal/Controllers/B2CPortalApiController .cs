using API_Base.Base;
using API_Base.Common;
using API_Base.PaymentMethod;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class B2CPortalApiController : BaseController
    {
        private readonly IOrders _orders = null;
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly IOrderTransection _orderTransection = null;
        private readonly PaymentMethodFacade _PaymentMethodFacade = null;
        public B2CPortalApiController(IOrders order, IProductMaster productMaster, ICart cart, IOrderTransection orderTransection)
        {
            _orders = order;
            _IProductMaster = productMaster;
            _cart = cart;
            _orderTransection = orderTransection;
            _PaymentMethodFacade = new PaymentMethodFacade();
        }

        // GET: Country
        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var obj = await _IProductMaster.GetProduct();
            try
            {
                List<ProductsVM> productsVM = new List<ProductsVM>();
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));
                var totalProduct = obj.Count();
                var productmasetr = obj.OrderByDescending(x => x.CreatedOn).ToList();

                foreach (var item in productmasetr)
                {
                    var productRating = _IProductMaster.GetProductRating(item.Id.ToString());// _dxcontext.Database.SqlQuery<ProductsVM>("exec GetProductRating " + Id + "").ToList<ProductsVM>();
                    var discount = item.ProductPrices.Select(x => x.Discount).FirstOrDefault();
                    var price = item.ProductPrices.Select(x => x.Price).FirstOrDefault();
                    var discountedprice = Math.Round(Convert.ToDecimal((price * (1 - (discount / 100))) / conversionvalue), 2);
                    //var packsize = item.ProductPackSize.UOM.ToString();// Select(x => x.).FirstOrDefault();
                    var producVMList = new ProductsVM
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Discount = item.ProductPrices.Select(x => x.Discount).FirstOrDefault(),
                        ImageUrl = item.ProductDetails.Select(x => x.ImageUrl).FirstOrDefault(),
                        MasterImageUrl = item.MasterImageUrl,
                        ShortDescription = item.ShortDescription,
                        LongDescription = item.LongDescription,
                        totalProduct = totalProduct,
                        Price = Math.Round(Convert.ToDecimal(price) / conversionvalue, 2),
                        DiscountedAmount = discountedprice,
                        UOM = item.ProductPackSize?.UOM,
                        TotalRating = productRating.Select(x => x.TotalRating).FirstOrDefault(),
                        AvgRating = productRating.Select(x => x.AvgRating).FirstOrDefault(),
                        ImageUrl2 = item.ProductDetails.Select(x => x.ImageUrl).ToList(),

                    };
                    productsVM.Add(producVMList);
                }
                //return SuccessResponse(productsVM);
                return Json(new { data = productsVM }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        [HttpPost]
        public async Task<ActionResult> StripePayment(PaymentVMRequest payment)
        {
            try
            {
                ViewBag.error = "";
                //string OrderTotalAmount = HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount);
                //string ordermasterId = HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId);

                if (string.IsNullOrEmpty(payment.Amount) || string.IsNullOrEmpty(payment.OrderId) || payment.UserId == null)
                {
                    return Json(new { success = false, msg = "please enter all data correctly !" }, JsonRequestBehavior.AllowGet);
                }
                string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
                decimal conversionvalue = Convert.ToDecimal(HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate));

                var paymentmodel = new PaymentVM
                {
                    Name = payment.Name,
                    Email = payment.Email,
                    Phone = payment.Phone,
                    Description = string.IsNullOrEmpty(payment.Description) ? "this payment from stripe" : payment.Description,
                    StripeToken = payment.stripeToken,
                    Amount = Convert.ToDecimal(payment.Amount) * 100 < 50 ? 100 : Convert.ToDecimal(payment.Amount) * 100,
                    StripeAmount = Convert.ToDecimal(payment.Amount),
                };
                dynamic result = _PaymentMethodFacade.CreateStripePayment(paymentmodel);
                //ResponseViewModel resmodel  =  HelperFunctions.ResponseHandler(result);
                Charge chargeobj = (Charge)result;
                if (result != null && chargeobj.Paid == true)
                {
                    var ordervm = new OrderVM
                    {
                        Id = Convert.ToInt32(payment.OrderId),
                        Currency = currency,
                        ConversionRate = conversionvalue,
                        PaymentMode = PaymentType.Stripe.ToString(),
                        Status = OrderStatus.Confirmed.ToString(),
                        TotalPrice = Convert.ToDecimal(payment.Amount),
                        PaymentStatus = true,
                    };
                    var ordermodel = await _orders.UpdateOrderMAster(ordervm);
                    ordervm = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, ordervm);
                    ordervm.OrderNo = HelperFunctions.GenrateOrderNumber(ordervm.Id.ToString());
                    ordervm.DiscountAmount = ordervm.OrderDetails.Sum(x => x.DiscountedPrice);
                    ordervm.SubTotalPrice = ordervm.OrderDetails.Sum(x => x.Price);
                    // ------------Remove from cart------------
                    //var customerId = Convert.ToInt32(HttpContext.Session["UserId"]);
                    //var cookie = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    var removeCart = await _cart.DisableCart(Convert.ToInt32(payment.UserId), "");
                    //-------------------add order transection record--------------
                    OrderTransection mod3el = new OrderTransection();
                    mod3el.FullName = payment.Name;
                    mod3el.EmailId = payment.Email;
                    mod3el.PhoneNo = payment.Phone;
                    mod3el.StripePaymentID = chargeobj.Id;
                    mod3el.IsActive = true;
                    mod3el.Status = chargeobj.Status.ToLower();
                    mod3el.FK_Customer = Convert.ToInt32(Session["UserId"]);
                    mod3el.FK_OrderMAster = Convert.ToInt32(HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId));
                    var dd = await _orderTransection.CreateOrderTransection(mod3el);
                    return Json(new { success = true, data = payment, msg = "" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TempData["error"] = chargeobj.FailureMessage;
                    return Json(new { success = false, msg = chargeobj.FailureMessage }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (StripeException ex)
            {
                TempData["error"] = ex.Message;
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
    }
    public class PaymentVMRequest
    {
        public string Id { get; set; }
        public string Amount { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string PaymentMode { get; set; }
        public string StripeToken { get; set; }
        public string OrderId { get; set; }
        public string stripeToken { get; set; }
        public string currency { get; set; }
        public string conversionrate { get; set; }
    }
}