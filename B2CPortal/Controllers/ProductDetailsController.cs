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

            var CommentData = await _IProductDetail.GetProductCommentbyId(id);
            var totalComment = CommentData.Count();
            var commentAndRateReult = CommentData.OrderBy(x => x.CreatedOn).Skip(0).Take(10).Where(x => x.FK_ProductMaster == id);


            foreach (var item in commentAndRateReult)
            {
                var commentObj = new CommentAndRatingVM
                {
                    CustomerName = item.AnonymousName,
                    CustomerComment = item.Comment,
                    CustomerRate = item.Rate,
                    CommentDate = item.CreatedOn,
                    totalComment = totalComment

                };

                commentAndRatingVM.Add(commentObj);
            }


            return SuccessResponse(commentAndRatingVM);
        }

        [HttpPost]
        public async Task<JsonResult> GetProductCommentWithPaggination(long id, int nextPage = 10, int prevPage = 0)
        {
            List<CommentAndRatingVM> commentAndRatingVM = new List<CommentAndRatingVM>();

            var commentReult = await _IProductDetail.GetProductCommentWithPaggination(id);
            var totalComment = commentReult.Count();
            var commentAndRateReult = commentReult.OrderBy(x => x.CreatedOn).Skip(prevPage).Take(nextPage).Where(x => x.FK_ProductMaster == id).ToList();

            foreach (var item in commentAndRateReult)
            {
                var commentObj = new CommentAndRatingVM
                {
                    CustomerName = item.AnonymousName,
                    CustomerComment = item.Comment,
                    CustomerRate = item.Rate,
                    totalComment = totalComment,

                    CommentDate = item.CreatedOn,

                };
                commentAndRatingVM.Add(commentObj);
            }


            return SuccessResponse(commentAndRatingVM);
        }
    }
}