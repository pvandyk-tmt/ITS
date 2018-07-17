using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("OFFENCE_REGISTER", Schema = "ITS")]
    public class OffenceRegister
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("REGISTER_ID")]
        public long ID { get; set; }

        [Column("EVIDENCE_LOG_ID")]
        public long EvidenceLogID { get; set; }
 
        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber  { get; set; }
 
        [Column("INFRINGEMENT_TYPE_ID")]
        public InfringementType InfringementType { get; set; } 

        [Column("INFRINGEMENT_DATE")]
        public DateTime InfringementDate { get; set; }
        
        [Column("VEHICLE_REGISTRATION")]
        public string VLN  { get; set; }

        [Column("COURT_DETAIL_ID")]
        public long? CourtID  { get; set; }

        [Column("COURT_DATE")]
        public DateTime? CourtDate  { get; set; }

        [Column("COURT_NO")]
        public string CourtNumber  { get; set; }

        [Column("GUILT_FINE_EXPIRY_DATE")]
        public DateTime? ExpiryDate  { get; set; }

        [Column("OFFICER_CREDENTIAL_ID")]
        public long CredentialID  { get; set; }

        [Column("CAPTURED_AMOUNT")]
        public decimal? CapturedAmount { get; set; }

        [Column("MODIFIED_CREDENTIAL_ID")]
        public long? ModifiedCredentialID { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime? ModifiedTimestamp  { get; set; }

        [Column("WORKFLOW_ID")]
        public long WorkflowID  { get; set; }

        [Column("CASE_NO")]
        public string CaseNumber  { get; set; }

        [Column("FIRST_PRINT_DATE")]
        public DateTime? FirstPrintDate { get; set; } 

        [Column("NOTES")]
        public string Notes { get; set; }

        [Column("EDIT_REASON_ID")]
        public long? EditReasonID { get; set; }

        [Column("DISTRICT_ID")]
        public long? DistrictID { get;  set; }

        [Column("INFRINGEMENT_LOCATION_ID")]
        public long? InfringementLocationID { get; set; }

        [ForeignKey("ID")]
        public Register Register { get; set; }

        [ForeignKey("CourtID")]
        public Court Court { get; set; }

        [ForeignKey("InfringementLocationID")]
        public InfringementLocation InfringementLocation { get; set; }

        [ForeignKey("EvidenceLogID")]
        public EvidenceLog EvidenceLog { get; set; }

        [ForeignKey("CredentialID")]
        public virtual Credential Credential { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }
    }
}
