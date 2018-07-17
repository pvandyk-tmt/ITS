using Kapsch.ITS.Gateway.Models.Correspondence;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.ITS.Gateway.Clients
{
    public class DocumentService : BaseService
    {
        public DocumentService(): base()
        {
        }

        public DocumentService(string sessionToken)
            : base(sessionToken)
        {
        }

        public IList<SendResponseModel> GenerateFirstNotice(IList<string> referenceNumbers)
        {
            var request = new RestRequest("/api/Document/FirstNotice", Method.POST);
            request.AddBody(referenceNumbers);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<SendResponseModel>>(response.Content);
        }
    }
}
