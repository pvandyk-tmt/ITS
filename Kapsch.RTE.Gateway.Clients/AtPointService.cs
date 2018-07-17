using System.Collections.Generic;
using System.Net;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.RTE.Gateway.Models.Camera;
using Newtonsoft.Json;
using RestSharp;

namespace Kapsch.RTE.Gateway.Clients
{
    public class AtPointService : BaseService
    {
        public AtPointService() : base()
        {
        }

        public AtPointService(string sessionToken)
            : base(sessionToken)
        {
        }
        
        public PaginationListModel<AtPointModel> GetPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("api/AtPoint/GetPaginatedList", Method.POST);
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSizex", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<AtPointModel>>(response.Content);
        }

        public void Create(AtPointModel model)
        {
            var request = new RestRequest("api/AtPoint", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }
    }
}
