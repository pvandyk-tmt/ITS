using Kapsch.Core.Gateway.Models;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.Core.Gateway.Models.Log;
using Kapsch.Core.Gateway.Models.Payment;
using Kapsch.Core.Gateway.Models.User;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class LogService : BaseService
    {
        public LogService(): base()
        {
        }

        public LogService(string sessionToken)
            : base(sessionToken)
        {
        }


        public void LogGatewayUsage(GatewayUsageLogModel model)
        {
            var request = new RestRequest("/api/Log/GatewayUsage", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }    
    }
}
