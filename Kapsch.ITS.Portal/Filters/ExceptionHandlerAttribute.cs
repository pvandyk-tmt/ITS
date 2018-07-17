using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Portal.Filters
{
    public class ExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        private static ILog Log = log4net.LogManager.GetLogger(typeof(ExceptionHandlerAttribute));

        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                Log.Error(filterContext.Exception);
                Elmah.ErrorSignal.FromCurrentContext().Raise(filterContext.Exception);

                //filterContext.ExceptionHandled = true;
            }
        }
    }
}