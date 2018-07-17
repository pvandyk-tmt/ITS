using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Kapsch.ITS.Reports.Financial.Models;
using System.Diagnostics;

namespace Kapsch.ITS.Reports.Financial
{
    public class InfringementsPaid : ReportViewerBase, IReportDefinition
    {

        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var districtList = new List<long>();
            var excludeBeforeDate = default(DateTime?);
            var infringementType = default(InfringementType?);
            var infringementValue = default(decimal?);

            foreach (var parameter in parameters)
            {
                var parts = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts[0].Equals("districtID", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    long districtID = 0;
                    if (long.TryParse(parts[1], out districtID))
                        districtList.Add(districtID);
                }
                else if (parts[0].Equals("infringementType", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    InfringementType infringementType_ = 0;
                    if (Enum.TryParse(parts[1], out infringementType_))
                        infringementType = infringementType_;
                }
                else if (parts[0].Equals("excludeBeforeDate", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    DateTime excludeBeforeDate_;
                    if (DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out excludeBeforeDate_))
                        excludeBeforeDate = excludeBeforeDate_;
                }
                else if (parts[0].Equals("infringementValue", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    decimal infringementValue_ = 0;
                    if (decimal.TryParse(parts[1], out infringementValue_))
                        infringementValue = infringementValue_;
                }
            }

            using (var dbContext = new DataContext())
            {
                //dbContext.Database.Log = f => Debug.WriteLine(f);

                var query = dbContext.OffenceRegister
                    .AsNoTracking()
                    .Include(f => f.District)
                    .Join(
                        dbContext.PaymentTransactionItems
                            .AsNoTracking()
                            .Include(f => f.PaymentTransaction), 
                        f => f.ReferenceNumber, 
                        f => f.ReferenceNumber,
                        (offenceRegister, paymentTransactionItems) => new 
                        { 
                            DistrictID = offenceRegister.District.ID, 
                            DistrictName = offenceRegister.District.BranchName,
                            Status = paymentTransactionItems.PaymentTransaction.Status, 
                            Amount = paymentTransactionItems.Amount,
                            CreatedTimestamp = paymentTransactionItems.PaymentTransaction.CreatedTimestamp,
                            InfringementType = offenceRegister.InfringementType,
                            CapturedAmount = offenceRegister.CapturedAmount,
                            ReferenceNumber = offenceRegister.ReferenceNumber
                        })
                    .Where(
                        f => districtList.Contains(f.DistrictID) && 
                        f.Status == PaymentTransactionStatus.Added);

                if (excludeBeforeDate.HasValue)
                {
                    query = query.Where(f => f.CreatedTimestamp >= excludeBeforeDate.Value);
                }

                if (infringementType.HasValue)
                {
                    query = query.Where(f => f.InfringementType == infringementType.Value);
                }

                if (infringementValue.HasValue)
                {
                    query = query.Where(f => f.CapturedAmount >= infringementValue.Value);
                }

                var models = query
                    .ToList()
                    .GroupBy(f => new { f.DistrictName, f.DistrictID })
                    .Select(f => new InfringementPaidModel
                    {
                        DistrictName = f.Key.DistrictName,
                        DistrictID = f.Key.DistrictID,
                        Count = f.GroupBy(g => g.ReferenceNumber).Count(),
                        Value = f.Sum(g => g.Amount)
                    })
                    .OrderBy(f => f.DistrictName)
                    .ToList();

                filterCriteria += string.Format("District: {0} ", districtList.Count == 1 ? dbContext.Districts.Find(districtList[0]).BranchName : "ALL");
                if (excludeBeforeDate.HasValue) filterCriteria += string.Format("Infringements From: {0:yyyy/MM/dd} ", excludeBeforeDate.Value);
                filterCriteria += string.Format("Infringement Type: {0} ", infringementType.HasValue ? infringementType.ToString() : "ALL");
                if (infringementValue.HasValue && infringementValue.Value > 0) filterCriteria += string.Format("Values >: {0:0.00} ", infringementValue.Value);
                

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

        private ReportViewer BuildReport(IList<InfringementPaidModel> models, string filterCriteria)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.InfringementsPaid.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FilterCriteria", filterCriteria) });
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", models));
            reportViewer.ShowReportBody = true;

            return reportViewer;
        }

        public string RequiredAccessRole
        {
            get { return "IMSPortalReportsViewOprationalReports"; }
        }

        public string CategoryName
        {
            get { return "RSE"; }
        }

        public string SubCategoryName
        {
            get { return "Financial"; }
        }

        public string ReportName
        {
            get { return "Payments Report: Infringements Paid"; }
        }

        public IList<Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get
            {
                return
                    new[] 
                    { 
                        Kapsch.Core.Reports.Enums.ParameterType.District,
                        Kapsch.Core.Reports.Enums.ParameterType.ExcludeBeforeDate,
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
