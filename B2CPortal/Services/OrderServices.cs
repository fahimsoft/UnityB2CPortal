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
        public async Task<OrderMaster> GetOrderMasterById(int id)
        {
            try
            {
                var obj = await _dxcontext.OrderMasters.Where(x => x.Id == id).Include(x => x.OrderDetails).FirstOrDefaultAsync();
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
                    Current.TotalPrice = Billing.OrderTotal;
                    Current.TotalQuantity = Billing.TotalQuantity;
                    Current.OrderNo= Billing.OrderNo;
                    Current.Currency = Billing.Currency;
                    Current.ConversionRate = Billing.ConversionRate;
                    Current.PaymentMode = Billing.PaymentMode;
                    Current.Status = Billing.Status;
                    Current.Country = Billing.Country;
                    Current.City = Billing.City;
                    Current.ShippingAddress = Billing.ShippingAddress;
                    Current.PaymentStatus = Billing.PaymentStatus;
                    Current.OrderDescription= Billing.OrderDescription;
                    Current.FK_CityId = Billing.FK_CityId;
                    Current.FK_ShippingDetails = Billing.FK_ShippingDetails;


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
            return await _dxcontext.OrderMasters.Where(x=>x.IsActive ==true && 
            x.FK_Customer == userid
            ).OrderByDescending(x =>x.Status.ToLower() == "inprocess").OrderByDescending(x => x.CreatedOn).Include(x=> x.City1).ToListAsync();
        }
        public async Task<OrderMaster> UpdateOrderMAster(OrderVM ordervm)
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
                    Current.PaymentMode= string.IsNullOrEmpty(ordervm.PaymentMode) ? Current.PaymentMode : ordervm.PaymentMode;
                    Current.Status= string.IsNullOrEmpty(ordervm.Status) ? Current.Status: ordervm.Status; 
                    Current.TotalPrice = (int) ordervm.TotalPrice;
                    Current.PaymentStatus = ordervm.PaymentStatus;
                    Current.FK_CityId = ordervm.FK_CityId;
                }
                Save();
                return Current;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public async Task<OrderMaster> ExestingOrder(int customerId)
        {
            try
            {
                var obj = await _dxcontext.OrderMasters.FirstOrDefaultAsync(x => x.FK_Customer == customerId
                && x.IsActive == true 
                && x.Status == "InProcess"//OrderStatus.InProcess.ToString()
                && x.PaymentMode.ToLower() != "cod"
                && x.PaymentStatus == false);
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteOrderMAster(int id)
        {
            try
            {
                Current = await _dxcontext.OrderMasters.Where(x => x.Id == id && x.IsActive == true).FirstOrDefaultAsync();
                if (Current == null)
                {
                    return false;
                    Current.CreatedOn = DateTime.Now;
                    New();
                }
                else
                {
                    PrimaryKeyValue = Current.Id;
                    Current.IsActive = false;
                Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //=================android==============================
        public async Task<OrderMaster> AndroidCreateOrder(OrderMaster Billing)
        {
            try
            {
               // Current = await _dxcontext.OrderMasters.Where(x => x.Id == Billing.Id).FirstOrDefaultAsync();
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.FK_Customer = Billing.FK_Customer;
                    Current.PhoneNo = Billing.PhoneNo;
                    Current.EmailId = Billing.EmailId;
                    Current.BillingAddress = Billing.BillingAddress;
                    Current.IsActive = true;
                    Current.TotalPrice = Billing.TotalPrice;
                    Current.TotalQuantity = Billing.TotalQuantity;
                    Current.OrderNo = Billing.OrderNo;
                    Current.Currency = Billing.Currency;
                    Current.ConversionRate = Billing.ConversionRate;
                    Current.PaymentMode = Billing.PaymentMode;
                    Current.Status = Billing.Status;
                    Current.Country = Billing.Country;
                    Current.City = Billing.City;
                    Current.ShippingAddress = Billing.ShippingAddress;
                    Current.PaymentStatus = Billing.PaymentStatus;
                    Current.OrderDescription = Billing.OrderDescription;
                    Current.FK_CityId = Billing.FK_CityId;
                    Current.FK_ShippingDetails = Billing.FK_ShippingDetails <= 0 ? null : Billing.FK_ShippingDetails;
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

        public async Task<IEnumerable<OrderMaster>> AndroidGetOrderList(int userid)
        {
            return await _dxcontext.OrderMasters.Where(x => x.IsActive == true &&
            x.FK_Customer == userid
            ).OrderByDescending(x => x.Status.ToLower() == "inprocess").OrderByDescending(x => x.CreatedOn).Include(x => x.City1).ToListAsync();
        }
    }
 
}