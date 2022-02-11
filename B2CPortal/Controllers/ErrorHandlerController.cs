using API_Base.Base;
using API_Base.Common;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{

    public class ErrorHandlerController : BaseController
    {
        private readonly IExceptionHandling _exceptionHandling1 = null;
        public ErrorHandlerController(IExceptionHandling exceptionHandling)
        {
            _exceptionHandling1 = exceptionHandling;
        }
        // GET: ErrorHandler
        public async Task<ActionResult> HttpError404(string message)
        {
            try
            {
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                var model = new ExceptionHandling
                {
                    FK_userid = string.IsNullOrEmpty(userid) == null ? 0 : Convert.ToInt32(userid),
                    ErrorMessage = message,
                    StatusCode = "404",
                    ErrorURL = ""
                };
                await _exceptionHandling1.CreateExceptionHandling(model);
            }
            catch (Exception)
            {

                throw;
            }
            ViewBag.msg = message;

            return View();
        }
        public async Task<ActionResult> HttpError500(string message)
        {
            try
            {
                string userid = HelperFunctions.SetGetSessionData(HelperFunctions.UserId);

                var model = new ExceptionHandling
                {
                    FK_userid = string.IsNullOrEmpty(userid) == null ? 0 : Convert.ToInt32(userid),
                    ErrorMessage = message,
                    StatusCode = "500",
                    ErrorURL = ""
                };
                await _exceptionHandling1.CreateExceptionHandling(model);
            }
            catch (Exception)
            {

                throw;
            }
            ViewBag.msg = message;

            ViewBag.msg = message;
            return View();
        }
    }
}