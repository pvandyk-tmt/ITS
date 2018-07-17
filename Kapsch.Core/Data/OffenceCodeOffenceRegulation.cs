using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("OFFENCE_REGULATION_REL", Schema = "ITS")]
    public class OffenceCodeOffenceRegulation
    {
        [Key]
        [Column("OFFENCE_CODE_ID", Order = 1)]
        public long OffenceCodeID { get; set; }

        [Key]
        [Column("LANGUAGE_ID", Order = 2)]
        public long LanguageID { get; set; }

        [Key]
        [Column("REGULATION_ID", Order = 3)]
        public long OffenceRegulationID { get; set; }

        [ForeignKey("OffenceCodeID")]
        public virtual OffenceCode OffenCode { get; set; }

        [ForeignKey("OffenceRegulationID")]
        public virtual OffenceRegulation OffenceRegulation { get; set; }
    }
}
