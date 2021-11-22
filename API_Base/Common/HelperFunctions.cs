using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API_Base.Common
{
    public static class HelperFunctions
    {
        public static string cartguid = "cartguid";
        public static string commentguid = "commentguid";
        public static void SetCookie(string cookieName, string cookieValue, int days)
        {
            cookieName = cookieName.ToLower();
            var cookie = new HttpCookie(cookieName)
            {
                Value = cookieValue,
                Expires = DateTime.Now.AddDays(days)
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static string GetCookie(object commentguid)
        {
            throw new NotImplementedException();
        }

        public static String GetCookie(string cookieName)
        {
            try
            {
                cookieName = cookieName.ToLower();
                if (HttpContext.Current.Request.Cookies[cookieName] == null)
                    return string.Empty;

                return HttpContext.Current.Request.Cookies[cookieName].Value;
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
    }
}
