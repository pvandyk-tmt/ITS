using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Ticket
{
    public class HandWrittenCaptureModel
    {
        public string TicketNumber { get; set; }
        public long? PersonInfoID { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleNames { get; set; }
        public string Surname { get; set; }
        public string Initials { get; set; }
        public string IdentificationNumber { get; set; }
        public long? IdentificationTypeID { get; set; }
        public long? IdentificationCountryID { get; set; }
        public long? CitizenTypeID { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public DateTime? BirthDateTime { get; set; }
        public string Occupation { get; set; }
        public string Telephone { get; set; }
        public string MobileNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string BusinessTelephone { get; set; }
        public long? PhysicalAddressInfoID { get; set; }
        public long? PhysicalAddressTypeID { get; set; }
        public string PhysicalStreet1 { get; set; }
        public string PhysicalStreet2 { get; set; }
        public string PhysicalSuburb { get; set; }
        public string PhysicalTown { get; set; }
        public string PhysicalCode { get; set; }
        public long? PostalAddressInfoID { get; set; }
        public long? PostalAddressTypeID { get; set; }
        public string PostalPoBox { get; set; }
        public string PostalStreet { get; set; }
        public string PostalSuburb { get; set; }
        public string PostalTown { get; set; }
        public string PostalCode { get; set; }
        public string OffenceLocationStreet { get; set; }
        public string OffenceLocationSuburb { get; set; }
        public string OffenceLocationTown { get; set; }
        public decimal? OffenceLocationLatitude { get; set; }
        public decimal? OffenceLocationlongitude { get; set; }
        public string VehicleRegistrationMain { get; set; }
        public string VehicleRegistrationNo2 { get; set; }
        public string VehicleRegistrationNo3 { get; set; }
        public string VehicleMakeMain { get; set; }
        public string VehicleModelMain { get; set; }
        public string VehicleTypeMain { get; set; }
        public DateTime? VehicleLicenceExpiryDateTime { get; set; }
        public string VehicleColour { get; set; }
        public string VehicleRegisterNumber { get; set; }
        public string VehicleEngineNumber { get; set; }
        public string VehicleChassisNumber { get; set; }
        public string Gaurdian { get; set; }
        public string Direction { get; set; }
        public string MeterNumber { get; set; }
        public string CaseNumber { get; set; }
        public string CcNumber { get; set; }
        public string ChargeCode1 { get; set; }
        public long? ChargeCode1ID { get; set; }
        public decimal? Amount1 { get; set; }
        public string ChargeCode2 { get; set; }
        public long? ChargeCode2ID { get; set; }
        public decimal? Amount2 { get; set; }
        public string ChargeCode3 { get; set; }
        public long? ChargeCode3ID { get; set; }
        public decimal? Amount3 { get; set; }
        public int? HasAlternativeCharge { get; set; }
        public DateTime? OffenceDateTime { get; set; }
        public DateTime? IssueDateTime { get; set; }
        public DateTime? CourtDateTime { get; set; }
        public string CourtName { get; set; }
        public string CourtRoom { get; set; }
        public string DistrictName { get; set; }
        public string PaymentPlace { get; set; }
        public DateTime? PaymentDateTime { get; set; }
        public long? OfficerCredentialID { get; set; }
        public DateTime? CapturedDateTime { get; set; }
        public long? CapturedCredentialID { get; set; }
        public string LicenceCode { get; set; }
        public string LicenceType { get; set; }
        public string DriverLicenceCertificateNo { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public long? ModifiedCredentialID { get; set; }
        public int? Speed { get; set; }
        public decimal? MassPercentage { get; set; }
        public bool IsCancelled { get; set; }
        public string CancelReason { get; set; }
        public bool SendToCourtRole { get; set; }
        public string Notes { get; set; }
        public string ApplicationAndVersion { get; set; }
        public string DeviceID { get; set; }
        public string CameraID { get; set; }
        public string EventID { get; set; }
        public string InfringementLocationCode { get; set; }
        public string ExternalToken { get; set; }
        public string ExternalReference { get; set; }
        public string ChargeDescription1 { get; set; }
        public string ChargeDescription2 { get; set; }
        public string ChargeDescription3 { get; set; }
    }
}
