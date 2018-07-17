using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.EVR.Gateway.Models;
using Kapsch.EVR.Gateway.Models.Vehicle;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Kapsch.Core.Gateway.Models.Configuration;

namespace Kapsch.EVR.Gateway.Clients
{
    public class VehicleService : BaseService
    {
        public VehicleService(string sessionToken)
            : base(sessionToken)
        {
        }

        public VehicleTestModel GetVehicleTest(long vehicleBookingID)
        {
            var request = new RestRequest("api/Vehicle/Test", Method.GET);
            request.AddQueryParameter("vehicleBookingID", vehicleBookingID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleTestModel>(response.Content);
        }

        public VehicleMakeModel GetMake(long id)
        {
            var request = new RestRequest("api/Vehicle/Make", Method.GET);
            request.AddQueryParameter("id", id.ToString());
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleMakeModel>(response.Content);
        }

        public VehicleMakeModel AddMake(VehicleMakeModel model)
        {
            var request = new RestRequest("api/Vehicle/Make", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleMakeModel>(response.Content);
        }

        public void UpdateMake(VehicleMakeModel model)
        {
            var request = new RestRequest("api/Vehicle/Make", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public VehicleModelModel GetModel(long id)
        {
            var request = new RestRequest("api/Vehicle/Model", Method.GET);
            request.AddQueryParameter("id", id.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleModelModel>(response.Content);
        }

        public VehicleModelModel AddModel(VehicleModelModel model)
        {
            var request = new RestRequest("api/Vehicle/Model", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleModelModel>(response.Content);
        }

        public void UpdateModel(VehicleModelModel model)
        {
            var request = new RestRequest("api/Vehicle/Model", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public VehicleModelNumberModel GetModelNumber(long id)
        {
            var request = new RestRequest("api/Vehicle/ModelNumber", Method.GET);
            request.AddQueryParameter("id", id.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleModelNumberModel>(response.Content);
        }

        public VehicleModelNumberModel AddModelNumber(VehicleModelNumberModel model)
        {
            var request = new RestRequest("api/Vehicle/ModelNumber", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleModelNumberModel>(response.Content);
        }

        public void UpdateModelNumber(VehicleModelNumberModel model)
        {
            var request = new RestRequest("api/Vehicle/ModelNumber", Method.PUT);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
        }

        public VehicleBookingRecordModel AddBooking(VehicleBookingRecordModel model)
        {
            var request = new RestRequest("api/Vehicle/Booking", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleBookingRecordModel>(response.Content);
        }
        
        public List<VehicleCategoryModel> GetCategories()
        {
            var request = new RestRequest("api/Vehicle/Category", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);
           
            return JsonConvert.DeserializeObject<List<VehicleCategoryModel>>(response.Content);
        }

        public List<VehicleTypeModel> GetVehicleTypes()
        {
            var request = new RestRequest("api/Vehicle/VehicleTypes", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehicleTypeModel>>(response.Content);
        }

        public List<VehiclePropellerModel> GetPropellers()
        {
            var request = new RestRequest("api/Vehicle/Propeller", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehiclePropellerModel>>(response.Content);
        }

        public List<VehicleFuelTypeModel> GetFuelType()
        {
            var request = new RestRequest("api/Vehicle/FuelType", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehicleFuelTypeModel>>(response.Content);
        }

        public List<VehicleColorModel> GetVehicleColors()
        {
            var request = new RestRequest("api/Vehicle/Color", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehicleColorModel>>(response.Content);
        }
        
        public List<VehicleTestBookingModel> GetBookings()
        {
            var request = new RestRequest("api/Vehicle/Bookings", Method.GET);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehicleTestBookingModel>>(response.Content);
        }
        
        public List<VehicleMakeModel> GetMakes()
        {
            var request = new RestRequest("api/Vehicle/Make", Method.GET);
            //request.AddQueryParameter("vehicleMakeID", model.ID.ToString());
            //request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehicleMakeModel>>(response.Content);
        }

         public List<VehicleModelModel> GetModels(int vehicleMakeID)
        {
            var request = new RestRequest("api/Vehicle/Model", Method.GET);
            request.AddQueryParameter("vehicleMakeID", vehicleMakeID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehicleModelModel>>(response.Content);
        }

         public List<SiteModel> GetSites(int districtId)
        {
            var request = new RestRequest("api/Vehicle/Sites", Method.GET);
            request.AddQueryParameter("districtId", districtId.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<SiteModel>>(response.Content);
        }

        public List<VehicleModelNumberModel> GetModelNumbers(int vehicleModelID)
        {
            var request = new RestRequest("api/Vehicle/ModelNumber", Method.GET);
            request.AddQueryParameter("vehicleModelID", vehicleModelID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<VehicleModelNumberModel>>(response.Content);
        }


        public List<TestResultRecordModel> GetTestResultsByBookingID(int bookingID)
        {   
            var request = new RestRequest("/api/Vehicle/TestResults", Method.POST);
            request.AddQueryParameter("VehicleTestBookingID", bookingID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<TestResultRecordModel>>(response.Content);
        }

        public List<TestCategoryModel> GetTestCategories()
        {
            var request = new RestRequest("api/Vehicle/TestCategory", Method.GET);
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<TestCategoryModel>>(response.Content);
        }
        
        public QuestionAnswerResultModel AddQuestionAnswerResult(QuestionAnswerResultModel model)
        {
            var request = new RestRequest("api/Vehicle/Result", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<QuestionAnswerResultModel>(response.Content);
        }

        public PaginationListModel<VehicleModel> GetVehiclesPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/Vehicle/PaginationList", Method.POST);
            
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<VehicleModel>>(response.Content);
        }

        public PaginationListModel<VehicleMakeModel> GetVehicleMakesPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/Vehicle/PaginationList", Method.POST);

            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<VehicleMakeModel>>(response.Content);
        }

        public PaginationListModel<VehicleTestBookingModel> GetBookingsPaginatedList(IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            var request = new RestRequest("/api/Vehicle/BookingsPaginationList", Method.POST);

            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddQueryParameter("asc", asc.ToString());
            request.AddQueryParameter("orderPropertyName", orderPropertyName);
            request.AddQueryParameter("pageIndex", pageIndex.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<VehicleTestBookingModel>>(response.Content);
        }


        public PaginationListModel<TestBookingRecordModel> GetBookingTestResultsPaginatedList(BookingSearchTypeModel model)
        {
            var request = new RestRequest("/api/Vehicle/GetBookingTestResultsPaginatedList", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<PaginationListModel<TestBookingRecordModel>>(response.Content);
        }

        public List<TestBookingRecordModel> GetBookingTestResults(BookingSearchTypeModel model)
        {
            var request = new RestRequest("/api/Vehicle/GetBookingTestResults", Method.POST);
            request.AddJsonBody(model);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<List<TestBookingRecordModel>>(response.Content);
        }

        public VehicleModel GetVehicleDetail(IList<FilterModel> filters, FilterJoin filterJoin)
        {
            var request = new RestRequest("/api/Vehicle/VehicleDetail", Method.POST);
            
            request.AddQueryParameter("filterJoin", filterJoin.ToString());
            request.AddJsonBody(filters);

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<VehicleModel>(response.Content);       
        }
    }
}
