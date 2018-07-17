using Kapsch.Core.Data;
using Kapsch.Gateway.Shared;
using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Diagnostics;
using System.Web.Http.Description;
using Kapsch.ITS.Gateway.Models.Fine;
using System.IO;
using System.Web;
using Kapsch.Gateway.Shared.Helpers;
using System.Configuration;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Core.Gateway.Models.Payment;
using Kapsch.Core.Cryptography;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Oracle.ManagedDataAccess.Types;
using Kapsch.ThirdParty.Payment;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Fine")]
    [UsageLog]
    public class FineController : BaseController
    {
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(IList<FineModel>))]
        public IHttpActionResult Get(SearchCriteria searchCriteria, string searchValue, bool includeAccount, bool onlyImageEvidence, bool onlyPayable)
        {
            var models = new List<FineModel>();

            if (string.IsNullOrWhiteSpace(searchValue))
                return Ok(models);

            var isWildCardSearch = searchValue.Contains("%");
            searchValue = searchValue.Replace("%", string.Empty);

            using (var dbContext = new DataContext())
            {
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
                    .Include(f => f.EvidenceLog.HandWrittenCaptureLog);

                //var offences = query.OrderByDescending(f => f.InfringementDate).ToList();

                if (searchCriteria == SearchCriteria.IDNumber)
                {                   
                    query = isWildCardSearch ? query.Where(f => f.Register.Person.IDNumber.Contains(searchValue)) : query.Where(f => f.Register.Person.IDNumber == searchValue);
                }
                else if (searchCriteria == SearchCriteria.ReferenceNumber)
                {
                    query = isWildCardSearch ? query.Where(f => f.ReferenceNumber.Contains(searchValue)) : query.Where(f => f.ReferenceNumber == searchValue);
                }
                else if (searchCriteria == SearchCriteria.VLN)
                {
                    query = isWildCardSearch ? query.Where(f => f.VLN.Contains(searchValue)) : query.Where(f => f.VLN == searchValue);
                }
                else if (searchCriteria == SearchCriteria.TransactionToken)
                {
                    var generatedReferenceNumber = dbContext.GeneratedReferenceNumbers.AsNoTracking().FirstOrDefault(f => f.ExternalToken == searchValue);
                    if (generatedReferenceNumber == null)
                        return Ok(new List<FineModel>());

                    query = query.Where(f => f.ReferenceNumber == generatedReferenceNumber.ReferenceNumber);
                }
                else
                {
                    return Ok(new List<FineModel>());
                }

                if (onlyPayable)
                {
                    query = query.Where(f => f.Register.RegisterStatus < 3000);
                }

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

                    var model = new FineModel();
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
                    model.OffenderAddressSuburb = addressInfo == null ? string.Empty : addressInfo.Suburb;
                    model.OffenderAddressTown = addressInfo == null ? string.Empty : addressInfo.Town;
                    model.OfficerCredentialID = offence.CredentialID;
                    model.OfficerFirstName = offence.Credential.User.FirstName;
                    model.OfficerLastName = offence.Credential.User.LastName;
                    model.ExternalID = offence.Credential.User.ExternalID;
                    model.FirstPrintDate = offence.FirstPrintDate;
                    model.VLN = offence.VLN;
                    model.OffenceLocation = offence.InfringementLocation == null ? string.Empty : offence.InfringementLocation.Description;
                    model.OffenceAmount = offence.CapturedAmount;
                    model.OffenceDate = offence.InfringementDate;
                    model.OutstandingAmount = offence.Register.OutstandingAmount;
                    model.Status = (RegisterStatus)offence.Register.RegisterStatus;

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
                            model.SpeedLimit = offence.EvidenceLog.SpeedLog.Zone;
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
                        }

                        if (offence.EvidenceLog.InfringementEvidences != null)
                        {
                            if (onlyImageEvidence)
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
                                            EvidenceType = (EvidenceType)f.EvidenceType,
                                            MimeType = f.MimeType
                                        })
                                    .ToList();
                            }
                            else
                            {
                                model.FineEvidenceModels = offence.EvidenceLog.InfringementEvidences
                                    .Where(f => !f.FileName.Contains(".sml"))
                                    .Select(f =>
                                        new FineEvidenceModel
                                        {
                                            ID = f.ID,
                                            ReferenceNumber = model.ReferenceNumber,
                                            EvidenceType = (EvidenceType)f.EvidenceType,
                                            MimeType = f.MimeType
                                        })
                                    .ToList();
                            }
                        }
                    }
                    
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

                    if (includeAccount)
                    {
                        var accountTransactions = dbContext.AccountTransactions
                            .AsNoTracking()
                            .Where(f => f.AccountID == offence.Register.AccountID  && 
                                f.ReferenceNumber == offence.ReferenceNumber)
                            .OrderBy(f => f.CreatedTimestamp)
                            .ToList();

                        model.AccountTransactionModels = accountTransactions.Select(f => 
                            new AccountTransactionModel 
                            { 
                                CreatedTimestamp = f.CreatedTimestamp,
                                Description = f.Description,
                                Amount = f.Amount
                            }).ToList();
                    }

                    models.Add(model);
                }

                return Ok(models);
            }
        }
        
        [HttpGet]
        [Route("Evidence")]
        [ResponseType(typeof(FileResult))]
        public IHttpActionResult GetEvidence(long id)
        {
            var serverPath = string.Empty;

            using (var dbContext = new DataContext())
            {
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
                else
                {
                    serverPath = HttpContext.Current.Server.MapPath("~/Content/Images/404_FILE_NOT_FOUND.png");
                }
                
                var fileInfo = new FileInfo(serverPath);

                return !fileInfo.Exists ? (IHttpActionResult)NotFound() : new FileResult(fileInfo.FullName);
            }
        }

        [HttpPut]
        [Route("Amount")]
        [SessionAuthorize]
        [UsageLog]
        [ValidationActionFilter]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> ChangeAmount(ChangeAmountModel model)
        {
            using (var dbContext = new DataContext())
            {
                var credential = dbContext.Credentials.FirstOrDefault(f => f.UserName == model.UserName);
                if (credential == null)
                    return this.BadRequestEx(Error.CredentialNotFound);

                if ((credential.Password != model.Password) && (model.Password.ToUpper() != MessageDigest.HashSHA256(credential.Password)))
                    return this.BadRequestEx(Error.PasswordIncorrect);

                if (credential.Status !=  Core.Data.Enums.Status.Active)
                    return this.BadRequestEx(Error.CredentialNotActive);

                if (credential.ExpiryTimeStamp < DateTime.Now)
                    return this.BadRequestEx(Error.PasswordHasExpired);


                var offenceRegister = dbContext.OffenceRegister
                    .AsNoTracking()                
                    .Include(f => f.EvidenceLog)
                    .Include(f => f.EvidenceLog.ChargeInfos)
                    .FirstOrDefault(f => f.ReferenceNumber == model.ReferenceNumber);
                if (offenceRegister == null)
                {
                    return this.BadRequestEx(Error.RegisterItemDoesNotExist);
                }
              
                var representationTransaction = new RepresentationTransaction();
                representationTransaction.RegisterID = offenceRegister.ID;
                representationTransaction.ReferenceNumber = offenceRegister.ReferenceNumber;
                representationTransaction.Amount = (model.CurrentAmount > model.NewAmount) ? model.NewAmount - model.CurrentAmount : model.CurrentAmount - model.NewAmount;
                representationTransaction.Reason = model.ApplicantReason;
                representationTransaction.AccountTransactionType = (Core.Data.Enums.AccountTransactionType)model.AccountTransactionType;
                representationTransaction.AccountCurrencyType = (Core.Data.Enums.AccountCurrencyType)model.AccountCurrencyType;
                representationTransaction.CapturedCredentialID = credential.EntityID;
                representationTransaction.CapturedDate = DateTime.Now;
                representationTransaction.ResultType = Core.Data.Enums.ResultType.Approved;
                representationTransaction.EvaluatedDate = model.ApprovedDate.HasValue ? model.ApprovedDate.Value : DateTime.Now;
                representationTransaction.EvaluatedBy = model.ApprovedBy;
                representationTransaction.ChargeNumber = 1;
                representationTransaction.ChargeCode = offenceRegister.EvidenceLog.ChargeInfos[0].OffenceCode.Code;
                representationTransaction.ProcessedDate = DateTime.Now;
                representationTransaction.ProcessedTerminalName = model.TerminalName;
                representationTransaction.ProcessedCredentialID = credential.EntityID;

                dbContext.RepresentationTransactions.Add(representationTransaction);

                var generatedReferenceNumber = dbContext.GeneratedReferenceNumbers.First(f => f.ReferenceNumber == model.ReferenceNumber);

                var paymentTransaction = dbContext.PaymentTransactions.Include(f => f.TransactionItems).FirstOrDefault(f => f.TransactionToken == generatedReferenceNumber.ExternalToken && f.Status == Kapsch.Core.Data.Enums.PaymentTransactionStatus.Added);
                paymentTransaction.Amount = model.NewAmount;
                paymentTransaction.TransactionItems.Single().Amount = model.NewAmount;

                dbContext.SaveChanges();

                var connection = (OracleConnection)dbContext.Database.Connection;

                using (var command = new OracleCommand())
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        command.Parameters.Add("P_USER_ID", OracleDbType.Int32).Value = credential.EntityID;
                        command.Parameters.Add("P_REFERENCE_NUMBER", OracleDbType.Varchar2).Value = model.ReferenceNumber;
                        command.Parameters.Add("P_ACCOUNT_TRANS_TYPE_ID", OracleDbType.Int32).Value = (int)model.AccountTransactionType;
                        command.Parameters.Add("P_REFERENCE_TRANSACTION_ID", OracleDbType.Int32).Value = representationTransaction.ID;
                        command.Parameters.Add("P_REFERENCE_TRANSACTION_TYPEID", OracleDbType.Int32).Value = (int)model.ReferenceTransactionType;
                        command.Parameters.Add("P_AMOUNT", OracleDbType.Decimal).Value = representationTransaction.Amount;
                        command.Parameters.Add("O_MESSAGE", OracleDbType.Varchar2, 2056).Direction = ParameterDirection.Output;

                        ExcecuteNonQuery(command, "FINANCE.FINANCIALS.ADD_VALIDATE_TRANSACTION", connection);

                        if ((command.Parameters["O_MESSAGE"].Value is DBNull))
                            return Ok();

                        var message = ((OracleString)command.Parameters["O_MESSAGE"].Value).Value;
                        if (message != "Success")
                            return this.BadRequestEx(Error.PopulateMethodFailed(message));                       
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

                await Task.Run(() =>
                {
                    var paymentProvider = resolvePaymentProvider();
                    var transactionModel =
                        new ThirdParty.Payment.Models.TransactionModel
                        {
                            CompanyRef = model.ReferenceNumber,
                            CompanyAccRef = string.Empty,
                            Amount = model.NewAmount,
                            UserID = SessionModel.UserName,
                            ServiceDescription = "Notice issued.",
                            ServiceType = 6067
                        };

                    try
                    {
                        paymentProvider.UpdateTransaction( generatedReferenceNumber.ExternalToken, transactionModel);
                    }
                    catch (Exception)
                    {
                        using (var dataContext = new DataContext())
                        {
                            InsertQueueItem(
                                dataContext,
                                new PaymentProviderQueueItem
                                {
                                    CreatedTimestamp = DateTime.Now,
                                    OperationName = "UpdateTransaction",
                                    PaymentProvider = (Kapsch.Core.Data.Enums.PaymentProvider)paymentProvider.ID,
                                    Arguments = JsonConvert.SerializeObject(transactionModel),
                                    QueueStatus = Kapsch.Core.Data.Enums.QueueStatus.Queued,
                                    TransactionToken = generatedReferenceNumber.ExternalToken
                                });
                        }
                    }
                });

                return Ok();
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
