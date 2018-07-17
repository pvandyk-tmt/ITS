using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("CAMERA_LAST_STATS", Schema = "ITS")]
    public class CameraLastStatistics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("CAMERA_ID")]
        public long CameraID { get; set; }

        [Column("SERIAL_NUMBER")]
        public string SerialNumber { get; set; }

        [Column("OPERATOR")]
        public string Operator { get; set; }

        [Column("SMD_TYPE")]
        public string SmdType { get; set; }

        [Column("SYSTEM_STATUS")]
        public string SystemStatus { get; set; }

        [Column("LOCATION_CODE")]
        public string LocationCode { get; set; }

        [Column("LOCATION_DESCRIPTION")]
        public string LocationDescription { get; set; }

        [Column("LOCATION_TYPE")]
        public string LocationType { get; set; }

        [Column("LOCATION_GPS")]
        public string LocationGPS { get; set; }//" : "1024,1024",

        [Column("LOCATION_ZONE_LIGHT")]
        public long LocationZoneLight { get; set; }

        [Column("LOCATION_ZONE_PT")]
        public long LocationZonePT { get; set; }

        [Column("LOCATION_ZONE_HEAVY")]
        public long LocationZoneHeavy { get; set; }

        [Column("LOCATION_THRESHOLD_LIGHT")]
        public long LocationThresholdLight { get; set; }

        [Column("LOCATION_THRESHOLD_PT")]
        public long LocationThresholdPT { get; set; }

        [Column("LOCATION_THRESHOLD_HEAVY")]
        public long LocationThresholdHeavy { get; set; }

        [Column("LAST_INFINGEMENT_TIME")]
        public string LastInfingementTime { get; set; }//" : "14:08:55.38",

        [Column("LAST_INFINGEMENT_SPEED")]
        public long LastInfingementSpeed { get; set; }

        [Column("LAST_INFINGEMENT_DISTANCE")]
        public long LastInfingementDistance { get; set; }

        [Column("LAST_INFINGEMENT_PLATE")]
        public string LastInfingementPlate { get; set; }

        [Column("LAST_INFINGEMENT_TYPE")]
        public string LastInfingementType { get; set; }

        [Column("LAST_VOSI_TIME")]
        public string LastVoSITime { get; set; }//14:11:33.81",

        [Column("LAST_VOSI_PLATE")]
        public string LastVoSIPlate { get; set; }

        [Column("LAST_VOSI_REASON")]
        public string LastVoSIReason { get; set; }

        [Column("SESS_UPTIME")]
        public string SessionStatisticsUptime { get; set; }//" : "4:02:58",

        [Column("SESS_VEHICLE_COUNT")]
        public long SessionStatisticsVehicleCount { get; set; }

        [Column("SESS_INFR_COUNT")]
        public long SessionStatisticsInfringementCount { get; set; }

        [Column("SESS_INFR_RATE")]
        public decimal SessionStatisticsInfringementRate { get; set; }//2.2,

        [Column("SESS_VEHICLE_HOURLY_RATE")]
        public long SessionStatisticsVehicleHourlyRate { get; set; }

        [Column("SESS_SPEED_INFR_COUNT")]
        public long SessionStatisticsSpeedInfringementCount { get; set; }

        [Column("SESS_REDLIGHT_INFR_COUNT")]
        public long SessionStatisticsRedlightInfringementCount { get; set; }

        [Column("SESS_HEADWAY_INFR_COUNT")]
        public long SessionStatisticsHeadwayInfringementCount { get; set; }

        [Column("SESS_STOPLINE_INFR_COUNT")]
        public long SessionStatisticsStoplineInfringementCount { get; set; }

        [Column("SESS_YELLOWLINE_INFR_COUNT")]
        public long SessionStatisticsYellowlineInfringementCount { get; set; }

        [Column("SESS_LINEVIOLATION_COUNT")]
        public long SessionStatisticsLineViolationCount { get; set; }

        [Column("SESS_85PERC_SPEED")]
        public long SessionStatisticsEightyFivePercentileSpeed { get; set; }

        [Column("SESS_AVERAGESPEED")]
        public long SessionStatisticsAverageSpeed { get; set; }

        [Column("SESS_STANDARD_DEVIATION")]
        public long SessionStatisticsStandardDeviation { get; set; }

        [Column("SESS_MAXIMUM_SPEED")]
        public long SessionStatisticsMaximumSpeed { get; set; }

        [Column("SESS_VOSI_COUNT")]
        public long SessionStatisticsVoSICount { get; set; }

        [Column("DAY_UPTIME")]
        public string DayStatisticsUptime { get; set; }//7:21:19",

        [Column("DAY_VEHICLE_COUNT")]
        public long DayStatisticsVehicleCount { get; set; }

        [Column("DAY_INFR_COUNT")]
        public long DayStatisticsInfringementCount { get; set; }

        [Column("DAY_INFR_RATE")]
        public decimal DayStatisticsInfringementRate { get; set; }// 3.2,

        [Column("DAY_VEHICLE_HOURLY_RATE")]
        public long DayStatisticsVehicleHourlyRate { get; set; }

        [Column("DAY_SPEED_INFR_COUNT")]
        public long DayStatisticsSpeedInfringementCount { get; set; }

        [Column("DAY_REDLIGHT_INFR_COUNT")]
        public long DayStatisticsRedlightInfringementCount { get; set; }

        [Column("DAY_HEADWAY_INFR_COUNT")]
        public long DayStatisticsHeadwayInfringementCount { get; set; }

        [Column("DAY_STOPLINE_INFR_COUNT")]
        public long DayStatisticsStoplineInfringementCount { get; set; }

        [Column("DAY_YELLOWLINE_INFR_COUNT")]
        public long DayStatisticsYellowlineInfringementCount { get; set; }

        [Column("DAY_LINEVIOLATION_COUNT")]
        public long DayStatisticsLineViolationCount { get; set; }

        [Column("DAY_85PERCENT_SPEED")]
        public long DayStatisticsEightyFivePercentileSpeed { get; set; }

        [Column("DAY_AVERAGE_SPEED")]
        public long DayStatisticsAverageSpeed { get; set; }

        [Column("DAY_STANDARD_DEVIATION")]
        public long DayStatisticsStandardDeviation { get; set; }

        [Column("DAY_MAXIMUM_SPEED")]
        public long DayStatisticsMaximumSpeed { get; set; }

        [Column("DAY_VOSI_COUNT")]
        public long DayStatisticsVoSICount { get; set; }

        [Column("MODIFIED_TIMESTAMP")]
        public DateTime ModifiedTimeStamp { get; set; }

        [ForeignKey("CameraID")]
        public virtual Camera Camera { get; set; }
    }
}
