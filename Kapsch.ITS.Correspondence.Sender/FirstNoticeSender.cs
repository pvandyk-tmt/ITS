using Kapsch.Core;
using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Clients;
using Kapsch.Gateway.Models.Shared;
using Kapsch.ITS.Gateway.Clients;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kapsch.ITS.Correspondence.Sender
{
    public class FirstNoticeSender
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
                var documentService = new CorrespondenceService(sessionToken);
                var itemResponseModels = documentService.SendNoticeSms(batch.Select(f => f.ReferenceNumber).ToList());

                foreach (var itemResponseModel in itemResponseModels)
                {
                    if (itemResponseModel.IsError)
                    {
                        Log.ErrorFormat("SendResponseModel: Reference Number: {0} {1}", itemResponseModel.ReferenceNumber, itemResponseModel.Error);
                    }
                }
            }
            catch (GatewayException ex)
            {
                Log.ErrorFormat("SendNoticeSms: Gateway Exception: {0} {1}", ex.Code, ex.Message);
            }
        }

        private IList<OffenceRegister> GetOffenceRegisterList()
        {
            using (var dataContext = new DataContext())
            {
                // This a hack for now, we must find a better way
                var correspondenceAlreadySent = (from item in dataContext.CorrespondenceItems
                                                join route in dataContext.CorrespondenceRoutes on item.ID equals route.CorrespondenceItemID
                                                where item.Context == "FirstNoticeSms"
                                                select item.InternalReference);

                var offenceRegisters = (from offenceRegister in dataContext.OffenceRegister
                                        join register in dataContext.Registers on offenceRegister.ID equals register.ID
                                        join district in dataContext.Districts on offenceRegister.DistrictID equals district.ID
                                        join person in dataContext.Persons on register.PersonID equals person.ID
                                        where
                                         register.RegisterStatus == 2100 &&
                                         offenceRegister.FirstPrintDate != null &&
                                         district.ActiveIndicator == 1 &&
                                         district.ID != 0 &&
                                         district.ID != 99 &&
                                         !correspondenceAlreadySent.Contains(register.ReferenceNumber)
                                        select offenceRegister)
                                        .ToList();

                return offenceRegisters;              
            }
        }
    }
}
