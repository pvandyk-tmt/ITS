using Kapsch.Core.Data;
using Kapsch.Core.Filters;
using Kapsch.ITS.Gateway.Models.Monitor;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Core.Extensions;
using System.Globalization;
using Kapsch.Core;
using Newtonsoft.Json;
using Kapsch.Core.Gateway.Models.Enums;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Monitor")]
    [SessionAuthorize]
    public class MonitorController : BaseController
    {
        
        [ValidationActionFilter]
        [HttpPost]
        [Route("CameraStatus")]
        public IHttpActionResult PostCameraStatus([FromBody] CameraStatusModel model)
        {
            using (var dbContext = new DataContext())
            {
                var camera = dbContext.Cameras.Find(model.DeviceID);
                if (camera == null)
                    return this.BadRequestEx(Error.DeviceNotFound);

                camera.CameraStatusType = (Core.Data.Enums.CameraStatusType)model.CameraStatusType;
                camera.ModifiedTimeStamp = DateTime.Now;

                var cameraStatus = new CameraStatus();
                cameraStatus.Camera = camera;
                cameraStatus.CameraStatusType = (Core.Data.Enums.CameraStatusType) model.CameraStatusType;
                cameraStatus.CreatedTimeStamp = camera.ModifiedTimeStamp.Value;

                dbContext.DeviceStatuses.Add(cameraStatus);
                dbContext.SaveChanges();

                model.CreatedTimeStamp = cameraStatus.CreatedTimeStamp;
                
                return Ok(model);
            }
        }

        [UsageLog]
        [HttpPost]
        [Route("CameraStatus/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<CameraStatusModel>))]
        public IHttpActionResult GetPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {         
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Kapsch.Core.Data.CameraStatus>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .DeviceStatuses
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<CameraStatusModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<CameraStatusModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<CameraStatus>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);

                var paginationList = new PaginationListModel<CameraStatusModel>();
                paginationList.Models = entities.Select(f =>
                    new CameraStatusModel
                    {
                        DeviceID = f.CameraID,
                        CreatedTimeStamp = f.CreatedTimeStamp,
                        CameraStatusType = (Core.Gateway.Models.Enums.CameraStatusType)f.CameraStatusType                       
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [ValidationActionFilter]
        [UsageLog]
        [HttpPost]
        [Route("CameraStatistics")]
        public IHttpActionResult PostCameraStatistics([FromBody] CameraLastStatisticModel model)
        {
            using (var dbContext = new DataContext())
            {
                var camera = dbContext.Cameras.Find(model.DeviceID);
                if (camera == null)
                    return this.BadRequestEx(Error.DeviceNotFound);

                var infringementLocation = dbContext.InfringementLocations.SingleOrDefault(f => f.Code == model.LocationCode);
                if (infringementLocation != null)
                {
                    camera.InfringementLocationID = infringementLocation.ID;
                }
                else
                {
                    camera.InfringementLocationID = null;
                }

                if (model.LocationGPS == "1024,1024")
                {
                    if (infringementLocation != null && infringementLocation.GpsLatitude.HasValue)
                    {
                        camera.GpsLatitude = infringementLocation.GpsLatitude;
                        camera.GpsLongitude = infringementLocation.GpsLongitude;
                    }                   
                }
                else
                {
                    var splitParts = model.LocationGPS.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (splitParts != null && splitParts.Length == 2)
                    {
                        camera.GpsLatitude = decimal.Parse(splitParts[0], CultureInfo.InvariantCulture);
                        camera.GpsLongitude = decimal.Parse(splitParts[1], CultureInfo.InvariantCulture);
                    }
                }

                camera.ModifiedTimeStamp = DateTime.Now;
                dbContext.SaveChanges();

                var cameraLastStatistics = dbContext.CameraLastStatistics.SingleOrDefault(f => f.CameraID == camera.ID);
                if (cameraLastStatistics == null)
                {
                    cameraLastStatistics =
                        new CameraLastStatistics
                        {
                            CameraID = camera.ID,
                            SerialNumber = model.SerialNumber,
                            Operator = model.Operator,
                            SmdType = model.SmdType,
                            SystemStatus = model.SystemStatus,
                            LocationCode = model.LocationCode,
                            LocationDescription = model.LocationDescription,
                            LocationType = model.LocationType,
                            LocationGPS = model.LocationGPS, //" : "1024,1024",
                            LocationZoneLight = model.LocationZoneLight,
                            LocationZonePT = model.LocationZonePT,
                            LocationZoneHeavy = model.LocationZoneHeavy,
                            LocationThresholdLight = model.LocationThresholdLight,
                            LocationThresholdPT = model.LocationThresholdPT,
                            LocationThresholdHeavy = model.LocationThresholdHeavy,
                            LastInfingementTime = model.LastInfingementTime, //" : "14:08:55.38",
                            LastInfingementSpeed = model.LastInfingementSpeed,
                            LastInfingementDistance = model.LastInfingementDistance,
                            LastInfingementPlate = model.LastInfingementPlate,
                            LastInfingementType = model.LastInfingementType,
                            LastVoSITime = model.LastVoSITime, //14:11:33.81",
                            LastVoSIPlate = model.LastVoSIPlate,
                            LastVoSIReason = model.LastVoSIReason,
                            SessionStatisticsUptime = model.SessionStatisticsUptime, //" : "4:02:58",
                            SessionStatisticsVehicleCount = model.SessionStatisticsVehicleCount,
                            SessionStatisticsInfringementCount = model.SessionStatisticsInfringementCount,
                            SessionStatisticsInfringementRate = model.SessionStatisticsInfringementRate,//2.2,
                            SessionStatisticsVehicleHourlyRate = model.SessionStatisticsVehicleHourlyRate,
                            SessionStatisticsSpeedInfringementCount = model.SessionStatisticsSpeedInfringementCount,
                            SessionStatisticsRedlightInfringementCount = model.SessionStatisticsRedlightInfringementCount,
                            SessionStatisticsHeadwayInfringementCount = model.SessionStatisticsHeadwayInfringementCount,
                            SessionStatisticsStoplineInfringementCount = model.SessionStatisticsStoplineInfringementCount,
                            SessionStatisticsYellowlineInfringementCount = model.SessionStatisticsYellowlineInfringementCount,
                            SessionStatisticsLineViolationCount = model.SessionStatisticsLineViolationCount,
                            SessionStatisticsEightyFivePercentileSpeed = model.SessionStatisticsEightyFivePercentileSpeed,
                            SessionStatisticsAverageSpeed = model.SessionStatisticsAverageSpeed,
                            SessionStatisticsStandardDeviation = model.SessionStatisticsStandardDeviation,
                            SessionStatisticsMaximumSpeed = model.SessionStatisticsMaximumSpeed,
                            SessionStatisticsVoSICount = model.SessionStatisticsVoSICount,
                            DayStatisticsUptime = model.DayStatisticsUptime, //7:21:19",
                            DayStatisticsVehicleCount = model.DayStatisticsVehicleCount,
                            DayStatisticsInfringementCount = model.DayStatisticsInfringementCount,
                            DayStatisticsInfringementRate = model.DayStatisticsInfringementRate, // 3.2,
                            DayStatisticsVehicleHourlyRate = model.DayStatisticsVehicleHourlyRate,
                            DayStatisticsSpeedInfringementCount = model.DayStatisticsSpeedInfringementCount,
                            DayStatisticsRedlightInfringementCount = model.DayStatisticsRedlightInfringementCount,
                            DayStatisticsHeadwayInfringementCount = model.DayStatisticsHeadwayInfringementCount,
                            DayStatisticsStoplineInfringementCount = model.DayStatisticsStoplineInfringementCount,
                            DayStatisticsYellowlineInfringementCount = model.DayStatisticsYellowlineInfringementCount,
                            DayStatisticsLineViolationCount = model.DayStatisticsLineViolationCount,
                            DayStatisticsEightyFivePercentileSpeed = model.DayStatisticsEightyFivePercentileSpeed,
                            DayStatisticsAverageSpeed = model.DayStatisticsAverageSpeed,
                            DayStatisticsStandardDeviation = model.DayStatisticsStandardDeviation,
                            DayStatisticsMaximumSpeed = model.DayStatisticsMaximumSpeed,
                            DayStatisticsVoSICount = model.DayStatisticsVoSICount,
                            ModifiedTimeStamp = DateTime.Now
                        };

                    dbContext.CameraLastStatistics.Add(cameraLastStatistics);
                }
                else
                {
                    cameraLastStatistics.SerialNumber = model.SerialNumber;
                    cameraLastStatistics.Operator = model.Operator;
                    cameraLastStatistics.SmdType = model.SmdType;
                    cameraLastStatistics.SystemStatus = model.SystemStatus;
                    cameraLastStatistics.LocationCode = model.LocationCode;
                    cameraLastStatistics.LocationDescription = model.LocationDescription;
                    cameraLastStatistics.LocationType = model.LocationType;
                    cameraLastStatistics.LocationGPS = model.LocationGPS; //" : "1024;1024";
                    cameraLastStatistics.LocationZoneLight = model.LocationZoneLight;
                    cameraLastStatistics.LocationZonePT = model.LocationZonePT;
                    cameraLastStatistics.LocationZoneHeavy = model.LocationZoneHeavy;
                    cameraLastStatistics.LocationThresholdLight = model.LocationThresholdLight;
                    cameraLastStatistics.LocationThresholdPT = model.LocationThresholdPT;
                    cameraLastStatistics.LocationThresholdHeavy = model.LocationThresholdHeavy;
                    cameraLastStatistics.LastInfingementTime = model.LastInfingementTime; //" : "14:08:55.38";
                    cameraLastStatistics.LastInfingementSpeed = model.LastInfingementSpeed;
                    cameraLastStatistics.LastInfingementDistance = model.LastInfingementDistance;
                    cameraLastStatistics.LastInfingementPlate = model.LastInfingementPlate;
                    cameraLastStatistics.LastInfingementType = model.LastInfingementType;
                    cameraLastStatistics.LastVoSITime = model.LastVoSITime; //14:11:33.81";
                    cameraLastStatistics.LastVoSIPlate = model.LastVoSIPlate;
                    cameraLastStatistics.LastVoSIReason = model.LastVoSIReason;
                    cameraLastStatistics.SessionStatisticsUptime = model.SessionStatisticsUptime; //" : "4:02:58";
                    cameraLastStatistics.SessionStatisticsVehicleCount = model.SessionStatisticsVehicleCount;
                    cameraLastStatistics.SessionStatisticsInfringementCount = model.SessionStatisticsInfringementCount;
                    cameraLastStatistics.SessionStatisticsInfringementRate = model.SessionStatisticsInfringementRate;//2.2;
                    cameraLastStatistics.SessionStatisticsVehicleHourlyRate = model.SessionStatisticsVehicleHourlyRate;
                    cameraLastStatistics.SessionStatisticsSpeedInfringementCount = model.SessionStatisticsSpeedInfringementCount;
                    cameraLastStatistics.SessionStatisticsRedlightInfringementCount = model.SessionStatisticsRedlightInfringementCount;
                    cameraLastStatistics.SessionStatisticsHeadwayInfringementCount = model.SessionStatisticsHeadwayInfringementCount;
                    cameraLastStatistics.SessionStatisticsStoplineInfringementCount = model.SessionStatisticsStoplineInfringementCount;
                    cameraLastStatistics.SessionStatisticsYellowlineInfringementCount = model.SessionStatisticsYellowlineInfringementCount;
                    cameraLastStatistics.SessionStatisticsLineViolationCount = model.SessionStatisticsLineViolationCount;
                    cameraLastStatistics.SessionStatisticsEightyFivePercentileSpeed = model.SessionStatisticsEightyFivePercentileSpeed;
                    cameraLastStatistics.SessionStatisticsAverageSpeed = model.SessionStatisticsAverageSpeed;
                    cameraLastStatistics.SessionStatisticsStandardDeviation = model.SessionStatisticsStandardDeviation;
                    cameraLastStatistics.SessionStatisticsMaximumSpeed = model.SessionStatisticsMaximumSpeed;
                    cameraLastStatistics.SessionStatisticsVoSICount = model.SessionStatisticsVoSICount;
                    cameraLastStatistics.DayStatisticsUptime = model.DayStatisticsUptime; //7:21:19";
                    cameraLastStatistics.DayStatisticsVehicleCount = model.DayStatisticsVehicleCount;
                    cameraLastStatistics.DayStatisticsInfringementCount = model.DayStatisticsInfringementCount;
                    cameraLastStatistics.DayStatisticsInfringementRate = model.DayStatisticsInfringementRate; // 3.2;
                    cameraLastStatistics.DayStatisticsVehicleHourlyRate = model.DayStatisticsVehicleHourlyRate;
                    cameraLastStatistics.DayStatisticsSpeedInfringementCount = model.DayStatisticsSpeedInfringementCount;
                    cameraLastStatistics.DayStatisticsRedlightInfringementCount = model.DayStatisticsRedlightInfringementCount;
                    cameraLastStatistics.DayStatisticsHeadwayInfringementCount = model.DayStatisticsHeadwayInfringementCount;
                    cameraLastStatistics.DayStatisticsStoplineInfringementCount = model.DayStatisticsStoplineInfringementCount;
                    cameraLastStatistics.DayStatisticsYellowlineInfringementCount = model.DayStatisticsYellowlineInfringementCount;
                    cameraLastStatistics.DayStatisticsLineViolationCount = model.DayStatisticsLineViolationCount;
                    cameraLastStatistics.DayStatisticsEightyFivePercentileSpeed = model.DayStatisticsEightyFivePercentileSpeed;
                    cameraLastStatistics.DayStatisticsAverageSpeed = model.DayStatisticsAverageSpeed;
                    cameraLastStatistics.DayStatisticsStandardDeviation = model.DayStatisticsStandardDeviation;
                    cameraLastStatistics.DayStatisticsMaximumSpeed = model.DayStatisticsMaximumSpeed;
                    cameraLastStatistics.DayStatisticsVoSICount = model.DayStatisticsVoSICount;
                    cameraLastStatistics.ModifiedTimeStamp = DateTime.Now;
                }

                dbContext.SaveChanges();
               
                return Ok(model);
            }
        }

        [UsageLog]
        [ValidationActionFilter]
        [HttpPost]
        [Route("CameraStatistics/Query")]
        [ResponseType(typeof(IList<CameraStatisticsModel>))]
        public IHttpActionResult GetCameraStatistics([FromBody] FilterCameraLastStatisticsModel filterModel)
        {

            using (var dbContext = new DataContext())
            {
                //dbContext.Database.Log = f => System.Diagnostics.Debug.WriteLine(f);

                var query = from p in dbContext.Cameras
                            join t in dbContext.CameraLastStatistics on p.ID equals t.CameraID into g
                            from tps in g.DefaultIfEmpty()
                            orderby tps.CameraID descending
                            join l in dbContext.InfringementLocations on p.InfringementLocationID equals l.ID into g1
                            from tps1 in g1.DefaultIfEmpty()
                            orderby tps1.ID descending
                            select new { p, tps, tps1 };

                if (filterModel != null)
                {
                    if (filterModel.CameraStatusTypes != null && filterModel.CameraStatusTypes.Count > 0)
                    {
                        query = query.Where(f => filterModel.CameraStatusTypes.Contains((CameraStatusType)f.p.CameraStatusType));
                    }
                }

                var entities = query.Select(f => 
                    new
                    {
                        CameraID = f.p.ID,
                        ConfigJson = f.p.ConfigJson,
                        Name = f.p.FriendlyName,
                        AdapterType = f.p.CameraAdapterType,
                        ConnectToHost = f.p.ConnectToHost,
                        GpsLatitude = f.p.GpsLatitude,
                        GpsLongitude = f.p.GpsLongitude,
                        DeviceConnectionType = f.p.CameraConnectionType,
                        IsEnabled = f.p.IsEnabled,
                        CameraStatusType = f.p.CameraStatusType,
                        Operator = f.tps == null ? null : f.tps.Operator,
                        LastInfingementTime = f.tps == null ? null : f.tps.LastInfingementTime,
                        LastInfingementSpeed = f.tps == null ? 0 : f.tps.LastInfingementSpeed,
                        LastInfingementDistance = f.tps == null ? 0 : f.tps.LastInfingementDistance,
                        SessionStatisticsVehicleCount = f.tps == null ? 0 : f.tps.SessionStatisticsVehicleCount,
                        SessionStatisticsInfringementCount = f.tps == null ? 0 : f.tps.SessionStatisticsInfringementCount,
                        SessionStatisticsVoSICount = f.tps == null ? 0 : f.tps.SessionStatisticsVoSICount,
                        DayStatisticsVehicleCount = f.tps == null ? 0 : f.tps.DayStatisticsVehicleCount,
                        DayStatisticsInfringementCount = f.tps == null ? 0 : f.tps.DayStatisticsInfringementCount,
                        DayStatisticsVoSICount = f.tps == null ? 0 : f.tps.DayStatisticsVoSICount,
                        ModifiedTimeStamp = f.tps == null ? default(DateTime?) : f.tps.ModifiedTimeStamp,
                        InfringementLocation = f.tps1 == null ? null : f.tps1,
                        LocationCode = f.tps == null ? "" : f.tps.LocationCode,
                        LocationDescription = f.tps == null ? "" : f.tps.LocationDescription
                    });

                
                var models = new List<CameraStatisticsModel>();


                foreach (var f in entities.ToList())
                {
                    var model = 
                        new CameraStatisticsModel
                        {
                            DeviceID = f.CameraID,
                            Ip = ExtractIpAddress(f.ConfigJson),
                            Name = f.Name,
                            AdapterType = (Core.Gateway.Models.Enums.CameraAdapterType) f.AdapterType,
                            ConnectToHost = f.ConnectToHost == "1",
                            GpsLatitude = f.GpsLatitude,
                            GpsLongitude = f.GpsLongitude,
                            DeviceConnectionType = (Core.Gateway.Models.Enums.CameraConnectionType) f.DeviceConnectionType,
                            IsEnabled = f.IsEnabled == "1",
                            CameraStatusType = (Core.Gateway.Models.Enums.CameraStatusType)f.CameraStatusType,
                            Operator = f.Operator,
                            LastInfingementTime = f.LastInfingementTime,
                            LastInfingementSpeed = f.LastInfingementSpeed,
                            LastInfingementDistance = f.LastInfingementDistance,
                            SessionStatisticsVehicleCount = f.SessionStatisticsVehicleCount,
                            SessionStatisticsInfringementCount = f.SessionStatisticsInfringementCount,
                            SessionStatisticsVoSICount = f.SessionStatisticsVoSICount,
                            DayStatisticsVehicleCount = f.DayStatisticsVehicleCount,
                            DayStatisticsInfringementCount = f.DayStatisticsInfringementCount,
                            DayStatisticsVoSICount = f.DayStatisticsVoSICount,
                            ModifiedTimeStamp = f.ModifiedTimeStamp
                            
                        };

                    if (model.Name == "Hermanus(Laser V2)")
                    {
                        Console.Write("Hier");
                    }

                    if (f.InfringementLocation != null)
                    {
                        model.LocationID = f.InfringementLocation.ID;
                        model.LocationCode = f.InfringementLocation.Code;
                        model.LocationDescription = f.InfringementLocation.Description;

                        var court = f.InfringementLocation.Court;
                        if (court != null)
                        {
                            var district = court.District;
                            if (district != null)
                            {
                                model.DistrictID = district.ID;
                                model.DistrictName = district.BranchName;

                                var region = district.Region;
                                if (region != null)
                                {
                                    model.RegionID = region.ID;
                                    model.RegionName = region.Name;
                                }
                            }
                        }                                           
                    }
                    else
                    {
                        model.LocationID = default(long?);
                        model.LocationCode = string.Format("{0} (Unregistered)", f.LocationCode);
                        model.LocationDescription = f.LocationDescription;
                    }

                    models.Add(model);
                }

                if (filterModel != null)
                {
                    if (filterModel.RegionID.HasValue)
                    {
                        models = models.Where(f => f.RegionID == filterModel.RegionID).ToList();
                    }

                    if (filterModel.DistrictID.HasValue)
                    {
                        models = models.Where(f => f.DistrictID == filterModel.DistrictID).ToList();
                    }
                }

                return Ok(models);
            }
        }

        [ValidationActionFilter]
        [UsageLog]
        [HttpGet]
        [Route("CameraStatistics/Thumbnail")]      
        [ResponseType(typeof(CameraThumbNailModel))]
        public IHttpActionResult GetCameraLastThumbPrint(long deviceID)
        {
            using (var dbContext = new DataContext())
            {
                var camera = dbContext.Cameras.Find(deviceID);
                if (camera == null)
                    return this.BadRequestEx(Error.DeviceNotFound);

                var cameraLastStatistic = dbContext.CameraLastStatistics.SingleOrDefault(f => f.CameraID == deviceID);
                if (cameraLastStatistic == null)
                    return this.BadRequestEx(ErrorBase.PopulateUnexpectedException(new Exception("Statistic for device does not exist.")));

                if (string.IsNullOrWhiteSpace(cameraLastStatistic.LastInfingementTime) || cameraLastStatistic.LastInfingementTime.Contains("--"))
                    return this.BadRequestEx(ErrorBase.PopulateUnexpectedException(new Exception("No last Infrngement time recorded.")));

                var ipAddress = ExtractIpAddress(camera.ConfigJson);
                var lastInfringementTime = cameraLastStatistic.ModifiedTimeStamp.ToString("yyyy-MM-dd") + " " + cameraLastStatistic.LastInfingementTime;

                var model = new CameraThumbNailModel();
                model.DeviceID = deviceID;
                 
                // Do a 3s delay check
                for (var x = 0; x < 3; x++)
                {
                    var epoch = GetLatestEpoch(lastInfringementTime, x);
                    var document = GetImageAsBase64Url(string.Format("http://{0}/timestamp_thumb.jpg?{1}", ipAddress, epoch));

                    if (!string.IsNullOrWhiteSpace(document))
                    {
                        model.Document = document;
                        return Ok(model);
                    }
                }

                return Ok(model);
            }          
        }

        private static double GetLatestEpoch(string lastInfringementTime, int addSeconds)
        {
            var lit = DateTime.ParseExact(lastInfringementTime, "yyyy-MM-dd HH:mm:ss.ff", null).AddSeconds(addSeconds);
            var utc = lit.ToUniversalTime();

            var epoch = (utc - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            return epoch;
        }

        private static string GetImageAsBase64Url(string url)
        {
            var byte64String = string.Empty;

            using (var client = new WebClient())
            {
                var rsponseBytes = client.DownloadData(new Uri(url));
                if (rsponseBytes.Length > 0)
                {
                    return Convert.ToBase64String(rsponseBytes);
                }
            }

            return byte64String;
        }

        private string ExtractIpAddress(string jsonConfig)
        {
            try
            {
                var config = JsonConvert.DeserializeObject<dynamic>(jsonConfig);
                return config.Ip;
            }
            catch
            {
                return string.Empty;
            }           
        }
    }
}
