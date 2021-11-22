using API_Base.Base;
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
    public class TestController : BaseController
    {
        // GET: Country
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CountryPopup()
        {
            return PartialView("_CountryPopup");

        }

        private readonly ITest _test = null;

        public TestController(ITest test)
        {
            _test = test;
        }

        [HttpGet]
        [ActionName("GetTest")]
        public async Task<JsonResult> GetTest()
        {
            try
            {
                var obj = await _test.GetTest();
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }

        [HttpGet]
        [ActionName("GetTestbyID")]
        public async Task<JsonResult> GetTestbyID(long Id)
        {
            try
            {
                var obj = await _test.GetbyID(Id);
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }

        [HttpPost]
        [ActionName("AddTest")]
        public async Task<JsonResult> AddTest(Test objTest)
        {
            try
            {
                var obj = await _test.AddTest(objTest);
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        [HttpPost]
        [ActionName("DeleteTest")]
        public async Task<JsonResult> DeleteTest(long Id)
        {
            try
            {
                var obj = await _test.DeleteTest(Id);
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
    }
}