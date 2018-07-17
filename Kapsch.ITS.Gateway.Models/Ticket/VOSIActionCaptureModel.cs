using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Ticket
{
    public class VosiActionCaptureModel
    {
        public long ID { get; set; }
        public long VosiActionID { get; set; }
        public string VLN { get; set; }
        public string LocationStreet { get; set; }
        public string LocationSuburb { get; set; }
        public string LocationTown { get; set; }
        public decimal? LocationLatitude { get; set; }
        public decimal? Locationlongitude { get; set; }
        public string Comments { get; set; }
        public DateTime CapturedDateTime { get; set; }
        public long CapturedCredentialID { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
