using Kapsch.Core.Gateway.Models.Authenticate;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class AuthenticationService : BaseService
    {
        public AuthenticationService(): base()
        {
        }

        public AuthenticationService(string sessionToken)
            : base(sessionToken)
        {
        }

        public SessionModel GetSession(CredentialModel model)
        {
            var request = new RestRequest("/api/Authentication", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<SessionModel>(response.Content);
        }

        public void RemoveSession(string sessionToken)
        {
            var request = new RestRequest("/api/Authentication", Method.DELETE);
            request.AddQueryParameter("sessionToken", sessionToken);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }
    }
}
