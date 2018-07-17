using Kapsch.Gateway.Models.Shared;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.RTE.Gateway.Clients
{
    public abstract class BaseService
    {
        public static readonly int TimeOutInMilliSeconds = 20 * 1000;
        public static string BaseEndpoint = ConfigurationManager.AppSettings["RTEGatewayEndpoint"];

        protected string _sessionToken;
        protected RestClient _restClient;

        public BaseService()
        {
        }

        public BaseService(string sessionToken)
        {
            this._sessionToken = sessionToken;
        }

        protected Exception CreateException(IRestResponse response)
        {
            try
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    dynamic o = JsonConvert.DeserializeObject(response.Content);

                    return new GatewayException((int)o.Code, (string)o.Message);
                }
            }
            catch
            {
                // Empty on purpose
            }


            if (response.ErrorException != null)
                return response.ErrorException;

            if (string.IsNullOrWhiteSpace(response.Content))
                return new Exception(response.StatusDescription);

            throw new Exception(response.Content);
        }

        public RestClient RestClient
        {
            get
            {
                if (_restClient != null)
                    return _restClient;

                _restClient = new RestClient(BaseEndpoint);
                _restClient.Timeout = TimeOutInMilliSeconds;

                if (!string.IsNullOrWhiteSpace(_sessionToken))
                {
                    _restClient.AddDefaultHeader("Authorization", string.Format("SessionToken {0}", _sessionToken));
                }

                return _restClient;
            }
        }

        public string SessionToken
        {
            get { return _sessionToken; }
        }
    }
}
