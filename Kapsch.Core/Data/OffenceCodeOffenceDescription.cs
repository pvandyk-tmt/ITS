using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("OFFENCE_DESCRIPTION_REL", Schema = "ITS")]
    public class OffenceCodeOffenceDescription
    {
        [Key]
        [Column("OFFENCE_CODE_ID", Order = 1)]
        public long OffenceCodeID { get; set; }

        [Key]
        [Column("LANGUAGE_ID", Order = 2)]
        public long LanguageID { get; set; }

        [Key]
        [Column("DESCRIPTION_ID", Order = 3)]
        public long OffenceDescriptionID { get; set; }

        [ForeignKey("OffenceCodeID")]
        public virtual OffenceCode OffenCode { get; set; }

        [ForeignKey("OffenceDescriptionID")]
        public virtual OffenceDescription OffenceDescription { get; set; }
    }
}
