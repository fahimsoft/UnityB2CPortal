using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class PaymentMethodController : Controller
    {
        // GET: PaymentMethod
        public ActionResult Stripe()
        {
            return View();
        }
    }
}