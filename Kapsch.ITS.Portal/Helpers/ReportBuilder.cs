using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.Core.Reports.Models;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Portal.Controllers;
using Kapsch.ITS.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace Kapsch.ITS.Portal.Helpers
{
    public class ReportBuilder
    {
        public static void PrepareReportParameters(ReportDefinitionModel reportDefinition, BaseController baseController)
        {
            foreach (var parameterType in reportDefinition.ParameterTypes)
            {
                switch (parameterType)
                {
                    case Core.Reports.Enums.ParameterType.Officer:
                        var userService = new UserService(baseController.AuthenticatedUser.SessionToken);
                        var filters = new List<FilterModel>();
                        filters.Add(new FilterModel { PropertyName = "IsOfficer", Operation = Operation.Equals, Value = "1" });
                        baseController.ViewBag.Users = userService.GetPaginatedList(filters, FilterJoin.And, true, "LastName", 1, 10000000).Models;
                        break;

                    case Core.Reports.Enums.ParameterType.DistrictOfficer:
                        var configurationService = new ConfigurationService(baseController.AuthenticatedUser.SessionToken);
                        filters = new List<FilterModel>();
                        baseController.ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "BranchName", 1, 10000000).Models;
                        break;

                    case Core.Reports.Enums.ParameterType.User:
                        userService = new UserService(baseController.AuthenticatedUser.SessionToken);
                        filters = new List<FilterModel>();
                        filters.Add(new FilterModel { PropertyName = "Status", Operation = Operation.Equals, Value = ((int)Status.Active).ToString() });
                        baseController.ViewBag.Users = userService.GetPaginatedList(filters, FilterJoin.And, true, "LastName", 1, 10000000).Models;
                        break;

                    case Core.Reports.Enums.ParameterType.DistrictCourt:
                        baseController.ViewBag.Districts = baseController.AuthenticatedUser.UserData.Districts;
                        break;

                    case Core.Reports.Enums.ParameterType.DistrictCourtOfficer:
                        baseController.ViewBag.Districts = baseController.AuthenticatedUser.UserData.Districts;
                        break;

                    case Core.Reports.Enums.ParameterType.DistrictCourtUser:
                        baseController.ViewBag.Districts = baseController.AuthenticatedUser.UserData.Districts;
                        break;

                    case Core.Reports.Enums.ParameterType.District:
                        baseController.ViewBag.Districts = baseController.AuthenticatedUser.UserData.Districts;
                        break;

                    case Core.Reports.Enums.ParameterType.DistrictMobileDeviceOfficer:
                        baseController.ViewBag.Districts = baseController.AuthenticatedUser.UserData.Districts;
                        break;

                    case Core.Reports.Enums.ParameterType.MobileDeviceActivityCategory:
                        MobileDeviceService mobileDeviceService = new MobileDeviceService(baseController.AuthenticatedUser.SessionToken);
                        baseController.ViewBag.MobileDeviceActivityCategories = mobileDeviceService.GetActivityCategories();
                        break;

                    case Core.Reports.Enums.ParameterType.DistrictSite:
                        baseController.ViewBag.Districts = baseController.AuthenticatedUser.UserData.Districts;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}