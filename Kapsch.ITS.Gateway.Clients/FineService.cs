using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Fine;
using Kapsch.ITS.Gateway.Models.Monitor;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Clients
{
    public class FineService : BaseService
    {
        public FineService(): base()
        {
        }

        public FineService(string sessionToken)
            : base(sessionToken)
        {
        }

        public IList<FineModel> GetFines(SearchCriteria searchCriteria, string searchValue, bool includeAccount, bool onlyImageEvidence, bool onlyPayable)
        {
            var request = new RestRequest("/api/Fine", Method.GET);
            request.AddQueryParameter("searchCriteria", searchCriteria.ToString());
            request.AddQueryParameter("searchValue", searchValue);
            request.AddQueryParameter("includeAccount", includeAccount.ToString());
            request.AddQueryParameter("onlyImageEvidence", onlyImageEvidence.ToString());
            request.AddQueryParameter("onlyPayable", onlyPayable.ToString());
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<FineModel>>(response.Content);
        }

        
        public static string EvidenceURL(string endpoint, long id)
        {
            string url = Path.Combine(endpoint, "api/Fine/Evidence");

            return string.Format("{0}?id={1}", url, id);
        }

        public void ChangeAmount(ChangeAmountModel model)
        {
            var request = new RestRequest("/api/Fine/Amount", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }
    }
}
