namespace TMT.Core.Camera.Base
{
    public interface IStatEntry
    {
        //Actual Line Item
        string LineItem { get; set; }
        bool Extract(out string error);

        string Direction { get; set; }
        string Distance { get; set; }
        string Date { get; set; }
        string Time { get; set; }
        int Speed { get; set; }
        int Zone { get; set; }
        string Lane { get; set; }
        string Error { get; set; }
        string Vehicleclass { get; set; }
        string Axles { get; set; }
        bool Captured { get; set; }
        string Axlespacings { get; set; }
        string EncryptedFilename { get; set; }
        int LowSpeed { get; }
        int TestPhoto { get; }
        int Infringement { get; }
        int HighSpeedNonInfringement { get; }
        int VehiclesChecked { get; }
        int MeasurementErrors { get; }
        int CaptureErrors { get; }
    }
}