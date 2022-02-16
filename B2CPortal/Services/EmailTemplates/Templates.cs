using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace B2CPortal.Services.EmailTemplates
{
    public static class Templates
    {
        public static string OrderEmail(string filePath, string name, string OrderDescription, string PhoneNo, string EmailId,
            string CreatedOn, string ShippingAddress, string BillingAddress, string PaymentMode,
            string Status, string TotalQuantity, string currency, string TotalPrice, string ordermasterId,
            string CartSubTotalDiscount, string SubTotalPrice, string recepiturl)
        {
            string FilePath = filePath;
            //string FilePath = "D:\\B2C\\B2CPortal\\B2CPortal\\B2CPortal\\Services\\EmailTemplates\\OrderEmail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            //Repalce [newusername] = signup user name   
            MailText = MailText.Replace("[name]", name);
            MailText = MailText.Replace("[orderdescription]", OrderDescription);
            MailText = MailText.Replace("[phoneno]", PhoneNo);
            MailText = MailText.Replace("[email]", EmailId);
            MailText = MailText.Replace("[orderno]", ordermasterId);
            MailText = MailText.Replace("[orderdate]", CreatedOn.ToString());
            MailText = MailText.Replace("[shippingaddress]", ShippingAddress);
            MailText = MailText.Replace("[billingaddress]", BillingAddress.ToString());
            MailText = MailText.Replace("[paymentmode]", PaymentMode.ToString());
            MailText = MailText.Replace("[paymentstatus]", Status.ToString());
            MailText = MailText.Replace("[quentity]", TotalQuantity.ToString());
            // MailText = MailText.Replace("[ordertotalamount]", Billing.orderVMs.Sum(x => x.Price).ToString());
            MailText = MailText.Replace("[totaldiscount]", CartSubTotalDiscount.ToString());
            MailText = MailText.Replace("[subtotal]", SubTotalPrice.ToString());
            MailText = MailText.Replace("[ShippingandHanding]", "0.0");
            MailText = MailText.Replace("[Vat]", "0.0");
            MailText = MailText.Replace("[Ordertotal]", currency + " " + TotalPrice.ToString());
            if (!string.IsNullOrEmpty(recepiturl))
            {
            MailText = MailText.Replace("[recepit]", "Order Receipt:  <a href='"+recepiturl+"'>click here</a>");
            }
            else
            {
                MailText = MailText.Replace("[recepit]", "");
            }



            //Fetching Email Body Text from EmailTemplate File.  
            //string FilePath = "D:\\MBK\\SendEmailByEmailTemplate\\EmailTemplates\\SignUp.html";



            //MailText = MailText.Replace("[bankname]", CreatedOn.ToString());
            //MailText = MailText.Replace("[type]", CreatedOn.ToString());
            //MailText = MailText.Replace("[recepiturl]", CreatedOn.ToString());
            //MailText = MailText.Replace("[accountno]", CreatedOn.ToString());
            return MailText;
        }
    }
}