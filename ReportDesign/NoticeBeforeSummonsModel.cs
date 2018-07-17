using System;

namespace Kapsch.ITS.Gateway.Models
{
    public class NoticeBeforeSummonsModel
    {
        public string Regulation { get; set; }
        public string OffRegInfringementDate { get; set; }
        public string LocationDescription { get; set; }
        public string OffRegTicketNo { get; set; }
        public string LocationCode { get; set; }
        public string CamReference { get; set; }
        public string TicketType { get; set; }
        public string PersonName { get; set; }
        public string CompanyText { get; set; }
        public object ChargeDecription { get; set; }
        public string OffRegGuiltFineExpDate { get; set; }
        public string FineAmount { get; set; }
        public string CourtName { get; set; }
        public string PaymentRef { get; set; }
        public string RegulationText { get; set; }
        public string ChargeCode { get; set; }
        public string InspNo { get; set; }
        public string IssuedByLine1 { get; set; }
        public string IssuedByLine2 { get; set; }
        public string IssuedByLine3 { get; set; }
        public string IssuedDate { get; set; }
        public string PersonalDetails1 { get; set; }
        public string PersonalDetails2 { get; set; }
        public string PersonalDetails3 { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BranchCode { get; set; }
        public string DateOfOffence { get; set; }
        public string TimeOfOffence { get; set; }
        public string Location { get; set; }
        public string Zone { get; set; }
        public string Speed { get; set; }
        public string VehRegistration { get; set; }
        public string IDNo { get; set; }
        public string Officer { get; set; }
        public string VehicleBrand { get; set; }
        public string VehicleType { get; set; }
        public byte[] VehicleImage { get; set; }
        public byte[] VehicleNumberPlate { get; set; }
        public byte[] QrCode { get; set; }
        public string Distance { get; set; }
    }
}
