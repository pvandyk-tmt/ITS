using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Kapsch.ITS.Reports.Dev.Operational.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational
{
    class SummonsStatsPerCourtDateAG : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var dateNoTime = DateTime.Today.AddMonths(1).ToString("d/M/yyyy");
            var dateNoTime2 = DateTime.Today.AddMonths(-1).ToString("d/M/yyyy");
            var province = string.Empty;
            var district = string.Empty;
            var models = new List<SummonsStatsPerCourtDateModelAG>();

            models.Add(
            new SummonsStatsPerCourtDateModelAG
            {
                IforceLogo = File.ReadAllBytes(@"C:\Users\agabone\Desktop\iforce.png"),
                PrintDate = dateNoTime,
                DistrictName = "Stellenbosch",
                CourtName = "Stellenbosch Court",
                CourtDate = dateNoTime2,
                TotalSummonses = 108,
                TotalTickets = 108,
                TotalSummonsPaid = 7,
                TotalTicketsPaid = 13,
                TotalSummonsServed = 75,
                TotalTicketsServed = 71,
                TicketsPaidAfterServed = 5,
                TotalSummonsWarrants = 24,
                TotalTicketsWarrant = 24,
                SummonsType = "SECTION 54",
                TicketType = "Parking",
                SearchDistrictName = "ALL",
                SearchCourtName = "ALL",
                SearchSummonsType = "ALL",
                SearchTicketType = "ALL",
                FromDate = dateNoTime,
                ToDate = dateNoTime2
            });

            for (int i = 0; i < 22; i++)
            {
                if (i < 7)
                {
                    models.Add(
                    new SummonsStatsPerCourtDateModelAG
                    {
                        DistrictName = "BELA BELA",
                        CourtName = "BELA BELA",
                        CourtDate = dateNoTime2,
                        TotalSummonses = 108 + i,
                        TotalTickets = 108 + i,
                        TotalSummonsPaid = i,
                        TotalTicketsPaid = i,
                        TotalSummonsServed = 78 - i,
                        TotalTicketsServed = 78 - i,
                        TicketsPaidAfterServed = 58 - i,
                        TotalSummonsWarrants = 24 - i,
                        TotalTicketsWarrant = 24 - i,
                        SummonsType = "SECTION 54",
                        TicketType = "Speed"
                    });
                }
                else
                {
                    models.Add(
                    new SummonsStatsPerCourtDateModelAG
                    {
                        DistrictName = "CARLETONVILLE",
                        CourtName = "FOCHVILLE",
                        CourtDate = dateNoTime2,
                        TotalSummonses = 108 + i,
                        TotalTickets = 108 + i,
                        TotalSummonsPaid = i,
                        TotalTicketsPaid = i,
                        TotalSummonsServed = 78 - i,
                        TotalTicketsServed = 78 - i,
                        TicketsPaidAfterServed = 58 - i,
                        TotalSummonsWarrants = 24 - i,
                        TotalTicketsWarrant = 24 - i,
                        SummonsType = "SECTION 54",
                        TicketType = "Parking"
                    });
                }

            }
            if (exportType == ExportType.PDF)
            {
                return StreamPdfReport(BuildReport(models));
            }
            else if (exportType == ExportType.Excel)
            {
                return StreamExcelReport(BuildReport(models));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private ReportViewer BuildReport(List<SummonsStatsPerCourtDateModelAG> models)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.SummonsStatsPerCourtDateAG.rdlc";
            //reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FilterCriteria", filterCriteria) });
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", models));
            reportViewer.ShowReportBody = true;

            return reportViewer;
        }

        public string CategoryName
        {
            get { return "RSE"; }
        }

        public string SubCategoryName
        {
            get { return "Operational"; }
        }

        public string RequiredAccessRole
        {
            get { return "IMSPortalReportsViewOprationalReports"; }
        }

        public string ReportName
        {
            get { return "iTicket: Summons Stats Per Court Date AG"; }
        }

        public IList<Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get
            {
                return
                    new[]
                    {
                        Kapsch.Core.Reports.Enums.ParameterType.DistrictMobileDeviceOfficer,
                        Kapsch.Core.Reports.Enums.ParameterType.Period,
                        Kapsch.Core.Reports.Enums.ParameterType.MobileDeviceActivityCategory
                    }
                    .ToList();
            }
        }

        public IList<Core.Reports.Enums.ExportType> ExportTypes
        {
            get
            {
                return
                    new[]
                    {
                        Kapsch.Core.Reports.Enums.ExportType.PDF,
                        Kapsch.Core.Reports.Enums.ExportType.Excel
                    }
                    .ToList();
            }
        }
    }
}
