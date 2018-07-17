using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using Kapsch.Core.Reports;
using Microsoft.Reporting.WebForms;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kapsch.ITS.Reports.Models;
using Kapsch.Core.Reports.Enums;

namespace Kapsch.ITS.Reports.Operational
{
    public class OutstandingInfringers : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(Core.Reports.Enums.ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var startDate = default(DateTime);
            var endDate = default(DateTime);
            var districtList = new List<long>();
            var infringementType = default(InfringementType?);
            var infringementValue = default(decimal?);
            var registerStatus = default(RegisterStatus?);
            var periodType = string.Empty;
                    
            foreach (var parameter in parameters)
            {
                var parts = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                if (parts[0].Equals("districtID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long districtID = 0;
                    if (long.TryParse(parts[1], out districtID))
                        districtList.Add(districtID);
                }
                else if (parts[0].Equals("startDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out startDate);

                }
                else if (parts[0].Equals("endDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out endDate);
                }
                else if (parts[0].Equals("infringementType", StringComparison.InvariantCultureIgnoreCase))
                {
                    InfringementType infringementType_ = 0;
                    if (Enum.TryParse(parts[1], out infringementType_))
                        infringementType = infringementType_;
                }
                else if (parts[0].Equals("registerStatus", StringComparison.InvariantCultureIgnoreCase))
                {
                    RegisterStatus registerStatus_ = 0;
                    if (Enum.TryParse(parts[1], out registerStatus_))
                        registerStatus = registerStatus_;
                }
                else if (parts[0].Equals("infringementValue", StringComparison.InvariantCultureIgnoreCase))
                {
                    decimal infringementValue_ = 0;
                    if (decimal.TryParse(parts[1], out infringementValue_))
                        infringementValue = infringementValue_;
                }
                else if (parts[0].Equals("periodType", StringComparison.InvariantCultureIgnoreCase))
                {
                    periodType = parts[1];
                }                
            }

            if (periodType.Equals("thisMonth", StringComparison.InvariantCultureIgnoreCase))
            {
                var now = DateTime.Now;

                startDate = new DateTime(now.Year, now.Month, 1);
                endDate = now.Date.AddDays(1);
            }
            else if (periodType.Equals("lastMonth", StringComparison.InvariantCultureIgnoreCase))
            {
                var now = DateTime.Now;

                startDate = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                endDate = startDate.AddMonths(1).AddMilliseconds(-1);
            }
            else if (periodType.Equals("thisYear", StringComparison.InvariantCultureIgnoreCase))
            {
                var now = DateTime.Now;

                startDate = new DateTime(now.Year, 1, 1);
                endDate = now.Date.AddDays(1);
            }
            else if (periodType.Equals("lastYear", StringComparison.InvariantCultureIgnoreCase))
            {
                var now = DateTime.Now;

                startDate = new DateTime(now.Year, 1, 1).AddYears(-1);
                endDate = startDate.AddYears(1).AddMilliseconds(-1);
            }

            using (var dbContext = new DataContext())
            {
                //dbContext.Database.Log = f => Debug.WriteLine(f);
                endDate = endDate.AddDays(1).AddMilliseconds(-1);
                
                var query = dbContext.OffenceRegister
                    .AsNoTracking()
                    .Include(f => f.Register)
                    .Include(f => f.Register.District)
                    .Include(f => f.Court)
                    .Include(f => f.Register.Person)
                    .Include(f => f.InfringementLocation)
                    .Include(f => f.EvidenceLog)
                    .Include(f => f.EvidenceLog.SpeedLog)
                    .Include(f => f.EvidenceLog.ChargeInfos)
                    .Include(f => f.EvidenceLog.InfringementEvidences)
                    .Include(f => f.EvidenceLog.HandWrittenCaptureLog)
                    .Where(f => (f.Register.DistrictID.HasValue && districtList.Contains(f.Register.DistrictID.Value)) &&
                                (f.InfringementDate >= startDate && f.InfringementDate <= endDate));

                if (infringementType.HasValue)
                {
                    query = query.Where(f => f.InfringementType == infringementType.Value);
                }

                if (registerStatus.HasValue)
                {
                    query = query.Where(f => f.Register.RegisterStatus == (int)registerStatus.Value);
                }

                if (infringementValue.HasValue)
                {
                    query = query.Where(f => f.CapturedAmount.HasValue && f.CapturedAmount.Value >= infringementValue.Value);
                }

                var offences = query.OrderByDescending(f => f.InfringementDate).ToList();
                var models = new List<OutstandingInfringerModel>();

                foreach (var offence in offences)
                {
                    var model = new OutstandingInfringerModel();

                    model.ReferenceNumber = offence.ReferenceNumber;
                    model.DistrictName = offence.Register.District == null ? string.Empty : offence.Register.District.BranchName;                  
                    model.OffenceLocation = offence.InfringementLocation == null ? string.Empty : offence.InfringementLocation.Description;
                    model.OutstandingAmount = offence.Register.OutstandingAmount.Value;
                    model.OffenceDate = offence.InfringementDate;
                    model.OffenderName = offence.Register.Person == null ? string.Empty : string.Format("{0} {1}", offence.Register.Person.FirstNames, offence.Register.Person.LastName);                  
                    model.OffenderEmail = offence.Register.Person == null ? string.Empty : offence.Register.Person.Email;
                    model.OffenderMobile = offence.Register.Person == null ? string.Empty : offence.Register.Person.MobileNumber;
                    model.FormattedRegisterStatus = offence.Register.RegisterStatus.ToString();

                    if (offence.EvidenceLog != null)
                    {
                        if (offence.EvidenceLog.ChargeInfos != null && offence.EvidenceLog.ChargeInfos.Count > 0)
                        {
                            var firstChargeInfo = offence.EvidenceLog.ChargeInfos.First();
                            model.OffenceCode = firstChargeInfo.OffenceCode.Code;
                            model.OffenceDescription = firstChargeInfo.PrimaryDescription;
                        }

                        if (offence.EvidenceLog.HandWrittenCaptureLog != null)
                        {
                            model.OffenceLocation = offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationStreet;
                            if (!string.IsNullOrWhiteSpace(offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationSuburb))
                                model.OffenceLocation += string.Format("\n{0}", offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationSuburb);
                            if (!string.IsNullOrWhiteSpace(offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationTown))
                                model.OffenceLocation += string.Format("\n{0}", offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationTown);
                        }
                    }

                    models.Add(model);
                }

                filterCriteria += string.Format("District: {0} ", districtList.Count == 1 ? dbContext.Districts.Find(districtList[0]).BranchName : "ALL");
                filterCriteria += string.Format("Period: {0:yyyy/MM/dd} - {1:yyyy/MM/dd} ", startDate, endDate);
                filterCriteria += string.Format("Infringement Type: {0} ", infringementType.HasValue ? infringementType.ToString() : "ALL");
                if (infringementValue.HasValue && infringementValue.Value > 0) filterCriteria += string.Format("Values >: {0:0.00} ", infringementValue.Value);
                filterCriteria += string.Format("Register Status: {0} ", registerStatus.HasValue ? registerStatus.ToString() : "ALL");

                if (exportType == ExportType.PDF)
                {
                    return StreamPdfReport(BuildReport(models, filterCriteria));
                }
                else if (exportType == ExportType.Excel)
                {
                    return StreamExcelReport(BuildReport(models, filterCriteria));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        private ReportViewer BuildReport(IList<OutstandingInfringerModel> models, string filterCriteria)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.OutstandingInfringers.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FilterCriteria", filterCriteria) });
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", models));
            reportViewer.ShowReportBody = true;

            return reportViewer;
        }

        
        public string CategoryName
        {
            get
            {
                return "RSE";
            }
        }

        public string SubCategoryName
        {
            get
            {
                return "Operational";
            }
        }

        public string RequiredAccessRole
        {
            get { return "IMSPortalReportsViewOprationalReports"; }
        }

        public string ReportName
        {
            get { return "Outstanding Infringers"; }
        }

        public IList<Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get
            {
                return
                    new[] 
                    { 
                        Kapsch.Core.Reports.Enums.ParameterType.District,
                        Kapsch.Core.Reports.Enums.ParameterType.Period,
                        Kapsch.Core.Reports.Enums.ParameterType.InfringementType,
                        Kapsch.Core.Reports.Enums.ParameterType.InfringementValue,
                        Kapsch.Core.Reports.Enums.ParameterType.RegisterStatus
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
