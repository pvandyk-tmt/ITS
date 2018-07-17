using Kapsch.ITS.Gateway.Models.Verify;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.ITS.Gateway.Clients
{
    public class VerifyService : BaseService
    {
        public VerifyService(): base()
        {
        }

        public VerifyService(string sessionToken)
            : base(sessionToken)
        {
        }

        public IList<AddressInfoModel> GetAdditionalAddress(int personalID)
        {
            var request = new RestRequest("/api/Verify/Address", Method.GET);
            request.AddQueryParameter("personalID", personalID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<AddressInfoModel>>(response.Content);
        }

        public CaseModel CheckAddress(CaseModel model, bool isSummons)
        {
            var request = new RestRequest("/api/Verify/Address/Check", Method.POST);
            request.AddQueryParameter("isSummons", isSummons.ToString());
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<CaseModel>(response.Content);
        }

        public IList<PostalCodeModel> GetPostalCode(string city, string suburb, string code, bool isPhysical)
        {
            var request = new RestRequest("/api/Verify/PostalCode", Method.GET);
            request.AddQueryParameter("city", city);
            request.AddQueryParameter("suburb", suburb);
            request.AddQueryParameter("code", code);
            request.AddQueryParameter("isPhysical", isPhysical.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<PostalCodeModel>>(response.Content);
        }

        public FirstCaseModel GetFirstCase()
        {
            var request = new RestRequest("/api/Verify/Case/First", Method.GET);
                      
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstCaseModel>(response.Content);
        }

        public void UnlockCase(string ticketNumber)
        {
            var request = new RestRequest("/api/Verify/Case/Unlock", Method.PUT);
            request.AddQueryParameter("ticketNumber", ticketNumber);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public void UnlockSummons()
        {
            var request = new RestRequest("/api/Verify/Summons/Unlock", Method.PUT);
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public CaseModel LockSummons(CaseModel model)
        {
            var request = new RestRequest("/api/Verify/Summons/Lock", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<CaseModel>(response.Content);
        }

        public FirstCaseModel AcceptCase(CaseModel @case, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, int printImageID, string computerName)
        {
            var request = new RestRequest("/api/Verify/Case/Accept", Method.PUT);
            request.AddQueryParameter("printImageID", printImageID.ToString());
            request.AddQueryParameter("addressChanged", addressChanged.ToString());
            request.AddQueryParameter("personChanged", personChanged.ToString());
            request.AddQueryParameter("typeID", typeID.ToString());
            request.AddQueryParameter("typeAmount", typeAmount.ToString());
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstCaseModel>(response.Content);
        }

        public void AcceptFishpondCase(CaseModel @case, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, int printImageID, string computerName)
        {
            var request = new RestRequest("/api/Verify/Fishpond/Accept", Method.PUT);
            request.AddQueryParameter("printImageID", printImageID.ToString());
            request.AddQueryParameter("addressChanged", addressChanged.ToString());
            request.AddQueryParameter("personChanged", personChanged.ToString());
            request.AddQueryParameter("typeID", typeID.ToString());
            request.AddQueryParameter("typeAmount", typeAmount.ToString());
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public CaseModel AcceptSummons(CaseModel @case, bool addressChanged, bool personChanged, string computerName)
        {
            var request = new RestRequest("/api/Verify/Summons/Accept", Method.POST);
            request.AddQueryParameter("addressChanged", addressChanged.ToString());
            request.AddQueryParameter("personChanged", personChanged.ToString());
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<CaseModel>(response.Content);
        }

        public FirstCaseModel RejectCase(CaseModel @case, int reasonID, bool registrationNumberChanged, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, string computerName)
        {
            var request = new RestRequest("/api/Verify/Case/Reject", Method.PUT);
            request.AddQueryParameter("registrationNumberChanged", registrationNumberChanged.ToString());
            request.AddQueryParameter("addressChanged", addressChanged.ToString());
            request.AddQueryParameter("personChanged", personChanged.ToString());
            request.AddQueryParameter("typeID", typeID.ToString());
            request.AddQueryParameter("typeAmount", typeAmount.ToString());
            request.AddQueryParameter("reasonID", reasonID.ToString());
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstCaseModel>(response.Content);
        }

        public void RejectFishpondCase(CaseModel @case, int reasonID, bool registrationNumberChanged, bool addressChanged, bool personChanged, int typeID, decimal typeAmount, string computerName)
        {
            var request = new RestRequest("/api/Verify/Fishpond/Reject", Method.PUT);
            request.AddQueryParameter("registrationNumberChanged", registrationNumberChanged.ToString());
            request.AddQueryParameter("addressChanged", addressChanged.ToString());
            request.AddQueryParameter("personChanged", personChanged.ToString());
            request.AddQueryParameter("typeID", typeID.ToString());
            request.AddQueryParameter("typeAmount", typeAmount.ToString());
            request.AddQueryParameter("reasonID", reasonID.ToString());
            request.AddQueryParameter("computerName", computerName);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public FirstFishpondCaseModel GetFirstFishpondCase()
        {
            var request = new RestRequest("/api/Verify/Fishpond", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstFishpondCaseModel>(response.Content);
        }

        public FishpondCaseModel GetFishpondCase(string ticketNumber)
        {
            var request = new RestRequest("/api/Verify/Fishpond", Method.GET);
            request.AddQueryParameter("ticketNumber", ticketNumber);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FishpondCaseModel>(response.Content);
        }

        public void UnlockFishpondCase(string ticketNumber)
        {
            var request = new RestRequest("/api/Verify/Fishpond/Unlock", Method.PUT);
            request.AddQueryParameter("ticketNumber", ticketNumber);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }
    }
}
