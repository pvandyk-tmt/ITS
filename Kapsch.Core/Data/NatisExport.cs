using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("NATIS_EXPORT", Schema = "ITS")]
    public class NatisExport
    {
        [Key]
        [Column("VEHICLE_REGISTRATION")]
        public string VehicleRegistration { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("INFRINGEMENT_DATE")]
        public string InfringementDate { get; set; }

        [Column("EXPORT_DATE")]
        public DateTime? ExportDate { get; set; }

        [Column("DISTRICT_ID")]
        public long DistrictID { get; set; }

        [Column("LOCKED_BY_CREDENTIAL_ID")]
        public long? LockedByCredentialID { get; set; }

        [ForeignKey("LockedByCredentialID")]
        public virtual User User { get; set; }

        [ForeignKey("VehicleRegistration")]
        public virtual TISData VLN { get; set; }

        [ForeignKey("ReferenceNumber")]
        public virtual TISData RefNum { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

    }
}
