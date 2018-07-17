using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("CORRESPONDENCE_ROUTE", Schema = "CORRESPONDENCE")]
    public class CorrespondenceRoute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("CorrespondenceItem")]
        [Column("CORRESPONDENCE_ITEM_ID")]
        public long CorrespondenceItemID { get; set; }

        [Column("SOURCE_VALUE")]
        public string Source { get; set; }

        [Column("TARGET_VALUE")]
        public string Target { get; set; }

        public virtual CorrespondenceItem CorrespondenceItem { get; set; }
    }
}
