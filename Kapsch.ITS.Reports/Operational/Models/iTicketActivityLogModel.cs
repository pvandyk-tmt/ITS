using System;

namespace Kapsch.ITS.Reports.Operational.Models
{
    public class iTicketActivityLogModel
    {
        public DateTime CreatedTimestamp { get; set; }
        public string OfficerNumber { get; set; }
        public string OfficerName { get; set; }
        public string Category { get; set; }
        public string ActionDescription { get; set; }
        public string DeviceID { get; set; }
    }
}
