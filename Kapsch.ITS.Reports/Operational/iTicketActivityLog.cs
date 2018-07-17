using Kapsch.Core.Data;
using Kapsch.Core.Reports;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WebForms;
using Kapsch.ITS.Reports.Operational.Models;
using Kapsch.Core.Reports.Enums;
using System.Diagnostics;
using System.Globalization;

namespace Kapsch.ITS.Reports.Operational
{
    public class iTicketActivityLog : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(Core.Reports.Enums.ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var districtList = new List<long>();
            var mobileDeviceID = default(string);
            var officerID = default(long?);
            var mobileDeviceActivityCategory = string.Empty;
            var startDate = default(DateTime);
            var endDate = default(DateTime);
            var periodType = string.Empty;
            
            foreach (var parameter in parameters)
            {
                var parts = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                if (parts[0].Equals("startDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out startDate);

                }
                else if (parts[0].Equals("endDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out endDate);
                }
                else if (parts[0].Equals("districtID", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    long districtID = 0;
                    if (long.TryParse(parts[1], out districtID))
                        districtList.Add(districtID);
                }
                else if (parts[0].Equals("mobileDeviceID", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    mobileDeviceID = parts[1];
                }
                else if (parts[0].Equals("officerID", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    long officerID_ = 0;
                    if (long.TryParse(parts[1], out officerID_))
                        officerID = officerID_;
                }
                else if (parts[0].Equals("mobileDeviceActivityCategory", StringComparison.InvariantCultureIgnoreCase) && parts.Length == 2)
                {
                    mobileDeviceActivityCategory = parts[1];
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

                var query = dbContext.UserMobileDeviceActivities
                    .AsNoTracking()                  
                    .Include(f => f.Credential)
                    .Include(f => f.Credential.User)
                    .Where(f => f.CreatedTimestamp >= startDate && f.CreatedTimestamp <= endDate);

                query = query.Join(dbContext.MobileDevices, f => f.DeviceID, g => g.DeviceID, (f, g) => new { f, g })
                    .Where(f => f.g.DistrictID != null && districtList.Contains(f.g.DistrictID.Value)).Select(f => f.f);

                if (!string.IsNullOrWhiteSpace(mobileDeviceID))
                    query = query.Where(f => f.DeviceID == mobileDeviceID);

                if (officerID.HasValue)
                {
                    var officerID_ = officerID.Value;
                    query = query.Where(f => f.Credential.EntityID == officerID_);
                }
                    
                if (!string.IsNullOrWhiteSpace(mobileDeviceActivityCategory))
                    query = query.Where(f => f.Category == mobileDeviceActivityCategory);

                var activities = query.OrderByDescending(f => f.CreatedTimestamp).ToList();
                var models = new List<iTicketActivityLogModel>();

                foreach (var activity in activities)
                {
                    var model = new iTicketActivityLogModel();
                    model.CreatedTimestamp = activity.CreatedTimestamp;
                    model.DeviceID = activity.DeviceID;
                    model.Category = activity.Category;
                    model.ActionDescription = activity.ActionDescription;
                    model.OfficerNumber = activity.Credential == null ? string.Empty : activity.Credential.User.ExternalID;
                    model.OfficerName = activity.Credential == null ? string.Empty : string.Format("{0} {1}", activity.Credential.User.FirstName, activity.Credential.User.LastName);

                    models.Add(model);
                }

                filterCriteria += string.Format("District: {0} ", districtList.Count == 1 ? dbContext.Districts.Find(districtList[0]).BranchName : "ALL");
                filterCriteria += string.Format("Period: {0:yyyy/MM/dd} - {1:yyyy/MM/dd} ", startDate, endDate);
                if (string.IsNullOrWhiteSpace(mobileDeviceActivityCategory))
                {
                    filterCriteria += string.Format("Action Category: ALL ");
                }
                else
                {
                    filterCriteria += string.Format("Action Category: {0} ", mobileDeviceActivityCategory);
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

        private ReportViewer BuildReport(IList<iTicketActivityLogModel> models, string filterCriteria)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.iTicketActivityLog.rdlc";
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
            get { return "iTicket: Activity Log Report"; }
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
