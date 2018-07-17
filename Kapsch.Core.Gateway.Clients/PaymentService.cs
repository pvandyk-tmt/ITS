using Kapsch.Core.Gateway.Clients.Serializer;
using Kapsch.Core.Gateway.Models.Payment;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class PaymentService : BaseService
    {
        public PaymentService(): base()
        {
        }

        public PaymentService(string sessionToken)
            : base(sessionToken)
        {
        }

        public PaymentTerminalModel CreatePaymentTerminal(PaymentTerminalModel model)
        {
            var request = new RestRequest("/api/Payment/Terminal", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaymentTerminalModel>(response.Content);
        }

        public PaymentTerminalModel GetPaymentTerminal(long id)
        {
            var request = new RestRequest("/api/Payment/Terminal", Method.GET);
            request.AddQueryParameter("id", id.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaymentTerminalModel>(response.Content);
        }

        public void UpdatePaymentTerminal(PaymentTerminalModel model)
        {
            var request = new RestRequest("/api/Payment/Terminal", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }


        public PaginationListModel<PaymentTerminalModel> GetPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/Payment/Terminal/PaginatedList", Method.POST);
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<PaymentTerminalModel>>(response.Content);
        }

        public void SettlePayment(ConfirmedPaymentModel model)
        {
            var request = new RestRequest("/api/Payment/Transaction", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public string RegisterPayment(PaymentTransactionModel model)
        {
            var request = new RestRequest("/api/Payment/Transaction", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.JsonSerializer = new RestSharpJsonNetSerializer();
            request.AddBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<string>(response.Content);
        }
    }
}
