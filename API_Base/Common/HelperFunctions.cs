using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static object CopyPropertiesTo(object fromObject, object toObject)
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

    }
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        InProcess = 3,
        Delivered = 4,
        Cancelled = 5,
        Rejected = 6,
    }
}
