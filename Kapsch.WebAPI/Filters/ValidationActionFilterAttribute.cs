using System;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using Kapsch.Core;

namespace Kapsch.Gateway.Shared.Filters
{
    public class ValidationActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var modelState = actionContext.ModelState;
            
            if (!modelState.IsValid)
            {
                var errorlist = modelState.Values.Aggregate("Validation errors : ",
                     (current1, value) =>
                value.Errors.Aggregate(current1, (current, error) => current + (error.ErrorMessage)));

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, ErrorBase.PopulateUnexpectedException(new Exception(errorlist)));              
            }
        }
    }
}