using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("HANDWRITTEN_CAPTURE_LOG", Schema = "ITS")]
    public class HandWrittenCaptureLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("EVIDENCE_LOG_ID")]
        public long EvidenceLog { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("OFFENCE_LOCATION_STREET")]
        public string OffenceLocationStreet { get; set; }

        [Column("OFFENCE_LOCATION_SUBURB")]
        public string OffenceLocationSuburb { get; set; }

        [Column("OFFENCE_LOCATION_TOWN")]
        public string OffenceLocationTown { get; set; }

        [Column("OFFENCE_LOCATION_LATITUDE")]
        public decimal? OffenceLocationLatitude { get; set; }

        [Column("OFFENCE_LOCATION_LONGITUDE")]
        public decimal? OffenceLocationLongitude { get; set; }

        [Column("CHARGE_CODE_1")]
        public string ChargeCode1 { get; set; }

        [Column("CHARGE_CODE_1_ID")]
        public long? ChargeCode1ID { get; set; }

        [Column("AMOUNT_1")]
        public decimal? Amount1 { get; set; }

        [Column("CHARGE_CODE_2")]
        public string ChargeCode2 { get; set; }

        [Column("CHARGE_CODE_2_ID")]
        public long? ChargeCode2ID { get; set; }

        [Column("AMOUNT_2")]
        public decimal? Amount2 { get; set; }

        [Column("CHARGE_CODE_3")]
        public string Chargecode3 { get; set; }

        [Column("CHARGE_CODE_3_ID")]
        public decimal? ChargeCode3ID { get; set; }

        [Column("AMOUNT_3")]
        public decimal? Amount3 { get; set; }

        [Column("SPEED")]
        public decimal? Speed { get; set; }

        [Column("VEHICLE_MAKE_MAIN")]
        public string VehicleMake { get; set; }

        [Column("VEHICLE_MODEL_MAIN")]
        public string VehicleModel { get; set; }
    }
}
