using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjetoCEEM.Models
{
    public class ElmahExceptionLogger : IExceptionFilter
    {
        public void OnException (ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                ErrorSignal.FromCurrentContext().Raise(context.Exception);
            }
        }
    }
}