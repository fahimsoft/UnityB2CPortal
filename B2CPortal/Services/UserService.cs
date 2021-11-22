using API_Base.Base;
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
    public class UserService : DALBase<User>, IUser
    {
        public async Task<IEnumerable<User>> GetUser()
        {
            try
            {
                var obj = await _dxcontext.Users.OrderByDescending(a => a.ID).ToListAsync();//  GetAll();
                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<User> GetUserById(long Id)
        {
            try
            {
                var obj = await GetSingleByField(a => a.ID == Id);

                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        public async Task<User> AddUser(User obj)
        {
            try
            {
                Current = await _dxcontext.Users.FirstOrDefaultAsync(o => o.ID == obj.ID);
                if (Current == null)
                {
                    New();
           
                }
                else
                {
                    PrimaryKeyValue = Current.ID;
                 
                }
                Current.Name = Current.Name;
                Current.PhoneNo = Current.PhoneNo;

                Save();

                return Current;

            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<bool> DeleteUser(long Id)
        {
            try
            {
                Current = await _dxcontext.Users.FirstOrDefaultAsync(o => o.ID == Id);
                if (Current != null)
                    PrimaryKeyValue = Current.ID;
                Delete();

                return true;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

  

    }
}