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
    public class ExceptionHandlingService : DALBase<ExceptionHandling>, IExceptionHandling
    {
        public async Task<ExceptionHandling> CreateExceptionHandling(ExceptionHandling model)
        {
            try
            {
                Current = await _dxcontext.ExceptionHandlings.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.FK_userid = model.FK_userid;
                    Current.ErrorURL = model.ErrorURL;
                    Current.ErrorMessage = model.ErrorMessage;
                }
                else
                {
                    PrimaryKeyValue = Current.Id;
                    Current.FK_userid = model.FK_userid;
                    Current.ErrorURL = model.ErrorURL;
                    Current.ErrorMessage = model.ErrorMessage;
                }
                Save();
                return Current;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}