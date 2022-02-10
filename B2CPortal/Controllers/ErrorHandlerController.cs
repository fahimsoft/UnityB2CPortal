using API_Base.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class ErrorHandlerController : BaseController
    {
        // GET: ErrorHandler
        public ActionResult HttpError404(string message)
        {
            ViewBag.msg = message;
            return View();
        }
        public ActionResult HttpError500(string message)
        {
            ViewBag.msg = message;
            return View();
        }
    }
}