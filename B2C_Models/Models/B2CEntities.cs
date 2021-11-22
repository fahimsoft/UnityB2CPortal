using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API_Base.Common;

namespace B2C_Models.Models
{
    public partial class B2CEntities : DbContext
    {
        public B2CEntities(string connectionString = "")

        {
            this.Database.Connection.ConnectionString = Connection.ConnectionString;
        }
    }
}
