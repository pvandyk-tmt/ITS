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
    [Table("DISTRICT_OFFENCE_SET_REL", Schema = "ITS")]
    public class DistrictOffenceSet
    {
        [Key]
        [Column("DISTRICT_ID", Order = 1)]
        public long DistrictID { get; set; }

        [Key]
        [Column("OFFENCE_SET_ID", Order = 2)]
        public long OffenceSetID { get; set; }

        [Column("EFFECTIVE_DATE")]
        public DateTime EffectiveDate { get; set; }

        [Column("END_DATE")]
        public DateTime EndDate { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

        [ForeignKey("OffenceSetID")]
        public virtual OffenceSet OffenceSet { get; set; }
    }
}
