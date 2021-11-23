using API_Base.Base;
using B2C_Models.Models;
using B2CPortal.Interfaces;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Services
{
    public class OrderServices : DALBase<OrderMaster>, IOrders
    {
        public async Task<customer> GetCustomerById(int id)
        {
            try
            {
                var obj = await _dxcontext.customers.FirstOrDefaultAsync(x => x.Id == id);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OrderMaster> CreateOrder(OrderVM Billing)
        {
            try
            {
                Current = await _dxcontext.OrderMasters.Where(x => x.Id == Billing.Id).FirstOrDefaultAsync();
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.FK_Customer = Billing.FK_Customer;
                    Current.PhoneNo = Billing.PhoneNo;
                    Current.EmailId = Billing.EmailId;
                    Current.BillingAddress = Billing.BillingAddress;
                    Current.IsActive = true;
                    Current.TotalPrice = (int)Billing.OrderTotal;
                    Current.TotalQuantity = Billing.TotalQuantity;
                    Current.OrderNo= Billing.OrderNo;
                    Current.Status= Billing.Status;
                    
                    

                }
                else
                {
                    PrimaryKeyValue = Current.Id;
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

        public async Task<IEnumerable<OrderMaster>> GetOrderList()
        {
            //return await _dxcontext.OrderMasters.Include(x=> x.OrderDetails).ToListAsync();
            return await _dxcontext.OrderMasters.ToListAsync();
        }
    }
}