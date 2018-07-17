using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Operational.Models
{
    public class OffencesPerOfficerModel
    {
        public string ReferenceNumber { get; set; }
        public long? DistrictID { get; set; }
        public string DistrictName { get; set; }
        public long? CourtID { get; set; }
        public string CourtName { get; set; }
        public DateTime? CourtDate { get; set; }
        public string OffenderIDNumber { get; set; }
        public string OffenderLastName { get; set; }
        public string OffenderFirstName { get; set; }
        public string VLN { get; set; }
        public decimal? SpeedLimit { get; set; }
        public DateTime? OffenceDate { get; set; }
        public decimal? OffenceSpeed { get; set; }
        public string OffenceLocation { get; set; }
        public decimal? OffenceAmount { get; set; }
        public decimal? OutstandingAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public RegisterStatus Status { get; set; }
        public InfringementType InfringementType { get; set; }
        public long OfficerID { get; set; }
        public string OfficerName { get; set; }
        public string OfficerExternalNumber { get; set; }
        
        public string FormattedOffenceDate
        {
            get { return string.Format("{0:dd MMM yyyy HH:mm}", OffenceDate); }
        }

        public string FormattedOutstandingAmount
        {
            get { return string.Format("{0:0.00}", OutstandingAmount); }
        }

        public string FormattedInfringementType
        {
            get { return InfringementType.ToString(); }
        }
    }
}
