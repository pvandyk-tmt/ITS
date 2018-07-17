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
    [Table("CHARGE_INFO", Schema = "ITS")]
    public class ChargeInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }
       
        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("CHARGE_NO")]
        public long ChargeNumber { get; set; }

        [Column("OFFENCE_CODES_ID")]
        public long OffenceCodesID { get; set; }

        [Column("PRIMARY_DESCRIPTION")]
        public string PrimaryDescription { get; set; }

        [Column("GUILT_FINE_AMOUNT")]
        public decimal? GuiltyFineAmount { get; set; }

        [Column("CREDENTIAL_ID")]
        public long CredentialID { get; set; }

        [Column("EDIT_DATE")]
        public DateTime? EditDate { get; set; }

        [Column("SECONDARY_DESCRIPTION")]
        public string SecondaryDescription { get; set; }

        [Column("SHORT_DESCRIPTION")]
        public string ShortDescription { get; set; }

        [Column("REGULATION_DESCRIPTION")]
        public string RegulationDescription { get; set; }

        [Column("CHARGE_TYPE_ID")]
        public long ChargeTypeID { get; set; }

        [Column("STATUS_ID")]
        public Status Status { get; set; }

        [Column("EVIDENCE_LOG_ID")]
        public long EvidenceLogID { get; set; }

        [ForeignKey("EvidenceLogID")]
        public virtual EvidenceLog EvidenceLog { get; set; }

        [ForeignKey("OffenceCodesID")]
        public virtual OffenceCode OffenceCode { get; set; }
    }
}
