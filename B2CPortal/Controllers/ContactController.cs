using API_Base.Base;
using API_Base.Common;
using B2CPortal.Models;
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

        [HttpPost]
        public ActionResult SendContactEmail(ContactUsVM contact)
        {
            try
            {
                var name = contact.fullName;
                string htmlString = @"<html>
<body>
<img src=" + "~/Content/Asset/img/img.PNG" + @">
<h1 style=" + "text-align:center;" + @">Thanks for Contacting us!</h1>
<p>Dear " + name + @",</p>
<p>Hello, " + name + @"! Thanks for Contacting us! You are now on our mailing list. This means you'll be the first to hear about our fresh collections and offers!</p>
<p>Thanks,</p>
<p>Unity Foods LTD!</p>
</body>
</html>";



                bool IsSendEmail = HelperFunctions.EmailSend(contact.emailaddress, "Thanks For Contacting Us!", htmlString, true);
                if (IsSendEmail)
                {
                    // return SuccessResponse("true");
                    return Json(new { data = IsSendEmail, msg = "Email sent Successful!", success = true, statuscode = 200 }, JsonRequestBehavior.AllowGet);



                }
                else
                {
                    //return BadResponse("Failed");
                    return Json(new { data = IsSendEmail, msg = "Email Failed", success = false, statuscode = 400 }, JsonRequestBehavior.AllowGet);



                }
            }
            catch (Exception ex)
            {
                return BadResponse(ex);
            }
        }


    }
}