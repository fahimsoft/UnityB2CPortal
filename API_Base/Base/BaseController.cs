using System.Web.Mvc;
using Newtonsoft.Json;

namespace API_Base.Base
{
    public class BaseController : Controller
    {
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
        protected JsonResult BadResponse(object data)
        {
            return this.CreateResponse(data, "Not Found");
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
}
