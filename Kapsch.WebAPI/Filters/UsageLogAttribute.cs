using JsonDotNet.CustomContractResolvers;
using Kapsch.Core.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Kapsch.Gateway.Shared.Filters
{
    public class UsageLogAttribute : ActionFilterAttribute
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";
        private const int MaxContentLength = 4000;

        private GatewayUsageLog _logItem;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            this._logItem = new GatewayUsageLog();
            this._logItem.CreatedTimestamp = DateTime.UtcNow;

            try
            {
                var authorizationHeader = actionContext.Request.Headers.Authorization;

                this._logItem.SessionToken = (authorizationHeader != null && authorizationHeader.Scheme == "SessionToken") ? authorizationHeader.Parameter : string.Empty;
                this._logItem.ClientIPAddress = GetClientIp(actionContext.Request);
                this._logItem.Method = actionContext.ActionDescriptor.ActionName;
                this._logItem.ControllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName;

                var arguments = actionContext.ActionArguments;
                if (arguments != null && arguments.Count > 0)
                {

                    var propertiesContractResolver = new PropertiesContractResolver();
                    propertiesContractResolver.ExcludeProperties.Add("CredentialModel.Password");

                    var serializerSettings = new JsonSerializerSettings();
                    serializerSettings.ContractResolver = propertiesContractResolver;

                    var args = Newtonsoft.Json.JsonConvert.SerializeObject(arguments, serializerSettings);

                    this._logItem.Arguments = Newtonsoft.Json.JsonConvert.SerializeObject(arguments).Substring(0, args.Length >= MaxContentLength ? MaxContentLength : args.Length);
                }
            }
            catch
            {
            }
        }

        public override System.Threading.Tasks.Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, System.Threading.CancellationToken cancellationToken)
        {
            try
            {
                if (actionExecutedContext.Exception != null)
                {
                    this._logItem.Exception = 
                        string.Format("{0} {1} {2}", 
                            actionExecutedContext.Exception.Message, 
                            actionExecutedContext.Exception.InnerException, 
                            actionExecutedContext.Exception.StackTrace);

                    this._logItem.Exception = this._logItem.Exception.Substring(0, this._logItem.Exception.Length >= MaxContentLength ? MaxContentLength : this._logItem.Exception.Length); ;
                }
                else if (actionExecutedContext.Response != null && actionExecutedContext.Response.Content as ObjectContent != null)
                {
                    this._logItem.ResponseType = ((ObjectContent)actionExecutedContext.Response.Content).ObjectType.FullName;
                    
                }

                if (actionExecutedContext.Response != null)
                {
                    this._logItem.ResponseCode = (int)actionExecutedContext.Response.StatusCode;
                }
                
                this._logItem.DurationInMilliSeconds = (decimal)DateTime.UtcNow.Subtract(this._logItem.CreatedTimestamp).TotalMilliseconds;

                using (var dbContext = new DataContext())
                {
                    dbContext.GatewayUsageLogs.Add(_logItem);
                    dbContext.SaveChanges();

                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        /// <summary>
        /// Gets the client ip as per http://stackoverflow.com/questions/9565889/get-the-ip-address-of-the-remote-host
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private string GetClientIp(HttpRequestMessage request)
        {
            // Web-hosting
            if (request.Properties.ContainsKey(HttpContext))
            {
                HttpContextWrapper ctx = (HttpContextWrapper)request.Properties[HttpContext];

                if (ctx != null)
                    return ctx.Request.UserHostAddress;
            }

            // Self-hosting
            //if (request.Properties.ContainsKey(RemoteEndpointMessage))
            //{
            //    RemoteEndpointMessageProperty remoteEndpoint = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessage];

            //    if (remoteEndpoint != null)
            //        return remoteEndpoint.Address;
            //}

            //// Self-hosting using Owin
            //if (request.Properties.ContainsKey(OwinContext))
            //{
            //    OwinContext owinContext = (OwinContext)request.Properties[OwinContext];

            //    if (owinContext != null)
            //        return owinContext.Request.RemoteIpAddress;
            //}

            return null;
        }
    }
}
