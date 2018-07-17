using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.Gateway.Models.Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Portal.Controllers
{
    public class DashboardController : BaseController
    {
        public ActionResult Index()
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Regions = configurationService.GetRegionPaginatedList(filters, FilterJoin.And, true, "Name", 1, 1000000).Models;
            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "BranchName", 1, 1000000).Models;

            var monitorService = new MonitorService(AuthenticatedUser.SessionToken);
            var cameraStatisticsModels = monitorService.GetCameraStatistics(new FilterCameraLastStatisticsModel());

            return View(cameraStatisticsModels);
        }

        public ActionResult FilterCameraStatistics(long regionID, long districtID, string cameraStatusTypes)
        {
            var filterModel = new FilterCameraLastStatisticsModel();
            filterModel.RegionID = regionID == 0 ? default(long?) : regionID;
            filterModel.DistrictID = districtID == 0 ? default(long?) : districtID;
            filterModel.CameraStatusTypes = 
                cameraStatusTypes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => (CameraStatusType)int.Parse(f.Replace("statusChkBox_", string.Empty)))
                .ToList();

            var monitorService = new MonitorService(AuthenticatedUser.SessionToken);
            var cameraStatisticsModels = monitorService.GetCameraStatistics(filterModel);

            return Json(cameraStatisticsModels, JsonRequestBehavior.AllowGet);
        }

        
        public string GetDeviceLastThumbnail(long deviceID)
        {
            try
            {
                var monitorService = new MonitorService(AuthenticatedUser.SessionToken);
                var lastThumbnail = monitorService.GetCameraLastThumbnail(deviceID);

                return "data:image/jpg;base64," + lastThumbnail.Document;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return "/Images/No_image.png";
            }
        }
    }
}