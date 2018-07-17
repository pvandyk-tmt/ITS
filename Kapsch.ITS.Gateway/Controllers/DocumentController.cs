using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.ITS.Gateway.Models;
using Kapsch.ITS.Gateway.Models.Correspondence;
using Kapsch.ITS.Gateway.Models.Document;
using Kapsch.ITS.Repositories;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ZXing;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Document")]
    public class DocumentController : BaseController
    {
        [HttpGet]
        [SessionTokenActionFilter]
        [ResponseType(typeof(FileResult))]
        [Route("FirstNotice")]
        public IHttpActionResult GetFirstNotice(string sessionToken, string referenceNumber)
        {
            using (var dataContext = new DataContext())
            {
                var documentRepository = new DocumentRepository(dataContext);
                var serverPath = documentRepository.GetDocumentPath(referenceNumber, (int)DocumentType.FirstNotice, "application/pdf");
                var fileInfo = new FileInfo(serverPath);

                return !fileInfo.Exists ? (IHttpActionResult)NotFound() : new FileResult(fileInfo.FullName);
            }
        }

        [HttpPost]
        [SessionAuthorize]
        [Route("FirstNotice")]
        [ResponseType(typeof(IList<SendResponseModel>))]
        public IHttpActionResult GenerateFirstNotice([FromBody] IList<string> referenceNumbers)
        {

            var response = new List<SendResponseModel>();

            using (var dbContext = new DataContext())
            {
                var models = RetrieveFineDetails(dbContext, referenceNumbers);
                
                foreach (var model in models)
                {
                    try
                    {
                        var vehicleImagePath = GetEvidence(dbContext, model.VehicleImageID);
                        var numberPlatePath = GetEvidence(dbContext, model.NumberPlateImageID);
                        
                        var reportModel =
                            new NoticeBeforeSummonsModel
                            {
                                PersonName = string.Format("{0} {1}", model.OffenderFirstName, model.OffenderLastName),
                                CompanyText = model.OffenderCompanyName,
                                PersonalDetails1 = string.Format("Mobile: {0}", model.OffenderMobileNumber),
                                PersonalDetails2 = string.Format("Email: {0}", model.OffenderEmail),
                                Regulation = "Section 162 of the Road Traffic Act no 11 of 2002 of the Laws of Zambia",
                                RegulationText = "Sect. 148 (1)(a) & 148 (4)(a) of the Road Traffic Act No. 11 of 2002 and Statutory Instrument No. 90 of 2016 (Road Traffic (Speed Limits)) Reg. 4 (I) (1)",
                                OffRegInfringementDate = string.Format("{0:dd-MMM-yyyy HH:mm}", model.OffenceDate.Value),
                                LocationDescription = model.OffenceLocation,
                                OffRegTicketNo = model.ReferenceNumber,
                                LocationCode = model.OffenceLocationCode,
                                CamReference = string.Format("{0:yyMMdd}/{1}/{2}", model.SessionDate, model.SessionIdentifier, model.SessionCase),
                                TicketType = "Camera",
                                ChargeCode = model.PrimaryFineChargeModel == null ? string.Empty : model.PrimaryFineChargeModel.Code,
                                InspNo = model.OfficerExternalID,
                                ChargeDecription = model.PrimaryFineChargeModel == null ? string.Empty : model.PrimaryFineChargeModel.RegulationDescription,
                                OffRegGuiltFineExpDate = string.Format("{0:dd-MMM-yyyy HH:mm}", model.OffenceDate),
                                FineAmount = string.Format("ZMW{0}", model.OffenceAmount),
                                CourtName = model.CourtName,
                                IssuedByLine1 = "RTSA/Zambia Police",
                                //IssuedByLine2 = "Traffic Dept",
                                IssuedByLine3 = model.DistrictName,
                                IssuedDate = string.Format("{0:dd-MMM-yyyy}", model.FirstPrintDate.HasValue ? model.FirstPrintDate.Value : DateTime.Now),
                                BankName = "STANBIC Bank Lusaka",
                                AccountName = "Intelligent Mobile Solutions",
                                AccountNumber = "45951835735",
                                BranchCode = "00012",
                                PaymentRef = model.ReferenceNumber,
                                DateOfOffence = string.Format("{0:dd-MMM-yyyy}", model.OffenceDate),
                                TimeOfOffence = string.Format("{0:HH:mm}", model.OffenceDate),
                                Location = model.OffenceLocationCode,
                                Zone = string.Format("{0}km/h", model.SpeedZone),
                                Speed = string.Format("{0}km/h", model.OffenceSpeed),
                                VehRegistration = model.VLN,
                                IDNo = model.OffenderIDNumber,
                                Officer = model.OfficerExternalID,
                                VehicleBrand = model.VehicleMake,
                                VehicleType = model.VehicleModel,
                                VehicleImage = string.IsNullOrWhiteSpace(vehicleImagePath) ? new byte[] { } : File.ReadAllBytes(vehicleImagePath),
                                VehicleNumberPlate = string.IsNullOrWhiteSpace(numberPlatePath) ? new byte[] { } : File.ReadAllBytes(numberPlatePath),
                                QrCode = GenerateQRCode(model.ReferenceNumber),
                            };

                        var documentRepository = new DocumentRepository(dbContext);
                        var serverPath = documentRepository.GetDocumentPath(model.ReferenceNumber, (int)DocumentType.FirstNotice, "application/pdf");
                        var fileContents = StreamPdfReport(BuildReport(new[] { reportModel }.ToList()));

                        var directoryInfo = new FileInfo(serverPath).Directory;
                        if (!directoryInfo.Exists)
                            directoryInfo.Create();

                        File.WriteAllBytes(serverPath, fileContents);

                        response.Add(new SendResponseModel { ReferenceNumber = model.ReferenceNumber, IsError = false });
                    }
                    catch (Exception ex)
                    {
                        response.Add(
                            new SendResponseModel
                            {
                                ReferenceNumber = model.ReferenceNumber,
                                IsError = true,
                                Error = string.Format("{0} {1}", ex.Message, ex.InnerException)
                            });
                        continue;
                    }
                }

                return Ok(response);
            }
        }

        public static byte[] GenerateQRCode(string data)
        { 
            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            using (var memoryStream = new MemoryStream())
            {
                barcodeWriter.Write(data).Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }

        private IList<FirstNoticeModel> RetrieveFineDetails(DataContext dbContext, IList<string> referenceNumbers)
        {
            var models = new List<FirstNoticeModel>();

            //dbContext.Database.Log = f => Debug.WriteLine(f);

            var query = dbContext.OffenceRegister
                .AsNoTracking()
                .Include(f => f.Register)
                .Include(f => f.Register.District)
                .Include(f => f.Register.ReferenceVehicle)
                .Include(f => f.Court)
                .Include(f => f.Register.Person)
                .Include(f => f.Register.Person.AddressInfos)
                .Include(f => f.InfringementLocation)
                .Include(f => f.EvidenceLog)
                .Include(f => f.EvidenceLog.SpeedLog)
                .Include(f => f.EvidenceLog.ChargeInfos)
                .Include(f => f.Credential.User)
                .Include(f => f.EvidenceLog.HandWrittenCaptureLog)
                .Where(f => referenceNumbers.Contains(f.ReferenceNumber));


            var offences = query.OrderByDescending(f => f.InfringementDate).ToList();

            foreach (var offence in offences)
            {
                var transactionToken = string.Empty;
                var generatedReferenceNumber = dbContext.GeneratedReferenceNumbers.FirstOrDefault(f => f.ReferenceNumber == offence.ReferenceNumber);
                if (generatedReferenceNumber != null)
                {
                    transactionToken = generatedReferenceNumber.ExternalToken;
                }

                AddressInfo addressInfo = null;
                if (offence.Register.Person != null && offence.Register.Person.AddressInfos.Count > 0)
                {
                    addressInfo = offence.Register.Person.AddressInfos.FirstOrDefault(f => f.AddressTypeID == Core.Data.Enums.AddressType.Phyiscal);
                }

                var model = new FirstNoticeModel();
                model.ReferenceNumber = offence.ReferenceNumber;
                model.TransactionToken = transactionToken;
                model.DistrictID = offence.DistrictID;
                model.DistrictName = offence.Register.District == null ? string.Empty : offence.Register.District.BranchName;
                model.PaymentOptions = offence.Register.District == null ? string.Empty : offence.Register.District.PaymentOptions;
                model.CourtID = offence.CourtID;
                model.CourtName = offence.Court == null ? string.Empty : offence.Court.CourtName;
                model.CourtDate = offence.CourtDate;
                model.OffenderIDType = offence.Register.Person == null ? 0 : offence.Register.Person.IDNumberType;
                model.OffenderIDNumber = offence.Register.Person == null ? string.Empty : offence.Register.Person.IDNumber;
                model.OffenderLastName = offence.Register.Person == null ? string.Empty : offence.Register.Person.LastName;
                model.OffenderFirstName = offence.Register.Person == null ? string.Empty : offence.Register.Person.FirstNames;
                model.OffenderEmail = offence.Register.Person == null ? string.Empty : offence.Register.Person.Email;
                model.OffenderMobileNumber = offence.Register.Person == null ? string.Empty : offence.Register.Person.MobileNumber;
                model.OffenderAddressLine1 = addressInfo == null ? string.Empty : addressInfo.Line1;
                model.OffenderAddressLine2 = addressInfo == null ? string.Empty : addressInfo.Line2;
                model.OffenderPostalCode = addressInfo == null ? string.Empty : addressInfo.Code;
                model.OffenderCompanyName = offence.Register.Person == null ? string.Empty : offence.Register.Person.CompanyName;
                model.OfficerCredentialID = offence.CredentialID;
                model.OfficerFirstName = offence.Credential.User.FirstName;
                model.OfficerLastName = offence.Credential.User.LastName;
                model.OfficerExternalID = offence.Credential.User.ExternalID;

                model.FirstPrintDate = offence.FirstPrintDate;
                model.VLN = offence.VLN;
                model.OffenceLocation = offence.InfringementLocation == null ? string.Empty : offence.InfringementLocation.Description;
                model.OffenceLocationCode = offence.InfringementLocation == null ? string.Empty : offence.InfringementLocation.Code;
                model.OffenceAmount = offence.CapturedAmount;
                model.OffenceDate = offence.InfringementDate;
                model.OutstandingAmount = offence.Register.OutstandingAmount;
                model.Status = (Models.Enums.RegisterStatus)offence.Register.RegisterStatus;

                if (offence.Register.ReferenceVehicle != null)
                {
                    model.VehicleMake = offence.Register.ReferenceVehicle.MakeDescription;
                    model.VehicleModel = offence.Register.ReferenceVehicle.ModelDescription;
                }

                if ("NO ID AVAILABLE".Equals(model.OffenderIDNumber, StringComparison.InvariantCultureIgnoreCase))
                {
                    model.OffenderIDNumber = string.Empty;
                    model.OffenderLastName = string.Empty;
                    model.OffenderFirstName = string.Empty;
                    model.OffenderEmail = string.Empty;
                    model.OffenderMobileNumber = string.Empty;
                }

                if (offence.EvidenceLog != null)
                {
                    if (offence.EvidenceLog.InfringementType == Core.Data.Enums.InfringementType.Speed && offence.EvidenceLog.SpeedLog != null)
                    {
                        model.OffenceSpeed = offence.EvidenceLog.SpeedLog.Speed;
                        model.SpeedZone = offence.EvidenceLog.SpeedLog.Zone;
                        model.SessionCase = offence.EvidenceLog.SpeedLog.SessionCase;
                        model.SessionDate = offence.EvidenceLog.SpeedLog.SessionDate;
                        model.SessionIdentifier = offence.EvidenceLog.SpeedLog.SessionIdentifier;
                        model.InfringementLocationCode = offence.EvidenceLog.SpeedLog.InfringementLocationCode;
                    }

                    if (offence.EvidenceLog.ChargeInfos != null)
                    {
                        model.FineChargeModels =
                            offence.EvidenceLog.ChargeInfos.Select(f =>
                                new FineChargeModel
                                {
                                    Code = f.OffenceCode.Code,
                                    Description = f.PrimaryDescription,
                                    SecondaryDescription = f.SecondaryDescription,
                                    ShortDescription = f.ShortDescription,
                                    FineAmount = f.OffenceCode.FineAmount,
                                    RegulationDescription = f.RegulationDescription
                                })
                                .ToList();

                        model.PrimaryFineChargeModel = model.FineChargeModels.FirstOrDefault();
                    }

                    if (offence.EvidenceLog.InfringementEvidences != null)
                    {
                        model.FineEvidenceModels = offence.EvidenceLog.InfringementEvidences
                                .Where(f =>
                                    f.EvidenceType != Core.Data.Enums.EvidenceType.VoiceRecording &&
                                    f.EvidenceType != Core.Data.Enums.EvidenceType.Other &&
                                    !f.FileName.Contains(".sml"))
                                .Select(f =>
                                    new FineEvidenceModel
                                    {
                                        ID = f.ID,
                                        ReferenceNumber = model.ReferenceNumber,
                                        EvidenceType = (Models.Enums.EvidenceType)f.EvidenceType,
                                        MimeType = f.MimeType,
                                        IsPrintImage = f.IsPrintImage == 1
                                    })
                                .ToList();
                    }
                }

                if (offence.EvidenceLog.HandWrittenCaptureLog != null)
                {
                    model.OffenceSpeed = offence.EvidenceLog.HandWrittenCaptureLog.Speed;
                    var chargeWithZone = offence.EvidenceLog.ChargeInfos.FirstOrDefault(f => f.OffenceCode != null && f.OffenceCode.Zone.HasValue && f.OffenceCode.Zone.Value > 0);
                    if (chargeWithZone != null)
                    {
                        model.SpeedZone = chargeWithZone.OffenceCode.Zone;
                    }

                    model.OffenceLocation = offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationStreet;
                    if (!string.IsNullOrWhiteSpace(offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationSuburb))
                        model.OffenceLocation += string.Format("\n{0}", offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationSuburb);
                    if (!string.IsNullOrWhiteSpace(offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationTown))
                        model.OffenceLocation += string.Format("\n{0}", offence.EvidenceLog.HandWrittenCaptureLog.OffenceLocationTown);

                    model.OffenceLocationCode = string.Empty;
                }

                models.Add(model);
            }

            return models;
        }

        private string GetEvidence(DataContext dbContext, long id)
        {
            var serverPath = string.Empty;


            var computerConfigSetting = dbContext.ComputerConfigSettings
                .FirstOrDefault(f =>
                    f.Computer.Name.ToUpper() == "IMS-IMAGE-SRV" &&
                    f.ComputerItemType == Core.Data.Enums.ComputerItemType.HandWrittenImageDirectory);

            if (computerConfigSetting != null)
            {
                serverPath = computerConfigSetting.Value;
            }

            var infringementEvidence = dbContext.InfringementEvidences.FirstOrDefault(f => f.ID == id);
            if (infringementEvidence != null)
            {
                serverPath = serverPath + Path.Combine(infringementEvidence.MimeDataPath, infringementEvidence.FileName);
            }
            
            var fileInfo = new FileInfo(serverPath);

            return !fileInfo.Exists ? string.Empty : fileInfo.FullName;
        }

        private ReportViewer BuildReport(List<NoticeBeforeSummonsModel> models)
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.ReportEmbeddedResource = "Kapsch.ITS.Gateway.Templates.NoticeBeforeSummons.rdlc";
            //reportViewer.LocalReport.SetParameters(new ReportParameter[] { new ReportParameter("FilterCriteria", filterCriteria) });
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", models));
            reportViewer.ShowReportBody = true;

            return reportViewer;
        }

        private byte[] StreamPdfReport(ReportViewer reportViewer)
        {
            Warning[] warnings;
            string[] strStreamIds;
            string strMimeType;
            string strEncoding;
            string strFileNameExtension;

            return reportViewer.LocalReport.Render("PDF", null, out strMimeType, out strEncoding, out strFileNameExtension, out strStreamIds, out warnings);
        }
    }
}
