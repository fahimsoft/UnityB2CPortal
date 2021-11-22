using API_Base.Base;
using B2CPortal.Interfaces;
using B2CPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class HomeController : BaseController
    {

        private readonly IProductMaster _IProductMaster = null;
        public HomeController(IProductMaster productMaster)
        {
            _IProductMaster = productMaster;
        }
        public ActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //[ActionName("GetProduct")]
        //public async Task<JsonResult> GetProduct()
        //{
        //    try
        //    {
        //        var obj = await _IProductMaster.GetProduct();
        //        return SuccessResponse(obj);
        //    }
        //    catch (Exception Ex)
        //    {

        //        return BadResponse(Ex);
        //    }
        //}
        ////public ActionResult PorductList()
        ////{
        ////    return View();
        ////}

        //[HttpGet]
        //[ActionName("GetProductbyId")]
        //public async Task<JsonResult> GetProductbyId(long Id)
        //{
        //    try
        //    {
        //        var obj = await _IProductMaster.GetProductById(Id);
        //        return SuccessResponse(obj);
        //    }
        //    catch (Exception Ex)
        //    {

        //        return BadResponse(Ex);
        //    }
        //}

    }
}