using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Kapsch.ITS.Reports;
using Kapsch.ITS.Reports.Dev.Operational.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational
{
    class AdjudicationsPerDistrictSummary : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(Core.Reports.Enums.ExportType exportType, string[] parameters)
        {
            var dateNoTime  = DateTime.Today.AddMonths(1).ToString("d/M/yyyy");
            var dateNoTime2 = DateTime.Today.AddMonths(-1).ToString("d/M/yyyy");
            var province = string.Empty;
            var district = string.Empty;
            var models = new List<AdjudicationsPerDistrictSummaryModelAG>();

            for (int i = 0; i < 20; i++)
            {
                if (i == 0)
                {
                    models.Add(
                    new AdjudicationsPerDistrictSummaryModelAG
                    {
                        Province = "Eastern Cape",
                        District = "HUMANSDORP",
                    });
                }
                else if (i == 5)
                {
                    models.Add(
                    new AdjudicationsPerDistrictSummaryModelAG
                    {
                        Province = "Western Cape",
                        District = "Somerser West",
                    });
                }
                else if (i == 15)
                {
                    models.Add(
                    new AdjudicationsPerDistrictSummaryModelAG
                    {
                        Province = "LIMPOP",
                        District = "BELA BELA",

                    });
                 }

                models.Add(
                new AdjudicationsPerDistrictSummaryModelAG
                {
                    TicketDate = dateNoTime,
                    VerificationDate = dateNoTime2,
                    NoOfTickets = 7 + i,
                    DaysToExpire = 13
                });


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

        private ReportViewer BuildReport(List<AdjudicationsPerDistrictSummaryModelAG> models)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.AdjudicationsPerDistrictSummary.rdlc";
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
            get { return "iTicket: Adjudications Per District Summary AG"; }
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
