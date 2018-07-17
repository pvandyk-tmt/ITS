using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using log4net;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Kapsch.Core.Correspondence
{
    public class Processor
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private long id;

        public static void Process(long id, bool wait)
        {
            if (wait)
            {
                Processor processor = new Processor(id);
                processor.Process();
            }
            else
            {
                Task t = Task.Factory.StartNew((object contextState) =>
                {
                    try
                    {
                        Processor processor = new Processor(id);
                        processor.Process();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }, null);
            }
        }

        internal Processor(long id)
        {
            this.id = id;
        }

        internal void Process()
        {
            using (var dataContext = new DataContext())
            {
                var correspondenceItem = dataContext.CorrespondenceItems
                    .Include(f => f.Route)
                    .Include(f => f.SmsPayload)
                    .Include(f => f.EmailPayload)
                    .FirstOrDefault(f => f.ID == this.id);
                if (correspondenceItem == null)
                    return;

                correspondenceItem.Status = CorrespondenceItemStatus.Queued;
                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            
                ISettings settings = (new ConfigurationManager()).GetSettings(correspondenceItem.CorrespondenceType, correspondenceItem.SubType);
                if (settings == null)
                {
                    correspondenceItem.Status = CorrespondenceItemStatus.FailedToDispatch;
                    correspondenceItem.StatusTimestamp = DateTime.Now;
                    correspondenceItem.FailureReason = "No provider configured for correspondence item.";
                    dataContext.SaveChanges();
                    return;
                }

                switch (settings.CorrespondenceProvider)
                {
                    case CorrespondenceProvider.MockSms:
                        ProcessSmsMock(dataContext, (MockSmsSettings)settings, correspondenceItem);
                        break;
                    case CorrespondenceProvider.CMSms:
                        ProcessSmsCM(dataContext, settings as CMSmsSettings, correspondenceItem);
                        break;
                    case CorrespondenceProvider.MockEmail:
                        ProcessEmailMock(dataContext, (MockEmailSettings)settings, correspondenceItem);
                        break;
                   
                    default:
                        correspondenceItem.Status = CorrespondenceItemStatus.FailedToDispatch;
                        correspondenceItem.StatusTimestamp = DateTime.Now;
                        correspondenceItem.FailureReason = "Provider not implemented for correspondence item.";
                        dataContext.SaveChanges();
                        break;
                }
            }
        }

        internal CorrespondenceItemStatus ProcessSmsCM(DataContext dataContext, CMSmsSettings settings, CorrespondenceItem correspondenceItem)
        {
            try
            {
                correspondenceItem.ExternalReference = "";

                // if there is a problem, invalidate the mobile number

                var messageBuilder = new CMSmsMessageBuilder();
                var request =
                    messageBuilder.CreateMessage(
                        settings.ProductToken,
                        correspondenceItem.Route.Source,
                        correspondenceItem.Route.Target,
                        correspondenceItem.SmsPayload.Message);

                var webClient = new WebClient();
                webClient.Headers["Content-Type"] = messageBuilder.GetContentType();
                webClient.Encoding = System.Text.Encoding.UTF8;

                var response = JObject.Parse(webClient.UploadString(settings.BaseUrl, request));
                var errorCode = (int)response["errorCode"];
                if (errorCode == 0)
                {
                    correspondenceItem.Status = CorrespondenceItemStatus.Dispatched;
                    
                }
                else
                {
                    correspondenceItem.Status = CorrespondenceItemStatus.FailedToDispatch;
                    try
                    {
                        JArray messages = (JArray)response["messages"];
                        correspondenceItem.FailureReason = (string)messages[0]["messageDetails"];
                    }
                    catch
                    {
                        correspondenceItem.FailureReason = "Failed to parse error.";
                    }                 
                }

                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            }
            catch (WebException wex)
            {
                Logger.Error(string.Format("{0} - {1}", wex.Status, wex.Message));

                correspondenceItem.Status = CorrespondenceItemStatus.FailedToDispatch;
                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                correspondenceItem.Status = CorrespondenceItemStatus.FailedToDispatch;
                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            }

            return correspondenceItem.Status;
        }

        internal void ProcessEmailMock(DataContext dataContext, MockEmailSettings settings, CorrespondenceItem correspondenceItem)
        {
            try
            {
                Thread.Sleep(settings.Delay);

                correspondenceItem.Status = CorrespondenceItemStatus.Dispatched;
                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                correspondenceItem.Status = CorrespondenceItemStatus.FailedToDispatch;
                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            }
        }

        internal void ProcessSmsMock(DataContext dataContext, MockSmsSettings settings, CorrespondenceItem correspondenceItem)
        {
            try
            {
                Thread.Sleep(settings.Delay);

                correspondenceItem.Status = CorrespondenceItemStatus.Dispatched;
                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                correspondenceItem.Status = CorrespondenceItemStatus.FailedToDispatch;
                correspondenceItem.StatusTimestamp = DateTime.Now;
                dataContext.SaveChanges();
            }
        }
    }
}
