using System;
using System.ComponentModel.DataAnnotations;


namespace Kapsch.ITS.Gateway.Models.TISCapture
{
    public class TISDataModel
    {
        //public long ID { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Reference Number")]
        public string ReferenceNumber { get; set; }
     
        [Required]
        [StringLength(100)]
        [Display(Name = "Vehicle Registration Number")]
        public string VehicleRegistrationNumber { get; set; }

        [Display(Name = "Vehicle Make ID")]
        public string VehicleMakeID { get; set; }

        [Display(Name = "Vehicle Make")]
        public string VehicleMake { get; set; }

        [Display(Name = "Vehicle Model ID")]
        public string VehicleModelID { get; set;}

        [Display(Name = "Vehicle Model")]
        public string VehicleModel { get; set; }

        [Display(Name = "Vehicle Type ID")]
        public string VehicleTypeID { get; set; }

        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Display(Name = "Vehicle Usage ID")]
        public string VehicleUsageID { get; set; }

        [Display(Name = "Vehicle Colour ID")]
        public string VehicleColourID { get; set; }

        public DateTime LicenseExpireDate { get; set; }

        [Display(Name = "Year of Make")]
        public string YearOfMake { get; set; }

        [Display(Name = "Owner ID Type")]
        public string OwnerIDType { get; set; }

        [Display(Name = "Owner ID")]
        public string OwnerID { get; set; }

        [Required]
        [Display(Name = "Owner Name")]
        [StringLength(128)]
        public string OwnerName { get; set; }

        [Required]
        [Display(Name = "Surname")]
        [StringLength(128)]
        public string Surname { get; set; }

        [Display(Name = "Owner Initials")]
        public string OwnerInitials { get; set; }

        [Required]
        [Display(Name = "Owner Gender")]
        public string OwnerGender { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email Adress")]
        public string EmailAddress { get; set; }

        //[RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Telephone Number")]
        [Display(Name = "Owner Telephone")]
        public string OwnerTelephone { get; set; }

        [Display(Name = "Owner Postal")]
        public string OwnerPostal { get; set; }

        [Display(Name = "Owner Postal Street")]
        public string OwnerPostalStreet { get; set; }

        [Display(Name = "Owner Postal Suburb")]
        public string OwnerPostalSuburb { get; set; }

        [Display(Name = "Owner Postal Town")]
        public string OwnerPostalTown { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "Owner Physical")]
        public string OwnerPhysical { get; set; }

        [Display(Name = "Owner Physical Street")]
        public string OwnerPhysicalStreet { get; set; }

        [Display(Name = "Owner Physical Suburb")]
        public string OwnerPhysicalSuburb { get; set; }

        [Display(Name = "Owner Physical Town")]
        public string OwnerPhysicalTown { get; set; }

        [Display(Name = "Physical Code")]
        public string PhysicalCode { get; set; }

        [Required]
        //[RegularExpression(@"^(\d{10})$", ErrorMessage = "Invalid Phone Number")]
        [Display(Name = "Owner Cellphone Number")]
        public string OwnerCellphone { get; set; }

        [Display(Name = "Vehicle Colour")]
        public string VehicleColour { get; set; }

        [Display(Name = "Clearance Certificate Number")]
        public string ClearanceCertificateNumber { get; set; }

        [Display(Name = "Date of Ownership")]
        public DateTime DateOfOwnership { get; set; }

        [Display(Name = "Owner Company")]
        public string OwnerCompany { get; set; }

        [Display(Name = "Import File Name")]
        public string ImportFileName { get; set; }

        [Display(Name = "Nature of Ownership")]
        public string NatureOfOwnership { get; set; }

        [Display(Name = "Proxy Indicator")]
        public string ProxyIndicator { get; set; }
    }
}
