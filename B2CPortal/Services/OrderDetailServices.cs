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
                    Current.Price = Billing.TotalPrice;
                    Current.Quantity = Billing.Quantity;
                    Current.Discount = Billing.Discount;
                    Current.IsActive = true;

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
           return await _dxcontext.OrderDetails.Where(x => x.FK_OrderMaster == id).ToListAsync();
        }
    }
}