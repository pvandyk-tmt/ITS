using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("VEHICLE_TEST_BOOKING", Schema = "TIS")]
    public class VehicleTestBooking
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("VEHICLE_DETAIL_ID")]
        public int VehicleDetailID { get; set; }

        [Column("BOOKING_REFERENCE")]
        public string BookingReference { get; set; }

        [Column("TEST_DATE")]
        public DateTime TestDate { get; set; }

        [Column("IS_PASSED")]
        public Nullable<int> IsPassed { get; set; }

        [Column("TEST_TYPE_ID")]
        public int TestTypeID { get; set; }

        [Column("CAPTURED_CREDENTIAL_ID")]
        public long CapturedCredentialID { get; set; }

        [Column("CAPTURED_DATE")]
        public DateTime CapturedDate { get; set; }

        [Column("TEST_STARTED_AT")]
        public DateTime? StartedTimestamp { get; set; }

        [Column("TEST_ENDED_AT")]
        public DateTime? EndedTimestamp { get; set; }

        [Column("SITE_ID")]
        public long SiteID { get; set; }

        [ForeignKey("CapturedCredentialID")]
        public virtual Credential CapturedCredential { get; set; }

        //[ForeignKey("EntityID")]
        //public virtual User User { get; set; }

        [ForeignKey("SiteID")]
        public virtual Site Site { get; set; }

        [ForeignKey("VehicleDetailID")]
        public virtual Vehicle Vehicle { get; set; }

        [ForeignKey("TestTypeID")]
        public virtual TestType TestType { get; set; }
    }
}
