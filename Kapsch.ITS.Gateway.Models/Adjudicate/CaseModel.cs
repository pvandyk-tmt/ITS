using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Adjudicate
{
    public class CaseModel
    {
        public CaseModel()
        {
            TicketNo = string.Empty;
            OffenceSet = -1;
            Notification = string.Empty;
            Image1 = string.Empty;
            Image2 = string.Empty;
            ImageNP = string.Empty;
            RemoteImage1 = string.Empty;
            RemoteImage2 = string.Empty;
            RemoteImageNP = string.Empty;
            VehicleRegNo = string.Empty;
            VehicleRegNoConfirmed = false;
            VehicleMake = string.Empty;
            VehicleModel = string.Empty;
            VehicleColour = string.Empty;
            VehicleType = string.Empty;
            VehicleLicenseExpire = DateTime.MinValue;
            OffenceDate = DateTime.MinValue;
            OffenceSpeed = -1;
            OffenceZone = -1;
            OffenceDirectionLane = string.Empty;
            OffenceCode = -1;
            OffenceNotes = string.Empty;
            OffenceAdditionalsXml = string.Empty;
        }

        public string TicketNo { get; set; }
        public string Notification { get; set; }
        public string VehicleRegNo { get; set; }
        public bool VehicleRegNoConfirmed { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColour { get; set; }
        public string VehicleType { get; set; }
        public DateTime VehicleLicenseExpire { get; set; }
        public int OffenceSet { get; set; }
        public DateTime OffenceDate { get; set; }
        public int OffenceSpeed { get; set; }
        public int OffenceZone { get; set; }
        public string OffenceDirectionLane { get; set; }
        public int OffenceCode { get; set; }
        public string OffenceNotes { get; set; }
        public string OffenceAdditionalsXml { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string ImageNP { get; set; }
        public string NumberPlateCentralPath { get; set; }
        public string RemoteImage1 { get; set; }
        public string RemoteImage2 { get; set; }
        public string RemoteImageNP { get; set; }
    }
}
