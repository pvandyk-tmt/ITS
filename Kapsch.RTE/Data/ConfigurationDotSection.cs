using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.RTE.Data
{
    [Table("CONFIGURATION_SECTION", Schema = "ITS")]
    public class ConfigurationDotSection
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("SECTION_CODE")]
        public string SectionCode { get; set; }

        [Column("SECTION_DESCRIPTION")]
        public string SectionDescription { get; set; }

        [Column("SECTION_DISTANCE")]
        public long SectionDistance { get; set; }

        [Column("SECTION_CODE_A")]
        public string SectionCodePointA { get; set; }

        [Column("SECTION_CODE_A_LAT")]
        public decimal? SectionCodePointALatitude { get; set; }

        [Column("SECTION_CODE_A_LON")]
        public decimal? SectionCodePointALongitude { get; set; }

        [Column("SECTION_CODE_B")]
        public string SectionCodePointB { get; set; }

        [Column("SECTION_CODE_B_LAT")]
        public decimal? SectionCodePointBLatitude { get; set; }

        [Column("SECTION_CODE_B_LON")]
        public decimal? SectionCodePointBLongitude { get; set; }

        [Column("LEVENSHTEIN_MATCH_DISTANCE")]
        public int LevenshteinMatchDistance { get; set; }

        [Column("IS_PHYSICAL_INFRINGEMENT")]
        public int CreatePhysicalInfringement { get; set; }
    }
}
