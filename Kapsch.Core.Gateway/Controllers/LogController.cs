using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Models.Log;
using Kapsch.Gateway.Shared;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/Log")]
    public class LogController : BaseController
    {
        [HttpPost]
        [Route("GatewayUsage")]
        public async Task<IHttpActionResult> LogGatewayUsage(GatewayUsageLogModel model)
        {
            using (var dbContext = new DataContext())
            {
                var logItem = new GatewayUsageLog();
                logItem.CreatedTimestamp = model.CreatedTimestamp;
                logItem.SessionToken = model.SessionToken;
                logItem.ClientIPAddress = model.ClientIPAddress;
                logItem.Method = model.Method;
                logItem.ControllerName = model.ControllerName;
                logItem.Arguments = model.Arguments;
                logItem.ResponseCode = model.ResponseCode;
                logItem.Exception = model.Exception;
                logItem.ResponseType = model.ResponseType;
                logItem.DurationInMilliSeconds = model.DurationInMilliSeconds;

                dbContext.GatewayUsageLogs.Add(logItem);
                await dbContext.SaveChangesAsync();

                return Ok();
            }
        }
    }
}
