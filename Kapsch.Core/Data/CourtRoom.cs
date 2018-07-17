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
    [Table("COURT_ROOMS", Schema = "COURT")]
    public class CourtRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("COURT_DETAILS_ID")]
        public long CourtID {get;set;}

        [Column("ROOM")]
        public string Number {get;set;}

        [Column("STATUS_ID")]
        public Status Status { get; set; }

        [ForeignKey("CourtID")]
        public virtual Court Court { get; set; }
    }
}
