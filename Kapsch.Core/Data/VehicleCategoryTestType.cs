using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("VEHICLE_CATEGORY_TEST_TYPE_REL", Schema = "TIS")]
    public class VehicleCategoryTestType
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("VEHICLE_TEST_CATEGORY_ID")]
        public int TestCategoryID { get; set; }

        [Column("VEHICLE_CATEGORY_ID")]
        public int VehicleCategoryID { get; set; }

        [Column("TEST_TYPE_ID")]
        public int TestTypeID { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey("TestCategoryID")]
        public virtual TestCategory TestCategory { get; set; }

        [ForeignKey("VehicleCategoryID")]
        public virtual VehicleCategory VehicleCategory { get; set; }

        [ForeignKey("TestTypeID")]
        public virtual TestType TestType { get; set; }
    }
}

