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
    class NatisNotFoundAG : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var now = DateTime.Now;
            var lastmonth = DateTime.Today.AddMonths(-1);
            var nextmonth = DateTime.Today.AddMonths(1);
            var time = DateTime.Now.ToString("HH:mm:ss tt");
            var no_time_Fine_Exp_Dat = DateTime.Today.AddMonths(1).ToString("d/M/yyyy");
            var no_time_Date_Of_Offence = DateTime.Today.AddMonths(-1).ToString("d/M/yyyy");

            var models = new List<NatisNotFoundModelAG>();
            models.Add(
                new NatisNotFoundModelAG
                {
                District = "All",
                Court = "All",
                Include = "Not Found",
                IforceLogo = File.ReadAllBytes(@"C:\Users\agabone\Desktop\iforce.png"),
                 TicketNo = "23/79614/602/159853",
                 TicketDate = now,
                 VehicleRegisteration = "GH 873483",
                 ReportDate = no_time_Date_Of_Offence
                });

                for (int i = 0; i < 20; i++)
                {
                    models.Add(
                    new NatisNotFoundModelAG
                    {
                        TicketNo = "22/65443/602/131510"+i,
                        TicketDate = DateTime.Today.AddMonths(-i),
                        VehicleRegisteration = "CF 87348"+i,
                        ReportDate = no_time_Date_Of_Offence,
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

        private ReportViewer BuildReport(List<NatisNotFoundModelAG> models)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Dev.Templates.NatisNotFoundAG.rdlc";
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
            get { return "iTicket: Natis Not Found AG"; }
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