using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Globalization;

namespace Kapsch.ITS.Gateway.Models.Monitor
{
    public class CameraStatisticsModel
    {
        public long DeviceID { get; set; }
        public string Ip { get; set; }
        public string Name { get; set; }
        public CameraStatusType CameraStatusType { get; set; }
        public string Operator { get; set; }
        public string LastInfingementTime { get; set; }//" : "14:08:55.38",
        public long LastInfingementSpeed { get; set; }
        public long LastInfingementDistance { get; set; }
        public long SessionStatisticsVehicleCount { get; set; }
        public long SessionStatisticsInfringementCount { get; set; }
        public long SessionStatisticsVoSICount { get; set; }
        public long DayStatisticsVehicleCount { get; set; }
        public long DayStatisticsInfringementCount { get; set; }
        public long DayStatisticsVoSICount { get; set; }
        public DateTime? ModifiedTimeStamp { get; set; }


        public CameraAdapterType AdapterType { get; set; }
        public bool? ConnectToHost { get; set; }
        public decimal? GpsLatitude { get; set; }
        public decimal? GpsLongitude { get; set; }
        public CameraConnectionType DeviceConnectionType { get; set; }      
        public bool IsEnabled { get; set; }
        public long? RegionID { get; set; }
        public string RegionName { get; set; }

        public long? DistrictID { get; set; }
        public string DistrictName { get; set; }

        public long? LocationID { get; set; }
        public string LocationCode { get; set; }
        public string LocationDescription { get; set; }

        public string FormattedModifiedTimestamp
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd HH:mm}", ModifiedTimeStamp); }
        }

        public string FormattedLastInfingementTime
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd} {1}", ModifiedTimeStamp, LastInfingementTime); }
        }

        public string FormattedGps
        {
            get 
            {
                if (!GpsLatitude.HasValue)
                    return "N/A";

                return string.Format(CultureInfo.InvariantCulture, "{0:0.00}, {1:0.00}", GpsLatitude, GpsLongitude); 
            }
        }
    }
}
