using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Core.Camera.Interfaces.Models
{
    public class CameraOffence
    {
        public bool HasDecryptedData { get; set; }
        public int NumberOfFrames { get; set; }
        public int? Speed { get; set; }
        public int Zone { get; set; }
        public int ZoneL { get; set; }
        public int ZoneH { get; set; }
        public int ZonePT { get; set; }
        public int? DecodeErrorNo { get; set; }
        public string InfringementType { get; set; }
        public string DecodeErrorMessage { get; set; }
        public string ExtraInfo { get; set; }
        public string NonCriticalDisplayMessage { get; set; }
        public string CriticalDisplayMessage { get; set; }
        public string FileType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string RedTime { get; set; }
        public string AmberTime { get; set; }
        public bool IsPartOfAverageSpeed { get; set; }
        public IList<VehicleLicensePlate> VehicleLicensePlates { get; set; }
        public string Classification { get; set; }
        public DateTime? DotIntialTime { get; set; }
        public DateTime? DotFinalTime { get; set; }
        public string DvdSession { get; set; }
        public string MachineID { get; set; }
        public string LocationCode { get; set; }
        public string Session { get; set; }
        public string OperatorId { get; set; }
        public string Direction { get; set; }
        public double? Distance { get; set; }
        public string Lane { get; set; }
        public DateTime? OffenceDate  { get; set; }
        public string OffenceCode { get; set; }
        public string FileName { get; set; }
        public string SourceFileName { get; set; }
        public string OfficerCode { get; set; }
        public IList<CameraImage> CameraImages { get; set; }
    }
}
