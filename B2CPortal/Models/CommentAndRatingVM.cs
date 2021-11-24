using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class CommentAndRatingVM
    {
         internal int totalComment;
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerComment { get; set; }
        public string Guid { get; set; }
        public int? CustomerRate { get; set; }
        public DateTime? CommentDate { get; set; }
        public bool  IsAnonymousUser { get; set; }



    }
}