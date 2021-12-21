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
    public class OrderTransectionService : DALBase<OrderTransection>, IOrderTransection
    {
        public async Task<OrderTransection> CreateOrderTransection(OrderTransection OrderTransection)
        {
            try
            {
                Current = await _dxcontext.OrderTransections.FirstOrDefaultAsync(x => x.ID == OrderTransection.ID);
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.StripePaymentID = OrderTransection.StripePaymentID;
                    Current.PaypalPaymentID = OrderTransection.PaypalPaymentID;
                    Current.IsActive = OrderTransection.IsActive;
                    Current.Status = OrderTransection.Status;
                    Current.FullName = OrderTransection.FullName;
                    Current.EmailId= OrderTransection.EmailId;
                    Current.PhoneNo= OrderTransection.PhoneNo;
                    Current.FK_OrderMAster= OrderTransection.FK_OrderMAster;
                    Current.FK_Customer= OrderTransection.FK_Customer;
                }
                else
                {
                    PrimaryKeyValue = Current.ID;
                    Current.IsActive = true;
                    Current.Status = OrderTransection.Status;
                    Current.StripePaymentID = OrderTransection.StripePaymentID;
                    Current.PaypalPaymentID = OrderTransection.PaypalPaymentID;
                    //Current.ModifiedOn = DateTime.Now;
                }
                Save();
                return Current;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OrderTransection> DeleteOrderTransection(int id)
        {
            Current = await _dxcontext.OrderTransections.FirstOrDefaultAsync(x => x.ID == id);
            return Current;
        }

        public  async Task<OrderTransection> GetOrderTransectionById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderTransection> UpdateOrderTransection(int  id)
        {
            throw new NotImplementedException();
        }
    }
}