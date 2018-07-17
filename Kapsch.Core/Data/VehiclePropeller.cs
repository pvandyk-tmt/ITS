using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("LK_PROPELLED_BY", Schema = "TIS")]
    public class VehiclePropeller
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("EXTERNAL_CODE")]
        public string ExternalCode { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime ModifiedDate { get; set; }
    }
}
