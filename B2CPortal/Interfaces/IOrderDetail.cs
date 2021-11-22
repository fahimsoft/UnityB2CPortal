using B2C_Models.Models;
using B2CPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Interfaces
{
    public interface IOrderDetail
    {
        Task<OrderDetail> CreateOrderDetail(OrderVM Billing);

    }
}