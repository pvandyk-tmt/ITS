using Hangfire;
using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using Kapsch.ITS.Gateway.Filters;
using Kapsch.ThirdParty.Payment;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Kapsch.ITS.Gateway.Jobs
{
    public class CheckPaymentProviderQueue : IRegisteredObject
    {
        public static void Execute()
        {          
            using (var dbContext = new DataContext())
            {
                while (true)
                {
                    var paymentProviderQueueItem = GetQueueItem(dbContext, PaymentProvider.DPO);
                    if (paymentProviderQueueItem == null)
                        return;

                    if (paymentProviderQueueItem.OperationName == "UpdateTransaction")
                    {
                        var paymentProvider = resolvePaymentProvider();
                        paymentProvider.Log = (paymentProviderID, requestDetail, responseDetail, exceptionMessage, isSuccess) =>
                        {
                            try
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
                            catch
                            {
                                // Empty on purpose
                            }
                        };

                        var transactionToken = paymentProviderQueueItem.TransactionToken;
                        var model = JsonConvert.DeserializeObject<ThirdParty.Payment.Models.TransactionModel>(paymentProviderQueueItem.Arguments);

                        try
                        {
                            paymentProvider.UpdateTransaction(transactionToken, model);

                            UpdateQueueItem(dbContext, paymentProviderQueueItem.ID, QueueStatus.Remove);
                        }
                        catch (ProviderException pex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(
                                new Exception(
                                    string.Format("Failed to update transaction ({0}). Error code {1}\n{2}", 
                                        model.CompanyRef, 
                                        pex.Code, 
                                        paymentProviderQueueItem.Arguments)));

                            UpdateQueueItem(dbContext, paymentProviderQueueItem.ID, QueueStatus.Remove);
                        }
                        catch (Exception ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(
                                new Exception(
                                    string.Format("Failed to update transaction ({0}). Exception: {1}\n{2}",
                                        model.CompanyRef,
                                        ex.Message,
                                        paymentProviderQueueItem.Arguments)));

                            UpdateQueueItem(dbContext, paymentProviderQueueItem.ID, QueueStatus.Queued);
                        }
                    }                   
                }
            }
        }

        public void Stop(bool immediate)
        {

        }

        private static PaymentProviderQueueItem GetQueueItem(DataContext dbContext, PaymentProvider paymentProvider)
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

                    command.Parameters.Add("P_PAYMENT_PROVIDER_ID", OracleDbType.Int32).Value = paymentProvider;
                    command.Parameters.Add("O_QUEUED_DATA", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    ExcecuteNonQuery(command, "finance.PAYMENT_PROVIDER_REQUEST_INFO.GET_NEXT_QUEUED_ENTRY", connection);

                    if ((command.Parameters["O_QUEUED_DATA"].Value == null) || (command.Parameters["O_QUEUED_DATA"].Value is DBNull))
                        return null;

                    var refCursor = (OracleRefCursor)command.Parameters["O_QUEUED_DATA"].Value;

                    using (var dataReader = refCursor.GetDataReader())
                    {
                        while (dataReader.Read())
                        {
                            var paymentProviderQueueItem = new PaymentProviderQueueItem();
                            paymentProviderQueueItem.ID = long.Parse(dataReader["ID"].ToString());
                            paymentProviderQueueItem.CreatedTimestamp = DateTime.Parse(dataReader["CREATED_DATE"].ToString());
                            paymentProviderQueueItem.QueueStatus = (QueueStatus)int.Parse(dataReader["QUEUE_STATUS_ID"].ToString());
                            paymentProviderQueueItem.Arguments = dataReader["ARGUMENTS"].ToString();
                            paymentProviderQueueItem.TransactionToken = dataReader["TRANSACTION_TOKEN"].ToString();
                            paymentProviderQueueItem.OperationName = dataReader["OPERATION_NAME"].ToString();
                            paymentProviderQueueItem.PaymentProvider = (PaymentProvider)int.Parse(dataReader["PAYMENT_PROVIDER_ID"].ToString());

                            return paymentProviderQueueItem;
                        }
                    }
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

            return null;
        }

        
        private static void UpdateQueueItem(DataContext dbContext, long id, QueueStatus queueStatus)
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

                    command.Parameters.Add("P_PAYMENT_PROVIDER_QUEUE_ID", OracleDbType.Int64).Value = id;
                    command.Parameters.Add("P_QUEUE_STATUS_ID", OracleDbType.Int32).Value = (int)queueStatus;
                    
                    ExcecuteNonQuery(command, "finance.PAYMENT_PROVIDER_REQUEST_INFO.UPDATE_QUEUE", connection);
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

        private static IProvider resolvePaymentProvider()
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
                                PaymentProvider = (Kapsch.Core.Data.Enums.PaymentProvider)paymentProviderID,
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

        private static void ExcecuteNonQuery(OracleCommand command, string storedProcName, OracleConnection dbConnection)
        {
            command.Connection = dbConnection;
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }
    }
}