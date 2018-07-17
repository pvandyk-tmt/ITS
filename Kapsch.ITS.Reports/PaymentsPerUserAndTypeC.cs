using Kapsch.Core.Data;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kapsch.ITS.Gateway.Models.Fine;
using Kapsch.ITS.Gateway.Models.Enums;
using Microsoft.Reporting.WebForms;
using System.Globalization;
using System.Diagnostics;
using Kapsch.ITS.Reports.Example.Models;
using CrystalDecisions.CrystalReports.Engine;
using Kapsch.ITS.Reports.Example.Templates;
using System.IO;

namespace Kapsch.ITS.Reports.Example
{
    public class PaymentsPerUserAndTypeC : IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var models = new List<PaymentsPerUser>();
            var officerName = string.Empty;

            var paymentDateFilter = 0;
            var startDate = default(DateTime);
            var endDate = default(DateTime);

            var districtID = default(long?);
            var courtID = default(long?);
            var userID = default(long?);
            var paymentMethod = 0;
            
            foreach (var parameter in parameters)
            {
                var parts = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts[0].Equals("paymentDateFilter", StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentDateFilter = int.Parse(parts[1]);
                }
                else if (parts[0].Equals("paymentMethod", StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentMethod = int.Parse(parts[1]);
                }
                else if (parts[0].Equals("districtID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long districtID_ = 0;
                    if (long.TryParse(parts[1], out districtID_))
                        districtID = districtID_;
                }
                else if (parts[0].Equals("courtID", StringComparison.InvariantCultureIgnoreCase))
                {
                    long courtID_ = 0;
                    if (long.TryParse(parts[1], out courtID_))
                        courtID = courtID_;
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
            }

            using (var dbContext = new DataContext())
            {

                //dbContext.Database.Log = f => Debug.WriteLine(f);
                var query = dbContext.PaymentTransactionItems
                    .AsNoTracking()
                    .Include(f => f.PaymentTransaction)
                    .Include(f => f.PaymentTransaction.Credential)
                    .Include(f => f.PaymentTransaction.Credential.User)
                    .Include(f => f.PaymentTransaction.Court)
                    .Include(f => f.PaymentTransaction.Court.District);
                    //.Where(f => 
                    //    f.PaymentTransaction.PaymentMethod == (Core.Data.Enums.PaymentMethod)paymentMethod &&
                    //    f.PaymentTransaction.Credential.EntityType == Core.Data.Enums.EntityType.User &&
                    //    (f.PaymentTransaction.Status == Core.Data.Enums.PaymentTransactionStatus.Processed || f.PaymentTransaction.Status == Core.Data.Enums.PaymentTransactionStatus.RollingRegister));

                //if (districtID.HasValue && districtID.Value != 0)
                //{
                //    var districtID_ = districtID.Value;
                //    query = query.Where(f => f.PaymentTransaction.Court.DistrictID == districtID_);
                //}

                //if (courtID.HasValue && courtID.Value != 0)
                //{
                //    var courtID_ = courtID.Value;
                //    query = query.Where(f => f.PaymentTransaction.CourtID == courtID_);
                //}

                //if (userID.HasValue && userID.Value != 0)
                //{
                //    var entityID = userID.Value;
                //    query = query.Where(f => f.PaymentTransaction.Credential.EntityID == entityID);
                //}

                //if (paymentDateFilter == 0) // Payment Date
                //{
                //    query = query.Where(f => f.PaymentTransaction.ReceiptTimestamp >= startDate && f.PaymentTransaction.ReceiptTimestamp <= endDate);
                //}
                //else if (paymentDateFilter == 1)  // Captured Date
                //{
                //    query = query.Where(f => f.PaymentTransaction.CreatedTimestamp >= startDate && f.PaymentTransaction.CreatedTimestamp <= endDate);
                //}


                models = query.ToList().Select(f => 
                    new PaymentsPerUser 
                    {
                        PaymentDate = f.PaymentTransaction.ReceiptTimestamp.Value,
                        CapturedDate = f.PaymentTransaction.ModifiedTimestamp.Value,
                        District = f.PaymentTransaction.Court.District.BranchName,
                        Court = f.PaymentTransaction.Court.CourtName,
                        User = string.Format("{0} {1}", f.PaymentTransaction.Credential.User.FirstName, f.PaymentTransaction.Credential.User.LastName),
                        PaymentType = f.PaymentTransaction.PaymentMethod.ToString(),
                        OffenceNumber = f.ReferenceNumber,
                        OffenceAmount = 0,
                        AmountPaid = f.PaymentTransaction.Amount,
                        Reference = f.PaymentTransaction.Receipt
                    })
                    .ToList();

                var referenceNumbers = models.Select(f => f.OffenceNumber).Distinct();

                var amountsRegisteredGroups = dbContext.AccountTransactions
                    .Where(f => referenceNumbers.Contains(f.ReferenceNumber) && f.RefTransactionType == 1)
                    .GroupBy(f => f.ReferenceNumber);

                foreach (var model in models)
                {
                    var amountsRegisteredGroup = amountsRegisteredGroups.FirstOrDefault(f => f.Key == model.OffenceNumber);
                    if (amountsRegisteredGroup != null)
                    {
                        model.OffenceAmount = amountsRegisteredGroup.Sum(f => f.Amount);
                    }                  
                }
            }

            if (exportType == ExportType.PDF)
            {
                return StreamPdfReport(BuildReport(officerName, models, parameters));
            }

            if (exportType == ExportType.Excel)
            {
                return StreamExcelReport(BuildReport(officerName, models, parameters));
            }

            if (exportType == ExportType.Html)
            {
                return StreamHtmlReport(BuildReport(officerName, models, parameters));
            }

            return null;
        }

        private ReportDocument BuildReport(string officerName, IList<PaymentsPerUser> models, string[] parameters)
        {
            var rpt = new PaymentsPerUserAndTypeRpt();
            rpt.SetDataSource(models);  

         
            return rpt;
        }

        protected byte[] StreamPdfReport(ReportDocument reportViewer)
        {
            var stream = reportViewer.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);  

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }

        protected byte[] StreamExcelReport(ReportDocument reportViewer)
        {
            var stream = reportViewer.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            stream.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }

        protected byte[] StreamHtmlReport(ReportDocument reportViewer)
        {
            var stream = reportViewer.ExportToStream(CrystalDecisions.Shared.ExportFormatType.HTML40);
            stream.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);

            return bytes;
        }

        public string ReportName
        {
            get
            {
                return "Payments Per User C";
            }
        }

        public IList<Kapsch.Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get { 
                return new[] 
                { 
                    Kapsch.Core.Reports.Enums.ParameterType.PaymentDateFilter,
                    Kapsch.Core.Reports.Enums.ParameterType.StartDate,
                    Kapsch.Core.Reports.Enums.ParameterType.EndDate,
                    Kapsch.Core.Reports.Enums.ParameterType.DistrictCourtUser,
                    Kapsch.Core.Reports.Enums.ParameterType.PaymentMethod,
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
