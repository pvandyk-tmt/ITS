namespace TMT.Enforcement.iLog
{
    public class ReportSource
    {
        public ReportSource()
        {
            CountOfInfringements = 0;
            CamDate = "";
            Session = "";
            Time = "";
            VehiclesChecked = 0;
            AverageSpeed = 0;
            HighestSpeed = 0;
            TestPhotoCount = 0;
            JammerCount = 0;
            ErrorsCount = 0;
            Operators = "";
            LocationCode = "";
            pGroup = "";
            //CamID = "";
            //StatsFileName = "";
            //LoggedBy = "";
        }

        public string CamDate { get; set; }
        public string Session { get; set; }
        public string Time { get; set; }
        public int CountOfInfringements { get; set; }
        public int VehiclesChecked { get; set; }
        public decimal AverageSpeed { get; set; }
        public int HighestSpeed { get; set; }
        public int TestPhotoCount { get; set; }
        public int JammerCount { get; set; }
        public int ErrorsCount { get; set; }
        public string Operators { get; set; }
        public string LocationCode { get; set; }
        public string pGroup { get; set; }
        //public string CamID { get; set; }
        //public string StatsFileName { get; set; }
        //public string LoggedBy { get; set; }
    }
}