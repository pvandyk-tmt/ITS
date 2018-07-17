using Kapsch.Core.Gateway.Models.Enums;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapsch.Core.Gateway.Hubs
{
    public class PaymentTransactionHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            var transactionToken = Context.Request.Headers.Get("TransactionToken");
            if (string.IsNullOrWhiteSpace(transactionToken))
            {
                transactionToken = Context.Request.QueryString.Get("transactionToken");
            }

            if (!string.IsNullOrWhiteSpace(transactionToken))
            {
                Groups.Add(Context.ConnectionId, transactionToken);
            }

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var transactionToken = Context.Request.Headers.Get("TransactionToken");
            if (string.IsNullOrWhiteSpace(transactionToken))
            {
                transactionToken = Context.Request.QueryString.Get("transactionToken");
            }

            if (!string.IsNullOrWhiteSpace(transactionToken))
            {
                Groups.Remove(Context.ConnectionId, transactionToken);
            }

            return base.OnDisconnected(stopCalled);
        }

        public static void SendStatusChanged(string transactionToken, PaymentTransactionStatus status, decimal amount)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<PaymentTransactionHub>();
            hubContext.Clients.Group(transactionToken).onStatusChanged(transactionToken, status, amount);
        }
    }
}