using Kapsch.Core.Data;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Kapsch.ITS.Reports.Operational.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;

namespace Kapsch.ITS.Reports.Operational
{
    public class OfficerVosiActions : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(Core.Reports.Enums.ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var districtList = new List<long>();
            var officerID = default(long?);
            var startDate = default(DateTime);
            var endDate = default(DateTime);
            var periodType = string.Empty;

            foreach (var parameter in parameters)
            {
                var parts = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                else if (parts[0].Equals("startDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out startDate);

                }
                else if (parts[0].Equals("endDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out endDate);
                }
                else if (parts[0].Equals("districtID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long districtID = 0;
                    if (long.TryParse(parts[1], out districtID))
                        districtList.Add(districtID);
                }
                else if (parts[0].Equals("officerID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long officerID_ = 0;
                    if (long.TryParse(parts[1], out officerID_))
                        officerID = officerID_;
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

                var query = dbContext.VosiActionCaptures
                    .AsNoTracking()
                    .Include(f => f.CapturedCredential)
                    .Include(f => f.CapturedCredential.User)
                    .Join(dbContext.MobileDevices, f => f.DeviceID, g => g.DeviceID, (f, g) => new { f, g })
                    .Where(f =>
                        f.g.DistrictID != null
                        && districtList.Contains(f.g.DistrictID.Value))
                    .Select(f => new { f.f, f.g.District.ID, f.g.District.BranchName });

                if (officerID.HasValue)
                {
                    var officerID_ = officerID.Value;
                    query = query.Where(f => f.f.CapturedCredential.EntityID == officerID_);
                }

                var models = query
                    .OrderBy(f => f.f.CapturedDateTime)
                    .ToList()
                    .Select(f => new OfficerVosiActionModel()
                    {
                        ActionTimestamp = f.f.CapturedDateTime,
                        ActionDescription = f.f.VosiAction.Description,
                        Detail = f.f.Comments,
                        DistrictID = f.ID,
                        DistrictName = f.BranchName,
                        OfficerExternalNumber = f.f.CapturedCredential.User.ExternalID,
                        OfficerID = f.f.CapturedCredential.User.ID,
                        OfficerName = string.Format("{0} {1}", f.f.CapturedCredential.User.FirstName, f.f.CapturedCredential.User.LastName),
                        VLN = f.f.VLN
                    })
                    .ToList()
                    .OrderBy(f => f.DistrictID)
                    .OrderBy(f => f.OfficerID)
                    .ToList();

                filterCriteria += string.Format("District: {0} ", districtList.Count == 1 ? dbContext.Districts.Find(districtList[0]).BranchName : "ALL");
                filterCriteria += string.Format("Period: {0:yyyy/MM/dd} - {1:yyyy/MM/dd} ", startDate, endDate);
                if (officerID.HasValue)
                {
                    var officer = dbContext.Users.Find(officerID.Value);
                    filterCriteria += string.Format("Officer: {0} {1} ", officer.FirstName, officer.LastName);
                }
                else
                {
                    filterCriteria += string.Format("Officer: ALL ");
                }

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

        private ReportViewer BuildReport(IList<OfficerVosiActionModel> models, string filterCriteria)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.OfficerVosiActions.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FilterCriteria", filterCriteria) });
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
            get { return "Officer Activity: VOSI Actions"; }
        }

        public IList<Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get
            {
                return
                    new[] 
                    { 
                        Kapsch.Core.Reports.Enums.ParameterType.Period,
                        Kapsch.Core.Reports.Enums.ParameterType.DistrictOfficer
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
