using Kapsch.ITS.Gateway.Models.Adjudicate;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.ITS.Gateway.Clients
{
    public class AdjudicateService : BaseService
    {
        public AdjudicateService(): base()
        {
        }

        public AdjudicateService(string sessionToken)
            : base(sessionToken)
        {
        }

        public void UnlockCase(string ticketNumber)
        {
            var request = new RestRequest("/api/Adjudicate/Case/Unlock", Method.PUT);
            request.AddQueryParameter("ticketNumber", ticketNumber);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public FirstCaseModel GetFirstCase(string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Case/First", Method.POST);
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstCaseModel>(response.Content);
        }

        public IList<OffenceCodeModel> GetAdditionals(int offenceSet)
        {
            var request = new RestRequest("/api/Adjudicate/Case/Additionals", Method.GET);
            request.AddQueryParameter("offenceSet", offenceSet.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<OffenceCodeModel>>(response.Content);
        }

        public FirstCaseModel RejectCase(string ticketNumber, int reasonID, string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Case/Reject", Method.POST);
            request.AddQueryParameter("ticketNumber", ticketNumber);
            request.AddQueryParameter("reasonID", reasonID.ToString());
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstCaseModel>(response.Content);
        }

        public FirstCaseModel AcceptCase(CaseModel model, string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Case/Accept", Method.POST);
            request.AddQueryParameter("computerName", computerName);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstCaseModel>(response.Content);
        }

        public FishpondCaseModel GetFirstFishpondCase(CaseModel model, string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Fishpond", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FishpondCaseModel>(response.Content);
        }

        public CaseModel GetFishpondCase(string ticketNumber, string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Fishpond", Method.GET);
            request.AddQueryParameter("ticketNumber", ticketNumber);
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<CaseModel>(response.Content);
        }

        public FirstFishpondCaseModel GetFishpondCases(string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Fishpond/AllCases", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstFishpondCaseModel>(response.Content);
        }

        public void AcceptFishpondCase(CaseModel model, string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Fishpond/Accept", Method.POST);           
            request.AddQueryParameter("computerName", computerName);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public void RejectFishpondCase(string ticketNumber, int reasonID, string notes, string computerName)
        {
            var request = new RestRequest("/api/Adjudicate/Fishpond/Reject", Method.POST);
            request.AddQueryParameter("ticketNumber", ticketNumber);
            request.AddQueryParameter("reasonID", reasonID.ToString());
            request.AddQueryParameter("notes", notes);
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public void UnlockFishpondCase(string ticketNumber)
        {
            var request = new RestRequest("/api/Adjudicate/Fishpond/Unlock", Method.POST);
            request.AddQueryParameter("ticketNumber", ticketNumber);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }
    }
}
