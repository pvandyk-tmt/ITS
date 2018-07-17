using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("OFFENCE_DESCRIPTIONS", Schema = "ITS")]
    public class OffenceDescription
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Column("LANGUAGE_ID")]
        public long LanguageID { get; set; }

        [Column("SHORT_DESCRIPTION")]
        public string ShortDescription { get; set; }

        [Column("MEDIUM_DESCRIPTION")]
        public string MediumDescription { get; set; }

        [Column("NORMAL_DESCRIPTION")]
        public string NormalDescription { get; set; }

        [Column("PRINT_DESCRIPTION")]
        public string PrintDescription { get; set; }

        public virtual IList<OffenceCodeOffenceDescription> OffenceCodeOffenceDescriptions { get; set; }
    }
}
