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
    public class OrderDetailServices : DALBase<OrderDetail>, IOrderDetail
    {
        public async Task<OrderDetail> CreateOrderDetail(OrderVM Billing)
        {
            try
            {
                Current = await _dxcontext.OrderDetails.Where(x => x.Id == Billing.Id).FirstOrDefaultAsync();
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;   
                    Current.FK_OrderMaster = Billing.FK_OrderMaster;
                    Current.FK_ProductMaster = Billing.FK_ProductMaster;
                    Current.FK_Customer = Billing.FK_Customer;
                    Current.Price = Billing.Price;
                    Current.Quantity = Billing.Quantity;
                    Current.Discount = Billing.Discount;
                    Current.IsActive = true;
                    Current.TotalPrice = Billing.SubTotalPrice;
                    Current.DiscountedPrice = Billing.DiscountAmount;
                    Current.ConversionRate = Billing.ConversionRate;
                    Current.Currency = Billing.Currency;
                    Current.Tax= Billing.Tax;
                    Current.TaxAmount= Billing.TaxAmount;
                    Current.ProductName= Billing.Name;

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

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsById(int id)
        {
           return await _dxcontext.OrderDetails.Where(x => x.FK_OrderMaster == id && x.IsActive == true).ToListAsync();
        }
        public async Task<OrderDetail> UpdateOrderDetails(OrderDetailsViewModel ordervm)
        {
            try
            {
                Current = await _dxcontext.OrderDetails.Where(x => x.Id == ordervm.Id && x.IsActive == true).FirstOrDefaultAsync();
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
                    Current.Quantity = ordervm.Quantity;
                    Current.Price = ordervm.Price;
                    Current.Discount = ordervm.Discount;
                    Current.TotalPrice = ordervm.SubTotalPrice;
                    Current.DiscountedPrice = ordervm.DiscountAmount;
                    Current.Currency = ordervm.Currency;
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
        //==================android==============
        public async Task<IEnumerable<OrderDetail>> AndroidGetOrderDetailsById(int id)
        {
            return await _dxcontext.OrderDetails.Where(x => x.FK_OrderMaster == id && x.IsActive == true).ToListAsync();
        }
    }
}