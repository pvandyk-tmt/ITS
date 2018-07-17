using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational.Models
{
    class VehicleRejectedModel
    {
        public string TicketNo { get; set; }
        public string VehicleReg { get; set; }
        public string TicketDate { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string RejectionReason { get; set; }
        public string RejectedBy { get; set; }
        public string VerifiedDate { get; set; }
        public object IforceLogo { get; set; }
        public string PrintDate { get; set; }
        public string DistrictName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TicketType { get; set; }
    }
}
