using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("LK_VEHICLE_MODEL_NUMBER", Schema = "TIS")]
    public class VehicleModelNumber
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("VEHICLE_MODEL_ID")]
        public int VehicleModelID { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("EXTERNAL_CODE")]
        public string ExternalCode { get; set; }

        [Column("EXTERNAL_MODEL_CODE")]
        public string ExternalModelCode { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("VehicleModelID")]
        public virtual VehicleModel VehicleModel { get; set; }
    }
}