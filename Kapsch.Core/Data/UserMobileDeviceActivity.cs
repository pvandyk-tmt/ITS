using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("USER_MOBILE_DEVICE_ACTIVITY", Schema = "ITS")]
    public class UserMobileDeviceActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("DEVICE_ID")]
        public string DeviceID { get; set; }

        [Column("CREDENTIAL_ID")]
        public long? CredentialID { get; set; }

        [Column("CATEGORY")]
        public string Category { get; set; }

        [Column("ACTION_DESCRIPTION")]
        public string ActionDescription { get; set; }

        [ForeignKey("CredentialID")]
        public virtual Credential Credential { get; set; }
    }
}
