﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Base.PaymentMethod.Interfaces
{
   public interface IHBL
    {
        void CreatePayment(PaymentVM paymentViewModel);

    }
}
