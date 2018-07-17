using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("LK_SITE", Schema = "ITS")]
    public class Site
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("SITE_TYPE_ID")]
        public long SiteTypeID { get; set; }

        [Column("DISTRICT_ID")]
        public long? DistrictID { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

    }
}
