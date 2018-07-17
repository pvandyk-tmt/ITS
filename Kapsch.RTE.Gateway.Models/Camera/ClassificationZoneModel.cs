using Kapsch.RTE.Gateway.Models.Camera.Enum;

namespace Kapsch.RTE.Gateway.Models.Camera
{
    public class ClassificationZoneModel
    {
        public int Zone { get; set; }
        public int Grace { get; set; }
        public VehicleClassificationEnum Classification { get; set; }
    }
}
