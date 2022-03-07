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
    public class CityService : DALBase<City>, ICity
    {
        public async Task<List<City>> GetCityList(string guid, int customerid)
        {
            List<City> list = await _dxcontext.Cities.Where(x => x.IsActive == true).ToListAsync();
            return list;
        }
        public async Task<City> CreateCity(City city)
        {
            try
            {
                _dxcontext.Configuration.LazyLoadingEnabled = false;
                Current = await _dxcontext.Cities.FirstOrDefaultAsync(x => x.IsActive == true);
                if (Current == null)
                {
                    New();
                    Current.CreatedOn = DateTime.Now;
                    Current.CreatedBy = city.CreatedBy;
                    Current.Name = city.Name;


                }
                else
                {
                    Current.Id =  city.Id;
                    Current.ModifiedOn = DateTime.Now;
                    Current.ModifiedBy = city.ModifiedBy;
                    Current.Name = city.Name;
                    Save();
                }
                    return Current;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<City> UpdateCity(City cart)
        {
            throw new NotImplementedException();
        }

        public async Task<City> GetCityByIdOrName(long Id, string name)
        {
            var  model = await _dxcontext.Cities.Where(x => (x.IsActive == true && x.Name.ToLower() == name.ToLower())
           || (x.IsActive == true && x.Id ==  Id) ).FirstOrDefaultAsync();

            return model;
        }
    }
}