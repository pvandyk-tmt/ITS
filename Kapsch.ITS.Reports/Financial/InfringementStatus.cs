using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Kapsch.ITS.Reports.Financial.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace Kapsch.ITS.Reports.Financial
{
    public class InfringementStatus : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var districtList = new List<long>();
            var startDate = default(DateTime);
            var endDate = default(DateTime);
            var periodType = string.Empty;
            var infringementType = default(InfringementType?);
            var infringementValue = default(decimal?);

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
                else if (parts[0].Equals("infringementType", StringComparison.InvariantCultureIgnoreCase))
                {
                    InfringementType infringementType_ = 0;
                    if (Enum.TryParse(parts[1], out infringementType_))
                        infringementType = infringementType_;
                }
                else if (parts[0].Equals("periodType", StringComparison.InvariantCultureIgnoreCase))
                {
                    periodType = parts[1];
                }
                else if (parts[0].Equals("startDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out startDate);

                }
                else if (parts[0].Equals("endDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out endDate);
                }
                else if (parts[0].Equals("infringementValue", StringComparison.InvariantCultureIgnoreCase))
                {
                    decimal infringementValue_ = 0;
                    if (decimal.TryParse(parts[1], out infringementValue_))
                        infringementValue = infringementValue_;
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
                    .Where(f => (f.Register.DistrictID.HasValue && districtList.Contains(f.Register.DistrictID.Value)) &&
                                (f.InfringementDate >= startDate && f.InfringementDate <= endDate));

                if (infringementType.HasValue)
                {
                    query = query.Where(f => f.InfringementType == infringementType.Value);
                }

                if (infringementValue.HasValue)
                {
                    query = query.Where(f => f.CapturedAmount.HasValue && f.CapturedAmount.Value >= infringementValue.Value);
                }

                var entities = query
                    .OrderBy(f => new { f.InfringementDate, f.District.BranchName })
                    //.OrderBy(f => f.District.BranchName)
                    .ToList();

                var models = entities.Select(f =>
                    new InfringementStatusModel
                    {
                        InfringementDate = f.InfringementDate,
                        InfringementNumber = f.ReferenceNumber,
                        OriginalAmount = !f.CapturedAmount.HasValue ? 0 : f.CapturedAmount.Value,
                        CurrentAmount = !f.Register.OutstandingAmount.HasValue ? 0 : f.Register.OutstandingAmount.Value,
                        FormattedRegisterStatus = (f.Register.RegisterStatus).ToString(),
                        FormattedInfringementType = (f.InfringementType).ToString(),
                        CourtName = f.Court.CourtName,
                        DistrictName = f.Register.District.BranchName
                    })
                    .ToList();

                filterCriteria += string.Format("District: {0} ", districtList.Count == 1 ? dbContext.Districts.Find(districtList[0]).BranchName : "ALL");
                filterCriteria += string.Format("Period: {0:yyyy/MM/dd} - {1:yyyy/MM/dd} ", startDate, endDate);
                filterCriteria += string.Format("Infringement Type: {0} ", infringementType.HasValue ? infringementType.ToString() : "ALL");
                if (infringementValue.HasValue && infringementValue.Value > 0) filterCriteria += string.Format("Values >: {0:0.00} ", infringementValue.Value);


                if (exportType == ExportType.PDF)
                {
                    return StreamPdfReport(BuildReport(models, filterCriteria));
                }

                if (exportType == ExportType.Excel)
                {
                    return StreamExcelReport(BuildReport(models, filterCriteria));
                }

                return null;
            }
        }

        private ReportViewer BuildReport(IList<InfringementStatusModel> models, string filterCriteria)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.InfringementStatus.rdlc";
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
                return "Financial";
            }
        }

        public string RequiredAccessRole
        {
            get { return "IMSPortalReportsViewOprationalReports"; }
        }

        public string ReportName
        {
            get { return "Infringement Status"; }
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
                        Kapsch.Core.Reports.Enums.ParameterType.InfringementValue
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
