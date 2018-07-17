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
    [Table("LK_INFRINGEMENT_LOCATION", Schema = "ITS")]
    public class InfringementLocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("CODE")]
        public string Code { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("COURT_DETAIL_ID")]
        public long CourtID { get; set; }

        [Column("GPS_LATITUDE")]
        public decimal? GpsLatitude { get; set; }

        [Column("GPS_LONGITUDE")]
        public decimal? GpsLongitude { get; set; }

        [Column("INFRINGEMENT_LOCATION_TYPE_ID")]
        public InfringementLocationType InfringementLocationType { get; set; }

        [ForeignKey("CourtID")]
        public virtual Court Court { get; set; }
    }
}
