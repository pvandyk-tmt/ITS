using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Models.TISCapture;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.ITS.Gateway.Clients
{
    public class TISService :BaseService
    {
        public TISService() : base()
        {
        }

        public TISService (string sessionToken)
            : base(sessionToken)
        {
        }

        public IList<NatisExportModel> GetExports(long numberToExport, long districtID)
        {
            var request = new RestRequest("/api/TIS/Export", Method.GET);
            request.AddQueryParameter("numberToExport", numberToExport.ToString());
            request.AddQueryParameter("districtID", districtID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<NatisExportModel>>(response.Content);
        }

        public PaginationListModel<NatisExportModel> GetNatisExportPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/TIS/TISPaginatedList", Method.POST);
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<NatisExportModel>>(response.Content);
        }

        public IList<TISDataModel> CaptureTISData (IList<TISDataModel> models)
        {
            var request = new RestRequest("/api/TIS/CaptureTISData", Method.POST);
            request.AddJsonBody(models);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<TISDataModel>>(response.Content);
        }

        public bool CaptureLock (string referenceNumber, string vehicleRegistration)
        {
            var request = new RestRequest("/api/TIS/LockCapture", Method.PUT);
            request.AddQueryParameter("referenceNumber", referenceNumber);
            request.AddQueryParameter("vehicleRegistration", vehicleRegistration);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<bool>(response.Content);
        }

        //public void UpdateTISData (TISDataModel model)
        //{
        //    var request = new RestRequest("/api/TISData/Update", Method.PUT);
        //    request.AddJsonBody(model);

        //    var response = RestClient.Execute(request);
        //    if (response.StatusCode != HttpStatusCode.OK)
        //        throw CreateException(response);
        //}
        //public TISDataModel GetTISData (long id)
        //{
        //    var request = new RestRequest("/api/TISData", Method.GET);
        //    request.AddQueryParameter("id", id.ToString());

        //    var response = RestClient.Execute(request);
        //    if (response.StatusCode != HttpStatusCode.OK)
        //        throw CreateException(response);

        //    return JsonConvert.DeserializeObject<TISDataModel>(response.Content);
        //}
    }
}
