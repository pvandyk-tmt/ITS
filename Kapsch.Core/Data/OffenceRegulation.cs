using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("OFFENCE_REGULATIONS", Schema = "ITS")]
    public class OffenceRegulation
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Column("LANGUAGE_ID")]
        public long LanguageID { get; set; }

        [Column("REGULATION")]
        public string Regulation { get; set; }

        public virtual IList<OffenceCodeOffenceRegulation> OffenceCodeOffenceRegulations { get; set; }
    }
}
