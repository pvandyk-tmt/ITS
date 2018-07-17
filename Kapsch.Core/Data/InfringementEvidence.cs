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
    [Table("INFRINGEMENT_EVIDENCE", Schema = "ITS")]
    public class InfringementEvidence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("EVIDENCE_TYPE_ID")]
        public EvidenceType EvidenceType { get; set; }

        [Column("MIME_TYPE")]
        public string MimeType { get; set; }

        [Column("MIME_DATA_PATH")]
        public string MimeDataPath { get; set; }

        [Column("STATUS_ID")]
        public Status Status { get; set; }

        [Column("EVIDENCE_LOG_ID")]
        public long? EvidenceLogID { get; set; }

        [Column("IS_PRINT_IMAGE")]
        public long IsPrintImage { get; set; }

        [Column("FILENAME")]
        public string FileName { get; set; }

        [Column("EVIDENCE_FILE_NUMBER")]
        public long? EvidenceFileNumber { get; set; }

        [ForeignKey("EvidenceLogID")]
        public virtual EvidenceLog EvidenceLog { get; set; }
    }
}
