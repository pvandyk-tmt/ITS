using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Kapsch.ITS.Gateway.Models.Fine

{
    public class FineModel
    {
        [DisplayName("Reference Number")]
        public string ReferenceNumber { get; set; }

        public long OfficerCredentialID { get; set; }
        public string OfficerFirstName { get; set; }
        public string OfficerLastName { get; set; }

        public string ExternalID { get; set; }
        public long? DistrictID { get; set; }

        [DisplayName("District")]
        public string DistrictName { get; set; }

        [DisplayName("PaymentOptions")]
        public string PaymentOptions { get; set; }

        public long? CourtID { get; set; }

        [DisplayName("Court")]
        public string CourtName { get; set; }
        public DateTime? CourtDate { get; set; }

        [DisplayName("Offender ID Type")]
        public long OffenderIDType { get; set; }

        [DisplayName("Offender ID No.")]
        public string OffenderIDNumber { get; set; }

        [DisplayName("Offender Email")]
        public string OffenderEmail { get; set; }

        [DisplayName("Offender Mobile")]
        public string OffenderMobileNumber { get; set; }

        [DisplayName("Offender")]
        public string OffenderLastName { get; set; }

        [DisplayName("Offender")]
        public string OffenderFirstName { get; set; }

        [DisplayName("Address")]
        public string OffenderAddressLine1 { get; set; }

        [DisplayName("Address")]
        public string OffenderAddressLine2 { get; set; }

        [DisplayName("Address")]
        public string OffenderAddressSuburb { get; set; }

        [DisplayName("Address")]
        public string OffenderAddressTown { get; set; }

        [DisplayName("VLN")]
        public string VLN { get; set; }

        [DisplayName("Vehicle Make")]
        public string VehicleMake { get; set; }

        [DisplayName("Vehicle Model")]
        public string VehicleModel { get; set; }

        [DisplayName("Speed Limit")]
        public decimal? SpeedLimit { get; set; }

        [DisplayName("Offence Date")]
        public DateTime? OffenceDate { get; set; }

        [DisplayName("Offence Speed")]
        public decimal? OffenceSpeed { get; set; }

        [DisplayName("Offence Location")]
        public string OffenceLocation { get; set; }

        [DisplayName("Offence Amount")]
        public decimal? OffenceAmount { get; set; }

        [DisplayName("Outstanding Amount")]
        public decimal? OutstandingAmount { get; set; }

        [DisplayName("Paid Amount")]
        public decimal PaidAmount { get; set; }

        public DateTime? FirstPrintDate { get; set; }

        public string TransactionToken { get; set; }

        public RegisterStatus Status { get; set; }
        public IList<FineEvidenceModel> FineEvidenceModels { get; set; }
        public IList<FineChargeModel> FineChargeModels { get; set; }
        public IList<AccountTransactionModel> AccountTransactionModels { get; set; }

        public string FormattedOffenceDate
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:dd MMM yyyy HH:mm}", OffenceDate); }
        }

        public string FormattedOutstandingAmount
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", OutstandingAmount); }
        }
    }

    public class FineEvidenceModel 
    {
        public long ID { get; set; }
        public EvidenceType EvidenceType { get; set; }
        public string ReferenceNumber { get; set; }
        public string MimeType { get; set; }
    }

    public class FineChargeModel
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string SecondaryDescription { get; set; }
        public string ShortDescription { get; set; }
        public string RegulationDescription { get; set; }
        public decimal FineAmount { get; set; }
    }
}
