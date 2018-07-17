using Kapsch.Core.Data;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;


namespace Kapsch.EVR.Reports.Printable
{
    public class InspectionSummary : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(Core.Reports.Enums.ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var userID = default(long?);
            var startDate = default(DateTime);
            var endDate = default(DateTime);
            var testCategoryID = default(long?);
            var siteID = default(long?);
            var periodType = string.Empty;
            var districtList = new List<long>();

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
                else if (parts[0].Equals("userID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long userID_ = 0;
                    if (long.TryParse(parts[1], out userID_))
                        userID = userID_;
                }
                else if (parts[0].Equals("startDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out startDate);

                }
                else if (parts[0].Equals("endDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime.TryParseExact(parts[1], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out endDate);
                }
                else if (parts[0].Equals("testCategoryID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long testCategoryID_ = 0;
                    if (long.TryParse(parts[1], out testCategoryID_))
                        testCategoryID = testCategoryID_;
                }
                else if (parts[0].Equals("periodType", StringComparison.InvariantCultureIgnoreCase))
                {
                    periodType = parts[1];
                }
                else if (parts[0].Equals("siteID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long siteID_ = 0;
                    if (long.TryParse(parts[1], out siteID_))
                        siteID = siteID_;
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
                //dbContext.Database.Log = s => Debug.WriteLine(s);
                endDate = endDate.AddDays(1).AddMilliseconds(-1);

                var categoryTestTypes = dbContext.VehicleCategoryTestTypes
                    .AsNoTracking()
                    .Include(f => f.TestCategory)
                    .ToList();

                var query = dbContext.VehicleTestBookings
                    .AsNoTracking()
                    .Include(f => f.CapturedCredential)
                    .Include(f => f.CapturedCredential.User)
                    .Include(f => f.Vehicle.VehicleCategory)
                    .Include(f => f.Site)
                    .Where(f =>
                        f.IsPassed.HasValue &&
                        f.TestDate >= startDate &&
                        f.TestDate <= endDate);

                if (siteID.HasValue)
                {
                    query = query.Where(f => f.SiteID == siteID.Value);

                }
                else
                {
                    query = query.Where(f => f.Site.DistrictID.HasValue && districtList.Contains(f.Site.DistrictID.Value));
                }

                if (userID.HasValue)
                {
                    query = query.Where(f => f.CapturedCredential.EntityID == userID.Value);
                }

                if (testCategoryID.HasValue)
                {
                    query = query.Join(dbContext.VehicleCategoryTestTypes.Where(f => f.TestCategoryID == testCategoryID.Value), f => f.TestTypeID, f => f.TestTypeID, (f, g) => f);
                }

                var vehicleBookings = query
                    .OrderBy(f => new { f.SiteID, f.TestDate })
                    //.OrderBy(f => f.SiteID)
                    .ToList();

                var models = new List<Kapsch.EVR.Reports.Models.VehicleInspectionModel>();
                foreach (var vehicleBooking in vehicleBookings)
                {
                    var categoryTestType = categoryTestTypes.FirstOrDefault(f =>
                        f.TestTypeID == vehicleBooking.TestTypeID &&
                        f.VehicleCategoryID == vehicleBooking.Vehicle.VehicleCategoryId);

                    var model =
                        new Kapsch.EVR.Reports.Models.VehicleInspectionModel
                        {
                            BookingReference = vehicleBooking.BookingReference,
                            ID = vehicleBooking.ID,
                            EndedTimestamp = vehicleBooking.EndedTimestamp,
                            IsPassed = vehicleBooking.IsPassed == 1,
                            SiteID = vehicleBooking.SiteID,
                            SiteName = vehicleBooking.Site.Name,
                            StartedTimestamp = vehicleBooking.StartedTimestamp,
                            TestCategoryID = categoryTestType != null ? categoryTestType.TestCategoryID : 0,
                            TestCategoryName = categoryTestType != null ? categoryTestType.TestCategory.Name : string.Empty,
                            TestDate = vehicleBooking.TestDate,
                            UserID = vehicleBooking.CapturedCredential.EntityID,
                            UserFullName = string.Format("{0} {1}", vehicleBooking.CapturedCredential.User.FirstName, vehicleBooking.CapturedCredential.User.LastName),
                            VLN = vehicleBooking.Vehicle.VLN
                        };

                    models.Add(model);
                }

                filterCriteria += string.Format("Inspection Summary for {0:yyyy/MM/dd} to {1:yyyy/MM/dd}.", startDate, endDate);

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

        private ReportViewer BuildReport(IList<Kapsch.EVR.Reports.Models.VehicleInspectionModel> models, string filterCriteria)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.EVR.Reports.Templates.InspectionSummary.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FilterCriteria", filterCriteria) });
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", models));
            reportViewer.ShowReportBody = true;

            return reportViewer;
        }

        public string RequiredAccessRole
        {
            get { return "IMSPortalVehicleViewTestReports"; }
        }

        public string CategoryName
        {
            get { return "EVR"; }
        }

        public string SubCategoryName
        {
            get { return "Vehicle Testing"; }
        }

        public string ReportName
        {
            get { return "Inspection Summary"; }
        }

        public IList<Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get
            {
                return
                    new Core.Reports.Enums.ParameterType[] 
                    { 
                        ParameterType.Period,
                        ParameterType.DistrictSite,
                        ParameterType.TestCategory,
                        ParameterType.User
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
