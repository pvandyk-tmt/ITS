using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("LK_TEST_CATEGORY", Schema = "TIS")]
    public class TestCategory
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("ACTIVE_STATUS_ID")]
        public int ActiveStatusID { get; set; }

        [Column("IS_TIS_CHECK_REQUIRED")]
        public int IsTisCheckRequired { get; set; }

        [Column("CAN_CONTINUE_NO_TIS")]
        public int CanContinueNoTIS { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime ModifiedDate { get; set; }

    }
}
