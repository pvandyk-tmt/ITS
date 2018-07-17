using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Clients;
using Kapsch.Gateway.Models.Shared;
using Kapsch.ITS.Gateway.Clients;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kapsch.ITS.Document.Publisher
{
    public class NoticePublisher
    {
        public static readonly string CoreGatewayEndpoint = ConfigurationManager.AppSettings["CoreGatewayEndpoint"];
        public static readonly string CoreGatewayUserName = ConfigurationManager.AppSettings["CoreGatewayUserName"];
        public static readonly string CoreGatewayPassword = ConfigurationManager.AppSettings["CoreGatewayPassword"];
        public static readonly int BatchSize = int.Parse(ConfigurationManager.AppSettings["BatchSize"]);

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async void Execute(CancellationToken cancellationToken)
        {
            var sessionToken = string.Empty;

            try
            {
                var authenticationService = new AuthenticationService();
                var sessionModel = authenticationService.GetSession(new Core.Gateway.Models.Authenticate.CredentialModel { UserName = CoreGatewayUserName, Password = CoreGatewayPassword });
                sessionToken = sessionModel.SessionToken;
            }
            catch (GatewayException ex)
            {
                Log.ErrorFormat("GetSession: Gateway Exception: {0} {1}", ex.Code, ex.Message);
                return;
            }

            var offenceRegisters = GetOffenceRegisterList();

            var tasks = new List<Task>();
            var batches = offenceRegisters.Batch(BatchSize);

            Log.InfoFormat("Number of batches, {0}, to execute.", batches.Count());

            foreach (var batch in batches)
            {
                tasks.Add(Task.Factory.StartNew(() => Execute(sessionToken, batch), cancellationToken));
            }

            try
            {
                await Task.WhenAll(tasks.ToArray());
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                {
                    if (v is TaskCanceledException)
                        Log.ErrorFormat("TaskCanceledException: Task {0}", ((TaskCanceledException)v).Task.Id);
                    else
                        Log.ErrorFormat("Exception: {0}", v.GetType().Name);
                }
            }
        }

        private void Execute(string sessionToken, IEnumerable<OffenceRegister> batch)
        {
            try
            {
                var documentService = new DocumentService(sessionToken);
                var itemResponseModels = documentService.GenerateFirstNotice(batch.Select(f => f.ReferenceNumber).ToList());

                using (var dataContext = new DataContext())
                {
                    foreach (var itemResponseModel in itemResponseModels)
                    {
                        var offenceRegister = dataContext.OffenceRegister.FirstOrDefault(f => f.ReferenceNumber == itemResponseModel.ReferenceNumber);
                        if (offenceRegister == null)
                        {
                            Log.ErrorFormat("SendResponseModel: OffenceRegister record not found {0}", itemResponseModel.ReferenceNumber);
                            continue;
                        }

                        if (!itemResponseModel.IsError)
                        {
                            offenceRegister.FirstPrintDate = DateTime.Now;
                        }
                        else
                        {
                            Log.ErrorFormat("SendResponseModel: Reference Number: {0} {1}", itemResponseModel.ReferenceNumber, itemResponseModel.Error);
                        }
                    }

                    dataContext.SaveChanges();
                }
            }
            catch (GatewayException ex)
            {
                Log.ErrorFormat("GenerateFirstNotice: Gateway Exception: {0} {1}", ex.Code, ex.Message);
            }
        }

        private IList<OffenceRegister> GetOffenceRegisterList()
        {
            using (var dataContext = new DataContext())
            {
                return dataContext.OffenceRegister
                    .AsNoTracking()
                    .Include(f => f.Register)
                    .Where(f => 
                        f.Register.RegisterStatus == 2100 && 
                        f.FirstPrintDate == null &&
                        f.District.ActiveIndicator == 1 &&
                        f.DistrictID != 99 &&
                        f.DistrictID != 0)
                    .ToList();
            }
        }
    }
}
