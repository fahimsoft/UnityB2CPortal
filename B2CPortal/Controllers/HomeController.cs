using API_Base.Base;
using API_Base.Common;
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
        private readonly ICity _ICity = null;
        public HomeController(IProductMaster productMaster, ICity city)
        {
            _IProductMaster = productMaster;
            _ICity = city;
        }
        public async Task<ActionResult> Index()
        {
            return View();
        }
        public async Task<ActionResult> GetCityList()
        {
            string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
            if (string.IsNullOrEmpty(cookiecity))
            {
                HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
            }
            string guid = Guid.NewGuid().ToString();
            var citylist = await _ICity.GetCityList(guid, 2);
            Session["citylist"] = citylist;
            List<CityVM> list = citylist.Select(x => new CityVM
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            list.Where(x => x.Name.ToLower() == cookiecity.ToLower()).ToList().ForEach(x => x.Selected = true);
            return Json(new
            {
                success = true,
                data = list,
                msg = "",
            },JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> ChangeCity(int id)
        {
            var cityitem = await _ICity.GetCityByIdOrName(id, null);
            HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, cityitem.Name, true);
            return Json(new { success = true, msg = "Change your City Successfully !" });
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
        ////public ActionResult ProductList()
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
    class CityVM
    {
        public string Name { get; internal set; }
        public int Id { get; internal set; }
        public bool Selected { get;  set; }
    }
}