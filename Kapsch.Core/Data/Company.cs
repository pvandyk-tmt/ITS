using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("COMPANY_DETAIL", Schema = "CREDENTIALS")]
    public class Company : ICorrespondent
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("COMPANY_NAME")]
        public string Name { get; set; }

        [Column("CONTACT_PERSON")]
        public string ContactPerson { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("MOBILE_NUMBER")]
        public string MobileNumber { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("EXTERNAL_ID")]
        public string ExternalID { get; set; }

        [Column("ENTITY_STATUS_ID")]
        public Status Status { get; set; }
    }
}
