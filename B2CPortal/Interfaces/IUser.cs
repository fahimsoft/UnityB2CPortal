using System;
using System.Collections.Generic;
using System.Linq;
using B2C_Models.Models;
using System.Text;
using System.Threading.Tasks;

namespace B2CPortal.Interfaces
{
  public interface IUser
    {
        Task<IEnumerable<User>> GetUser();
        Task<User> GetUserById(long Id);
        Task<User> AddUser(User user);
        Task<bool> DeleteUser(long Id);

    }
}
