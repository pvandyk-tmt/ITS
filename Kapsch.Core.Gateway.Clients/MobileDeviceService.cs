using Kapsch.Core.Gateway.Models.MobileDevice;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class MobileDeviceService : BaseService
    {
        public MobileDeviceService(): base()
        {
        }

        public MobileDeviceService(string sessionToken)
            : base(sessionToken)
        {
        }

        public PaginationListModel<MobileDeviceModel> GetPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/MobileDevice/PaginatedList", Method.POST);
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<MobileDeviceModel>>(response.Content);
        }

        public IList<string> GetActivityCategories()
        {
            var request = new RestRequest("/api/MobileDevice/UserMobileDeviceActivity/Category", Method.GET);
           
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<string>>(response.Content);
        }
    }
}
