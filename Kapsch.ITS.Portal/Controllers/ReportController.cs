using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Configuration;
using Kapsch.Core.Gateway.Models.User;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.Portal.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Portal.Controllers
{
    public class ReportController : BaseController
    {
        public ActionResult Category(string categoryName)
        {
            return View(AuthenticatedUser.ReportData.ReportCategories.FirstOrDefault(f => f.CategoryName == categoryName));
        }

        // GET: Report
        public ActionResult Index(string categoryName, string subCategoryName, string reportName)
        {
            var reportDefination = AuthenticatedUser.ReportData.ReportCategories
                .SelectMany(f => f.ReportSubCategories)
                .SelectMany(f => f.ReportDefinitions)
                .FirstOrDefault(f => f.ReportName == reportName && f.CategoryName == categoryName && f.SubCategoryName == subCategoryName);

            ReportBuilder.PrepareReportParameters(reportDefination, this);

            return View(reportDefination);
        }

        public ActionResult GetOfficersByDistrict(long? districtID)
        {
            var ticketService = new TicketService(AuthenticatedUser.SessionToken);
            var userModels = new List<UserModel>();

            if (!districtID.HasValue || districtID.Value == 0)
            {
                var districts = AuthenticatedUser.UserData.Districts;
                foreach (var district in districts)
                {
                    userModels.AddRange(ticketService.GetOfficers(district.ID));
                }

                return new JsonResult { Data = userModels.GroupBy(f => f.ID).Select(f => f.First()).OrderBy(f => f.LastName).ToList(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

           
            return new JsonResult { Data = ticketService.GetOfficers(districtID.Value), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetSitesByDistrict(long? districtID)
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();
            if (!districtID.HasValue || districtID.Value == 0)
            {
                var districts = AuthenticatedUser.UserData.Districts;
                foreach (var district in districts)
                {
                    filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = district.ID });
                }

            }
            else
            {
                filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = districtID });
            }

            return new JsonResult { Data = configurationService.GetSitePaginatedList(filters, FilterJoin.And, true, "Name", 1, 10000000).Models, JsonRequestBehavior = JsonRequestBehavior.AllowGet }; 
        }

        public ActionResult GetMobileDevicesByDistrict(long? districtID)
        {          
            var userService = new MobileDeviceService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();
            if (!districtID.HasValue || districtID.Value == 0)
            {
                var districts = AuthenticatedUser.UserData.Districts;
                foreach (var district in districts)
                {
                    filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = district.ID });
                }
                
            }
            else
            {
                filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = districtID });
            }

            return new JsonResult { Data = userService.GetPaginatedList(filters, FilterJoin.And, true, "SerialNumber", 1, 10000000).Models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };         
        }

        public ActionResult GetUserByDistrict(long? districtID)
        {
            if (!districtID.HasValue || districtID.Value == 0)
            {
                var userService = new UserService(AuthenticatedUser.SessionToken);
                var filters = new List<FilterModel>();
                filters.Add(new FilterModel { PropertyName = "IsOfficer", Operation = Operation.Equals, Value = "1" });
                return new JsonResult { Data = userService.GetPaginatedList(filters, FilterJoin.And, true, "LastName", 1, 10000000).Models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            var ticketService = new TicketService(AuthenticatedUser.SessionToken);

            return new JsonResult { Data = ticketService.GetOfficers(districtID.Value), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetCourtsByDistrict(long? districtID)
        {
            if (districtID.HasValue && districtID != 0)
            {
                var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
                var filters = new List<FilterModel>();
                filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = districtID });
                return new JsonResult { Data = configurationService.GetCourtPaginatedList(filters, FilterJoin.And, true, "CourtName", 1, 10000000).Models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            var districts = AuthenticatedUser.UserData.Districts;
            var courtModels = new List<CourtModel>();

            foreach (var district in districts)
            {
                var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
                var filters = new List<FilterModel>();
                filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = district.ID });
                courtModels.AddRange(configurationService.GetCourtPaginatedList(filters, FilterJoin.And, true, "CourtName", 1, 10000000).Models);
            }

            return new JsonResult { Data = courtModels, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetUsersByDistrict(long? districtID)
        {
            if (districtID.HasValue && districtID != 0)
            {
                var userService = new UserService(AuthenticatedUser.SessionToken);
                return new JsonResult { Data = userService.GetUsersByDistrict(districtID.Value), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }

            var districts = AuthenticatedUser.UserData.Districts;
            var userModels = new List<UserModel>();

            foreach (var district in districts)
            {
                var userService = new UserService(AuthenticatedUser.SessionToken);
                userModels.AddRange(userService.GetUsersByDistrict(district.ID));
            }

            return new JsonResult { Data = userModels, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}