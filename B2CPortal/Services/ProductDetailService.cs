using API_Base.Base;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using API_Base.Common;
using System.Web;
using B2CPortal.Models;

namespace B2CPortal.Services
{
    public class ProductDetailService : DALBase<CommentAndRating>, IProductDetail
    {

        public async Task<CommentAndRating> SaveComment(CommentAndRating commentAndRating)
        {
            try
            {

                string checkGuid = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                string userId = Convert.ToString(System.Web.HttpContext.Current.Session["UserId"]);


                int? uId = null;
                if (!string.IsNullOrEmpty(userId))
                    uId = Convert.ToInt32(userId);


                if (!string.IsNullOrEmpty(checkGuid))
                {
                    Current = await _dxcontext.CommentAndRatings.FirstOrDefaultAsync(o =>
                    o.FK_ProductMaster == commentAndRating.FK_ProductMaster &&
                   !string.IsNullOrEmpty(o.Guid) && o.Guid == checkGuid);
                }
                else
                {
                    Current = await _dxcontext.CommentAndRatings.FirstOrDefaultAsync(o =>
                    o.FK_ProductMaster == commentAndRating.FK_ProductMaster &&
                    o.CustomerId != null && o.CustomerId == commentAndRating.CustomerId);
                }

                if (Current == null)
                {
                    New();

                    Current.CreatedOn = DateTime.Now;
                    Current.AnonymousName = commentAndRating.AnonymousName;
                    Current.FK_ProductMaster = commentAndRating.FK_ProductMaster;
                    Current.EmailId = commentAndRating.EmailId;
                    Current.Comment = commentAndRating.Comment;
                    Current.Rate = commentAndRating.Rate;

                    if (String.IsNullOrEmpty(userId) && String.IsNullOrEmpty(checkGuid))
                    {
                        Current.IsAnonymousUser = true;
                        HelperFunctions.SetCookie(HelperFunctions.cartguid, Guid.NewGuid().ToString(), 365);
                        Current.Guid = HelperFunctions.GetCookie(HelperFunctions.cartguid);

                    }
                    else
                    {
                        Current.CustomerId = uId;
                        Current.Guid = HelperFunctions.GetCookie(HelperFunctions.cartguid);
                    }



                }
                else
                {
                    //update herer

                    PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                    Current.AnonymousName = Current.AnonymousName;
                    Current.CustomerId = uId;
                    Current.FK_ProductMaster = commentAndRating.FK_ProductMaster;
                    Current.EmailId = commentAndRating.EmailId;
                    Current.Comment = commentAndRating.Comment;
                    Current.Rate = commentAndRating.Rate;
                }

                Save();

                return Current;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public async Task<IEnumerable<CommentAndRating>> GetProductCommentbyId(long Id)
        {
            try
            {
                var obj = await _dxcontext.CommentAndRatings.Include(x => x.ProductMaster).OrderBy(x => x.CreatedOn).Where(x => x.FK_ProductMaster == Id).ToListAsync();
                //var obj = await _dxcontext.CommentAndRatings.Include(x => x.ProductMaster).OrderBy(x => x.CreatedOn).Skip(0).Take(10).Where(x => x.FK_ProductMaster == Id).ToListAsync();

                return obj;

                //return  new {
                //    obj = obj,
                //totalComment= totalComment

                //};
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public async Task<IEnumerable<CommentAndRatingVM>> GetProductCommentWithPaggination(long Id, int nextPage = 10, int prevPage = 0)
        {
            try
            {
                List<CommentAndRatingVM> commentAndRatingVM = new List<CommentAndRatingVM>();

                var totalComment = _dxcontext.CommentAndRatings.Count(x => x.FK_ProductMaster == Id);
                var commentAndRateResult = await _dxcontext.CommentAndRatings.OrderByDescending(X => X.CreatedOn).Skip(prevPage).Take(nextPage).Where(x => x.FK_ProductMaster == Id).ToListAsync();

                foreach (var item in commentAndRateResult)
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



                return commentAndRatingVM.ToList();
            }
            catch (Exception Ex)
            {



                throw Ex;
            }
        }
        public async Task<bool> DeleteComment(long Id)
        {
            try
            {
                Current = await _dxcontext.CommentAndRatings.FirstOrDefaultAsync(o => o.Id == Id);
                if (Current != null)
                    PrimaryKeyValue = Current.Id;
                Delete();

                return true;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }



        //public async Task<IEnumerable<ProductDetail>> GetProduct()
        //{
        //    try
        //    {
        //          var obj = await _dxcontext.ProductDetails.OrderByDescending(a => a.Id).ToListAsync();//  GetAll();




        //        return obj;
        //    }
        //    catch (Exception Ex)
        //    {

        //        throw Ex;
        //    }
        //}

        //public async Task<ProductDetail> GetProductDetailById(long Id)
        //{
        //    try
        //    {
        //        var obj = await GetSingleByField(a => a.Id == Id);

        //        return obj;
        //    }
        //    catch (Exception Ex)
        //    {

        //        throw Ex;
        //    }
        //}

    }
}