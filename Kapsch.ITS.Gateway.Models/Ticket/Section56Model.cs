using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Ticket
{
    public class Section56Model
    {
        public string TicketNo { get; set; }
        public string Surname { get; set; }
        public string FirstNames { get; set; }
        public string CaseNo { get; set; }
        public string PersonIdNo { get; set; }
        public string AgNo { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string CompanyName { get; set; }
        public decimal Telephone { get; set; }
        public string Gender { get; set; }
        public decimal Age { get; set; }
        public string Occupation { get; set; }
        public DateTime IssueDate { get; set; }
        public string Location { get; set; }
        public string VehRegNo { get; set; }
        public string VehRegNo2 { get; set; }
        public string Make { get; set; }
        public string ChargeCode1 { get; set; }
        public string ChargeCode2 { get; set; }
        public string ChargeCode3 { get; set; }
        public decimal AlternativeCharge { get; set; }
        public DateTime CourtDate { get; set; }
        public string CourtName { get; set; }
        public string CourtNo { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Amount3 { get; set; }
        public DateTime PaymentDate { get; set; }
        public string OfficerId { get; set; }
        public decimal UserId { get; set; }
        public string EvsNo { get; set; }
        public string VehRegNo3 { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessTel { get; set; }
        public string BusinessPostalCode { get; set; }
        public DateTime OffenceDate { get; set; }
        public string City { get; set; }
        public string LicenceCode { get; set; }
        public string Nationality { get; set; }
        public string LocalityCode { get; set; }
        public string Classification { get; set; }
        public string Status { get; set; }
        public string CitizenType { get; set; }
        public string CitizeOther { get; set; }
        public string PoliceStation { get; set; }
        public string PoliceDistrict { get; set; }
        public string PolicePlace { get; set; }
        public string PoliceGroup { get; set; }
        public decimal SendToCourRole { get; set; }
        public decimal AddressType { get; set; }
        public decimal BusinessAddressType { get; set; }
        public string CourtRoleRejectionReason { get; set; }
        public decimal Speed { get; set; }
        public DateTime EditDate { get; set; }
        public decimal EditUser { get; set; }
        public string DistrictName { get; set; }
        public string PaymentPlace { get; set; }
        public decimal TicketYear { get; set; }
        public decimal IdNoOld { get; set; }
        public decimal MassPercentage { get; set; }
        public string DeviceId { get; set; }
        public decimal OffenceLocationLat { get; set; }
        public decimal OffenceLocationLong { get; set; }
        public string OfficerSignature { get; set; }
        public string AccusedSignature { get; set; }
        public decimal OffenceSet { get; set; }
        public decimal IsCancelled { get; set; }
        public string CancelReason { get; set; }
        public string Notes { get; set; }
    }
}
