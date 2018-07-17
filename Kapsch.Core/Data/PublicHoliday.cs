using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("LK_PUBLIC_HOLIDAYS", Schema = "ITS")]
    public class PublicHoliday
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("HOLIDAY_DESCRIPTION")]
        public string HolidayDescription { get; set; }

        [Column("HOLIDAY_DATE")]
        public DateTime HolidayDate { get; set; }

        [Column("ACTIVE")]
        public string IsActive { get; set; }
    }
}
