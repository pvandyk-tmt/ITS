using Kapsch.ITS.Gateway.Models.CameraEvent;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Kapsch.ITS.Gateway.Clients
{
    public class CameraEventService : BaseService
    {
        public CameraEventService(): base()
        {
        }

        public CameraEventService(string sessionToken)
            : base(sessionToken)
        {
        }

        public EventModel AddEvent(EventModel model)
        {
            var request = new RestRequest("/api/CameraEvent/Event", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<EventModel>(response.Content);
        }

        public EncStatisticModel AddEncStatistic(EncStatisticModel model)
        {
            var request = new RestRequest("/api/CameraEvent/EncStatistic", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<EncStatisticModel>(response.Content);
        }

        public EventMediaModel AddEvent(EventMediaModel model)
        {
            var request = new RestRequest("/api/CameraEvent/EventMedia", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<EventMediaModel>(response.Content);
        }
    }
}
