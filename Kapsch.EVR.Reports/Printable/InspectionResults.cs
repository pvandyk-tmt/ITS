using Kapsch.Core.Data;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WebForms;
using Kapsch.EVR.Gateway.Models.Vehicle;
using Kapsch.EVR.Reports.Models;
using System.Diagnostics;

namespace Kapsch.EVR.Reports.Printable
{
    public class InspectionResults : ReportViewerBase, IReportDefinition
    {
        public byte[] Export(ExportType exportType, string[] parameters)
        {
            var filterCriteria = string.Empty;
            var bookingReference = string.Empty;
            
            foreach (var parameter in parameters)
            {
                var parts = parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                if (parts[0].Equals("vehicleTestBookingReference", StringComparison.InvariantCultureIgnoreCase))
                {
                   bookingReference = parts[1];
                }                
            }

            var models = new List<VehicleTestQuestionAnswerModel>();

            using (var dbContext = new DataContext())
            {
                // dbContext.Database.Log = s => Debug.WriteLine(s);

                var vehicleBooking = dbContext.VehicleTestBookings
                    .AsNoTracking()
                    .Include(f => f.CapturedCredential)
                    .Include(f => f.Vehicle.VehicleMake)
                    .Include(f => f.Vehicle.VehicleModel)
                    .Include(f => f.Vehicle.VehicleModelNumber)
                    .Include(f => f.Vehicle.VehicleColor)
                    .Include(f => f.CapturedCredential.User)
                    .FirstOrDefault(f => f.BookingReference == bookingReference);

                var testCategory = dbContext.VehicleCategoryTestTypes
                    .Include(f => f.TestCategory)
                    .First(f =>
                        f.TestTypeID == vehicleBooking.TestTypeID &&
                        f.VehicleCategoryID == vehicleBooking.Vehicle.VehicleCategoryId);

                var entities = dbContext.VehicleTestResults
                    .AsNoTracking()
                    .Include(f => f.VehicleTestQuestion)
                    .Include(f => f.VehicleTestQuestionAnswer)
                    .Where(f => f.VehicleTestBookingID == vehicleBooking.ID)
                    .OrderBy(f => f.ID)
                    .ToList();

                foreach (var entity in entities)
                {
                    var model = new VehicleTestQuestionAnswerModel();
                    model.ID = entity.ID;
                    model.Comments = entity.Comments;
                    model.Question = entity.VehicleTestQuestion.Description;
                    model.QuestionType = entity.TestTypeID == 1 ? "Text" : entity.TestTypeID == 2 ? "Multiple Choice" : "Text / Multiple Choice";
                    model.Answer = entity.TestQuestionsAnswersID.HasValue ? entity.VehicleTestQuestionAnswer.Description : entity.TextAnswer;
                    
                    models.Add(model);
                }

                filterCriteria += string.Format("{0}, Booking Ref. {1}, was performed by {2} {3} on {4:yyyy/MM/dd HH:mm} at {5} with result {6}", 
                    testCategory.TestCategory.Name,
                    bookingReference, 
                    vehicleBooking.CapturedCredential.User.FirstName, 
                    vehicleBooking.CapturedCredential.User.LastName, 
                    vehicleBooking.StartedTimestamp, 
                    vehicleBooking.Site.Name, 
                    vehicleBooking.IsPassed == 1 ? "PASSED" : "FAILED");
                
                if (exportType == ExportType.PDF)
                {
                    return StreamPdfReport(BuildReport(models, filterCriteria, vehicleBooking.Vehicle));
                }
                else if (exportType == ExportType.Excel)
                {
                    return StreamExcelReport(BuildReport(models, filterCriteria, vehicleBooking.Vehicle));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }           
        }

        private ReportViewer BuildReport(IList<VehicleTestQuestionAnswerModel> models, string filterCriteria, Vehicle vehicle)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.EVR.Reports.Templates.InspectionResults.rdlc";
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FilterCriteria", filterCriteria) });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("EngineNumber", vehicle.EngineNumber) });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("VIN", vehicle.VehicleIDNumber) });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("Make", vehicle.VehicleMake.Description) });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("Model", vehicle.VehicleModel.Description) });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("ModelNumber", vehicle.VehicleModelNumber.Description) });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("Color", vehicle.VehicleColor.Description) });
            reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("GVM", vehicle.GVM.ToString()) });

            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", models));
            reportViewer.ShowReportBody = true;

            return reportViewer;
        }

        public string RequiredAccessRole
        {
            get { return "IMSPortalVehicleViewVehicleTestResults"; }
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
            get { return "Inspection Results"; }
        }

        public IList<Core.Reports.Enums.ParameterType> ParameterTypes
        {
            get
            {
                return
                    new[] 
                    { 
                        Kapsch.Core.Reports.Enums.ParameterType.VehicleTestBookingReference
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
