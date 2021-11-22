using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2CPortal.Interfaces
{
    public interface ITest
    {
        Task<IEnumerable<Test>> GetTest();
        Task<Test> GetbyID(long Id);
        Task<Test> AddTest(Test test);
        Task<bool> DeleteTest(long Id);

    }
}
