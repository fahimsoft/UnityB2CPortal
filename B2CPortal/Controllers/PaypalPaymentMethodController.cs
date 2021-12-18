using API_Base.Common;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class PaypalPaymentMethodController : Controller
    {
        private readonly IOrders _orders = null;
        public PaypalPaymentMethodController(IOrders orders)
        {
            _orders = orders;

        }
        public ActionResult Paypal()
        {
            ViewBag.ClientId = PayPalClient.ClientId;
            ViewBag.Amount = HelperFunctions.SetGetSessionData(HelperFunctions.OrderTotalAmount);
            if (string.IsNullOrEmpty(ViewBag.Amount))
            {
                return RedirectToAction("Checkout", "Orders");
            }
            //ViewBag.CurrencyCode = "GBP"; // Get from a data store
            //ViewBag.CurrencySign = "£";   // Get from a data store

            return View();
        }

        /// <summary>
        /// This action is called when the user clicks on the PayPal button.
        /// </summary>
        /// <returns></returns>
        //[Route("api/paypal/checkout/order/create")]
        [HttpPost]
        public async Task<JsonResult> Create()
        {
            try
            {
                string orderid = HelperFunctions.SetGetSessionData(HelperFunctions.ordermasterId);
                OrderVM model = new OrderVM();
                var ordermodel = await _orders.GetOrderMasterById(Convert.ToInt32(orderid));
                model = (OrderVM)HelperFunctions.CopyPropertiesTo(ordermodel, model);
                if (ordermodel != null)
                {
                    var request = new OrdersCreateRequest();
                    request.Prefer("return=representation");
                    request.RequestBody(OrderBuilder.Build(model));

                    // Call PayPal to set up a transaction
                    var response = await PayPalClient.Client().Execute(request);

                    // Create a response, with an order id.
                    var result = response.Result<Order>();
                    var payPalHttpResponse = new SmartButtonHttpResponse(response)
                    {
                        orderID = result.Id
                    };
                    return Json(result);
                }
                else
                {
                    return Json(new { Status = false, message = "Please Try again"} , JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(new { Status = false, message = ex.Message}, JsonRequestBehavior.AllowGet);
            }

        }

        //[Route("api/paypal/checkout/order/create")]
        //[HttpPost]
        //public async  Task<HttpResponse> createOrder()
        //{
        //    HttpResponse response;
        //    // Construct a request object and set desired parameters
        //    // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
        //    var order = new OrderRequest()
        //    {
        //        CheckoutPaymentIntent = "CAPTURE",
        //        PurchaseUnits = new List<PurchaseUnitRequest>()
        //        {
        //            new PurchaseUnitRequest()
        //            {
        //                AmountWithBreakdown = new AmountWithBreakdown()
        //                {
        //                    CurrencyCode = "USD",
        //                    Value = "100.00"
        //                }
        //            }
        //        },
        //        ApplicationContext = new ApplicationContext()
        //        {
        //            ReturnUrl = "https://www.example.com",
        //            CancelUrl = "https://www.example.com"
        //        }
        //    };


        //    // Call API with your client and get a response for your call
        //    var request = new OrdersCreateRequest();
        //    request.Prefer("return=representation");
        //    request.RequestBody(order);
        //    response = await PayPalClient.Client().Execute(request);
        //    var statusCode = response.StatusCode;
        //    Order result = response.Result<Order>();
        //    Console.WriteLine("Status: {0}", result.Status);
        //    Console.WriteLine("Order Id: {0}", result.Id);
        //    Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
        //    Console.WriteLine("Links:");
        //    foreach (LinkDescription link in result.Links)
        //    {
        //        Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
        //    }
        //    return response;
        //}

        [HttpPost]
        public async Task<JsonResult> captureOrder(string orderid)
        {
            try
            {
                var request = new OrdersCaptureRequest(orderid);
                request.Prefer("return=representation");
                request.RequestBody(new OrderActionRequest());
                //3. Call PayPal to capture an order
                var response = await PayPalClient.Client().Execute(request);
                //4. Save the capture ID to your database. Implement logic to save capture to your database for future reference.

                var result = response.Result<Order>();
                Console.WriteLine("Status: {0}", result.Status);
                Console.WriteLine("Order Id: {0}", result.Id);
                Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
                Console.WriteLine("Links:");
                foreach (LinkDescription link in result.Links)
                {
                    Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
                }
                Console.WriteLine("Capture Ids: ");
                foreach (PurchaseUnit purchaseUnit in result.PurchaseUnits)
                {
                    foreach (Capture capture in purchaseUnit.Payments.Captures)
                    {
                        Console.WriteLine("\t {0}", capture.Id);
                    }
                }
                AmountWithBreakdown amount = result.PurchaseUnits[0].AmountWithBreakdown;
                Console.WriteLine("Buyer:");
                //Console.WriteLine("\tEmail Address: {0}\n\tName: {1}\n\tPhone Number: {2}{3}", result.Payer.Email, result.Payer.Name.FullName, result.Payer.PhoneWithType.PhoneType, result.Payer.PhoneWithType.PhoneNumber);
                var ordermodel = await GetOrder(result.Id);


                //return RedirectToAction("PaymentStatus", "Payment",);
                return Json(ordermodel);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public async static Task<HttpResponse> GetOrder(string orderId)
        {
            OrdersGetRequest request = new OrdersGetRequest(orderId);
            //3. Call PayPal to get the transaction
            var response = await PayPalClient.Client().Execute(request);
            //4. Save the transaction in your database. Implement logic to save transaction to your database for future reference.
            var result = response.Result<Order>();
            Console.WriteLine("Retrieved Order Status");
            Console.WriteLine("Status: {0}", result.Status);
            Console.WriteLine("Order Id: {0}", result.Id);
            Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
            Console.WriteLine("Links:");
            foreach (LinkDescription link in result.Links)
            {
                Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
            }
            AmountWithBreakdown amount = result.PurchaseUnits[0].AmountWithBreakdown;
            Console.WriteLine("Total Amount: {0} {1}", amount.CurrencyCode, amount.Value);

            return response;
        }

        //2. Set up your server to receive a call from the client
        //Use this function to perform authorization on the approved order.
        public async static Task<HttpResponse> AuthorizeOrder(string OrderId)
        {
            var request = new OrdersAuthorizeRequest(OrderId);
            request.Prefer("return=representation");
            request.RequestBody(new AuthorizeRequest());
            //3. Call PayPal to authorization an order
            var response = await PayPalClient.Client().Execute(request);
            //4. Save the authorization ID to your database. Implement logic to save the authorization to your database for future reference.
            var result = response.Result<Order>();
            Console.WriteLine("Status: {0}", result.Status);
            Console.WriteLine("Order Id: {0}", result.Id);
            Console.WriteLine("Authorization Id: {0}",
                            result.PurchaseUnits[0].Payments.Authorizations[0].Id);
            Console.WriteLine("Intent: {0}", result.CheckoutPaymentIntent);
            Console.WriteLine("Links:");
            foreach (LinkDescription link in result.Links)
            {
                Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel,
                                                                link.Href,
                                                                link.Method);
            }
            AmountWithBreakdown amount = result.PurchaseUnits[0].AmountWithBreakdown;
            Console.WriteLine("Buyer:");
            Console.WriteLine("\tEmail Address: {0}", result.Payer.Email);
            Console.WriteLine("Response JSON: \n {0}",
                                        PayPalClient.ObjectToJSONString(result));

            return response;
        }

        /// <summary>
        /// This action is called once the PayPal transaction is approved
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //[Route("api/paypal/checkout/order/approved/{orderId}")]
        //public  Task<ActionResult> Approved(string orderId)
        //{
        //   //var HttpResponse =  await captureOrder(orderId);
        //    return Json(new {d =  "success" }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// This action is called once the PayPal transaction is complete
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //[Route("api/paypal/checkout/order/complete/{orderId}")]
     
        public ActionResult Complete(string orderId)
        {
            // 1. Update the database.
            // 2. Complete the order process. Create and send invoices etc.
            // 3. Complete the shipping process.
            return Json("Completed", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This action is called once the PayPal transaction is complete
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //[Route("api/paypal/checkout/order/cancel/{orderId}")]
        public ActionResult Cancel(string orderId)
        {
            // 1. Remove the orderId from the database.
            return Json("Cancel", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// This action is called once the PayPal transaction is complete
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //[Route("api/paypal/checkout/order/error/{orderId}/{error}")]
        public ActionResult Error(string orderId,
                                   string error)
        {
            // Log the error.
            // Notify the user.
            string temp = System.Web.HttpUtility.UrlDecode(error);
            return Json(temp, JsonRequestBehavior.AllowGet);
        }
        //public async  Task<JsonResult> captureOrder(string orderid)
        //{
        //    // Construct a request object and set desired parameters
        //    // Replace ORDER-ID with the approved order id from create order
        //    var request = new OrdersCaptureRequest(orderid);
        //    request.RequestBody(new OrderActionRequest());
        //    HttpResponse response = await PayPalClient.Client().Execute(request);
        //    var statusCode = response.StatusCode;
        //    Order result = response.Result<Order>();
        //    Console.WriteLine("Status: {0}", result.Status);
        //    Console.WriteLine("Capture Id: {0}", result.Id);
        //    var ordermodel = GetOrder(result.Id);
        //    return Json(ordermodel);
        //}
    }

    public static class OrderBuilder
    {
        /// <summary>
        /// Use classes from the PayPalCheckoutSdk to build an OrderRequest
        /// </summary>
        /// <returns></returns>
        public static OrderRequest Build(OrderVM model)
        {
            // Construct a request object and set desired parameters
            // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
            OrderRequest order = new OrderRequest()
            {
                //Payer = new Payer()
                //{
                //    Name = new Name
                //    {
                //        FullName = model.Name,
                //        GivenName = model.GivenName
                //    },
                //    PhoneWithType = new PhoneWithType()
                //    {
                //        PhoneNumber = new Phone
                //        {
                //            ExtensionNumber = model.PhoneNo,
                //        },
                //    }
                //},
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>()
                {
                    new PurchaseUnitRequest()
                    {
                        AmountWithBreakdown = new AmountWithBreakdown()
                        {
                            CurrencyCode = "USD",
                            Value = model.TotalPrice.ToString()
                        }
                    }
                },
                ApplicationContext = new ApplicationContext()
                {
                    ReturnUrl = "https://www.example.com",
                    CancelUrl = "https://www.example.com"
                }
            };
            // Call API with your client and get a response for your call
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);
            return order;
        }
    }

    public class PayPalClient
    {

        public static string ClientId = "AXvAhr8mO93ZzIkj8chLQYzC-pcblG-tnIyZYNMT2iAHh98QZHP__mfFMkMIVL-z1jIAf4fNS-xsJBA_";
        public static string Secrets = "EOgPbIJBm3BVpqIP9SK9JkF79HOzMGEEZvp7mNQ7e8XkOuFOaE0EVBkIiubhpDqSS3bj_1zU1CNjyVZm";
        public static string SandboxClientId { get; set; } =
                             "<alert>{SandboxClientId}</alert>";
        public static string SandboxClientSecret { get; set; } =
                             "<alert>{SandboxClientSecret}</alert>";

        public static string LiveClientId { get; set; } =
                      "<alert>{PayPal LIVE Client Id}</alert>";
        public static string LiveClientSecret { get; set; } =
                      "<alert>{PayPal LIVE Client Secret}</alert>";


        public static PayPalEnvironment Environment()
        {
            //return new SandboxEnvironment("<alert>SandboxClientId</alert>",
            //                              "<alert>SandboxClientSecret</alert>");
            return new SandboxEnvironment(ClientId, Secrets);
        }
        public static PayPalHttpClient Client()
        {
            return new PayPalHttpClient(Environment());
        }

        public static PayPalCheckoutSdk.Core.PayPalHttpClient Client(string refreshToken)
        {
            return new PayPalHttpClient(Environment(), refreshToken);
        }

        public static String ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(memoryStream,
                                                                  Encoding.UTF8,
                                                                  true,
                                                                  true,
                                                                  "  ");

            var ser = new DataContractJsonSerializer(serializableObject.GetType(),
                                                 new DataContractJsonSerializerSettings
                                                 {
                                                     UseSimpleDictionaryFormat = true
                                                 });

            ser.WriteObject(writer,
                            serializableObject);

            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);

            return sr.ReadToEnd();
        }
    }

    public class SmartButtonHttpResponse
    {
        readonly PayPalCheckoutSdk.Orders.Order _result;
        public SmartButtonHttpResponse(PayPalHttp.HttpResponse httpResponse)
        {
            Headers = httpResponse.Headers;
            StatusCode = httpResponse.StatusCode;
            _result = httpResponse.Result<PayPalCheckoutSdk.Orders.Order>();
        }

        public HttpHeaders Headers { get; }
        public HttpStatusCode StatusCode { get; }

        public PayPalCheckoutSdk.Orders.Order Result()
        {
            return _result;
        }

        public string orderID { get; set; }

    }
}