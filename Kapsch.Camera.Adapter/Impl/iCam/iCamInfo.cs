using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public class Location
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string GPS { get; set; }

        [JsonProperty("Zone-Light")]
        public string ZoneLight { get; set; }

        [JsonProperty("Zone-PT")]
        public string ZonePT { get; set; }

        [JsonProperty("Zone-Heavy")]
        public string ZoneHeavy { get; set; }

        [JsonProperty("Threshold-Light")]
        public string ThresholdLight { get; set; }

        [JsonProperty("Threshold-PT")]
        public string ThresholdPT { get; set; }

        [JsonProperty("Threshold-Heavy")]
        public string ThresholdHeavy { get; set; }
    }

    public class LastInfingement
    {
        public string Time { get; set; }
        public string Speed { get; set; }
        public string Distance { get; set; }
        public string Plate { get; set; }
        public string Type { get; set; }
    }

    public class LastVoSI
    {
        public string Time { get; set; }
        public string Plate { get; set; }
        public string Reason { get; set; }
    }

    public class SessionStatistics
    {
        public string Uptime { get; set; }
        public int VehicleCount { get; set; }
        public int InfringementCount { get; set; }
        public double InfringementRate { get; set; }
        public int VehicleHourlyRate { get; set; }
        public int SpeedInfringementCount { get; set; }
        public int RedlightInfringementCount { get; set; }
        public int HeadwayInfringementCount { get; set; }
        public int StoplineInfringementCount { get; set; }
        public int YellowlineInfringementCount { get; set; }
        public int LineViolationCount { get; set; }
        public int EightyFivePercentileSpeed { get; set; }
        public int AverageSpeed { get; set; }
        public int StandardDeviation { get; set; }
        public int MaximumSpeed { get; set; }
        public int VoSICount { get; set; }
    }

    public class DayStatistics
    {
        public string Uptime { get; set; }
        public int VehicleCount { get; set; }
        public int InfringementCount { get; set; }
        public double InfringementRate { get; set; }
        public int VehicleHourlyRate { get; set; }
        public int SpeedInfringementCount { get; set; }
        public int RedlightInfringementCount { get; set; }
        public int HeadwayInfringementCount { get; set; }
        public int StoplineInfringementCount { get; set; }
        public int YellowlineInfringementCount { get; set; }
        public int LineViolationCount { get; set; }
        public int EightyFivePercentileSpeed { get; set; }
        public int AverageSpeed { get; set; }
        public int StandardDeviation { get; set; }
        public int MaximumSpeed { get; set; }
        public int VoSICount { get; set; }
    }

    public class iCamInfo
    {
        public string SerialNumber { get; set; }
        public string Operator { get; set; }
        public string SmdType { get; set; }
        public string SystemStatus { get; set; }
        public Location Location { get; set; }
        public LastInfingement LastInfingement { get; set; }
        public LastVoSI LastVoSI { get; set; }
        public SessionStatistics SessionStatistics { get; set; }
        public DayStatistics DayStatistics { get; set; }
    }
}
