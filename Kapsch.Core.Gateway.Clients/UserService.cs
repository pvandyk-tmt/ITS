using Kapsch.Core.Gateway.Models;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.Core.Gateway.Models.User;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.Core.Gateway.Clients
{
    public class UserService : BaseService
    {
        public UserService(): base()
        {
        }

        public UserService(string sessionToken)
            : base(sessionToken)
        {
        }

        public UserModel CreateUser(UserModel model)
        {
            var request = new RestRequest("/api/User", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<UserModel>(response.Content);
        }

        public void UpdateUser(UserModel model)
        {
            var request = new RestRequest("/api/User", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        
        public PaginationListModel<UserModel> GetPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/User/PaginatedList", Method.POST);
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<UserModel>>(response.Content);
        }

        public UserModel GetUser(long id)
        {
            var request = new RestRequest("/api/User", Method.GET);
            request.AddQueryParameter("id", id.ToString());
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<UserModel>(response.Content);
        }

        public void ChangePassword(ChangePasswordModel model)
        {
            var request = new RestRequest("/api/User/Password", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public void ChangePasswordWithToken(ChangePasswordWithTokenModel model)
        {
            var request = new RestRequest("/api/User/PasswordWithToken", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public void ResetPassword(ResetPasswordModel model)
        {
            var request = new RestRequest("/api/User/ResetPassword", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public IList<UserModel> GetUsersByDistrict(long districtID)
        {
            var request = new RestRequest("/api/User/District", Method.GET);
            request.AddQueryParameter("districtID", districtID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<UserModel>>(response.Content);
        }
    }
}
