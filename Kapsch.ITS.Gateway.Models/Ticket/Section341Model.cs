using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Ticket
{
    public class Section341Model
    {
        public string TicketNo { get; set; }
        public decimal PersonId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerSurname { get; set; }
        public string OwnerInit { get; set; }
        public string OwnerIdNo { get; set; }
        public string OwnerTel { get; set; }
        public string OwnerCell { get; set; }
        public string OwnerCompany { get; set; }
        public string CaseNo { get; set; }
        public string AgNo { get; set; }
        public decimal OwnerPhysId { get; set; }
        public string OwnerPhysStreet1 { get; set; }
        public string OwnerPhysStreet2 { get; set; }
        public string OwnerPhysSuburb { get; set; }
        public string OwnerPhysTown { get; set; }
        public string OwnerPhysCode { get; set; }
        public decimal OwnerPostalId { get; set; }
        public string OwnerPostalPoBox { get; set; }
        public string OwnerPostalStreet { get; set; }
        public string OwnerPostalSuburb { get; set; }
        public string OwnerPostalTown { get; set; }
        public string OwnerPostalCode { get; set; }
        public string CompanyName { get; set; }
        public decimal Telephone { get; set; }
        public string Gender { get; set; }
        public decimal Age { get; set; }
        public string Occupation { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal VehicleLocationId { get; set; }
        public string LocationDescription { get; set; }
        public string LocationTown { get; set; }
        public string LocationSuburb { get; set; }
        public string VehRegNo { get; set; }
        public string VehRegNo2 { get; set; }
        public string VehRegNo3 { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleType { get; set; }
        public string Gaurdian { get; set; }
        public string Direction { get; set; }
        public string MeterNo { get; set; }
        public string LicenceType { get; set; }
        public string MvaNo { get; set; }
        public string CcNo { get; set; }
        public decimal ChargeId { get; set; }
        public string ChargeCode1 { get; set; }
        public string ChargeCode2 { get; set; }
        public string ChargeCode3 { get; set; }
        public decimal AlternativeCharge { get; set; }
        public decimal OffenceId { get; set; }
        public DateTime CourtDate { get; set; }
        public string CourtName { get; set; }
        public string CourtNo { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Amount3 { get; set; }
        public DateTime PaymentDate { get; set; }
        public string OfficerId { get; set; }
        public DateTime CapturedDate { get; set; }
        public decimal UserId { get; set; }
        public DateTime OffenceDate { get; set; }
        public string City { get; set; }
        public string LicenceCode { get; set; }
        public string Nationality { get; set; }
        public string LocalityCode { get; set; }
        public string Classification { get; set; }
        public decimal Status { get; set; }
        public decimal IsSaCitizen { get; set; }
        public string CitizenOther { get; set; }
        public decimal Speed { get; set; }
        public string DistrictName { get; set; }
        public string PaymentPlace { get; set; }
        public decimal IsCancelled { get; set; }
        public string CancelReason { get; set; }
        public string Notes { get; set; }
        public decimal OffenceLocationLat { get; set; }
        public decimal OffenceLocationLong { get; set; }
        public string DeviceId { get; set; }
    }
}
