using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Base.Base
{
    public class DTO
    {
        public bool isSuccessful { get; set; }
        public object data { get; set; }
        public object errors { get; set; }
    }
}
