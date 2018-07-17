using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Kapsch.ITS.Reports.Dev.Operational.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace Kapsch.ITS.Reports.Dev.Operational
{
    class StandardReportMonthlyTotals : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var dateNoTime = DateTime.Today.AddMonths(1).ToString("d/M/yyyy");
            var dateNoTime2 = DateTime.Today.AddMonths(-1).ToString("d/M/yyyy");
            var district = string.Empty;
            DateTimeFormatInfo mfi = new DateTimeFormatInfo();



        var models = new List<StandardReportMonthlyTotalsModelAG>();
            models.Add(
                new StandardReportMonthlyTotalsModelAG
                {

                    IforceLogo = File.ReadAllBytes(@"C:\Users\agabone\Desktop\TMT\iforce.png"),
                    PrintDate = dateNoTime,
                    FromDate = dateNoTime,
                    ToDate = dateNoTime2,
                    CourtName = "LANGEBERG MUNICIPALTY"
                });

            for (int i = 1; i < 10; i++)
            {
                if (i < 5)
                {
                    models.Add(
                    new StandardReportMonthlyTotalsModelAG
                    {
                        Province = "WESTERN CAPE",
                        CameraType = "RADAR",
                        District = "ROBERTSON",
                        Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i),
                        TotalLogged = 25 * i,
                        TotalTest = 45 * i,
                        TotalCaptured = 24 * i,
                        AmountCaptured = 6545 * i,
                        IncomeReceived = 99453 * i,
                        TotalPrinted = 25 * i,
                        AmountPrinted = 4 * i,
                        NumberPaid = 1 * i,
                        AveragePaidFine = 3 * i,
                        SuccessRatePayment = 1 * i,
                        SuccessRateIssued = 2 * i,
                        RepsIssued = i,
                        RepsAmount = 69 * i
                    });
                }
                else if (i > 5 && i < 10)
                {
                    models.Add(
                    new StandardReportMonthlyTotalsModelAG
                    {
                        Province = "EASTERN CAPE",
                        CameraType = "CAMERA",
                        District = "NORTH GATE",
                        Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i),
                        TotalLogged = 25 * i,
                        TotalTest = 45 * i,
                        TotalCaptured = 24 * i,
                        AmountCaptured = 6545 * i,
                        IncomeReceived = 99453 * i,
                        TotalPrinted = 25 * i,
                        AmountPrinted = 4 * i,
                        NumberPaid = 1 * i,
                        AveragePaidFine = 3 * i,
                        SuccessRatePayment = 1 * i,
                        SuccessRateIssued = 2 * i,
                        RepsIssued = i,
                        RepsAmount = 69 * i
                    });
                }
                else
                {
                    models.Add(
                    new StandardReportMonthlyTotalsModelAG
                    {
                        Province = "WESTERN CAPE",
                        CameraType = "SAFETYCAM",
                        District = "ROBERTSON",
                        Month = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName(i),
                        TotalLogged = 25 * i,
                        TotalTest = 45 * i,
                        TotalCaptured = 24 * i,
                        AmountCaptured = 6545 * i,
                        IncomeReceived = 99453 * i,
                        TotalPrinted = 25 * i,
                        AmountPrinted = 4 * i,
                        NumberPaid = 1 * i,
                        AveragePaidFine = 3 * i,
                        SuccessRatePayment = 1 * i,
                        SuccessRateIssued = 2 * i,
                        RepsIssued = i,
                        RepsAmount = 69 * i
                    });

                    models.Add(
                    new StandardReportMonthlyTotalsModelAG
                    {
                        TotalCaptured = 24 * i,
                        MonthlyGRIDDescription = Membership.GeneratePassword(20, 30)
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

        private ReportViewer BuildReport(List<StandardReportMonthlyTotalsModelAG> models)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Dev.Templates.StandardReportMonthlyTotalsAG.rdlc";
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
            get { return "iTicket: Standard Report Monthly Totals Model AG"; }
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