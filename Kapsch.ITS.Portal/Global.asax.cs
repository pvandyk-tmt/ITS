using Kapsch.ITS.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Kapsch.ITS.Portal
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static ObjectCache UserCache = new MemoryCache("User");

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_PostAuthenticateRequest()
        {
            try
            {
                var httpContext = HttpContext.Current;

                if (!httpContext.Request.IsAuthenticated)
                    return;

                var sessionToken = ((FormsIdentity)httpContext.User.Identity).Ticket.UserData;
                if (!UserCache.Contains(sessionToken))
                    return;

                httpContext.User = Thread.CurrentPrincipal = UserCache.Get(sessionToken) as AuthenticatedUser;
            }
            catch (Exception)
            {
            }
        }
    }
}
