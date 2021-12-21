using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Interfaces
{
    public interface IShippingDetails
    {
        Task<ShippingDetail> CreateShippingDetail(ShippingDetail ShippingDetail);
        //Task<ShippingDetail> CreateShippingDetailBilling(ShippingDetail ShippingDetail);
        //Task<ShippingDetail> SelectByIdPassword(ShippingDetail ShippingDetail);
        //Task<ShippingDetail> SelectById(int id);
        //Task<ShippingDetail> uniqueEmailCheck(string email);
        //Task<ShippingDetail> ResetPassword(ShippingDetail ShippingDetail);
    }
}