using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("REFERENCE_VEHICLE_DETAIL", Schema = "ITS")]
    public class ReferenceVehicle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("VEHICLE_REGISTRATION_NUMBER")]
        public string RegistrationNumber  { get; set; } 

        [Column("VEHICLE_MAKE_DESCRIPTION")]
        public string MakeDescription { get; set; } 

        [Column("VEHICLE_MODEL_DESCRIPTION")]
        public string ModelDescription { get; set; } 

        [Column("VEHICLE_CATEGORY_DESCRIPTION")]
        public string CategoryDescription { get; set; } 

        [Column("VEHICLE_TYPE_DESCRIPTION")]
        public string TypeDescription { get; set; } 

        [Column("VEHICLE_COLOUR_DESCRIPTION")]
        public string ColourDescription { get; set; } 

        [Column("VEHICLE_YEAR_OF_MAKE")]
        public int? YearMake { get; set; }
    }
}
