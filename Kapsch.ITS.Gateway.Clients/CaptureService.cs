using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Models.Capture;
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
    public class CaptureService : BaseService
    {
        public CaptureService(): base()
        {
        }

        public CaptureService(string sessionToken)
            : base(sessionToken)
        {
        }

        public SessionsModel GetSessions()
        {
            var request = new RestRequest("/api/Capture/Session", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<SessionsModel>(response.Content);
        }

        public FirstCaseModel GetFirstCase(bool onlyNew, string computerName, SessionModel model)
        {
            var request = new RestRequest("/api/Capture/Case/First", Method.POST);
            request.AddQueryParameter("computerName", computerName);
            request.AddQueryParameter("onlyNew", onlyNew.ToString());
            request.AddJsonBody(model);
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<FirstCaseModel>(response.Content);
        }

        public void SaveNPImage (string mimeType, string mimeDataPath, string fileName, int evidenceFileNumber)
        {
            var request = new RestRequest("/api/Capture/Case/SaveNP", Method.PUT);
            request.AddQueryParameter("mimeType", mimeType);
            request.AddQueryParameter("mimeDataPath", mimeDataPath);
            request.AddQueryParameter("fileName", fileName);
            request.AddQueryParameter("evidenceFileNumber", evidenceFileNumber.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public NewCaseModel RejectCase(RejectCaseModel model, string computerName)
        {
            var request = new RestRequest("/api/Capture/Case/Reject", Method.POST);
            request.AddQueryParameter("computerName", computerName);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<NewCaseModel>(response.Content);
        }

        public NewCaseModel AcceptCase(AcceptCaseModel model, string computerName)
        {
            var request = new RestRequest("/api/Capture/Case/Accept", Method.POST);
            request.AddQueryParameter("computerName", computerName);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<NewCaseModel>(response.Content);
        }

        public SubmitCaseModel SubmitCase(string cameraDate, string cameraSessionID, string locationCode)
        {
            var request = new RestRequest("/api/Capture/Case/Submit", Method.PUT);
            request.AddQueryParameter("cameraDate", cameraDate);
            request.AddQueryParameter("cameraSessionID", cameraSessionID);
            request.AddQueryParameter("locationCode", locationCode);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<SubmitCaseModel>(response.Content);
        }

        public void UnlockCase(string cameraDate, string cameraSessionID, string locationCode)
        {
            var request = new RestRequest("/api/Capture/Case/Unlock", Method.PUT);
            request.AddQueryParameter("cameraDate", cameraDate);
            request.AddQueryParameter("cameraSessionID", cameraSessionID);
            request.AddQueryParameter("locationCode", locationCode);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public NewCaseModel PreviousCase(int offenceSetID, int fileNumber, string computerName, SessionModel model)
        {
            var request = new RestRequest("/api/Capture/Case/Previous", Method.POST);
            request.AddQueryParameter("offenceSetID", offenceSetID.ToString());
            request.AddQueryParameter("fileNumber", fileNumber.ToString());
            request.AddQueryParameter("computerName", computerName);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<NewCaseModel>(response.Content);
        }
    }
}
