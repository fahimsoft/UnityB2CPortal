using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using API_Base.Common;

namespace API_Base.Base
{
    public class DALBase<T> where T : class, new()
    {
        //protected readonly db_Context _dbcontext = new db_Context();
        protected readonly B2C_Models.Models.B2CEntities _dxcontext = new B2C_Models.Models.B2CEntities(Connection.ConnectionString);   // dX_Models _dxcontext = new dX_Models();

        public DALBase()
        {
            _dxcontext.Configuration.LazyLoadingEnabled = false;
            _dxcontext.Configuration.ProxyCreationEnabled = false;

        }

        public T Current { get; set; }
        public object PrimaryKeyValue { get; set; }
        protected void New()
        {
            //_dxcontext.
            PrimaryKeyValue = null;
            Current = Activator.CreateInstance<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return _dxcontext.Set<T>().AsEnumerable();
        }

        public async Task<IList<T>> GetAllByField(Expression<Func<T, bool>> predicate)
        {
            return await _dxcontext.Set<T>().Where(predicate).ToListAsync<T>();
        }

        public async Task<T> GetSingleByField(Expression<Func<T, bool>> predicate)
        {
            return await _dxcontext.Set<T>().FirstOrDefaultAsync(predicate);
        }
        protected void Save()
        {
            try
            {
                if (PrimaryKeyValue == null)
                    _dxcontext.Set<T>().Add(Current);
                _dxcontext.SaveChanges();
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
        protected void Delete()
        {
            try
            {
                if (PrimaryKeyValue != null)
                    _dxcontext.Set<T>().Remove(Current);
                _dxcontext.SaveChangesAsync();
            }
            catch (Exception Ex)
            {

                throw Ex;
            }
        }
    }
}
