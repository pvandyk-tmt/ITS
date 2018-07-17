using System;

namespace Kapsch.EVR.Reports.Models
{
    public class VehicleInspectionModel
    {
        public int ID { get; set; }
        public string VLN { get; set; }
        public string BookingReference { get; set; }
        public DateTime TestDate { get; set; }
        public bool IsPassed { get; set; }
        public long TestCategoryID { get; set; }
        public string TestCategoryName { get; set; }
        public long UserID { get; set; }
        public string UserFullName { get; set; }
        public DateTime? StartedTimestamp { get; set; }
        public DateTime? EndedTimestamp { get; set; }
        public long SiteID { get; set; }
        public string SiteName { get; set; }

        public string Duration
        {
            get
            {
                if (!StartedTimestamp.HasValue || !EndedTimestamp.HasValue)
                    return string.Empty;

                return EndedTimestamp.Value.Subtract(StartedTimestamp.Value).ToString(@"hh\:mm\:ss");

            }
        }

        public string FormattedStartedTimestamp
        {
            get
            {
                if (!StartedTimestamp.HasValue)
                    return string.Empty;

                return StartedTimestamp.Value.TimeOfDay.ToString(@"hh\:mm\:ss");

            }
        }

        public string FormattedEndedTimestamp
        {
            get
            {
                if (!EndedTimestamp.HasValue)
                    return string.Empty;

                return EndedTimestamp.Value.TimeOfDay.ToString(@"hh\:mm\:ss");

            }
        }

        public string FormattedIsPassed
        {
            get
            {

                return IsPassed ? "Pass" : "Fail";

            }
        }
    }
}
