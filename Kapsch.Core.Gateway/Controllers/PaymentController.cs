using DPO.API.V5;
using Kapsch.Core.Extensions;
using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Hubs;
using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.Core.Gateway.Models.Payment;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.ThirdParty.Payment;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using Kapsch.Gateway.Shared.Filters;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DPO.API.V5.PushPayment;
using System.Web.Http.Description;
using System.Globalization;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Core.Filters;
using Kapsch.Core.Gateway.Clients;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using Kapsch.Core.Cryptography;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/Payment")]
    public class PaymentController : BaseController
    {
        [HttpPost]
        [Route("Terminal")]
        [SessionAuthorize]
        [UsageLog]
        [ValidationActionFilter]
        [ResponseType(typeof(PaymentTerminalModel))]
        public IHttpActionResult AddPaymentTerminal(PaymentTerminalModel model)
        {
            using (var dbContext = new DataContext())
            {
                if (dbContext.PaymentTerminals.Any(f => f.TerminalType == (Data.Enums.TerminalType) model.TerminalType && f.UUID == model.UUID))
                    return this.BadRequestEx(Error.PaymentTerminalAlreadyExist);

                var paymentTerminal = new PaymentTerminal();
                paymentTerminal.UUID = model.UUID;
                paymentTerminal.TerminalType = (Data.Enums.TerminalType)model.TerminalType;
                paymentTerminal.ModifiedTimestamp = DateTime.Now;
                paymentTerminal.Status = Data.Enums.Status.Active;

                dbContext.PaymentTerminals.Add(paymentTerminal);
                dbContext.SaveChanges();

                model.ID = paymentTerminal.ID;
                model.ModifiedTimestamp = paymentTerminal.ModifiedTimestamp;

                return Ok(model);
            }
        }

        [HttpGet]
        [Route("Terminal")]
        [SessionAuthorize]
        [UsageLog]
        [ValidationActionFilter]
        [ResponseType(typeof(PaymentTerminalModel))]
        public IHttpActionResult GetPaymentTerminal(long id)
        {
            using (var dbContext = new DataContext())
            {
                var paymentTerminal = dbContext.PaymentTerminals.FirstOrDefault(f => f.ID == id);
                if (paymentTerminal == null)
                    return this.BadRequestEx(Error.PaymentTerminalDoesNotExist);

                return Ok(
                    new PaymentTerminalModel 
                    { 
                        ID = paymentTerminal.ID,
                        UUID = paymentTerminal.UUID,
                        Status = (Status)paymentTerminal.Status,
                        ModifiedTimestamp = paymentTerminal.ModifiedTimestamp,
                        TerminalType = (TerminalType)paymentTerminal.TerminalType
                    });
            }
        }

        [SessionAuthorize]
        [HttpPost]
        [UsageLog]
        [Route("Terminal/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<PaymentTerminalModel>))]
        public IHttpActionResult GetTerminalPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.PaymentTerminal>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .PaymentTerminals
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<PaymentTerminalModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<PaymentTerminalModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<PaymentTerminal>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<PaymentTerminalModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new PaymentTerminalModel
                    {
                        ID = f.ID,
                        ModifiedTimestamp = f.ModifiedTimestamp,
                        Status = (Models.Enums.Status)f.Status,
                        TerminalType = (Models.Enums.TerminalType)f.TerminalType,
                        UUID = f.UUID
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPut]
        [Route("Terminal")]
        [SessionAuthorize]
        [UsageLog]
        [ValidationActionFilter]
        public IHttpActionResult UpdatePaymentTerminal(PaymentTerminalModel model)
        {
            using (var dbContext = new DataContext())
            {
                var paymentTerminal = dbContext.PaymentTerminals.FirstOrDefault(f => f.ID == model.ID);
                if (paymentTerminal == null)
                    return this.BadRequestEx(Error.PaymentTerminalDoesNotExist);

                paymentTerminal.UUID = model.UUID;
                paymentTerminal.TerminalType = (Data.Enums.TerminalType)model.TerminalType;
                paymentTerminal.ModifiedTimestamp = DateTime.Now;
                paymentTerminal.Status = Data.Enums.Status.Active;
              
                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [Route("Transaction")]
        [SessionAuthorize]
        [UsageLog]
        [ValidationActionFilter]
        [ResponseType(typeof(string))]
        public IHttpActionResult RegisterTransaction(PaymentTransactionModel model)
        {
            if (model.Amount != model.PaymentTransactionItems.Sum(f => f.Amount))
            {
                return this.BadRequestEx(Error.PaymentTransactionInvalidAmount);
            }

            using (var dbContext = new DataContext())
            {
                var paymentTerminal = dbContext.PaymentTerminals.FirstOrDefault(f => f.TerminalType == (Data.Enums.TerminalType)model.TerminalType && f.UUID == model.TerminalUUID);
                if (paymentTerminal == null)
                {
                    return this.BadRequestEx(Error.PaymentTerminalDoesNotExist);
                }

                if (paymentTerminal.Status != Data.Enums.Status.Active)
                {
                    return this.BadRequestEx(Error.PaymentTerminalIsNotActive);
                }

                if (dbContext.PaymentTransactions.Any(f => f.Receipt == model.Receipt))
                {
                    return this.BadRequestEx(Error.PaymentTransactionAlreadyExist);
                }

                foreach (var modelItem in model.PaymentTransactionItems)
                {
                    var register = dbContext.Registers.AsNoTracking().FirstOrDefault(f => f.ReferenceNumber == modelItem.ReferenceNumber);
                    if (register == null)
                        return this.BadRequestEx(Error.RegisterItemDoesNotExist);

                    if (register.RegisterStatus >= 3000)
                        return this.BadRequestEx(Error.RegisterItemNotPayable);

                    if (string.IsNullOrWhiteSpace(modelItem.TransactionToken))
                    {
                        var generatedReferenceNumber = dbContext.GeneratedReferenceNumbers.FirstOrDefault(f => f.ReferenceNumber == modelItem.ReferenceNumber);
                        if (generatedReferenceNumber != null)
                        {
                            modelItem.TransactionToken = generatedReferenceNumber.ExternalToken;
                        }
                    }
                }
            
                PaymentTransaction paymentTransaction = null;

                if (model.PaymentTransactionItems.Count == 1)
                {
                    var transactionToken = model.PaymentTransactionItems.First().TransactionToken;

                    paymentTransaction = dbContext.PaymentTransactions.FirstOrDefault(f => f.TransactionToken == transactionToken && f.Status == Data.Enums.PaymentTransactionStatus.Added);
                    paymentTransaction.Receipt = model.Receipt;
                    paymentTransaction.ReceiptTimestamp = model.ReceiptTimestamp;
                    paymentTransaction.CourtID = model.CourtID;
                    paymentTransaction.TerminalID = paymentTerminal.ID;
                    paymentTransaction.CustomerContactNumber = model.CustomerContactNumber;
                    paymentTransaction.CustomerFirstName = model.CustomerFirstName;
                    paymentTransaction.CustomerIDNumber = model.CustomerIDNumber;
                    paymentTransaction.CustomerLastName = model.CustomerLastName;
                    paymentTransaction.PaymentMethod = (Data.Enums.PaymentMethod)model.PaymentSource;
                }
                else
                {
                    paymentTransaction = new PaymentTransaction();
                    paymentTransaction.Receipt = model.Receipt;
                    paymentTransaction.ReceiptTimestamp = model.ReceiptTimestamp;
                    paymentTransaction.CourtID = model.CourtID;
                    paymentTransaction.TerminalID = paymentTerminal.ID;
                    paymentTransaction.Amount = model.Amount;         
                    paymentTransaction.CreatedTimestamp = DateTime.Now;
                    paymentTransaction.CustomerContactNumber = model.CustomerContactNumber;
                    paymentTransaction.CustomerFirstName = model.CustomerFirstName;
                    paymentTransaction.CustomerIDNumber = model.CustomerIDNumber;
                    paymentTransaction.CustomerLastName = model.CustomerLastName;
                    paymentTransaction.PaymentMethod = (Data.Enums.PaymentMethod)model.PaymentSource;
                    paymentTransaction.Status = Data.Enums.PaymentTransactionStatus.Added;

                    foreach (var modelItem in model.PaymentTransactionItems)
                    {
                        var paymentTransactionItem = new PaymentTransactionItem();
                        paymentTransactionItem.PaymentTransaction = paymentTransaction;
                        paymentTransactionItem.ReferenceNumber = modelItem.ReferenceNumber;
                        paymentTransactionItem.TransactionToken = modelItem.TransactionToken;
                        paymentTransactionItem.EntityReferenceTypeID = long.Parse(modelItem.ReferenceNumber[1].ToString());
                        paymentTransactionItem.Description = modelItem.Description;
                        paymentTransactionItem.Amount = modelItem.Amount;
                        paymentTransactionItem.Status = Data.Enums.PaymentTransactionStatus.Added;

                        dbContext.PaymentTransactionItems.Add(paymentTransactionItem);
                    }

                    if (model.PaymentSource == PaymentMethod.DumaPay)
                    {
                        var paymentProvider = resolvePaymentProvider();
                        var transactionIDModel =
                            paymentProvider.RegisterTransaction(
                                new ThirdParty.Payment.Models.TransactionModel
                                {
                                    CustomerFirstName = paymentTransaction.CustomerFirstName,
                                    CustomerLastName = paymentTransaction.CustomerLastName,
                                    CompanyRef = string.Empty,
                                    CompanyAccRef = paymentTransaction.Receipt,
                                    Amount = paymentTransaction.Amount,
                                    UserID = SessionModel.UserName,
                                    ServiceDescription = "Payment Basket Registered",
                                    ServiceType = 6067
                                });

                        paymentTransaction.TransactionToken = transactionIDModel.TransactionToken;
                        paymentTransaction.TransactionReference = transactionIDModel.TransactionReference;
                    }
                    else // Cash
                    {
                        paymentTransaction.TransactionToken = Guid.NewGuid().ToString();
                        paymentTransaction.TransactionReference = string.Empty;
                    }

                    dbContext.PaymentTransactions.Add(paymentTransaction);             
                }

                dbContext.SaveChanges();

                PaymentTransactionHub.SendStatusChanged(
                    paymentTransaction.TransactionToken, 
                    (PaymentTransactionStatus)paymentTransaction.Status, 
                    paymentTransaction.Amount);

                return Ok(paymentTransaction.TransactionToken);
            }
        }

        [HttpDelete]
        [SessionAuthorize]
        [UsageLog]
        [Route("Transaction")]
        public IHttpActionResult CancelTransaction(string transactionTokenOrReceipt)
        {
            using (var dbContext = new DataContext())
            {
                var paymentTransaction = dbContext.PaymentTransactions
                    .Include(f => f.TransactionItems)
                    .FirstOrDefault(f => 
                        (f.TransactionToken == transactionTokenOrReceipt || f.Receipt == transactionTokenOrReceipt) && 
                        f.Status == Data.Enums.PaymentTransactionStatus.Added);
                if (paymentTransaction == null)
                {
                    return this.BadRequestEx(Error.PaymentTransactionDoesNotExist);               
                }

                paymentTransaction.Status = Data.Enums.PaymentTransactionStatus.Cancelled;
                foreach (var paymentTransactionItem in paymentTransaction.TransactionItems)
                    paymentTransactionItem.Status = Data.Enums.PaymentTransactionStatus.Cancelled;

                PaymentTransactionHub.SendStatusChanged(
                    paymentTransaction.TransactionToken, 
                    (PaymentTransactionStatus)paymentTransaction.Status, 
                    paymentTransaction.Amount);

                return Ok();
            }
        }

        [HttpPut]
        [SessionAuthorize]
        [UsageLog]
        [Route("Transaction")]
        public IHttpActionResult SettleTransaction(ConfirmedPaymentModel confirmedPaymentModel)
        {
            using (var dbContext = new DataContext())
            {
                var paymentTransactions = dbContext.PaymentTransactions
                    .Include(f => f.TransactionItems).Where(f => f.TransactionToken == confirmedPaymentModel.TransactionToken)
                    .ToList();
             
                if (paymentTransactions == null || paymentTransactions.Count == 0)
                {
                    ProcessUnallocated(dbContext, confirmedPaymentModel);
                    return Ok();
                }

                // Iffy
                var paymentTransaction = paymentTransactions.FirstOrDefault(f => f.Status == Data.Enums.PaymentTransactionStatus.Added);
                if (paymentTransaction == null)
                {
                    ProcessDuplicate(dbContext, confirmedPaymentModel);
                    return Ok();
                }

                if (confirmedPaymentModel.Amount < paymentTransaction.Amount)
                {
                    ProcessUnderpayment(dbContext, paymentTransaction, confirmedPaymentModel);
                    return Ok();
                }
                
                if (confirmedPaymentModel.Amount > paymentTransaction.Amount)
                {
                    ProcessOverpayment(dbContext, paymentTransaction, confirmedPaymentModel);
                    return Ok();
                }

                var paymentProvider = resolvePaymentProvider();

                if (!paymentTransaction.TerminalID.HasValue)
                {
                    var paymentTerminal = dbContext.PaymentTerminals.FirstOrDefault(f => f.TerminalType == (Data.Enums.TerminalType)confirmedPaymentModel.TerminalType && f.UUID == confirmedPaymentModel.TerminalUUID);
                    if (paymentTerminal == null)
                    {
                        return this.BadRequestEx(Error.PaymentTerminalDoesNotExist);
                    }

                    paymentTransaction.TerminalID = paymentTerminal.ID;
                }

                if (string.IsNullOrEmpty(paymentTransaction.Receipt))
                {
                    paymentTransaction.Receipt = confirmedPaymentModel.Receipt;
                    paymentTransaction.ReceiptTimestamp = confirmedPaymentModel.TransactionTimestamp;
                }

                paymentTransaction.CredentialID = SessionModel.CredentialID;
                paymentTransaction.ModifiedTimestamp = DateTime.Now;
                paymentTransaction.Status = Data.Enums.PaymentTransactionStatus.Settled;

                foreach (var paymentTransactionItem in paymentTransaction.TransactionItems)
                {
                    paymentTransactionItem.Status = Data.Enums.PaymentTransactionStatus.Settled;

                    try
                    {
                        paymentProvider.CancelTransaction(paymentTransactionItem.TransactionToken);
                    }
                    catch
                    {
                        // Empty on purpose
                    }                  
                }

                dbContext.SaveChanges();

                PaymentTransactionHub.SendStatusChanged(
                    paymentTransaction.TransactionToken, 
                    (PaymentTransactionStatus)paymentTransaction.Status, 
                    paymentTransaction.Amount);

                Task.Run(() => ProcessTransaction(paymentTransaction.ID, SessionModel.CredentialID));
            }
        
            return Ok();
        }

        [HttpGet]
        [SessionAuthorize]
        [UsageLog]
        [Route("Transaction")]
        [ResponseType(typeof(PaymentTransactionModel))]
        public IHttpActionResult Get(string transactionTokenOrReceipt)
        {
            using (var dbContext = new DataContext())
            {
                var paymentTransaction = dbContext.PaymentTransactions
                    .Include(f => f.TransactionItems)
                    .FirstOrDefault(f => f.TransactionToken == transactionTokenOrReceipt || f.Receipt == transactionTokenOrReceipt);
                if (paymentTransaction == null)
                {
                    return this.BadRequestEx(Error.PaymentTransactionDoesNotExist);
                }

                var model = new PaymentTransactionModel();
                model.TransactionToken = paymentTransaction.TransactionToken;
                model.Receipt = paymentTransaction.Receipt;
                model.ReceiptTimestamp = paymentTransaction.ReceiptTimestamp;
                model.TerminalUUID = paymentTransaction.PaymentTerminal.UUID;
                model.TerminalType = (TerminalType) paymentTransaction.PaymentTerminal.TerminalType;
                model.Amount = paymentTransaction.Amount;
                model.CustomerContactNumber = paymentTransaction.CustomerContactNumber;
                model.CustomerFirstName = paymentTransaction.CustomerFirstName;
                model.CustomerIDNumber = paymentTransaction.CustomerIDNumber;
                model.CustomerLastName = paymentTransaction.CustomerLastName;
                model.PaymentSource = (PaymentMethod)paymentTransaction.PaymentMethod;
                model.CredentialID = paymentTransaction.CredentialID;
                model.CreatedTimestamp = paymentTransaction.CreatedTimestamp;
                model.ModifiedTimestamp = paymentTransaction.ModifiedTimestamp;
                model.Status = (PaymentTransactionStatus)paymentTransaction.Status;
                model.PaymentTransactionItems = 
                    paymentTransaction.TransactionItems.Select(f => 
                        new PaymentTransactionItemModel 
                        { 
                            Amount = f.Amount,
                            Description = f.Description,
                            EntityReferenceTypeID = f.EntityReferenceTypeID,
                            ReferenceNumber = f.ReferenceNumber,
                            TransactionToken = f.TransactionToken,
                            Status = (PaymentTransactionStatus)f.Status
                        })
                        .ToList();

                return Ok(model);
            }
        }

        private void ProcessTransaction(long paymentTransactionID, long credentialID)
        {
            using (var dbContext = new DataContext())
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

                        command.Parameters.Add("P_PAYMENT_TRANSACTION_ID", OracleDbType.Int32).Value = paymentTransactionID;
                        command.Parameters.Add("P_CREDENTIAL_ID", OracleDbType.Int32).Value = credentialID;
                        command.Parameters.Add("P_PAYMENT_TYPE_ID", OracleDbType.Int32).Value = 1;

                        ExcecuteNonQuery(command, "finance.financials.process_payment", connection);
                    }
                    catch (Exception)
                    {

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
        }

        private void ProcessDuplicate(DataContext dbContext, ConfirmedPaymentModel confirmedPaymentModel)
        {
            var rollingRegister = new RollingRegister();
            rollingRegister.Amount = confirmedPaymentModel.Amount;
            rollingRegister.CredentialID = SessionModel.CredentialID;
            rollingRegister.DateOpened = DateTime.Now;
            rollingRegister.PaymentMethod = (Data.Enums.PaymentMethod)confirmedPaymentModel.PaymentSource;
            rollingRegister.PaymentTransactionID = null;
            rollingRegister.Reference = string.Format("Token: {0}, Transaction Approval: {1}", confirmedPaymentModel.TransactionToken, confirmedPaymentModel.Receipt);
            rollingRegister.RollingRegisterStatus = Data.Enums.RollingRegisterStatus.Open;
            rollingRegister.RollingRegisterType = Data.Enums.RollingRegisterType.OverPaymentDuplicated;

            dbContext.RollingRegisters.Add(rollingRegister);
            dbContext.SaveChanges();
        }

        private void ProcessUnallocated(DataContext dbContext, ConfirmedPaymentModel confirmedPaymentModel)
        {
            var rollingRegister = new RollingRegister();
            rollingRegister.Amount = confirmedPaymentModel.Amount;
            rollingRegister.CredentialID = SessionModel.CredentialID;
            rollingRegister.DateOpened = DateTime.Now;
            rollingRegister.PaymentMethod = (Data.Enums.PaymentMethod)confirmedPaymentModel.PaymentSource;
            rollingRegister.PaymentTransactionID = null;
            rollingRegister.Reference = string.Format("Token: {0}, Transaction Approval: {1}", confirmedPaymentModel.TransactionToken, confirmedPaymentModel.Receipt);
            rollingRegister.RollingRegisterStatus = Data.Enums.RollingRegisterStatus.Open;
            rollingRegister.RollingRegisterType = Data.Enums.RollingRegisterType.Unallocated;

            dbContext.RollingRegisters.Add(rollingRegister);
            dbContext.SaveChanges();
        }

        private void ProcessOverpayment(DataContext dbContext, PaymentTransaction paymentTransaction, ConfirmedPaymentModel confirmedPaymentModel)
        {
            var paymentProvider = resolvePaymentProvider();

            var rollingRegister = new RollingRegister();
            rollingRegister.Amount = confirmedPaymentModel.Amount;
            rollingRegister.CredentialID = SessionModel.CredentialID;
            rollingRegister.DateOpened = DateTime.Now;
            rollingRegister.PaymentMethod = (Data.Enums.PaymentMethod)confirmedPaymentModel.PaymentSource;
            rollingRegister.PaymentTransactionID = paymentTransaction.ID;
            rollingRegister.Reference = string.Format("Token: {0}, Transaction Approval: {1}", confirmedPaymentModel.TransactionToken, confirmedPaymentModel.Receipt);
            rollingRegister.RollingRegisterStatus = Data.Enums.RollingRegisterStatus.Open;
            rollingRegister.RollingRegisterType = Data.Enums.RollingRegisterType.OverPaymentExceedsbalance;

            dbContext.RollingRegisters.Add(rollingRegister);

            paymentTransaction.CredentialID = SessionModel.CredentialID;
            paymentTransaction.ModifiedTimestamp = DateTime.Now;
            paymentTransaction.Status = Data.Enums.PaymentTransactionStatus.RollingRegister;

            foreach (var paymentTransactionItem in paymentTransaction.TransactionItems)
            {
                paymentTransactionItem.Status = Data.Enums.PaymentTransactionStatus.RollingRegister;

                try
                {
                    paymentProvider.CancelTransaction(paymentTransactionItem.TransactionToken);
                }
                catch
                {
                    // Empty on purpose
                }
            }

            dbContext.SaveChanges();
        }

        private void ProcessUnderpayment(DataContext dbContext, PaymentTransaction paymentTransaction, ConfirmedPaymentModel confirmedPaymentModel)
        {
            var paymentProvider = resolvePaymentProvider();

            var rollingRegister = new RollingRegister();
            rollingRegister.Amount = confirmedPaymentModel.Amount;
            rollingRegister.CredentialID = SessionModel.CredentialID;
            rollingRegister.DateOpened = DateTime.Now;
            rollingRegister.PaymentMethod = (Data.Enums.PaymentMethod)confirmedPaymentModel.PaymentSource;
            rollingRegister.PaymentTransactionID = paymentTransaction.ID;
            rollingRegister.Reference = string.Format("Token: {0}, Transaction Approval: {1}", confirmedPaymentModel.TransactionToken, confirmedPaymentModel.Receipt);
            rollingRegister.RollingRegisterStatus = Data.Enums.RollingRegisterStatus.Open;
            rollingRegister.RollingRegisterType = Data.Enums.RollingRegisterType.UnderPayment;

            dbContext.RollingRegisters.Add(rollingRegister);

            paymentTransaction.CredentialID = SessionModel.CredentialID;
            paymentTransaction.ModifiedTimestamp = DateTime.Now;
            paymentTransaction.Status = Data.Enums.PaymentTransactionStatus.RollingRegister;

            foreach (var paymentTransactionItem in paymentTransaction.TransactionItems)
            {
                paymentTransactionItem.Status = Data.Enums.PaymentTransactionStatus.RollingRegister;

                try
                {
                    paymentProvider.CancelTransaction(paymentTransactionItem.TransactionToken);
                }
                catch
                {
                    // Empty on purpose
                }
            }

            dbContext.SaveChanges();
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
                                PaymentProvider = (Data.Enums.PaymentProvider)paymentProviderID,
                                CreatedTimestamp = DateTime.Now,
                                RequestDetail = requestDetail,
                                ResponseDetail = responseDetail,
                                ExceptionMessage = exceptionMessage,
                                Status = isSuccess ? Data.Enums.RequestStatus.Success : Data.Enums.RequestStatus.Failed

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
