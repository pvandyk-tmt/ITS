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
    [Table("USER_DETAIL", Schema = "CREDENTIALS")]
    public class User : ICorrespondent
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("FIRST_NAME")]
        public string FirstName { get; set; }

        [Column("LAST_NAME")]
        public string LastName { get; set; }

        [Column("EMAIL")]
        public string Email { get; set; }

        [Column("MOBILE_NUMBER")]
        public string MobileNumber { get; set; }

        [Column("IS_OFFICER")]
        public string IsOfficer { get; set; }

        [Column("EXTERNAL_ID")]
        public string ExternalID { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("ENTITY_STATUS_ID")]
        public Status Status { get; set; }

        public virtual IList<UserDistrict> UserDistricts { get; set; }
    }
}
