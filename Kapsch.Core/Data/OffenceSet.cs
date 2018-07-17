using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("OFFENCE_SETS", Schema = "ITS")]
    public class OffenceSet
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Column("SET_NAME")]
        public string SetName { get; set; }

        [Column("INFRINGEMENT_TYPE_ID")]
        public long InfringementTypeID { get; set; }
    }
}
