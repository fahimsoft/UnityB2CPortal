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
                    Current.CreatedOn = DateTime.Now;
                    Current.FK_Customer = Billing.FK_Customer;
                    Current.PhoneNo = Billing.PhoneNo;
                    Current.EmailId = Billing.EmailId;
                    Current.BillingAddress = Billing.BillingAddress;
                    Current.IsActive = true;
                    Current.TotalPrice = (int)Billing.OrderTotal;
                    Current.TotalQuantity = Billing.TotalQuantity;
                    Current.OrderNo= Billing.OrderNo;
                    Current.Currency = Billing.Currency;
                    Current.ConversionRate = Billing.ConversionRate;
                    Current.PaymentMode = Billing.PaymentMode;
                    Current.Status = Billing.Status;
                    Current.Country = Billing.Country;
                    Current.City = Billing.City;
                    Current.ShippingAddress = Billing.ShippingAddress;
                    New();



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

        public async Task<IEnumerable<OrderMaster>> GetOrderList(int userid)
        {
            //return await _dxcontext.OrderMasters.Include(x=> x.OrderDetails).ToListAsync();
            return await _dxcontext.OrderMasters.Where(x=>x.IsActive ==true && x.FK_Customer == userid).OrderByDescending(x =>x.Id).ToListAsync();
        }

        public async Task<bool> UpdateOrderMAster(OrderVM ordervm)
        {
            try
            {
                Current = await _dxcontext.OrderMasters.Where(x => x.Id == ordervm.Id && x.IsActive == true).FirstOrDefaultAsync();
                if (Current == null)
                {
                    Current.CreatedOn = DateTime.Now;
                    New();
                }
                else
                {
                   PrimaryKeyValue = Current.Id;
                    Current.ModifiedOn = DateTime.Now;
                    Current.Currency = ordervm.Currency;
                    Current.ConversionRate = ordervm.ConversionRate;
                    Current.PaymentMode= ordervm.PaymentMode;
                    Current.Status= ordervm.Status;
                    Current.TotalPrice = ordervm.TotalPrice;
                }
                Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
    }
}