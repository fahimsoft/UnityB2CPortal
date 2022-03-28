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
    public class UserController : BaseController
    {
        // GET: User
        private readonly IUser _user = null;
        public UserController(IUser user)
        {
            _user = user;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ActionName("GetUser")]
        public async Task<JsonResult> GetUser()
        {
            try
            {
                var obj = await _user.GetUser();
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }

        [HttpGet]
        [ActionName("GetUserByID")]
        public async Task<JsonResult> GetUserbyID(long Id)
        {
            try
            {
                var obj = await _user.GetUserById(Id);
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }

        [HttpPost]
        [ActionName("AddUser")]
        public async Task<JsonResult> AddUser(User objUser)
        {
            try
            {
                var obj = await _user.AddUser(objUser);
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        [ActionName("DeleteUser")]
        public async Task<JsonResult> DeleteUser(long Id)
        {
            try
            {
                var obj = await _user.DeleteUser(Id);
                return SuccessResponse(obj);
            }
            catch (Exception Ex)
            {

                return BadResponse(Ex);
            }
        }

    }
}