using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using API_Base.Common;
using Newtonsoft.Json;

namespace API_Base.Base
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            GetCountryByIP(Request);

            //==============disable currency conversion rate and price symbol=============
            //string currency = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
            //if (currency.ToLower() == "pkr")
            //{
            //    HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate, "1", true);
            //}
            //else
            //{
            //    var conversionrate = HelperFunctions.GetConvertedCurrencyAmount("USD", "PKR");
            //    conversionrate = HelperFunctions.SetGetSessionData(HelperFunctions.ConversionRate, conversionrate, true);
            //}

            //------------------url for common js----------------------
            base.OnActionExecuting(filterContext);
            var url = filterContext.HttpContext.Request.Url;
            ViewBag.URL = url;
        }
        public static string GetCountryByIP(HttpRequestBase request)
        {
            string pricesymbolvalue = "";
            //pricesymbolvalue = HelperFunctions.GetCookie(HelperFunctions.pricesymbol);
            pricesymbolvalue = HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol);
            if (string.IsNullOrEmpty(pricesymbolvalue) || pricesymbolvalue != "undefined")
            {
                IpInfo ipInfo = new IpInfo();
                string info = new WebClient().DownloadString("http://ip-api.com/json/" + request.ServerVariables["REMOTE_ADDR"]);
                JavaScriptSerializer jsonObject = new JavaScriptSerializer();
                ipInfo = jsonObject.Deserialize<IpInfo>(info);
                if (ipInfo.Country == null || (ipInfo.Country?.ToLower() == "pakistan" || ipInfo.Country?.ToLower() == "pk"))
                {
                    pricesymbolvalue = "PKR";
                    HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol, pricesymbolvalue, true);
                }
                else
                {
                    pricesymbolvalue = "$";
                    HelperFunctions.SetGetSessionData(HelperFunctions.pricesymbol, pricesymbolvalue, true);
                }
            }
            return pricesymbolvalue;
        }
        public BaseController()
        {

        }
        private JsonResult CreateResponse(object data, string statuscode)
        {
            DTO result = new DTO();

            switch (statuscode)
            {


                case "Ok":
                    {
                        var objresult = Newtonsoft.Json.JsonConvert.SerializeObject(data,
                                Formatting.None,
                                 new Newtonsoft.Json.JsonSerializerSettings
                                 {
                                     ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                 });

                        result.data = objresult;
                        result.isSuccessful = true;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                case "Not Found":
                    {
                        result.errors = data;
                        result.isSuccessful = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                default:
                    break;
            }

            return null;
        }

        protected JsonResult SuccessResponse(object data)
        {
            return this.CreateResponse(data, "Ok");
        }
        //protected JsonResult BadResponse(object data)
        //{
        //    return this.CreateResponse(data, "Not Found");
        //}
        protected JsonResult BadResponse(object data)
        {
            //return this.CreateResponse(data, "Not Found");

            DTO result = new DTO();
            var objresult = Newtonsoft.Json.JsonConvert.SerializeObject(data,
                            Formatting.None,
                             new Newtonsoft.Json.JsonSerializerSettings
                             {
                                 ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                             });

            result.data = objresult;
            result.isSuccessful = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //private JsonResult CreateResponse(object data)
        //{
        //    throw new NotImplementedException();
        //}


        // GET: Base
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);
        //    //Thread.Sleep(1000);
        //}
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
