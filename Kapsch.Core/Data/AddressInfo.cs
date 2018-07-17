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
    [Table("ADDRESS_INFO", Schema = "ITS")]
    public class AddressInfo
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("ADDRESS_TYPE_ID")]
        public AddressType AddressTypeID { get; set; }

        [Column("SOURCE_ID")]
        public long SourceID { get; set; }

        [Column("PERSON_INFO_ID")]
        public long PersonInfoID { get; set; }

        [Column("LINE_1")]
        public string Line1 { get; set; }

        [Column("LINE_2")]
        public string Line2 { get; set; }

        [Column("SUBURB")]
        public string Suburb { get; set; }

        [Column("TOWN")]
        public string Town { get; set; }

        [Column("COUNTRY")]
        public string Country { get; set; }

        [Column("CODE")]
        public string Code { get; set; }

        [Column("LATITUDE")]
        public decimal? Latitude { get; set; }

        [Column("LONGITUDE")]
        public decimal? Longitude { get; set; }

        [Column("CREATED_CREDENTIAL_ID")]
        public long? CreatedCredentialID { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CreatedDate { get; set; }

        [Column("IS_PREFFERED_INDICATOR")]
        public long? IsPrefferedIndicator { get; set; }

        [ForeignKey("PersonInfoID")]
        public virtual Person Person { get; set; }
    }
}
