﻿using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Interfaces
{
    public interface IAccount
    {
        Task<customer> CreateCustomer(customer customer);
        Task<customer> CreateCustomerBilling(customer customer);
        Task<customer> SelectByIdPassword(customer customer);
        Task<customer> SelectById(int id);
        Task<customer> uniqueEmailCheck(string email);
    }
}