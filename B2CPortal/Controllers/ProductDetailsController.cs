using API_Base.Base;
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
    public class ProductDetailsController : BaseController
    {
        private readonly IProductDetail _IProductDetail = null;

        public ProductDetailsController(IProductDetail productDetail)
        {
            _IProductDetail = productDetail;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> SaveComment(CommentAndRating commentAndRating)
        {

            var res = await _IProductDetail.SaveComment(commentAndRating);
            return SuccessResponse(res);
        }

        [HttpGet]
        public async Task<JsonResult> GetProductCommentbyId(long id)
        {
            List<CommentAndRatingVM> commentAndRatingVM = new List<CommentAndRatingVM>();


            var commentAndRateReult = await _IProductDetail.GetProductCommentbyId(id);
            foreach (var item in commentAndRateReult)
            {
                var commentObj = new CommentAndRatingVM
                {
                    CustomerName = item.AnonymousName,
                    CustomerComment = item.Comment,
                    CustomerRate = item.Rate,

                    CommentDate = item.CreatedOn,

                };
                commentAndRatingVM.Add(commentObj);
            }


            return SuccessResponse(commentAndRatingVM);
        }

    }
}