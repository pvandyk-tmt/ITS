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
    [Table("EVIDENCE_LOG", Schema = "ITS")]
    public class EvidenceLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("INFRINGEMENT_TYPE_ID")]
        public InfringementType InfringementType { get; set; }

        [Column("INFRINGEMENT_DATE")]
        public DateTime InfringementDate { get; set; }

        [Column("DISTRICT_ID")]
        public long? DistrictID { get; set; }

        [Column("STATUS_ID")]
        public Status? Status { get; set; }

        [ForeignKey("ID")]
        public virtual SpeedLog SpeedLog { get; set; }

        [ForeignKey("ID")]
        public virtual HandWrittenCaptureLog HandWrittenCaptureLog { get; set; }

        public virtual IList<ChargeInfo> ChargeInfos { get; set; }

        public virtual IList<InfringementEvidence> InfringementEvidences { get; set; }
    }
}
