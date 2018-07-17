using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational.Models
{
    class SummonsStatsPerCourtDateModelAG
    {
        public string DistrictName { get; set; }
        public string CourtName { get; set; }
        public string CourtDate { get; set; }
        public int TotalSummonses { get; set; }
        public int TotalTickets { get; set; }
        public int TotalSummonsPaid { get; set; }
        public int TotalSummonsServed { get; set; }
        public int TotalTicketsPaid { get; set; }
        public int TotalTicketsServed { get; set; }
        public int TicketsPaidAfterServed { get; set; }
        public int TotalSummonsWarrants { get; set; }
        public int TotalTicketsWarrant { get; set; }
        public string SummonsType { get; set; }
        public string TicketType { get; set; }
        public object IforceLogo { get; set; }
        public string PrintDate { get; set; }
        public string SearchDistrictName { get; set; }
        public string SearchCourtName { get; set; }
        public string SearchSummonsType { get; set; }
        public string SearchTicketType { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
