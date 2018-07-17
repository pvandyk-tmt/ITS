using Kapsch.Core.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("CORRESPONDENCE_SMS_PAYLOAD", Schema = "CORRESPONDENCE")]
    public class CorrespondenceSmsPayload
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("CorrespondenceItem")]
        [Column("CORRESPONDENCE_ITEM_ID")]
        public long CorrespondenceItemID { get; set; }

        [Column("MESSAGE")]
        public string Message { get; set; }

        public virtual CorrespondenceItem CorrespondenceItem { get; set; }
    }
}
