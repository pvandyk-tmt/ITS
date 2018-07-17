
namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class CaseModel
    {
        public CaseModel()
        {
            OnlyOneImage = false;
            Image1ID = -1;
            Image2ID = -1;
            Image3ID = -1;
            Image4ID = -1;
            RemoteImage1ID = -1;
            RemoteImage2ID = -1;
            RemoteImage3ID = -1;
            RemoteImage4ID = -1;
            PrintImageNumber = 0;
            PreviousRejectID = -1;
            OffenceSpeed = -1;
            OffenceZone = -1;
        }

        public string VehicleRegisterNumber { get; set; }
        public string VehicleType { get; set; }
        public string OffenceDate { get; set; }
        public string OffencePlace { get; set; }
        public int PreviousRejectID { get; set; }
        public int OffenceSpeed { get; set; }
        public int OffenceZone { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public string Image4 { get; set; }
        public string ImageNP { get; set; }
        public int Image1ID { get; set; }
        public int Image2ID { get; set; }
        public int Image3ID { get; set; }
        public int Image4ID { get; set; }
        public bool OnlyOneImage { get; set; }
        public int PrintImageNumber { get; set; }
        public bool IsNumberPlateCentralCaptured { get; set; }
        public string NumberPlateCentralPath { get; set; }

        public string RemoteImage1 { get; set; }
        public string RemoteImage2 { get; set; }
        public string RemoteImage3 { get; set; }
        public string RemoteImage4 { get; set; }
        public string RemoteImageNP { get; set; }
        public int RemoteImage1ID { get; set; }
        public int RemoteImage2ID { get; set; }
        public int RemoteImage3ID { get; set; }
        public int RemoteImage4ID { get; set; }
        public int RemotePrintImageNumber { get; set; }
    }
}
