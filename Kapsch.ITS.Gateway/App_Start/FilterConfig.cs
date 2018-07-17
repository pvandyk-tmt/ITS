using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Gateway
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
