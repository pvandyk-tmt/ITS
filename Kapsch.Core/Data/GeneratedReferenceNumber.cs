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
    [Table("GENERATED_REFERENCE_NUMBER", Schema = "ITS")]
    public class GeneratedReferenceNumber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("ENTITY_REFERENCE_TYPE_ID")]
        public long EntityReferenceTypeID { get; set; }

        [Column("REFERENCE_DOCUMENT_TYPE_ID")]
        public long ReferenceDocumentTypeID { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("SEQUENCE_VALUE")]
        public long SequenceValue { get; set; }

        [Column("GENERATED_DATE")]
        public DateTime GeneratedDate { get; set; }

        [Column("CREATED_CREDENTIAL_ID")]
        public long CreatedCredentialID { get; set; }

        [Column("DISTRICT_ID")]
        public long DistrictID { get; set; }

        [Column("DEVICE_ID")]
        public string DeviceID { get; set; }

        [Column("IS_USED")]
        public long IsUsed { get; set; }

        [Column("EXTERNAL_TOKEN")]
        public string ExternalToken { get; set; }

        [Column("EXTERNAL_REFERENCE")]
        public string ExternalReference { get; set; }

    }
}
