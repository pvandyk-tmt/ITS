using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("LK_VEHICLE_MODEL", Schema = "TIS")]
    public class VehicleModel
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        [Column("VEHICLE_MAKE_ID")]
        public int VehicleMakeID { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("EXTERNAL_CODE")]
        public string ExternalCode { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("VehicleMakeID")]
        public virtual VehicleMake VehicleMake { get; set; }

        public virtual IList<VehicleModelNumber> VehicleModelNumbers { get; set; }
    }
}