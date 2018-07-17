using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Document
{
    public class FirstNoticeModel
    {
        public string ReferenceNumber { get; set; }

        public long OfficerCredentialID { get; set; }
        public string OfficerFirstName { get; set; }
        public string OfficerLastName { get; set; }
        public string OfficerExternalID { get; set; }

        public long? DistrictID { get; set; }

        public string DistrictName { get; set; }

        public string PaymentOptions { get; set; }

        public long? CourtID { get; set; }

        public string CourtName { get; set; }
        public DateTime? CourtDate { get; set; }
        public string OffenderCompanyName { get; set; }
        public long OffenderIDType { get; set; }

        public string OffenderIDNumber { get; set; }

        public string OffenderEmail { get; set; }

        public string OffenderMobileNumber { get; set; }

        public string OffenderLastName { get; set; }

        public string OffenderFirstName { get; set; }

        public string OffenderAddressLine1 { get; set; }

        public string OffenderAddressLine2 { get; set; }

        public string OffenderPostalCode { get; set; }

        public string VLN { get; set; }

        public string VehicleMake { get; set; }

        public string VehicleModel { get; set; }

        public decimal? SpeedZone { get; set; }

        public DateTime? OffenceDate { get; set; }

        public decimal? OffenceSpeed { get; set; }

        public string OffenceLocation { get; set; }
        public string OffenceLocationCode { get; set; }

        public decimal? OffenceAmount { get; set; }

        public decimal? OutstandingAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public DateTime? FirstPrintDate { get; set; }

        public string TransactionToken { get; set; }
        public string InfringementLocationCode { get; set; }

        public DateTime? SessionDate { get; set; }

        public string SessionIdentifier { get; set; }

        public long SessionCase { get; set; }

        public RegisterStatus Status { get; set; }
        public IList<FineEvidenceModel> FineEvidenceModels { get; set; }
        public IList<FineChargeModel> FineChargeModels { get; set; }
        public FineChargeModel PrimaryFineChargeModel { get; set; }

        public long VehicleImageID
        {
            get
            {
                var fineEvidenceModel = FineEvidenceModels.FirstOrDefault(f => f.EvidenceType == EvidenceType.VehiclePhoto && f.IsPrintImage);
                if (fineEvidenceModel == null)
                    return 0;

                return fineEvidenceModel.ID;
            }
        }

        public long NumberPlateImageID
        {
            get
            {
                var fineEvidenceModel = FineEvidenceModels.FirstOrDefault(f => f.EvidenceType == EvidenceType.VehicleNumberPlate);
                if (fineEvidenceModel == null)
                    return 0;

                return fineEvidenceModel.ID;
            }
        }

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
        public bool IsPrintImage { get; set; }
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
