using API_Base.Base;
using API_Base.Common;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace B2CPortal.Controllers
{
    public class AndriodDataController : Controller
    {
        private readonly IProductMaster _IProductMaster = null;
        private readonly ICart _cart = null;
        private readonly ICity _ICity = null;
        public AndriodDataController(IProductMaster productMaster, ICart cart, ICity city)
        {
            _IProductMaster = productMaster;
            _cart = cart;
            _ICity = city;
        }
        // GET: AndriodData
        [HttpPost]
        public async Task<ActionResult> AndroidHomePageData(string guid = "", string userid = "")
        {
            try
            {
                //get location from cookie
                string cookiecity = HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                if (string.IsNullOrEmpty(cookiecity))
                {
                    HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity, HelperFunctions.DefaultCity, true);
                }
                cookiecity = string.IsNullOrEmpty(cookiecity) ? HelperFunctions.DefaultCity : HelperFunctions.SetGetSessionData(HelperFunctions.LocationCity);
                City citymodel = await _ICity.GetCityByIdOrName(0, cookiecity);
                SideBarVM[] filterList = null;
                string search = string.Empty;
                int nextPage = 0;
                int prevPage = 0;
                var filter = await _IProductMaster.AndriodProductList(filterList, search, nextPage, prevPage, citymodel.Id);
                BrandCategoryVM model = await _IProductMaster.AndroidBrandCatagory();
                return Json(new { status = 200,
                    sucess = 1, 
                    message = ResultStatus .Success.ToString(),
                    ProductList = filter,
                    Brand = model.Brand,
                    Catagory = model.Category
                }); 
            }
            catch (Exception Ex)
            {
                return Json(new
                {
                    status = 500,
                    sucess = 0,
                    message = Ex.Message,
                    ProductList = new object(),
                    Brand = new object(),
                    Catagory = new object()
                });
            }
        }

        public ActionResult AndroidNewGuid()
        {
            return Json(new { guid = Guid.NewGuid().ToString() }, JsonRequestBehavior.AllowGet);
        }

    }
}