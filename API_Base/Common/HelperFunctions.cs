﻿using API_Base.PaymentMethod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace API_Base.Common
{
    public static class HelperFunctions
    {
        //--------session ids---------
        public static string cartguid = "cartguid";
        public static string pricesymbol = "pricesymbol";
        public static string ConversionRate = "ConversionRate";
        public static string ordermasterId = "ordermasterId";
        public static string OrderTotalAmount = "ordertotalamount";
        public static string LocationCity = "locationcity";
        public static string CityList = "CityList";
        //---------------readonly data-----------
        public static string DefaultCity = "Karachi";
        public static string from = "USD";
        public static string to = "PKR";
        //---------------account session to cookie------------------
        public static string UserId = "userid";
        public static string UserEmail = "useremail";
        public static string UserName = "username";
        
        public static Regex MobileCheck = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        public static Regex MobileVersionCheck = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        public static string SetGetSessionData(string key , string value ="", bool isset = false)
        {
            if (key == ConversionRate)
            {//disable conversion rate
                return "1";
            }
            if (key == pricesymbol)
            {// disable currency
                return "pkr";
            }
            string datavalue = string.Empty;
            if (isset == true)
            {
                    SetCookie(key, value, 360);
                if (key == pricesymbol || key == LocationCity)
                {
                }
               // HttpContext.Current.Session.Add(key,value);
            }
            else
            {
                datavalue = GetCookie(key);
                    //   datavalue =  string.IsNullOrEmpty(HttpContext.Current.Session[key]?.ToString()) ? "" : HttpContext.Current.Session[key].ToString(); 
            }
            return datavalue;
        }
        public static void SetCookie(string cookieName, string cookieValue, int days)
        {
            //HttpContext.Current.Response.Cookies[cookieName].Expires = DateTime.Now.AddDays(-1);
            cookieName = cookieName.ToLower();
            var cookie = new HttpCookie(cookieName)
            {
                Value = cookieValue,
                Expires = DateTime.Now.AddDays(days)
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
            if (HttpContext.Current.Request.Cookies["ASP.NET_SessionId"] == null)
            {

                HttpContext.Current.Session.Add(cookieName, cookieValue);
            }
        }
        public static String GetCookie(string cookieName)
        {
            try
            {
                cookieName = cookieName.ToLower();
                if (HttpContext.Current.Request.Cookies[cookieName] == null)
                    return string.Empty;

                    var cookieres = HttpContext.Current.Request.Cookies[cookieName].Value;
                if (string.IsNullOrEmpty(cookieres))
                {
                    cookieres =  HttpContext.Current.Session[cookieName]?.ToString();
                }
                return cookieres;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static decimal TotalPrice(decimal total)
        {
            return total += total;
        }
        public static List<T> ToNonNullList<T>(IEnumerable<T> obj)
        {
            return obj == null ? new List<T>() : obj.ToList();
        }
        public static object CopyPropertiesTo(object fromObject, object toObject)
        {
            try
            {
                PropertyInfo[] toObjectProperties = toObject.GetType().GetProperties();
                foreach (PropertyInfo propTo in toObjectProperties)
                {
                    PropertyInfo propFrom = fromObject.GetType().GetProperty(propTo.Name);
                    if (propFrom != null && propFrom.CanWrite)
                        propTo.SetValue(toObject, propFrom.GetValue(fromObject, null), null);
                }
                return toObject;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        public static string GenrateOrderNumber(string num)
        {
            string num2 = num;
            for (int i = 8; i > num.Length; i--)
            {
                if (i > num.Length)
                {
                    num2 = "0" + num2;
                }
                else
                {
                    break;
                }

            }
            return num2;
        }
        public static int GenerateRandomNo()
        {
            int _min = 100000;
            int _max = 999999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        public static bool fBrowserIsMobile()
        {
            Debug.Assert(HttpContext.Current != null); if (HttpContext.Current.Request != null && HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                var u = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"].ToString(); if (u.Length < 4)
                    return false; if (MobileCheck.IsMatch(u) || MobileVersionCheck.IsMatch(u.Substring(0, 4)))
                    return true;
            }
            return false;
        }
        public static string GetConvertedCurrencyAmount(string from, string to)
        {
            try
            {
                Root root = new Root();
                var result = new WebClient().DownloadString("https://api.fastforex.io/fetch-one?from=" + from + "&to=" + to + "&api_key=2579a782cd-8833020115-r61uzq");
                JavaScriptSerializer jsonObject = new JavaScriptSerializer();
                root = jsonObject.Deserialize<Root>(result);
                return root.result.PKR.ToString();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static  ResponseViewModel ResponseHandler(dynamic dynamicmodel)
        {

            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var json = serializer.Serialize(dynamicmodel);
            ResponseViewModel responsemodel = serializer.Deserialize<ResponseViewModel>(json);
            return responsemodel;
            //var properties = dynamicmodel.GetType().GetProperties();
            //foreach (var property in properties)
            //{
            //    var PropertyName = property.Name;
            //    //You get "Property1" as a result

            //    var PropetyValue = dynamicmodel.GetType().GetProperty(property.Name).GetValue(dynamicmodel, null);
            //    //You get "Value1" as a result

            //    // you can use the PropertyName and Value here
            //}
        }
      //----------price calculation----------------
        public static decimal DiscountedPrice(decimal? price, decimal? discount , decimal? tax)
        {
            return Math.Round(Convert.ToDecimal((price / tax) + (price - (price / tax)) - (price * (discount / 100))));
        }
        public static decimal DiscountAmount(decimal? price, decimal? discount)
        {
            return Math.Round(((decimal)(price * (discount / 100))));
        }
        public static decimal BasePrice(decimal? price, decimal? tax)
        {
            return  Math.Round((decimal)((price / tax)));
        }
        // Email configuration
        public static bool EmailSend(string SenderEmail, string Subject, string Message, bool IsBodyHtml = false)
        {
            bool status = false;
            try
            {
                string HostAddress = ConfigurationManager.AppSettings["Host"].ToString();
                string FormEmailId = ConfigurationManager.AppSettings["MailFrom"].ToString();
                string Password = ConfigurationManager.AppSettings["Password"].ToString();
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());

                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(FormEmailId);
                mailMessage.Subject = Subject;
                mailMessage.Body = Message;
                mailMessage.IsBodyHtml = IsBodyHtml;
                mailMessage.To.Add(new MailAddress(SenderEmail));


                using (SmtpClient smtp = new SmtpClient(HostAddress, Port))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(FormEmailId, Password);
                    smtp.EnableSsl = true;
                    smtp.Timeout = 20000;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mailMessage);
                    status = true;
                }
                return status;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }
    }
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success
        {
            get;
            set;
        }
        [JsonProperty("error-codes")]
        public List<string> ErrorMessage
        {
            get;
            set;
        }
        public string Recaptcha { get; set; }
    }
    public class Result
    {
        public double PKR { get; set; }
    }

    public class Root
    {
        public string @base { get; set; }
        public Result result { get; set; }
        public string updated { get; set; }
        public int ms { get; set; }
    }
    //public enum OrderStatus
    //{
    //    Pending = 1,
    //    Confirmed = 2,
    //    InProcess = 3,
    //    Delivered = 4,
    //    Cancelled = 5,
    //    Rejected = 6,
    //}
    //public enum PaymentType
    //{
    //    Stripe = 1,
    //    HBL = 2,
    //    JazzCash = 3,
    //    EasyPaisa = 4
    //}
}
