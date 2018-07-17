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
    [Table("NATIS_VEHICLE_DETAIL", Schema = "ITS")]
    public class TISData
    {
        
            //[Column("ID")]
            //[DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
            //public long ID { get; set; }

            [Key]
            [Column("VEHICLE_REGISTRATION_NUMBER", Order = 0)]
            public string VehicleRegistrationNumber { get; set; }

            [Column("REFERENCE_NUMBER", Order = 1)]
            public string ReferenceNumber { get; set; }

            [Column("VEHICLE_MAKE_ID")]
            public string VehicleMakeID { get; set; }

            [Column("VEHICLE_MAKE")]
            public string VehicleMake { get; set; }

            [Column("VEHICLE_MODEL_ID")]
            public string VehicleModelID { get; set;}

            [Column("VEHICLE_TYPE_ID")]
            public string VehicleTypeID { get; set; }

            [Column("VEHICLE_TYPE")]
            public string VehicleType { get; set; }

            [Column("VEHICLE_USAGE_ID")]
            public string VehicleUsageID { get; set; }

            [Column("VEHICLE_COLOUR_ID")]
            public string VehicleColourID { get; set; }

            [Column("LICENSE_EXPIRE_DATE")]
            public DateTime LicenseExpireDate { get; set; }

            [Column("YEAR_OF_MAKE")]
            public string YearOfMake { get; set; }

            [Column("CLEARENCE_CERT_NO")]
            public string ClearanceCertificateNumber { get; set; }

            [Column("OWNER_INIT")]
            public string OwnerInitials { get; set; }

            [Column("OWNER_ID_TYPE")]
            public string OwnerIDType { get; set; }

            [Column("OWNER_ID")]
            public string OwnerID { get; set; }

            [Column("OWNER_NAME")]
            public string OwnerName { get; set; }

            [Column("SURNAME")]
            public string Surname { get; set; }

            [Column("OWNER_GENDER")]
            public string OwnerGender { get; set; }

            [Column("EMAIL_ADDRESS")]
            public string Email { get; set; }

            [Column("OWNER_TELEPHONE")]
            public string OwnerTelephone { get; set; }

            [Column("OWNER_POSTAL")]
            public string OwnerPostal { get; set; }

            [Column("OWNER_POSTAL_STREET")]
            public string OwnerPostalStreet { get; set; }

            [Column("OWNER_POSTAL_SUBURB")]
            public string OwnerPostalSuburb { get; set; }

            [Column("OWNER_POSTAL_TOWN")]
            public string OwnerPostalTown { get; set; }

            [Column("OWNER_POSTAL_CODE")]
            public string PostalCode { get; set; }

            [Column("OWNER_PHYS")]
            public string OwnerPhysical { get; set; }

            [Column("OWNER_PHYS_STREET")]
            public string OwnerPhysicalStreet { get; set; }

            [Column("OWNER_PHYS_SUBURB")]
            public string OwnerPhysicalSuburb { get; set; }

            [Column("OWNER_PHYS_TOWN")]
            public string OwnerPhysicalTown { get; set; }

            [Column("OWNER_PHYS_CODE")]
            public string PhysicalCode { get; set; }

            [Column("OWNER_CELLPHONE")]
            public string OwnerCellphone { get; set; }

            [Column("VEHICLE_MODEL")]
            public string VehicleModel { get; set; }

            [Column("OWNER_COMPANY")]
            public string OwnerCompany { get; set; }

            [Column("DATE_OF_OWNERSHIP")]
            public DateTime DateOfOwnership { get; set; }

            [Column("IMPORT_FILE_NAME")]
            public string ImportFileName { get; set; }

            [Column("NATURE_OF_OWNERSHIP")]
            public string NatureOfOwnership { get; set; }

            [Column("PROXY_INDICATOR")]
            public string ProxyIndicator { get; set; }
    }
    }


