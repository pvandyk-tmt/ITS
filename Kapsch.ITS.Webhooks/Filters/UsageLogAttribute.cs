using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Log;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Webhooks.Filters
{
    public class UsageLogAttribute : ActionFilterAttribute
    {
        private static ILog Log = log4net.LogManager.GetLogger(typeof(UsageLogAttribute));
        private const int MaxContentLength = 4000;

        private GatewayUsageLogModel _logItem;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this._logItem = new GatewayUsageLogModel();
            this._logItem.CreatedTimestamp = DateTime.UtcNow;

            try
            {
                var descriptor = filterContext.ActionDescriptor;

                this._logItem.SessionToken = string.Empty;
                this._logItem.ClientIPAddress = GetClientIp(filterContext.HttpContext.Request);
                this._logItem.Method = descriptor.ActionName;
                this._logItem.ControllerName = descriptor.ControllerDescriptor.ControllerType.FullName;

                var arguments = filterContext.ActionParameters;
                if (arguments != null && arguments.Count > 0)
                {
                    var args = Newtonsoft.Json.JsonConvert.SerializeObject(arguments);

                    this._logItem.Arguments = Newtonsoft.Json.JsonConvert.SerializeObject(arguments).Substring(0, args.Length >= MaxContentLength ? MaxContentLength : args.Length); ;
                }
                else
                {
                    filterContext.HttpContext.Request.InputStream.Position = 0;
                    var reader = new StreamReader(filterContext.HttpContext.Request.InputStream);
                    {
                        this._logItem.Arguments = reader.ReadToEnd();

                        Log.Debug(this._logItem.Arguments);
                    }
                }
            }
            catch
            {
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                this._logItem.ResponseCode = 200;

                if (filterContext.Exception != null)
                {
                    this._logItem.Exception = filterContext.Exception.Message + " " + filterContext.Exception.StackTrace;
                    if (filterContext.Exception is HttpException)
                    {
                        this._logItem.ResponseCode= ((HttpException)filterContext.Exception).GetHttpCode();
                    }
                }
                else if (filterContext.Result != null)
                {
                    this._logItem.ResponseType = filterContext.Result.GetType().FullName;
                }

                this._logItem.DurationInMilliSeconds = (decimal)DateTime.UtcNow.Subtract(this._logItem.CreatedTimestamp).TotalMilliseconds;


                Task.Run(() =>
                    {
                        var logService = new LogService();
                        logService.LogGatewayUsage(this._logItem);
                    });
            }
            catch (Exception ex)
            {               
            }
        }

        private string GetClientIp(HttpRequestBase request)
        {
            // Web-hosting
            return request.UserHostAddress;
        }
    }
}