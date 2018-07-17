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
    [Table("PERSON_INFO", Schema = "ITS")]
    public class Person : ICorrespondent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }
 
        [Column("SURNAME")]
        public string LastName  { get; set; } 

        [Column("INITIALS")]
        public string Initials { get; set; } 

        [Column("FULL_NAMES")]
        public string FirstNames { get; set; } 

        [Column("IDENTIFICATION_NUMBER")]
        public string IDNumber { get; set; } 

        [Column("IDENTIFICATION_NUMBER_TYPE_ID")]
        public long IDNumberType { get; set; } 

        [Column("BIRTH_DATE")]
        public DateTime? BirthDate { get; set; } 

        [Column("TITLE")]
        public string Title { get; set; } 
 
        [Column("SEX")]
        public string Sex { get; set; } 

        [Column("OCCUPATION")]
        public string Occupation  { get; set; } 
 
        [Column("TELEPHONE")]
        public string TelephoneNumber { get; set; } 
 
        [Column("CELLPHONE")]
        public string MobileNumber { get; set; } 

        [Column("FAX")]
        public string FaxNumber { get; set; } 
 
        [Column("EMAIL")]
        public string Email { get; set; } 

        [Column("COMPANY")]
        public string CompanyName { get; set; } 

        [Column("BUSINESS_TELEPHONE")]
        public string BusinessTelephoneNumber { get; set; } 

        [Column("CITIZEN_TYPE_ID")]
        public long? CitizienTypeID { get; set; } 

        [Column("COUNTRY_ID")]
        public int? CountryID { get; set; }

        [Column("MODIFIED_BY_CREDENTIAL_ID")]
        public long? ModifiedCredentialD { get; set; } 

        [Column("MODIFIED_DATE")]
        public DateTime? ModifiedTiestamp { get; set; }

        [ForeignKey("IDNumberType")]
        public virtual IdentificationType IdentificationType { get; set; }

        public virtual IList<AddressInfo> AddressInfos { get; set; }
    }
}
