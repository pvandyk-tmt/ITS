using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
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
    public class OffencesPerOfficer : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var startDate = default(DateTime);
            var endDate = default(DateTime);
            var districtList = new List<long>();
            var infringementType = default(InfringementType?);
            var officerID = default(long?);
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
                else if (parts[0].Equals("officerID", StringComparison.InvariantCultureIgnoreCase))
                {
                    officerID = int.Parse(parts[1]);
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
                    .Include(f => f.Credential)
                    .Include(f => f.Register)
                    .Include(f => f.Register.District)
                    .Include(f => f.Court)
                    .Include(f => f.Register.Person)
                    .Include(f => f.InfringementLocation)
                    .Include(f => f.EvidenceLog)
                    .Include(f => f.EvidenceLog.HandWrittenCaptureLog)
                    .Where(f => (f.Register.DistrictID.HasValue && districtList.Contains(f.Register.DistrictID.Value)) &&
                        f.EvidenceLog.HandWrittenCaptureLog != null &&
                        f.EvidenceLog.InfringementDate >= startDate &&
                        f.EvidenceLog.InfringementDate < endDate);

                if (officerID.HasValue)
                {
                    query = query.Where(f => f.Credential.EntityID == officerID.Value && f.Credential.EntityType == Core.Data.Enums.EntityType.User);
                }

                if (infringementType.HasValue)
                {
                    query = query.Where(f => f.InfringementType == infringementType.Value);
                }

                var offences = query
                    .OrderBy(f => new { f.InfringementDate, f.CredentialID, f.Court.DistrictID })
                    //.OrderBy(f => f.CredentialID)
                    //.OrderBy(f => f.Court.DistrictID)
                    .ToList();

                var models = new List<OffencesPerOfficerModel>();

                foreach (var offence in offences)
                {
                    var transactionToken = string.Empty;
                    var generatedReferenceNumber = dbContext.GeneratedReferenceNumbers.FirstOrDefault(f => f.ReferenceNumber == offence.ReferenceNumber);
                    if (generatedReferenceNumber != null)
                    {
                        transactionToken = generatedReferenceNumber.ExternalToken;
                    }

                    var model = new OffencesPerOfficerModel();
                    model.ReferenceNumber = offence.ReferenceNumber;
                    model.InfringementType = (Gateway.Models.Enums.InfringementType)offence.InfringementType;
                    model.DistrictID = offence.DistrictID;
                    model.DistrictName = offence.Register.District == null ? string.Empty : offence.Register.District.BranchName;
                    model.CourtID = offence.CourtID;
                    model.CourtName = offence.Court == null ? string.Empty : offence.Court.CourtName;
                    model.CourtDate = offence.CourtDate;
                    model.OffenderIDNumber = offence.Register.Person == null ? string.Empty : offence.Register.Person.IDNumber;
                    model.OffenderLastName = offence.Register.Person == null ? string.Empty : offence.Register.Person.LastName;
                    model.OffenderFirstName = offence.Register.Person == null ? string.Empty : offence.Register.Person.FirstNames;
                    model.VLN = offence.VLN;
                    model.OffenceLocation = offence.InfringementLocation == null ? string.Empty : offence.InfringementLocation.Description;
                    model.OffenceAmount = offence.CapturedAmount;
                    model.OffenceDate = offence.InfringementDate;
                    model.OutstandingAmount = offence.Register.OutstandingAmount;
                    model.Status = (Gateway.Models.Enums.RegisterStatus)offence.Register.RegisterStatus;
                    model.OfficerExternalNumber = offence.Credential.User.ExternalID;
                    model.OfficerID = offence.Credential.User.ID;
                    model.OfficerName = string.Format("{0} {1}", offence.Credential.User.FirstName, offence.Credential.User.LastName);

                    if (offence.EvidenceLog.HandWrittenCaptureLog != null)
                    {
                        model.OffenceSpeed = offence.EvidenceLog.HandWrittenCaptureLog.Speed;
                        var chargeWithZone = offence.EvidenceLog.ChargeInfos.FirstOrDefault(f => f.OffenceCode != null && f.OffenceCode.Zone.HasValue && f.OffenceCode.Zone.Value > 0);
                        if (chargeWithZone != null)
                        {
                            model.SpeedLimit = chargeWithZone.OffenceCode.Zone;
                        }

                        model.OffenceLocation = offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationStreet;
                        if (!string.IsNullOrWhiteSpace(offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationSuburb))
                            model.OffenceLocation += string.Format("\n{0}", offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationSuburb);
                        if (!string.IsNullOrWhiteSpace(offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationTown))
                            model.OffenceLocation += string.Format("\n{0}", offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationTown);
                    }

                    models.Add(model);
                }

                filterCriteria += string.Format("District: {0} ", districtList.Count == 1 ? dbContext.Districts.Find(districtList[0]).BranchName : "ALL");
                filterCriteria += string.Format("\t Period: {0:yyyy/MM/dd} - {1:yyyy/MM/dd} ", startDate, endDate);
                filterCriteria += string.Format("\t Infringement Type: {0} ", infringementType.HasValue ? infringementType.ToString() : "ALL");
                if (officerID.HasValue)
                {
                    var officer = dbContext.Users.Find(officerID.Value);
                    filterCriteria += string.Format("\t Officer: {0} {1} ", officer.FirstName, officer.LastName);
                }
                else
                {
                    filterCriteria += string.Format("\t Officer: ALL");
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

        private ReportViewer BuildReport(IList<OffencesPerOfficerModel> models, string filterCriteria)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Reports.Templates.OffencesPerOfficer.rdlc";
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

        public string ReportName
        {
            get
            {
                return "Officer Activity: Offences Per Officer";
            }
        }

        public IList<Kapsch.Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get
            {
                return new[] 
                {                  
                    Kapsch.Core.Reports.Enums.ParameterType.DistrictOfficer,
                    Kapsch.Core.Reports.Enums.ParameterType.Period,
                    Kapsch.Core.Reports.Enums.ParameterType.InfringementType
                }
                .ToList();
            }
        }

        public IList<Kapsch.Core.Reports.Enums.ExportType> ExportTypes
        {
            get
            {
                return
                    new[] 
                    { 
                        Kapsch.Core.Reports.Enums.ExportType.PDF,                
                        Kapsch.Core.Reports.Enums.ExportType.Html,               
                        Kapsch.Core.Reports.Enums.ExportType.Excel 
                    }
                    .ToList();
            }
        }


        public string RequiredAccessRole
        {
            get { return "IMSPortalReportsViewOprationalReports"; }
        }
    }
}
