namespace Kapsch.RTE.Gateway.Models.Configuration.Dot
{
    public class SectionConfigurationModel
    {
        public long SectionDistanceInMeter { get; set; }
        public string SectionCode { get; set; }
        public string SectionDescription { get; set; }

        public string SectionCodePointA { get; set; }
        public string SectionCodePointB { get; set; }

        //Levenshtein Match Distance on Vln
        public int LevenshteinMatchDistance { get; set; }

        public bool CreatePhysicalInfringement { get; set; }
    }
}
