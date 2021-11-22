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
    public class TestService : DALBase<Test>, ITest
    {
        public async Task<Test> AddTest(Test obj)
        {
            try
            {
                Current = await _dxcontext.Tests.FirstOrDefaultAsync(o => o.ID == obj.ID);
                if (Current == null)
                {
                    New();
                    //Current.CreatedBy = obj.CreatedBy; //int.Parse(System.Web.HttpContext.Current.Session["UserId"].ToString());// obj.CreatedBy;
                    //Current.CreatedByName = obj.CreatedByName;
                    //Current.CreatedOn = DateTime.Now;
                }
                else
                {
                    PrimaryKeyValue = Current.ID;
                    //Current.ModifiedBy = obj.CreatedBy;
                    //Current.ModifiedByName = obj.CreatedByName;
                    //Current.ModifiedOn = DateTime.Now;
                }

                //Current.CountryID = obj.CountryID;
                //Current.IsActive = obj.IsActive;
                //Current.CountryName = obj.CountryName;

                Save();

                return Current;

            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }

        public async Task<bool> DeleteTest(long Id)
        {
            try
            {
                Current = await _dxcontext.Tests.FirstOrDefaultAsync(o => o.ID == Id);
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

        public async Task<Test> GetbyID(long Id)
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

        public async Task<IEnumerable<Test>> GetTest()
        {
            try
            {
                var obj = await _dxcontext.Tests.OrderByDescending(a => a.ID).ToListAsync();//  GetAll();
                return obj;
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
    }
}