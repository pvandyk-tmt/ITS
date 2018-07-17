using System;
using System.Net;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Enums;
using Kapsch.RTE.Gateway.Models.Configuration.Dot;
using Kapsch.RTE.Gateway.Models.Configuration.iTicket;
using Newtonsoft.Json;
using RestSharp;

namespace Kapsch.RTE.Gateway.Clients
{
    public class ConfigurationDotService : BaseService
    {
        public ConfigurationDotService(): base()
        {
        }

        public ConfigurationDotService(string sessionToken)
            : base(sessionToken)
        {
        }

        public SectionConfigurationModel GetSectionConfiguration(string sectionCodeAtPointA, string sectionCodeAtPointB)
        {
            var request = new RestRequest("api/Configuration/DOT/SectionConfiguration", Method.GET);
            request.AddQueryParameter("sectionCodeAtPointA", sectionCodeAtPointA);
            request.AddQueryParameter("sectionCodeAtPointB", sectionCodeAtPointB);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<SectionConfigurationModel>(response.Content);
        }

        public bool RegisterAdapter(SectionConfigurationModel model, ListenerTypeEnum listener, long heartBeatSeconds)
        {
            var request = new RestRequest("api/Configuration/DOT/Registration/Adapter", Method.POST);
            request.AddQueryParameter("listener", listener.ToString());
            request.AddQueryParameter("heartBeatSeconds", heartBeatSeconds.ToString());
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<bool>(response.Content);
        }

        public bool SendHeartbeatToAdapter(SectionConfigurationModel model, ListenerTypeEnum listener, long heartBeatSeconds)
        {
            var request = new RestRequest("api/Configuration/DOT/Heartbeat/Adapter", Method.POST);
            request.AddQueryParameter("listener", listener.ToString());
            request.AddQueryParameter("heartBeatSeconds", heartBeatSeconds.ToString());
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<bool>(response.Content);
        }

        public SectionConfigurationModel RegisterMobileDevice(iTicketConfigurationModel model, DateTime eventsAfter)
        {
            var request = new RestRequest("api/Configuration/DOT/Registration/MobileDevice", Method.POST);
            request.AddQueryParameter("eventsAfter", eventsAfter.ToString());
            request.AddJsonBody(model);
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<SectionConfigurationModel>(response.Content);
        }
    }
}
