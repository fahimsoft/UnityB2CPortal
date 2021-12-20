using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Interfaces
{
    public interface IOrderTransection
    {
        Task<OrderTransection> CreateOrderTransection(OrderTransection OrderTransection);
        Task<OrderTransection> UpdateOrderTransection(int id);
        Task<OrderTransection> DeleteOrderTransection(int id);
        Task<OrderTransection> GetOrderTransectionById(int id);

    }
}