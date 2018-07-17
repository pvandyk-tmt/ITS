using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("LK_TEST_TYPE", Schema = "TIS")]
    public class TestType
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime ModifiedDate { get; set; }

        public virtual IList<VehicleCategoryTestType> VehicleCategoryTestTypes { get; set; }
    }
}

