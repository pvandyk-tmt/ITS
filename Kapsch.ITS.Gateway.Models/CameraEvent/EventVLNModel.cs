
namespace Kapsch.ITS.Gateway.Models.CameraEvent
{
    /// <summary>
    /// The model gets exposed to add device events to the iTrac Core.
    /// </summary>
    public class EventVLNModel
    {
        public string VLN { get; set; }
        public string Confidence { get; set; }
    }
}
