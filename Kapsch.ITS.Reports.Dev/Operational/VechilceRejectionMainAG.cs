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
    class VechilceRejectionMainAG : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var now = DateTime.Now;
            var lastmonth = DateTime.Today.AddMonths(-1);
            var nextmonth = DateTime.Today.AddMonths(1);
            var time = DateTime.Now.ToString("HH:mm:ss tt");
            var no_time_Fine_Exp_Dat = DateTime.Today.AddMonths(1).ToString("d/M/yyyy");
            var no_time_Date_Of_Offence = DateTime.Today.AddMonths(-1).ToString("d/M/yyyy");

            var models = new List<VehicleRejectedModel>();
            models.Add(
                new VehicleRejectedModel
                {   
                    PrintDate       =   no_time_Fine_Exp_Dat,
                    DistrictName    =   "CERES",
                    FromDate        =   no_time_Date_Of_Offence,
                    ToDate          =   no_time_Fine_Exp_Dat,
                    TicketType      =   "All",
                    TicketNo        =   "1235 7895 4562 1954",
                    VehicleReg      =   "CA 124566",
                    TicketDate      =   no_time_Date_Of_Offence,
                    Make            =   "Ducati",
                    Model           =   "848",
                    RejectionReason =   "This is the reason - 1234",
                    RejectedBy      =   "Dillan Davids",
                    VerifiedDate    =   no_time_Date_Of_Offence,
                    IforceLogo      = File.ReadAllBytes(@"C:\Users\agabone\Desktop\iforce.png")
                });

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

        private ReportViewer BuildReport(List<VehicleRejectedModel> models)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.VehicleRejectedAG.rdlc";
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
            get { return "iTicket: Vehicle Rejected AG"; }
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
