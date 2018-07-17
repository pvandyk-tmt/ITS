using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Models.Monitor;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Clients
{
    public class MonitorService : BaseService
    {
        public MonitorService(): base()
        {
        }

        public MonitorService(string sessionToken)
            : base(sessionToken)
        {
        }

        public CameraStatusModel AddCameraStatus(CameraStatusModel model)
        {
            var request = new RestRequest("/api/Monitor/CameraStatus", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<CameraStatusModel>(response.Content);
        }

        public PaginationListModel<CameraStatusModel> GetCameraStatusPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/Monitor/CameraStatus/PaginatedList", Method.POST);
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSizex", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<CameraStatusModel>>(response.Content);
        }

        public async Task<CameraLastStatisticModel> AddCameraStatistics(CameraLastStatisticModel model)
        {
            var request = new RestRequest("/api/Monitor/CameraStatistics", Method.POST);
            request.AddJsonBody(model);

            var response = await RestClient.ExecuteTaskAsync(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<CameraLastStatisticModel>(response.Content);
        }

        public CameraLastStatisticModel GetCameraStatistics(long id)
        {
            var request = new RestRequest("/api/Monitor/CameraStatistics", Method.GET);
            request.AddQueryParameter("id", id.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<CameraLastStatisticModel>(response.Content);
        }

        public IList<CameraStatisticsModel> GetCameraStatistics(FilterCameraLastStatisticsModel filterModel)
        {
            var request = new RestRequest("/api/Monitor/CameraStatistics/Query", Method.POST);
            request.AddJsonBody(filterModel);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<CameraStatisticsModel>>(response.Content);
        }

        public CameraThumbNailModel GetCameraLastThumbnail(long deviceID)
        {
            var request = new RestRequest("/api/Monitor/CameraStatistics/Thumbnail", Method.GET);
            request.AddQueryParameter("deviceID", deviceID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            return JsonConvert.DeserializeObject<CameraThumbNailModel>(response.Content);
        }
    }
}
