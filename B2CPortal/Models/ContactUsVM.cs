using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2CPortal.Models
{
    public class ContactUsVM
    {
        public string fullName { get; set; }
        public string emailaddress { get; set; }
        public string subject { get; set; }
        public string PhoneNo { get; set; }
        public string message { get; set; }
    }
}