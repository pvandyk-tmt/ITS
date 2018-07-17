using Kapsch.Core.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("CORRESPONDENCE_EMAIL_PAYLOAD", Schema = "CORRESPONDENCE")]
    public class CorrespondenceEmailPayload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("CorrespondenceItem")]
        [Column("CORRESPONDENCE_ITEM_ID")]
        public long CorrespondenceItemID { get; set; }

        [Column("TEXT_CONTENTS")]
        public byte[] TextContents { get; set; }

        [Column("HTML_CONTENTS")]
        public byte[] HtmlContents { get; set; }

        public virtual CorrespondenceItem CorrespondenceItem { get; set; }
    }
}
