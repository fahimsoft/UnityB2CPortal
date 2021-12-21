using API_Base.Base;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Services
{
    public class ShippingDetailsService : DALBase<ShippingDetail>, IShippingDetails
    {
        public async Task<ShippingDetail> CreateShippingDetail(ShippingDetail ShippingDetail)
        {
            try
            {
                Current = await _dxcontext.ShippingDetails.FirstOrDefaultAsync(x => x.Id == ShippingDetail.Id); if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.FirstName = ShippingDetail.FirstName;
                    Current.LastName= ShippingDetail.LastName;
                    Current.EmailId= ShippingDetail.EmailId;
                    Current.PhoneNo= ShippingDetail.PhoneNo;
                    Current.IsActive= ShippingDetail.IsActive;
                    Current.City= ShippingDetail.City;
                    Current.Country= ShippingDetail.Country;
                    Current.Address= ShippingDetail.Address;
                }
                else
                {
                    PrimaryKeyValue = Current.Id;
                    Current.FirstName = ShippingDetail.FirstName;
                    Current.LastName = ShippingDetail.LastName;
                    Current.EmailId = ShippingDetail.EmailId;
                    Current.PhoneNo = ShippingDetail.PhoneNo;
                    Current.IsActive = ShippingDetail.IsActive;
                    Current.City = ShippingDetail.City;
                    Current.Country = ShippingDetail.Country;
                    Current.Address = ShippingDetail.Address;
                    Current.ModifiedOn = DateTime.Now;
                }

                Save();
                return Current;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}