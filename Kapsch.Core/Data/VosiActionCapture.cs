using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("VOSI_ACTION_CAPTURE", Schema = "ITS")]
    public class VosiActionCapture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("VOSI_ACTION_ID")]
        public long VosiActionID { get; set; }

        [Column("VLN")]
        public string VLN { get; set; }

        [Column("LOCATION_STREET")]
        public string LocationStreet { get; set; }

        [Column("LOCATION_SUBURB")]
        public string LocationSuburb { get; set; }

        [Column("LOCATION_TOWN")]
        public string LocationTown { get; set; }

        [Column("LOCATION_LATITUDE")]
        public decimal? LocationLatitude { get; set; }

        [Column("LOCATION_LONGITUDE")]
        public decimal? LocationLongitude { get; set; }

        [Column("COMMENTS")]
        public string Comments { get; set; }

        [Column("CAPTURED_DATE")]
        public DateTime CapturedDateTime { get; set; }

        [Column("DEVICE_ID")]
        public string DeviceID { get; set; }

        [Column("CAPTURED_CREDENTIAL_ID")]
        public long CapturedCredentialID { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [ForeignKey("VosiActionID")]
        public virtual VosiAction VosiAction { get; set; }

        [ForeignKey("CapturedCredentialID")]
        public virtual Credential CapturedCredential { get; set; }
    }
}
