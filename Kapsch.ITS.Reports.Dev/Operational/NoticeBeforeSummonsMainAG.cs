using Kapsch.Core.Data;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Kapsch.ITS.Reports.Dev.Operational.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Reports.Dev.Operational
{
    class NoticeBeforeSummonsAG : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var now = DateTime.Now;
            var lastmonth = DateTime.Today.AddMonths(-1);
            var nextmonth = DateTime.Today.AddMonths(1);
            var time      = DateTime.Now.ToString("HH:mm:ss tt");
            var no_time_Fine_Exp_Dat = DateTime.Today.AddMonths(1).ToString("d/M/yyyy");
            var no_time_Date_Of_Offence = DateTime.Today.AddMonths(-1).ToString("d/M/yyyy");

            var partOneCamRef   = DateTime.Today.AddMonths(-1).ToString("yyMMdd");
            string partTwoCamRef  = "00-00190F258CDE";
            int  partThreeCamRef = 406;

          var models = new List<NoticeBeforeSummonsModelAG>();
            models.Add(
                new NoticeBeforeSummonsModelAG
                {
                    PersonName = "JAMES ENGELBRECHT",
                    CompanyText = "POST WORLD 300",
                    PersonalDetails1 = "P.O BOX",
                    PersonalDetails2 = "MONTAGU",
                    PersonalDetails3 = "6720",
                    Regulation = "Section 162 of the Road Traffic Act no 11 of 2002 of the Laws of Zambia",
                    RegulationText = "Sect. 148 (1)(a) & 148 (4)(a) of the Road Traffic Act No. 11 of 2002 and Statutory Instrument No. 90 of 2016 (Road Traffic (Speed Limits)) Reg. 4 (I) (1)",
                    OffRegInfringementDate = now,
                    LocationDescription = "LUSAKA - KAFUE ROAD BETWEEN LILAYI CIRCLE AND BONAVENTURE CIRCLE DIRECTION SOUTH",
                    OffRegTicketNo = "0978 2365 0948 2673",
                    LocationCode = "LUS024F",
                    CamReference = partOneCamRef +'/'+ partTwoCamRef + '/' + partThreeCamRef,
                    TicketType = "Camera",
                    ChargeCode = 1480101000,
                    InspNo = "LIMA113",
                    ChargeDecription = "",
                    OffRegGuiltFineExpDate = no_time_Fine_Exp_Dat,
                    FineAmount = "ZMW300",
                    CourtName = "Lusaka",
                    IssuedByLine1 = "RTSA/Zambia Police",
                    IssuedByLine2 = "Traffic Dept",
                    IssuedByLine3 = "Lusaka",
                    IssuedDate = no_time_Date_Of_Offence,
                    BankForPayments = "STANBIC",
                    AccountName = "LUSAKA",
                    AccNumber = 45951835735,
                    PaymentRef = "0978 2365 0948 2673",
                    DateOfOffence = no_time_Date_Of_Offence,
                    TimeOfOffence = time,
                    Location = "LUS024F",
                    Zone = "40km/h",
                    Speed = "62km/h",
                    VehRegistration = "ZM 365489",
                    IDNo = 9158465466855,
                    Officer = "LIMA113",
                    VehicleBrand = "TOYOTA",
                    VehicleType = "HILUX",
                    VehicleImage = File.ReadAllBytes(@"C:\Users\agabone\Desktop\vehicle_Image.jpg"),
                    VehicleNumberPlate = File.ReadAllBytes(@"C:\Users\agabone\Desktop\vehicle_number_plate.png"),
                    QrCode = File.ReadAllBytes(@"C:\Users\agabone\Desktop\qr_code.png"),
                    Stanbic = File.ReadAllBytes(@"C:\Users\agabone\Desktop\stanbic.png"),
                    LusakaCourtLogo = File.ReadAllBytes(@"C:\Users\agabone\Desktop\lusakaCourtLogo.png"),
                    ZPRTSALogo = File.ReadAllBytes(@"C:\Users\agabone\Desktop\zambia_police_rtsa_logo_small.png")
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

        private ReportViewer BuildReport(List<NoticeBeforeSummonsModelAG> models)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.NoticeBeforeSummonsAG.rdlc";
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
            get { return "iTicket: Notice Before Summons AG"; }
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
