using DPO.API.V5.PushPayment;
using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.Core.Gateway.Models.Payment;
using Kapsch.ITS.Webhooks.Filters;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Kapsch.ITS.Webhooks.Controllers
{
    public class DPOController : Controller
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(DPOController));
        private static readonly XmlSerializer PushReceivedXmlSerializer = new XmlSerializer(typeof(PushReceivedModel.API3G));

        [HttpPost]
        [UsageLog]
        public ActionResult Index()
        {
            try
            {
                Request.InputStream.Position = 0;

                var receivedModel = (PushReceivedModel.API3G)PushReceivedXmlSerializer.Deserialize(Request.InputStream);

                var confirmedPayment = new ConfirmedPaymentModel();
                confirmedPayment.TransactionToken = receivedModel.TransactionToken;
                confirmedPayment.TerminalType = TerminalType.ExternalServer;
                confirmedPayment.TerminalUUID = "DPO";
                confirmedPayment.Receipt = receivedModel.TransactionApproval;
                confirmedPayment.Amount = receivedModel.TransactionAmount;
                //transactionModel.CreatedTimestamp = DateTime.ParseExact(receivedModel.TransactionSettlementDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                confirmedPayment.CustomerContactNumber = receivedModel.CustomerPhone;
                confirmedPayment.CustomerFirstName = receivedModel.CustomerName;

                var paymentService = new PaymentService(CoreGatewaySession.SessionToken);
                paymentService.SettlePayment(confirmedPayment);

                return Content("<?xml version=\"1.0\" encoding=\"utf-8\"?><API3G><Response>OK</Response></API3G>");
            }
            catch (Exception ex)
            {
                Request.InputStream.Position = 0;

                var reader = new StreamReader(Request.InputStream);
                var payload = reader.ReadToEnd();
               
                Log.Error(string.Format("Unxepected error when receiving: {0}", payload), ex);
                return Content("<?xml version=\"1.0\" encoding=\"utf-8\"?><API3G><Response>Failed</Response></API3G>");
            }           
        }
    }
}