using Kapsch.Core.Gateway.Models.Data;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class DataService : BaseService
    {
        public DataService(): base()
        {
        }

        public DataService(string sessionToken)
            : base(sessionToken)
        {
        }
     
        public DataModel Query(string query)
        {
            RestClient.Timeout = 2 * 60 * 1000;
            var request = new RestRequest("/api/Data", Method.GET);
            request.AddQueryParameter("query", query);
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<DataModel>(response.Content);
        }    
    }
}
