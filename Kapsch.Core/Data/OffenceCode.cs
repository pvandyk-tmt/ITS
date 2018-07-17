using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("OFFENCE_CODES", Schema = "ITS")]
    public class OffenceCode
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Column("OFFENCE_SET_ID")]
        public long OffenceSetID { get; set; }

        [Column("CODE")]
        public string Code { get; set; }

        [Column("FINE_AMOUNT")]
        public decimal FineAmount { get; set; }

        [Column("VEHICLE_TYPE")]
        public string VehicleType { get; set; }

        [Column("ZONE")]
        public int? Zone { get; set; }

        [Column("MIN_SPEED")]
        public int? MinSpeed { get; set; }

        [Column("MAX_SPEED")]
        public int? MaxSpeed { get; set; }

        [Column("WIM_VEHICLE_TYPE_ID")]
        public int? WimVehicleTypeID { get; set; }

        [Column("WIM_OFFENCE_DESCRIPTION")]
        public string WimOffenceDescription { get; set; }

        [Column("MIN_OVERWEIGHT_PERSENT")]
        public long? MinOverweightPercentage { get; set; }

        [Column("MAX_OVERWEIGHT_PERSENT")]
        public long? MaxOverweightPercentage { get; set; }

        [Column("CASE_TYPE_ID")]
        public int CaseTypeID { get; set; }

        public virtual IList<OffenceCodeOffenceDescription> OffenceCodeOffenceDescriptions { get; set; }
    }
}
