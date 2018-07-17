namespace Kapsch.RTE.Gateway.Models.Camera 
{
    public class OverSectionModel
    {
        public AtPointModel AtPointA { get; set; }
        public AtPointModel AtPointB { get; set; }
        public double? AverageSpeed { get; set; }
        public int? GraceSpeed { get; set; }
        public int? Zone { get; set; }
        public double? TripDuration { get; set; }
        public double? TravelDistance { get; set; }
        public double SectionDistanceInMeter { get; set; }
        public string SectionCode { get; set; }
        public string SectionDescription { get; set; }
        public string Vln { get; set; }
        public string DateFormat { get; set; }
        public double? AverageAnprAccuracy { get; set; }
        public string MachineId { get; set; }
        public bool IsOffence { get; set; }
        public string FileName { get; set; }
        public int FrameNumber { get; set; }

        /// <summary>
        /// The Source of the listener that created the Point Data.
        /// For Socket use 'IPAddress:Port'
        /// For Mock use 'Mock'
        /// For Disk use 'FilePath'
        /// </summary>
        public string ListenerSource { get; set; }
    }

}
