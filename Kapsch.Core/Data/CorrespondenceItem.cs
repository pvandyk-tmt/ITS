using Kapsch.Core.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("CORRESPONDENCE_ITEM", Schema = "CORRESPONDENCE")]
    public class CorrespondenceItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("CORRESPONDENCE_TYPE_ID")]
        public CorrespondenceType CorrespondenceType { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("SOURCE_ENTITY_ID")]
        public long SourceEntityID { get; set; }

        [Column("SOURCE_ENTITY_TYPE_ID")]
        public EntityType SourceEntityType { get; set; }

        [Column("TARGET_ENTITY_ID")]
        public long TargetEntityID { get; set; }

        [Column("TARGET_ENTITY_TYPE_ID")]
        public EntityType TargetEntityType { get; set; }

        [Column("ITEM_STATUS_ID")]
        public CorrespondenceItemStatus Status { get; set; }

        [Column("SUB_TYPE")]
        public string SubType { get; set; }

        [Column("CONTEXT_VALUE")]
        public string Context { get; set; }

        [Column("STATUS_TIMESTAMP")]
        public DateTime StatusTimestamp { get; set; }

        [Column("INTERNAL_REFERENCE")]
        public string InternalReference { get; set; }

        [Column("EXTERNAL_REFERENCE")]
        public string ExternalReference { get; set; }

        [Column("FAILURE_REASON")]
        public string FailureReason { get; set; }

        [Column("VALUE")]
        public string Value { get; set; }

        public virtual CorrespondenceRoute Route { get; set; }
        public virtual CorrespondenceSmsPayload SmsPayload { get; set; }
        public virtual CorrespondenceEmailPayload EmailPayload { get; set; }
    }
}
