using B2C_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace B2CPortal.Interfaces
{
    public interface IExceptionHandling
    {
        Task<ExceptionHandling> CreateExceptionHandling(ExceptionHandling exceptionHandling);

    }
}