using Kapsch.Core.Reports.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class ReportService : BaseService
    {
        public ReportService(): base()
        {
        }

        public ReportService(string sessionToken)
            : base(sessionToken)
        {
        }

        public ReportMetaDataModel GetMetaData()
        {
            var request = new RestRequest("/api/Report/MetaData", Method.GET);
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<ReportMetaDataModel>(response.Content);
        }     
    }
}
