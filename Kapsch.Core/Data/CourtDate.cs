using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("COURT_DATES", Schema = "COURT")]
    public class CourtDate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("COURT_DETAILS_ID")]
        public long CourtID {get;set;}

        [Column("COURT_DATE")]
        public DateTime Date {get;set;}

        [Column("MAX_ALLOCATE")]
        public long? MaxAllocate {get;set;}

        [Column("ALLOCATED")]
        public long Allocated { get; set; }

        [Column("COURT_ROOM_ID")]
        public long? CourtRoomID { get; set; }

        [Column("ALLOCATION_TYPE")]
        public long AllocationType { get; set; }

        [ForeignKey("CourtID")]
        public virtual Court Court { get; set; }

        [ForeignKey("CourtRoomID")]
        public virtual CourtRoom CourtRoom { get; set; }
    }
}
