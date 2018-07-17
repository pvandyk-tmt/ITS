using System;

namespace Kapsch.ITS.Gateway.Models.Verify
{
    public class CaseModel
    {
        public string TicketNo { get; set; }
        public string VehicleRegNo { get; set; }
        public bool VehicleRegNoConfirmed { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleColour { get; set; }
        public string VehicleType { get; set; }
        public string VehicleCaptureType { get; set; }
        public DateTime OffenceDate { get; set; }
        public string OffenceOldNotes { get; set; }
        public string OffenceNewNotes { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string ImageNP { get; set; }
        public int Image1ID { get; set; }
        public int Image2ID { get; set; }
        public int Image3ID { get; set; }
        public int Image4ID { get; set; }
        public bool OnlyOneImage { get; set; }
        public int PrintImageNo { get; set; }

        public int PersonKey { get; set; }
        public string PersonName { get; set; }
        public string PersonSurname { get; set; }
        public string PersonMiddleNames { get; set; }
        public string PersonTelephone { get; set; }
        public string PersonID { get; set; }
        public int PersonPhysicalAddressKey { get; set; }
        public int PersonPostalAddressKey { get; set; }

        public bool UseGismoAddress { get; set; }
        public AddressInfoModel NatisPhysical { get; set; }
        public AddressInfoModel NatisPostal { get; set; }
        public AddressInfoModel SystemPhysical { get; set; }
        public AddressInfoModel SystemPostal { get; set; }
        public bool IsNumberPlateCentralCaptured { get; set; }

        public string NumberPlateCentralPath { get; set; }
    }
}
