using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational.Models
{
    class NatisNotFoundModelAG
    {
        public string TicketNo { get; set; }
        public DateTime TicketDate { get; set; }
        public string VehicleRegisteration { get; set; }
        public string ReportDate { get; set; }
        public object IforceLogo { get; set; }
        public string District { get; internal set; }
        public string Court { get; internal set; }
        public string Include { get; internal set; }
    }
}
