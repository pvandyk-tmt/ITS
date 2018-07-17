using System;

namespace Kapsch.ITS.Reports.Operational.Models
{
    public class OfficerVosiActionModel
    {
        public long DistrictID { get; set; }
        public string DistrictName { get; set; }
        public long OfficerID { get; set; }
        public string OfficerName { get; set; }
        public string OfficerExternalNumber { get; set; }
        public DateTime ActionTimestamp { get; set; }
        public string VLN { get; set; }
        public string Detail { get; set; }
        public string ActionDescription { get; set; }
    }
}
