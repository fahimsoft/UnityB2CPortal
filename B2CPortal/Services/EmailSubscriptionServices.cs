﻿using API_Base.Base;
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
    public class EmailSubscriptionServices : DALBase<EmailSubscription>, IEmailSubscription
    {
        public async Task<EmailSubscription> CreateEmailSubscription(EmailSubscription emailSubscription)
        {
            try
            {
                Current = await _dxcontext.EmailSubscriptions.FirstOrDefaultAsync(x => x.SubEmail == emailSubscription.SubEmail);
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.IsActive= emailSubscription.IsActive;
                    Current.SubEmail= emailSubscription.SubEmail;
          
                }
                else
                {
                    PrimaryKeyValue = Current.Id;
                    Current.IsActive = emailSubscription.IsActive;
                }
                Save();
                return Current;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public async Task<EmailSubscription> CreateCus(customer customer)
        //{
        //    try
        //    {
        //        Current = await _dxcontext.customers.FirstOrDefaultAsync(x => x.Id == customer.Id); if (Current == null)
        //        {
        //            New();
        //            Current.CreatedOn = DateTime.Now;
        //            Current.IsWebUser = customer.IsWebUser;
        //            Current.IsAppUser = customer.IsAppUser;
        //            Current.Guid = customer.Guid;
        //            Current.RegisteredFrom = customer.RegisteredFrom;
        //        }
        //        else
        //        {
        //            PrimaryKeyValue = Current.Id;
        //            Current.ModifiedOn = DateTime.Now;
        //        }
        //        Current.FirstName = customer.FirstName;
        //        Current.LastName = customer.LastName;
        //        Current.PhoneNo = customer.PhoneNo;
        //        Current.EmailId = customer.EmailId;
        //        Current.Password = customer.Password;
        //        Current.Gender = customer.Gender;
        //        Current.DateOfBirth = customer.DateOfBirth;
        //        Current.IsVerified = true;
        //        //Current.City = customer.City;
        //        //Current.Address = customer.Address;
        //        Save();
        //        return Current;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public async Task<customer> CreateCustomerBilling(customer customer)
        //{
        //    try
        //    {
        //        Current = await _dxcontext.customers.FirstOrDefaultAsync(x => x.Id == customer.Id);

        //        if (Current == null)
        //        {
        //            New();
        //            Current.CreatedOn = DateTime.Now;
        //        }
        //        else
        //        {
        //            PrimaryKeyValue = Current.Id;
        //            Current.ModifiedOn = DateTime.Now;

        //        }
        //        Current.FirstName = customer.FirstName;
        //        Current.LastName = customer.LastName;
        //        Current.PhoneNo = customer.PhoneNo;
        //        Current.EmailId = customer.EmailId;
        //        Current.Country = customer.Country;
        //        Current.City = customer.City;
        //        Current.Address = customer.Address;

        //        Save();
        //        return Current;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public async Task<customer> SelectByIdPassword(customer customer)
        //{
        //    try
        //    {
        //        var obj = await _dxcontext.customers.Where(x => x.EmailId == customer.EmailId && x.Password == customer.Password && x.IsVerified == true).FirstOrDefaultAsync();
        //        return obj;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public async Task<customer> SelectById(int id)
        //{
        //    try
        //    {
        //        var res = await GetSingleByField(x => x.Id == id);

        //        return res;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //public async Task<customer> uniqueEmailCheck(string email)
        //{
        //    try
        //    {
        //        var obj = await _dxcontext.customers.Where(x => x.EmailId == email).FirstOrDefaultAsync();
        //        return obj;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public async Task<customer> ResetPassword(customer customer)
        //{
        //    try
        //    {
        //        Current = await _dxcontext.customers.Where(x => x.EmailId == customer.EmailId).FirstOrDefaultAsync();
        //        PrimaryKeyValue = Current.Id;
        //        Current.Password = customer.Password;
        //        Save();
        //        return Current;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public async Task<customer> verification(string email)
        //{
        //    try
        //    {
        //        Current = await _dxcontext.customers.FirstOrDefaultAsync(x => x.EmailId == email);
        //        if (Current != null)
        //        {
        //            PrimaryKeyValue = Current.Id;
        //            Current.IsVerified = true;
        //        }
        //        Save();
        //        return Current;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

       
    }
}