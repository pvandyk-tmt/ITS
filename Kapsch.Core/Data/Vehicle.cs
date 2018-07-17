using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Kapsch.Core.Data
{
    [Table("VEHICLE_DETAIL", Schema = "TIS")]
    public class Vehicle
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("VEHICLE_IDENTIFICATION_NUMBER")]
        public string VehicleIDNumber { get; set; }


        [Column("ENGINE_NUMBER")]
        public string EngineNumber { get; set; }


        [Column("VEHICLE_CATEGORY_ID")]
        public int VehicleCategoryId { get; set; }


        [Column("VEHICLE_TYPE_ID")]
        public int VehicleTypeId { get; set; }


        [Column("VEHICLE_MAKE_ID")]
        public int VehicleMakeId { get; set; }


        [Column("VEHICLE_MODEL_ID")]
        public int VehicleModelId { get; set; }


        [Column("VEHICLE_MODEL_NUMBER_ID")]
        public int VehicleModelNumberId { get; set; }


        [Column("COLOUR_ID")]
        public int ColourId { get; set; }


        [Column("YEAR_OF_MAKE")]
        public int YearOfMake { get; set; }


        [Column("VLN")]
        public string VLN { get; set; }


        [Column("NET_WEIGHT")]
        public int NetWeight { get; set; }


        [Column("GVM")]
        public int GVM { get; set; }


        [Column("PROPELLED_BY_ID")]
        public int ProplelledById { get; set; }

        [Column("FUEL_TYPE_ID")]
        public int FuelTypeId { get; set; }


        [Column("REGISTRATION_STATUS_ID")]
        public int RegistrationStatusId { get; set; }


        [Column("LICENCE_EXPIRY_DATE")]
        public DateTime? LicenceExpiryDate { get; set; }


        [Column("ROADWORTHINESS_EXPIRY_DATE")]        
        public DateTime? RoadworthyExpiryDate { get; set; }


        [Column("INSURANCE_EXPIRY_DATE")]
        public DateTime? InsuranceExpiryDate { get; set; }


        [Column("CAPTURED_DATE")]
        public DateTime CapturedDate { get; set; }


        [Column("CAPTURED_CREDENTIAL_ID")]
        public int CapturedCredentialId { get; set; }

        [Column("SEATING_CAPACITY")]
        public int? SeatingCapacity { get; set; }

        [ForeignKey("VehicleMakeId")]
        public virtual VehicleMake VehicleMake { get; set; }

        [ForeignKey("VehicleModelId")]
        public virtual VehicleModel VehicleModel { get; set; }

        [ForeignKey("VehicleModelNumberId")]
        public virtual VehicleModelNumber VehicleModelNumber { get; set; }

        [ForeignKey("ColourId")]
        public virtual VehicleColor VehicleColor { get; set; }

        public virtual IList<Kapsch.Core.Data.VehicleTestBooking> TestBookings { get; set; }

        [ForeignKey("VehicleTypeId")]
        public virtual VehicleCategory VehicleCategory { get; set; }

    }
}
