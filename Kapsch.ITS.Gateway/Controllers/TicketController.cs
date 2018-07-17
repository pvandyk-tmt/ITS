using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Models.User;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.ITS.Gateway.Models.Ticket;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using TMT.Build.OracleTableTypeClasses;
using Kapsch.Core.Gateway.Models.Configuration;
using Kapsch.Core.Data.Enums;
using Kapsch.Gateway.Shared.Helpers;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Threading.Tasks;
using Kapsch.ThirdParty.Payment;
using Kapsch.Core.Cryptography;
using Newtonsoft.Json;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Ticket")]
    [SessionAuthorize]
    [UsageLog]
    public class TicketController : BaseController
    {
        public readonly static string ApkExtention = ".apk";
        public readonly static string iTicketAppUpdatePath = ConfigurationManager.AppSettings["iTicketUpdatePath"];
        public readonly static string iTicketAppAPK = ConfigurationManager.AppSettings["iTicketApk"];
        public static string DataContextConnectionString = ConfigurationManager.ConnectionStrings["DataContext"].ConnectionString;
     
        [HttpGet]
        [Route("Officer")]
        [ResponseType(typeof(IList<UserModel>))]
        public IHttpActionResult GetOfficers()
        {
            using (var dbContext = new DataContext())
            {
                var credentials = dbContext.Credentials
                    .AsNoTracking()
                    .Include(f => f.User)
                    .Include(f => f.User.UserDistricts)
                    .Include(f => f.CredentialSystemFunctions)
                    .Where(f => f.EntityType == EntityType.User &&
                        f.User != null &&
                        f.User.IsOfficer == "1" &&
                        f.User.Status == Status.Active &&
                        f.Status == Status.Active)
                    .ToList();                     

                var models = credentials.Select(f =>
                    new UserModel
                    {
                        ID = f.User.ID,
                        FirstName = f.User.FirstName,
                        LastName = f.User.LastName,
                        MobileNumber = f.User.MobileNumber,
                        CreatedTimestamp = f.User.CreatedTimestamp,
                        Status = (Core.Gateway.Models.Enums.UserStatus)f.User.Status,
                        Email = f.User.Email,
                        IsOfficer = f.User.IsOfficer == "1",
                        ExternalID = f.User.ExternalID,
                        CredentialID = f.ID,
                        UserName = f.UserName,
                        Password = MessageDigest.HashSHA256(f.Password),
                        SystemFunctions = f.CredentialSystemFunctions
                            .Where(k => k.Status == Status.Active)
                            .Select(k => new SystemFunctionModel { ID = k.SystemFunction.ID, Name = k.SystemFunction.Name, Description = k.SystemFunction.Description })
                            .ToList(),
                        Districts = f.User.UserDistricts
                            .Select(k => new DistrictModel { ID = k.District.ID, BranchName = k.District.BranchName, PaymentOptions = k.District.PaymentOptions })
                            .ToList()
                    })
                    .ToList();                                           

                return Ok(models);
            }
        }

        [HttpGet]
        [Route("VOSIAction")]
        [ResponseType(typeof(IList<VosiActionModel>))]
        public IHttpActionResult GetVosiActions()
        {
            using (var dbContext = new DataContext())
            {
                var models = dbContext.VosiActions.Select(f => 
                    new VosiActionModel
                    {
                        ID = f.ID,
                        Description = f.Description
                    })
                    .ToList();

                return Ok(models);
            }
        }

        [HttpGet]
        [Route("OffenceCode")]
        [ResponseType(typeof(IList<OffenceCodeModel>))]
        public IHttpActionResult GetOffenceCodes(long districtID, LanguageType languageType = LanguageType.English)
        {
            using (var dbContext = new DataContext())
            {
                //dbContext.Database.Log = f => Debug.WriteLine(f);

                DateTime today = DateTime.Now.Date;

                var offenceCodes = (from districtOffenceSet in dbContext.DistrictOffenceSets
                                    join offenceSet in dbContext.OffenceSets on districtOffenceSet.OffenceSetID equals offenceSet.ID
                                    join offenceCode in dbContext.OffenceCodes on offenceSet.ID equals offenceCode.OffenceSetID
                                    join offenceCodeOffenceDescriptions in dbContext.OffenceCodeOffenceDescriptions on offenceCode.ID equals offenceCodeOffenceDescriptions.OffenceCodeID
                                    join offenceDescription in dbContext.OffenceDescriptions on offenceCodeOffenceDescriptions.OffenceDescriptionID equals offenceDescription.ID
                                    join offenceCodeOffenceRegulations in dbContext.OffenceCodeOffenceRegulations on offenceCode.ID equals offenceCodeOffenceRegulations.OffenceCodeID
                                    join offenceRegulation in dbContext.OffenceRegulations on offenceCodeOffenceRegulations.OffenceRegulationID equals offenceRegulation.ID
                                    where
                                        districtOffenceSet.DistrictID == districtID &&
                                        districtOffenceSet.EffectiveDate <= today &&
                                        districtOffenceSet.EndDate >= today && 
                                        offenceCodeOffenceDescriptions.LanguageID == (long)languageType &&
                                        offenceCodeOffenceRegulations.LanguageID == (long)languageType
                                    select new { offenceCode, offenceDescription, offenceRegulation }
                                     )
                                    .AsNoTracking()
                                    .ToList();

                var models = offenceCodes.Select(f =>
                    new OffenceCodeModel
                    {
                        ID = f.offenceCode.ID,
                        OffenceSetID = f.offenceCode.OffenceSetID,
                        Code = f.offenceCode.Code,
                        FineAmount = f.offenceCode.FineAmount,
                        VehicleType = f.offenceCode.VehicleType,
                        Zone = f.offenceCode.Zone,
                        MinSpeed = f.offenceCode.MinSpeed,
                        MaxSpeed = f.offenceCode.MaxSpeed,
                        WimVehicleTypeID = f.offenceCode.WimVehicleTypeID,
                        WimOffenceDescription = f.offenceCode.WimOffenceDescription,
                        MinOverweightPercentage = f.offenceCode.MinOverweightPercentage,
                        MaxOverweightPercentage = f.offenceCode.MaxOverweightPercentage,
                        CaseTypeID = f.offenceCode.CaseTypeID,
                        Description = f.offenceDescription.ShortDescription,
                        PrintDescription = f.offenceDescription.PrintDescription,
                        RegulationDescription = f.offenceRegulation.Regulation
                    });

                return Ok(models);
            }
        }

        [Route("ReferenceNumber")]
        [ResponseType(typeof(IList<ReferenceNumberModel>))]
        [HttpGet]
        public IHttpActionResult GetReferenceNumberRange(long districtID, long entityReferenceTypeID, long referenceDocumentTypeID, string deviceID, int numberCount, bool requireTokens = true)
        {
            var resultList = new List<ReferenceNumberModel>();

            using (var dbContext = new DataContext())
            {
                var mobileDevice = dbContext.MobileDevices.SingleOrDefault(f => f.DeviceID == deviceID);
                if (mobileDevice == null)
                {
                    return this.BadRequestEx(Error.DeviceNotFound);
                }

                var district = dbContext.Districts.Find(districtID);
                if (district == null)
                {
                    return this.BadRequestEx(Error.DistrictDoesNotExist);
                }

                var connection = (OracleConnection)dbContext.Database.Connection;

                using (var command = new OracleCommand())
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        command.Parameters.Add("P_DISTRICT_ID", OracleDbType.Int32).Value = districtID;
                        command.Parameters.Add("P_NUMBER_TICKETS", OracleDbType.Int32).Value = numberCount;
                        command.Parameters.Add("P_ENTITY_REFERENCE_TYPE_ID", OracleDbType.Int32).Value = 1;
                        command.Parameters.Add("P_REFERENCE_DOCUMENT_TYPE_ID ", OracleDbType.Int32).Value = 1;
                        command.Parameters.Add("P_DEVICE_ID", OracleDbType.Varchar2).Value = deviceID;
                        command.Parameters.Add("P_USER_DETAIL_ID", OracleDbType.Int32).Value = SessionModel.EntityID;
                        command.Parameters.Add("O_RESULT", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        ExcecuteNonQuery(command, "ITS.OFFENCE_CAPTURE.GET_REFERENCE_RANGE", connection);

                        if ((command.Parameters["O_RESULT"].Value is DBNull))
                            return Ok(resultList);

                        var refCursor = (OracleRefCursor)command.Parameters["O_RESULT"].Value;

                        using (var dataReader = refCursor.GetDataReader())
                        {
                            while (dataReader.Read())
                            {
                                 var referenceNumber = new ReferenceNumberModel();
                                referenceNumber.NumberValue = dataReader["REFERENCE_NUMBER"] is DBNull ? string.Empty : dataReader["REFERENCE_NUMBER"].ToString();
                                
                                resultList.Add(referenceNumber);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                    }
                    finally
                    {
                        foreach (OracleParameter parameter in command.Parameters)
                        {
                            if (parameter.Value is IDisposable)
                            {
                                ((IDisposable)(parameter.Value)).Dispose();
                            }

                            parameter.Dispose();
                        }
                    }
                }

                if (requireTokens)
                {
                    var numberValues = resultList.Select(f => f.NumberValue).ToList();
                    var referenceNumbers = dbContext.GeneratedReferenceNumbers.Where(f => numberValues.Contains(f.ReferenceNumber) && f.DeviceID == deviceID).ToList();

                    var paymentProvider = ProviderFactory.Get();

                    Parallel.ForEach(resultList, f => 
                        {
                            var transactionIDModel =
                                paymentProvider.RegisterTransaction(
                                    new ThirdParty.Payment.Models.TransactionModel
                                    {
                                        CompanyRef = f.NumberValue,
                                        CompanyAccRef = string.Empty,
                                        Amount = 300,
                                        UserID = SessionModel.UserName,
                                        ServiceDescription = "Reserved Payment Token",
                                        ServiceType = 6067
                                    });


                            f.ExternalToken = transactionIDModel.TransactionToken;
                            f.ExternalReference = transactionIDModel.TransactionReference;

                            var referenceNumber = referenceNumbers.FirstOrDefault(g => g.ReferenceNumber == f.NumberValue);
                            if (referenceNumber != null)
                            {
                                referenceNumber.ExternalToken = f.ExternalToken;
                                referenceNumber.ExternalReference = f.ExternalReference;
                            }


                        });

                    dbContext.SaveChanges();
                }
                
                return Ok(resultList);
            }
        }

        [HttpGet]
        [Route("CourtInfo")]
        [ResponseType(typeof(CourtsInfoModel))]
        public IHttpActionResult GetCourtInfo(long districtID)
        {
            using (var dbContext = new DataContext())
            {
                //dbContext.Database.Log = f => Debug.WriteLine(f);

                var courts = dbContext.Courts.Include(f => f.CourtRooms).Include(f => f.CourtDates).Where(f => f.DistrictID == districtID &&  f.Status == Status.Active).ToList();
                var courtRooms = courts.SelectMany(f => f.CourtRooms).Where(f => f.Status == Status.Active).ToList();
                var courtDates = courts.SelectMany(f => f.CourtDates).ToList();

                var model =
                    new CourtsInfoModel
                    {
                        Courts = courts.Select(f => new Kapsch.ITS.Gateway.Models.Ticket.CourtModel { ID = f.ID, Name = f.CourtName }).ToList(),
                        CourtRooms = courtRooms.Select(f =>
                            new CourtRoomModel
                            {
                                ID = f.ID,
                                CourtID = f.CourtID,
                                RoomNumber = f.Number
                            })
                            .ToList(),
                        CourtDates = courtDates.Select(f =>
                            new CourtDateModel
                            {
                                ID = f.ID,
                                CourtID = f.CourtID,
                                CourtRoomID = f.CourtRoomID,
                                Date = f.Date
                            })
                            .ToList()
                    };
                    
                return Ok(model);
            }
        }

        [Route("DbUpdateScript")]
        [ResponseType(typeof(IList<DbUpdateScript>))]
        [HttpGet]
        public IHttpActionResult GetDbUpdateScripts(int deviceScriptID, string deviceID)
        {
            using (var dbContext = new DataContext())
            {
                var entities = dbContext.MobileDeviceDbScripts.Where(f => f.ID > deviceScriptID);

                return Ok(entities.Select(f => new DbUpdateScript { ID = f.ID, ScriptValue = f.Script }).ToList());
            }
        }

        //[Route("Apk")]
        //[SessionAuthorize]
        //[ResponseType(typeof(byte[]))]
        //[HttpGet]
        //public IHttpActionResult GetAPK(string deviceID, string androidPackageName, int? version)
        //{
        //    if (!version.HasValue)
        //    {
        //        version = 0;
        //    }

        //    //var updateFile = Path.Combine(iTicketAppUpdatePath, iTicketAppAPK);
        //    var updateFile = Path.Combine(iTicketAppUpdatePath, androidPackageName);

        //    string mobileDeviceItemName = androidPackageName.Replace(ApkExtention, string.Empty);
     
        //    using (var dbContext = new DataContext())
        //    {
        //        var currentVersion = 0;
        //        var mobileDeviceItem = dbContext.MobileDeviceItems.SingleOrDefault(f => f.Name == mobileDeviceItemName);
        //        //var mobileDeviceItem = dbContext.MobileDeviceItems.SingleOrDefault(f => f.Name == "iTicket Current Version");
        //        if (mobileDeviceItem != null)
        //            currentVersion = int.Parse(mobileDeviceItem.Value);

        //        if (currentVersion <= version)
        //        {
        //            return Ok();
        //        }

        //        using (var binaryReader = new BinaryReader(File.Open(updateFile, FileMode.Open, FileAccess.Read)))
        //        {
        //            binaryReader.BaseStream.Position = 0;

        //            var bytes = binaryReader.ReadBytes(Convert.ToInt32(binaryReader.BaseStream.Length));
        //            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
        //            httpResponseMessage.Content = new ByteArrayContent(bytes);
        //            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //            httpResponseMessage.Content.Headers.ContentDisposition.FileName = androidPackageName;//ConfigurationManager.AppSettings["iTicketApk"].ToString();
        //            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        //            return ResponseMessage(httpResponseMessage);
        //        }
        //    }          
        //}

        [Route("VosiActionCapture")]
        [HttpPost]
        public IHttpActionResult PostVosiActionCapture([FromBody] VosiActionCaptureModel model)
        {
            using (var dbContext = new DataContext())
            {
                var vosiActionCapture = new VosiActionCapture();
                vosiActionCapture.CapturedDateTime = model.CapturedDateTime;
                vosiActionCapture.CapturedCredentialID = model.CapturedCredentialID;
                vosiActionCapture.Comments = model.Comments;
                vosiActionCapture.LocationLatitude = model.LocationLatitude;
                vosiActionCapture.LocationLongitude = model.Locationlongitude;
                vosiActionCapture.LocationStreet = model.LocationStreet;
                vosiActionCapture.LocationSuburb = model.LocationSuburb;
                vosiActionCapture.LocationTown = model.LocationTown;
                vosiActionCapture.VLN = model.VLN;
                vosiActionCapture.VosiActionID = model.VosiActionID;
                vosiActionCapture.ReferenceNumber = model.ReferenceNumber;

                dbContext.VosiActionCaptures.Add(vosiActionCapture);
                dbContext.SaveChanges();

                return Ok();
            }
        }

        [Route("HandWrittenCapture")]
        [HttpPost]
        public async Task<IHttpActionResult> PostHandWrittenCapture([FromBody] HandWrittenCaptureModel model, long offenceSetID, bool performNatisRequest)
        {            
            decimal totalAmount = 0;
            if (model.Amount1.HasValue)
                totalAmount += model.Amount1.Value;

            if (model.Amount2.HasValue)
                totalAmount += model.Amount2.Value;

            if (model.Amount3.HasValue)
                totalAmount += model.Amount3.Value;

            if (totalAmount > 0 && !model.IsCancelled)
            {
                await Task.Run(() =>
                {
                    var paymentProvider = resolvePaymentProvider();
                    var transactionModel =
                        new ThirdParty.Payment.Models.TransactionModel
                        {
                            CompanyRef = model.TicketNumber,
                            CompanyAccRef = string.Empty,
                            Amount = totalAmount,
                            UserID = SessionModel.UserName,
                            ServiceDescription = "Notice issued.",
                            ServiceType = 6067
                        };

                    try
                    {
                        paymentProvider.UpdateTransaction(
                            model.ExternalToken,
                            transactionModel);
                    }
                    catch (Exception)
                    {
                        using (var dbContext = new DataContext())
                        {
                            InsertQueueItem(
                                dbContext,
                                new PaymentProviderQueueItem
                                {
                                    CreatedTimestamp = DateTime.Now,
                                    OperationName = "UpdateTransaction",
                                    PaymentProvider = (PaymentProvider)paymentProvider.ID,
                                    Arguments = JsonConvert.SerializeObject(transactionModel),
                                    QueueStatus = QueueStatus.Queued,
                                    TransactionToken = model.ExternalToken
                                });
                        }
                    }
                });
            }

            using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
            {
                var handWrittenCapture =
                    new HandWrittenCapture
                    {
                        TICKET_NUMBER = model.TicketNumber,
                        PERSON_INFO_ID = model.PersonInfoID,
                        TITLE = model.Title,
                        FIRST_NAME = model.FirstName,
                        MIDDLE_NAMES = model.MiddleNames,
                        SURNAME = model.Surname,
                        INITIALS = model.Initials,
                        IDENTIFICATION_NUMBER = model.IdentificationNumber,
                        IDENTIFICATION_TYPE_ID = model.IdentificationTypeID,
                        IDENTIFICATION_COUNTRY_ID = model.IdentificationCountryID,
                        CITIZEN_TYPE_ID = model.CitizenTypeID,
                        GENDER = model.Gender,
                        AGE = model.Age,
                        BIRTHDATE = model.BirthDateTime,
                        OCCUPATION = model.Occupation,
                        TELEPHONE = model.Telephone,
                        MOBILE_NUMBER = model.MobileNumber,
                        FAX = model.Fax,
                        EMAIL = model.Email,
                        COMPANY = model.Company,
                        BUSINESS_TELEPHONE = model.BusinessTelephone,
                        PHYSICAL_ADDRESS_INFO_ID = model.PhysicalAddressInfoID,
                        PHYSICAL_ADDRESS_TYPE_ID = model.PhysicalAddressTypeID,
                        PHYSICAL_STREET_1 = model.PhysicalStreet1,
                        PHYSICAL_STREET_2 = model.PhysicalStreet2,
                        PHYSICAL_SUBURB = model.PhysicalSuburb,
                        PHYSICAL_TOWN = model.PhysicalTown,
                        PHYSICAL_CODE = model.PhysicalCode,
                        POSTAL_ADDRESS_INFO_ID = model.PostalAddressInfoID,
                        POSTAL_ADDRESS_TYPE_ID = model.PostalAddressTypeID,
                        POSTAL_PO_BOX = model.PostalPoBox,
                        POSTAL_STREET = model.PostalStreet,
                        POSTAL_SUBURB = model.PostalSuburb,
                        POSTAL_TOWN = model.PostalTown,
                        POSTAL_CODE = model.PostalCode,
                        OFFENCE_LOCATION_STREET = model.OffenceLocationStreet,
                        OFFENCE_LOCATION_SUBURB = model.OffenceLocationSuburb,
                        OFFENCE_LOCATION_TOWN = model.OffenceLocationTown,
                        OFFENCE_LOCATION_LATITUDE = model.OffenceLocationLatitude,
                        OFFENCE_LOCATION_LONGITUDE = model.OffenceLocationlongitude,
                        VEHICLE_REGISTRATION_MAIN = model.VehicleRegistrationMain,
                        VEHICLE_REGISTRATION_NO_2 = model.VehicleRegistrationNo2,
                        VEHICLE_REGISTRATION_NO_3 = model.VehicleRegistrationNo3,
                        VEHICLE_MAKE_MAIN = model.VehicleMakeMain,
                        VEHICLE_MODEL_MAIN = model.VehicleModelMain,
                        VEHICLE_TYPE_MAIN = model.VehicleTypeMain,
                        VEHICLE_LICENCE_EXPIRY_DATE = model.VehicleLicenceExpiryDateTime,
                        VEHICLE_COLOUR = model.VehicleColour,
                        VEHICLE_CHASSIS_NUMBER = model.VehicleChassisNumber,
                        VEHICLE_ENGINE_NUMBER = model.VehicleEngineNumber,
                        VEHICLE_REGISTER_NUMBER = model.VehicleRegisterNumber,
                        GUARDIAN = model.Gaurdian,
                        DIRECTION = model.Direction,
                        METER_NUMBER = model.MeterNumber,
                        CASE_NUMBER = model.CaseNumber,
                        CC_NUMBER = model.CcNumber,
                        CHARGE_CODE_1 = model.ChargeCode1,
                        AMOUNT_1 = model.Amount1,
                        CHARGE_CODE_1_ID = model.ChargeCode1ID,
                        CHARGE_CODE_2 = model.ChargeCode2,
                        CHARGE_CODE_2_ID = model.ChargeCode2ID,
                        AMOUNT_2 = model.Amount2,
                        CHARGE_CODE_3 = model.ChargeCode3,
                        CHARGE_CODE_3_ID = model.ChargeCode3ID,
                        AMOUNT_3 = model.Amount3,
                        HAS_ALTERNATIVE_CHARGE = model.HasAlternativeCharge,
                        OFFENCE_DATE = model.OffenceDateTime,
                        ISSUED_DATE = model.IssueDateTime,
                        COURT_DATE = model.CourtDateTime,
                        COURT_NAME = model.CourtName,
                        COURT_ROOM = model.CourtRoom,
                        DISTRICT_NAME = model.DistrictName,
                        PAYMENT_PLACE = model.PaymentPlace,
                        PAYMENT_DATE = model.PaymentDateTime,
                        OFFICER_CREDENTIAL_ID = model.OfficerCredentialID,
                        CAPTURED_DATE = DateTime.Now,
                        CAPTURED_CREDENTIAL_ID = SessionModel.CredentialID,
                        LICENCE_CODE = model.LicenceCode,
                        LICENCE_TYPE = model.LicenceType,
                        DRIVER_LICENCE_CERTIFICATE_NO = model.DriverLicenceCertificateNo,
                        MODIFIED_DATE = default(DateTime?),
                        MODIFIED_CREDENTIAL_ID = default(long?),
                        SPEED = model.Speed,
                        MASS_PERCENTAGE = model.MassPercentage,
                        IS_CANCELLED = model.IsCancelled ? 1 : 0,
                        CANCEL_REASON = model.CancelReason,
                        SEND_TO_COURT_ROLE = model.SendToCourtRole ? 1 : 0,
                        NOTES = model.Notes,
                        APPLICATION_AND_VERSION = model.ApplicationAndVersion,
                        MACHINE_IDENTIFIER = model.DeviceID,
                        CAMERA_HWID = model.CameraID,
                        EVENT_ID = model.EventID,
                        INFRINGEMENT_LOCATION_CODE = model.InfringementLocationCode,
                        EXTERNAL_TOKEN = model.ExternalToken,
                        EXTERNAL_REFERENCE = model.ExternalReference,
                        CHARGE_DESCRIPTION_1 = model.ChargeDescription1,
                        CHARGE_DESCRIPTION_2 = model.ChargeDescription2,
                        CHARGE_DESCRIPTION_3 = model.ChargeDescription3,
                    };

                using (var command = new Oracle.DataAccess.Client.OracleCommand())
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        command.Parameters.Add(
                            new Oracle.DataAccess.Client.OracleParameter("P_TYPE_HANDWRITTEN_CAPTURE", Oracle.DataAccess.Client.OracleDbType.Object)
                            {
                                Value = handWrittenCapture,
                                UdtTypeName = "ITS.TYPE_HANDWRITTEN_CAPTURE"
                            });
                        command.Parameters.Add("P_NATIS_REQUEST", Oracle.DataAccess.Client.OracleDbType.Int32).Value = performNatisRequest ? 1 : 0;
                        command.Parameters.Add("P_OFFENCE_SET", Oracle.DataAccess.Client.OracleDbType.Int32).Value = offenceSetID;
                        command.Parameters.Add("P_CREDENTIAL_ID", Oracle.DataAccess.Client.OracleDbType.Int32).Value = SessionModel.CredentialID;

                        ExcecuteNonQuery(command, "ITS.OFFENCE_CAPTURE.SUBMIT_HANDWRITTEN", connection);
                    }
                    catch (Exception ex)
                    {
                        return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                    }
                    finally
                    {
                        foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
                        {
                            if (parameter.Value is IDisposable)
                            {
                                ((IDisposable)(parameter.Value)).Dispose();
                            }

                            parameter.Dispose();
                        }
                    }
                }
            }

            if (!model.IsCancelled)
            {
                RegisterForPayment(model, totalAmount);
            }

            return Ok();
        }

        [Route("Evidence")]
        [HttpPost]
        public async Task<IHttpActionResult> PostEvidence(string noticeNumber, long districtID, EvidenceType evidenceType, string mimeType)
        {       
            using (Stream stream = await this.Request.Content.ReadAsStreamAsync())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(memoryStream);

                    using (var connection = new Oracle.DataAccess.Client.OracleConnection(DataContextConnectionString))
                    {
                        var infringementEvidenceType =
                            new InfringementEvidenceType
                            {
                                REFERENCE_NUMBER = noticeNumber,
                                EVIDENCE_TYPE = (int)evidenceType,
                                MIME_TYPE = mimeType,
                                DISTRICT_ID = districtID
                            };

                        using (var command = new Oracle.DataAccess.Client.OracleCommand())
                        {
                            try
                            {
                                if (connection.State != ConnectionState.Open)
                                {
                                    connection.Open();
                                }

                                command.Parameters.Add(
                                    new Oracle.DataAccess.Client.OracleParameter("P_INFRINGEMENT_EVIDENCE", Oracle.DataAccess.Client.OracleDbType.Object)
                                    {
                                        Value = infringementEvidenceType,
                                        UdtTypeName = "ITS.INFRINGEMENT_EVIDENCE_TYPE"
                                    });
                                command.Parameters.Add("O_RESULT", Oracle.DataAccess.Client.OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                                ExcecuteNonQuery(command, "ITS.OFFENCE_CAPTURE.SUBMIT_INFRINGEMENT_EVIDENCE", connection);

                                if ((command.Parameters["O_RESULT"].Value is DBNull))
                                    throw new Exception("Failed to return valid file paths.");

                                var refCursor = (Oracle.DataAccess.Types.OracleRefCursor)command.Parameters["O_RESULT"].Value;

                                using (var dataReader = refCursor.GetDataReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        if (dataReader["MIME_DATA_PATH"] is DBNull || dataReader["FILENAME"] is DBNull)
                                            continue;

                                        var filePath  = dataReader["MIME_DATA_PATH"] as string;
                                        var fileName = dataReader["FILENAME"] as string;

                                        Directory.CreateDirectory(filePath);
                                        filePath = Path.Combine(filePath, fileName);

                                        if (File.Exists(filePath))
                                            File.Delete(filePath);

                                        File.WriteAllBytes(filePath, memoryStream.ToArray());

                                        break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
                            }
                            finally
                            {
                                foreach (Oracle.DataAccess.Client.OracleParameter parameter in command.Parameters)
                                {
                                    if (parameter.Value is IDisposable)
                                    {
                                        ((IDisposable)(parameter.Value)).Dispose();
                                    }

                                    parameter.Dispose();
                                }
                            }
                        }

                        return Ok();
                    }
                }
            }           
        }

        private void RegisterForPayment(HandWrittenCaptureModel model, decimal amount)
        {
            using (var dbContext = new DataContext())
            {
                var paymentTransaction = new PaymentTransaction();
                paymentTransaction.TransactionToken = model.ExternalToken;
                paymentTransaction.TransactionReference = model.ExternalReference;
                paymentTransaction.Amount = amount;
                paymentTransaction.CreatedTimestamp = DateTime.Now;
                paymentTransaction.CustomerContactNumber = model.MobileNumber;
                paymentTransaction.CustomerFirstName = model.FirstName;
                paymentTransaction.CustomerIDNumber = model.IdentificationNumber;
                paymentTransaction.CustomerLastName = model.Surname;
                paymentTransaction.PaymentMethod = PaymentMethod.DumaPay;
                paymentTransaction.Status = PaymentTransactionStatus.Added;

                dbContext.PaymentTransactions.Add(paymentTransaction);

                var paymentTransactionItem = new PaymentTransactionItem();
                paymentTransactionItem.PaymentTransaction = paymentTransaction;
                paymentTransactionItem.ReferenceNumber = model.TicketNumber;
                paymentTransactionItem.TransactionToken = model.ExternalToken;
                paymentTransactionItem.EntityReferenceTypeID = long.Parse(model.TicketNumber[1].ToString());
                paymentTransactionItem.Description = "Notice issued.";
                paymentTransactionItem.Amount = amount;
                paymentTransactionItem.Status = PaymentTransactionStatus.Added;

                dbContext.PaymentTransactionItems.Add(paymentTransactionItem);

                dbContext.SaveChanges();
            }
        }

        private void InsertQueueItem(DataContext dbContext, PaymentProviderQueueItem paymentProviderQueueItem)
        {
            var connection = (OracleConnection)dbContext.Database.Connection;

            using (var command = new OracleCommand())
            {
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    command.Parameters.Add("P_CREATED_DATE", OracleDbType.Date).Value = paymentProviderQueueItem.CreatedTimestamp;
                    command.Parameters.Add("P_QUEUE_STATUS_ID", OracleDbType.Int32).Value = (int)paymentProviderQueueItem.QueueStatus;
                    command.Parameters.Add("P_ARGUMENTS", OracleDbType.Varchar2).Value = paymentProviderQueueItem.Arguments;
                    command.Parameters.Add("P_TRANSACTION_TOKEN", OracleDbType.Varchar2).Value = paymentProviderQueueItem.TransactionToken;
                    command.Parameters.Add("P_OPERATION_NAME", OracleDbType.Varchar2).Value = paymentProviderQueueItem.OperationName;
                    command.Parameters.Add("P_PAYMENT_PROVIDER_ID", OracleDbType.Int32).Value = (int)paymentProviderQueueItem.PaymentProvider;

                    ExcecuteNonQuery(command, "finance.PAYMENT_PROVIDER_REQUEST_INFO.INSERT_QUEUE", connection);
                }
                finally
                {
                    foreach (OracleParameter parameter in command.Parameters)
                    {
                        if (parameter.Value is IDisposable)
                        {
                            ((IDisposable)(parameter.Value)).Dispose();
                        }

                        parameter.Dispose();
                    }
                }
            }
        }

        private void ExcecuteNonQuery(OracleCommand command, string storedProcName, OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        private void ExcecuteNonQuery(Oracle.DataAccess.Client.OracleCommand command, string storedProcName, Oracle.DataAccess.Client.OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        private IProvider resolvePaymentProvider()
        {
            var paymentProvider = ProviderFactory.Get();
            paymentProvider.Log = (paymentProviderID, requestDetail, responseDetail, exceptionMessage, isSuccess) =>
            {
                try
                {
                    using (var dbContext = new DataContext())
                    {
                        dbContext.PaymentProviderRequests.Add(
                            new PaymentProviderRequest
                            {
                                PaymentProvider = (Kapsch.Core.Data.Enums.PaymentProvider)paymentProvider.ID,
                                CreatedTimestamp = DateTime.Now,
                                RequestDetail = requestDetail,
                                ResponseDetail = responseDetail,
                                ExceptionMessage = exceptionMessage,
                                Status = isSuccess ? Kapsch.Core.Data.Enums.RequestStatus.Success : Kapsch.Core.Data.Enums.RequestStatus.Failed

                            });
                        dbContext.SaveChanges();
                    }
                }
                catch
                {
                    // Empty on purpose
                }
            };

            return paymentProvider;
        }
    }
}
