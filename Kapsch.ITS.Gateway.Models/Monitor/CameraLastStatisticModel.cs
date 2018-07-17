using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Monitor
{
    public class CameraLastStatisticModel
    {
        public long ID { get; set; }
        public long DeviceID { get; set; }

        public string IPAddress { get; set; }

        public string SerialNumber { get; set; }
        public string Operator { get; set; }
        public string SmdType { get; set; }
        public string SystemStatus { get; set; }
        public string LocationCode { get; set; }//CurrentSite
        public string LocationDescription { get; set; }//CurrentSiteDescription
        public string LocationType { get; set; }
        public string LocationGPS { get; set; }//" : "1024,1024",
        public long LocationZoneLight { get; set; }
        public long LocationZonePT { get; set; }
        public long LocationZoneHeavy { get; set; }
        public long LocationThresholdLight { get; set; }
        public long LocationThresholdPT { get; set; }
        public long LocationThresholdHeavy { get; set; }

        public string LastInfingementTime { get; set; }//" : "14:08:55.38",
        public long LastInfingementSpeed { get; set; }
        public long LastInfingementDistance { get; set; }
        public string LastInfingementPlate { get; set; }
        public string LastInfingementType { get; set; }

        public string LastVoSITime { get; set; }//14:11:33.81",
        public string LastVoSIPlate { get; set; }
        public string LastVoSIReason { get; set; }

        public string SessionStatisticsUptime { get; set; }//" : "4:02:58",
        public long SessionStatisticsVehicleCount { get; set; }
        public long SessionStatisticsInfringementCount { get; set; }
        public decimal SessionStatisticsInfringementRate { get; set; }//2.2,
        public long SessionStatisticsVehicleHourlyRate { get; set; }
        public long SessionStatisticsSpeedInfringementCount { get; set; }
        public long SessionStatisticsRedlightInfringementCount { get; set; }
        public long SessionStatisticsHeadwayInfringementCount { get; set; }
        public long SessionStatisticsStoplineInfringementCount { get; set; }
        public long SessionStatisticsYellowlineInfringementCount { get; set; }
        public long SessionStatisticsLineViolationCount { get; set; }
        public long SessionStatisticsEightyFivePercentileSpeed { get; set; }
        public long SessionStatisticsAverageSpeed { get; set; }
        public long SessionStatisticsStandardDeviation { get; set; }
        public long SessionStatisticsMaximumSpeed { get; set; }
        public long SessionStatisticsVoSICount { get; set; }

        public string DayStatisticsUptime { get; set; }//7:21:19",
        public long DayStatisticsVehicleCount { get; set; }
        public long DayStatisticsInfringementCount { get; set; }
        public decimal DayStatisticsInfringementRate { get; set; }// 3.2,
        public long DayStatisticsVehicleHourlyRate { get; set; }
        public long DayStatisticsSpeedInfringementCount { get; set; }
        public long DayStatisticsRedlightInfringementCount { get; set; }
        public long DayStatisticsHeadwayInfringementCount { get; set; }
        public long DayStatisticsStoplineInfringementCount { get; set; }
        public long DayStatisticsYellowlineInfringementCount { get; set; }
        public long DayStatisticsLineViolationCount { get; set; }
        public long DayStatisticsEightyFivePercentileSpeed { get; set; }
        public long DayStatisticsAverageSpeed { get; set; }
        public long DayStatisticsStandardDeviation { get; set; }
        public long DayStatisticsMaximumSpeed { get; set; }
        public long DayStatisticsVoSICount { get; set; }

        public DateTime ModifiedTimeStamp { get; set; }
    }
}
