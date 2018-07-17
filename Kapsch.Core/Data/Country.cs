using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("LK_COUNTRY", Schema = "ITS")]
    public class Country
    {
        [Key]
        [Column("ID")]
        public int ID { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime ModifiedTimestamp { get; set; }
    }
}
