using Kapsch.Core.Gateway.Models.Computer;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class ComputerService : BaseService
    {
        public ComputerService(): base()
        {
        }

        public ComputerService(string sessionToken)
            : base(sessionToken)
        {
        }

        public PaginationListModel<ComputerModel> GetPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/Computer/PaginatedList", Method.POST);
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<ComputerModel>>(response.Content);
        }

        public ComputerModel GetComputer(long id)
        {
            var request = new RestRequest("/api/Computer", Method.GET);
            request.AddQueryParameter("id", id.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<ComputerModel>(response.Content);
        }

        public ComputerModel CreateComputer(ComputerModel model)
        {
            var request = new RestRequest("/api/Computer", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<ComputerModel>(response.Content);
        }

        public void UpdateComputer(ComputerModel model)
        {
            var request = new RestRequest("/api/Computer", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public ComputerConfigSettingModel CreateComputerSetting(ComputerConfigSettingModel model)
        {
            var request = new RestRequest("/api/Computer/Setting", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<ComputerConfigSettingModel>(response.Content);
        }

        public void UpdateComputerSetting(ComputerConfigSettingModel model)
        {
            var request = new RestRequest("/api/Computer/Setting", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public ComputerConfigSettingModel GetComputerSetting(long id)
        {
            var request = new RestRequest("/api/Computer/Setting", Method.GET);
            request.AddQueryParameter("id", id.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<ComputerConfigSettingModel>(response.Content);
        }
    }
}
