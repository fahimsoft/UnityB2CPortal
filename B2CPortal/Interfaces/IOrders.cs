using B2C_Models.Models;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Interfaces
{
    public interface IOrders
    {
        Task<customer> GetCustomerById(int Id);
        Task<OrderMaster> GetOrderMasterById(int Id);
        Task<IEnumerable<OrderMaster>> GetOrderList(int userid);

        Task<OrderMaster> CreateOrder(OrderVM Billing);
        Task<OrderMaster> UpdateOrderMAster(OrderVM Billing);
        Task<OrderMaster> ExestingOrder(int customerId);
        Task<bool> DeleteOrderMAster(int id);
        //==================androoid==============
        Task<OrderMaster> AndroidCreateOrder(OrderMaster Billing);
        Task<IEnumerable<OrderMaster>> AndroidGetOrderList(int userid);
    }
}