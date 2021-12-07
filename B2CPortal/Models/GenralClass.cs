using API_Base.Common;
using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class GenralClass
    {
        public customer UserAccount { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string email { get; set; }



    }
   
}