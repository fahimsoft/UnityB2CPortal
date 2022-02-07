using System;
using System.Collections.Generic;
using System.Linq;
using B2C_Models.Models;
using System.Text;
using System.Threading.Tasks;
using B2CPortal.Models;

namespace B2CPortal.Interfaces
{
    public interface ICity
    {
        Task<List<City>> GetCityList(string guid , int customerid);
        Task<City> CreateCity(City cart);

        Task<City> UpdateCity(City cart);

        Task<City> GetCityByIdOrName(long Id, string name);

    }
}
