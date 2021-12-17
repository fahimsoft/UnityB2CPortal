using API_Base.PaymentMethod.Interfaces;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace API_Base.PaymentMethod.Servicees
{
    public class PaypalService : IPaypal
    {
        ////public async Task<string> Create()
        //public async Task<dynamic> CreatePayment(PaymentVM paymentViewModel)
        //{
        //    var request = new OrdersCreateRequest();
        //    request.Prefer("return=representation");
        //    request.RequestBody(OrderBuilder.Build(paymentViewModel));
        //    // Call PayPal to set up a transaction
        //    var response = await PayPalClient.Client().Execute(request);
        //    // Create a response, with an order id.
        //    var result = response.Result<Order>();
        //    var payPalHttpResponse = new SmartButtonHttpResponse(response)
        //    {
        //        orderID = result.Id
        //    };
        //  var resultdata =   JsonConvert.SerializeObject(result);
        //    return resultdata;
        //}
        public Task<dynamic> CreatePayment(PaymentVM paymentViewModel)
        {
            throw new NotImplementedException();
        }
    }
    //public static class OrderBuilder
    //{
    //    public static OrderRequest Build(PaymentVM model)
    //    {
    //        // Construct a request object and set desired parameters
    //        // Here, OrdersCreateRequest() creates a POST request to /v2/checkout/orders
    //        OrderRequest order = new OrderRequest()
    //        {
    //            Payer = new Payer() {
    //                Name = new Name
    //                {
    //                    FullName = model.Name,
    //                    GivenName = model.GivenName
    //                },
    //                PhoneWithType = new PhoneWithType()
    //                {
    //                    PhoneNumber = new Phone
    //                    {
    //                        ExtensionNumber = model.Phone
    //                    },
    //                 }
    //             },
    //            CheckoutPaymentIntent = "CAPTURE",
    //            PurchaseUnits = new List<PurchaseUnitRequest>()
    //            {
    //                new PurchaseUnitRequest()
    //                {
    //                    AmountWithBreakdown = new AmountWithBreakdown()
    //                    {
    //                        CurrencyCode = "USD",
    //                        Value = model.Amount.ToString()
    //                    }
    //                }
    //            },
    //            ApplicationContext = new ApplicationContext()
    //            {
    //                ReturnUrl = "https://www.example.com",
    //                CancelUrl = "https://www.example.com"
    //            }
    //        };
    //        // Call API with your client and get a response for your call
    //        var request = new OrdersCreateRequest();
    //        request.Prefer("return=representation");
    //        request.RequestBody(order);
    //        return order;
    //    }
    //}

    //public class PayPalClient
    //{

    //    public static string ClientId = "AXvAhr8mO93ZzIkj8chLQYzC-pcblG-tnIyZYNMT2iAHh98QZHP__mfFMkMIVL-z1jIAf4fNS-xsJBA_";
    //    public static string Secrets = "EOgPbIJBm3BVpqIP9SK9JkF79HOzMGEEZvp7mNQ7e8XkOuFOaE0EVBkIiubhpDqSS3bj_1zU1CNjyVZm";
    //    public static string SandboxClientId { get; set; } =
    //                         "<alert>{AXvAhr8mO93ZzIkj8chLQYzC-pcblG-tnIyZYNMT2iAHh98QZHP__mfFMkMIVL-z1jIAf4fNS-xsJBA_}</alert>";
    //    public static string SandboxClientSecret { get; set; } =
    //                         "<alert>{EOgPbIJBm3BVpqIP9SK9JkF79HOzMGEEZvp7mNQ7e8XkOuFOaE0EVBkIiubhpDqSS3bj_1zU1CNjyVZm}</alert>";

    //    public static string LiveClientId { get; set; } =
    //                  "<alert>{PayPal LIVE Client Id}</alert>";
    //    public static string LiveClientSecret { get; set; } =
    //                  "<alert>{PayPal LIVE Client Secret}</alert>";


    //    public static PayPalEnvironment Environment()
    //    {
    //        //return new SandboxEnvironment("<alert>SandboxClientId</alert>",
    //        //                              "<alert>SandboxClientSecret</alert>");
    //        return new SandboxEnvironment(ClientId, Secrets);
    //    }
    //    public static PayPalHttpClient Client()
    //    {
    //        return new PayPalHttpClient(Environment());
    //    }

    //    public static PayPalCheckoutSdk.Core.PayPalHttpClient Client(string refreshToken)
    //    {
    //        return new PayPalHttpClient(Environment(), refreshToken);
    //    }

    //    public static String ObjectToJSONString(Object serializableObject)
    //    {
    //        MemoryStream memoryStream = new MemoryStream();
    //        var writer = JsonReaderWriterFactory.CreateJsonWriter(memoryStream,
    //                                                              Encoding.UTF8,
    //                                                              true,
    //                                                              true,
    //                                                              "  ");

    //        var ser = new DataContractJsonSerializer(serializableObject.GetType(),
    //                                             new DataContractJsonSerializerSettings
    //                                             {
    //                                                 UseSimpleDictionaryFormat = true
    //                                             });

    //        ser.WriteObject(writer,
    //                        serializableObject);

    //        memoryStream.Position = 0;
    //        StreamReader sr = new StreamReader(memoryStream);

    //        return sr.ReadToEnd();
    //    }
    //}

    //public class SmartButtonHttpResponse
    //{
    //    readonly PayPalCheckoutSdk.Orders.Order _result;
    //    public SmartButtonHttpResponse(PayPalHttp.HttpResponse httpResponse)
    //    {
    //        Headers = httpResponse.Headers;
    //        StatusCode = httpResponse.StatusCode;
    //        _result = httpResponse.Result<PayPalCheckoutSdk.Orders.Order>();
    //    }

    //    public HttpHeaders Headers { get; }
    //    public HttpStatusCode StatusCode { get; }

    //    public PayPalCheckoutSdk.Orders.Order Result()
    //    {
    //        return _result;
    //    }

    //    public string orderID { get; set; }

    //}
}
