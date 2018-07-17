using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("LK_IDENTIFICATION_TYPE", Schema = "ITS")]
    public class IdentificationType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime? ModifiedDate { get; set; }
    }
}
