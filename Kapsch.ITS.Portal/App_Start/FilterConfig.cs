using Kapsch.ITS.Portal.Filters;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Portal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionHandlerAttribute());
        }
    }
}
