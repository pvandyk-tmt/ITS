using System.Net;
using Kapsch.RTE.Gateway.Models.Camera;
using RestSharp;

namespace Kapsch.RTE.Gateway.Clients
{
    public class OverSectionService : BaseService
    {
        public OverSectionService() : base()
        {
        }

        public OverSectionService(string sessionToken)
            : base(sessionToken)
        {
        }


        public void PostData(OverSectionModel model)
        {
            var request = new RestRequest("api/OverSection", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }
    }
}
