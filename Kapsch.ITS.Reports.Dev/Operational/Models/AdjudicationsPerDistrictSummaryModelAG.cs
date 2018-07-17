using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational.Models
{
    class AdjudicationsPerDistrictSummaryModelAG
    {
        public string TicketDate { get; set; }
        public string VerificationDate { get; set; }
        public int NoOfTickets { get; set; }
        public int DaysToExpire { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }
}
