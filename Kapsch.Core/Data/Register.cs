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
    [Table("REGISTER", Schema = "ITS")]
    public class Register
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }
 
        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber  { get; set; }

        [Column("OUTSTANDING_AMOUNT")]
        public decimal? OutstandingAmount { get; set; }

        [Column("ACCOUNT_ID")]
        public long? AccountID { get; set; }

        [Column("CAPTURED_CREDENTIAL_ID")]
        public long? CapturedCredentialID { get; set; }

        [Column("CAPTURED_DATE")]
        public DateTime CapturedDate { get; set; }

        [Column("REGISTER_STATUS_ID")]
        public int RegisterStatus { get; set; }

        [Column("PERSON_INFO_ID")]
        public long? PersonID { get; set; }

        [Column("DISTRICT_ID")]
        public long? DistrictID { get; set; }

        [Column("MODIFIED_CREDENTIAL_ID")]
        public long? ModifiedCredentialID { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime? ModifiedTimestamp { get; set; }

        [ForeignKey("PersonID")]
        public virtual Person Person { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        [ForeignKey("ReferenceNumber")]
        public virtual ReferenceVehicle ReferenceVehicle { get; set; }
    }
}
