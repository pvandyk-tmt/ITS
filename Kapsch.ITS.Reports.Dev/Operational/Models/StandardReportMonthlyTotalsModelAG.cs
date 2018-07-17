using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational.Models
{
    class StandardReportMonthlyTotalsModelAG
    {
        public byte[] IforceLogo { get; internal set; }
        public string PrintDate { get; internal set; }
        public string FromDate { get; internal set; }
        public string ToDate { get; internal set; }
        public string Month { get; internal set; }
        public int TotalLogged { get; internal set; }
        public int TotalTest { get; internal set; }
        public int TotalCaptured { get; internal set; }
        public int AmountCaptured { get; internal set; }
        public int IncomeReceived { get; internal set; }
        public int TotalPrinted { get; internal set; }
        public int AmountPrinted { get; internal set; }
        public int NumberPaid { get; internal set; }
        public int AveragePaidFine { get; internal set; }
        public int SuccessRatePayment { get; internal set; }
        public int SuccessRateIssued { get; internal set; }
        public int RepsIssued { get; internal set; }
        public int RepsAmount { get; internal set; }
        public string Datedate { get; internal set; }
        public string MonthName { get; internal set; }
        public string CourtName { get; internal set; }
        public string Province { get; internal set; }
        public string CameraType { get; internal set; }
        public string District { get; internal set; }
        public string MonthlyGRIDDescription { get; internal set; }
    }
}
