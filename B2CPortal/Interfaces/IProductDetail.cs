using System;
using System.Collections.Generic;
using System.Linq;
using B2C_Models.Models;
using System.Text;
using System.Threading.Tasks;

namespace B2CPortal.Interfaces
{
  public interface IProductDetail
    {
        Task<CommentAndRating> SaveComment(CommentAndRating commentAndRating);
        Task<IEnumerable<CommentAndRating>> GetProductCommentbyId(long Id);
        Task<bool> DeleteComment(long Id);
        Task<IEnumerable<CommentAndRating>> GetProductCommentWithPaggination(long Id);

    }
}
