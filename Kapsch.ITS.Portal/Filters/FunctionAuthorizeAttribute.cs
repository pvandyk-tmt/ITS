using Kapsch.ITS.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kapsch.ITS.Portal.Filters
{
    public class FunctionAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.Request.IsAuthenticated)
                return false;

            if (!(httpContext.User is AuthenticatedUser))
                return false;

            var authenticatedUser = (AuthenticatedUser)httpContext.User;
            //if (AccessRoles != null && AccessRoles.Length > 0)
            //{
            //    return AccessRoles.Contains(authenticatedUser.User.AccessRole);
            //}

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if ((filterContext.RequestContext.HttpContext.Request.Headers)["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.RequestContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            }

            //base.HandleUnauthorizedRequest(filterContext);

            filterContext.Result =
                new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Account",
                            action = "Unauthorized"
                        })
                    );
        }

        //public AccessRole[] AccessRoles { get; set; }

    }
}