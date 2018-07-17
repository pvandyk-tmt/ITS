using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("SPEED_LOG", Schema = "ITS")]
    public class SpeedLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("EVIDENCE_LOG_ID")]
        public long EvidenceLog { get; set; }

        [Column("ZONE")]
        public decimal? Zone { get; set; }

        [Column("SPEED")]
        public decimal? Speed { get; set; }

        [Column("DISTANCE")]
        public decimal? Distance { get; set; }

        [Column("DIRECTION")]
        public string Direction { get; set; }

        [Column("INFRINGEMENT_LOCATION_CODE")]
        public string InfringementLocationCode { get; set; }        

        [Column("SESSION_DATE")]
        public DateTime? SessionDate { get; set; }

        [Column("SESSION_IDENTIFIER")]
        public string SessionIdentifier { get; set; }

        [Column("SESSION_CASE")]
        public long SessionCase { get; set; }
    }
}
