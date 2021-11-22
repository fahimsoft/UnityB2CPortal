using API_Base.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class ContactController : BaseController
    {
        // GET: Contact
        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }
    }
}