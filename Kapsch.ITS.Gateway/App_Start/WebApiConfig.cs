using Kapsch.ITS.Gateway.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Kapsch.ITS.Gateway
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Routes.MapHttpRoute(
            //    name: "NotFound",
            //    routeTemplate: "{*path}",
            //    defaults: new { controller = "Error", action = "NotFound" }
            //);

            config.Filters.Add(new ElmahHandleWebApiErrors());
        }
    }
}
