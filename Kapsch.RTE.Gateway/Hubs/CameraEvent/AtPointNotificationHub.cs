using Microsoft.AspNet.SignalR;

namespace Kapsch.RTE.Gateway.Hubs.CameraEvent
{
    public class AtPointNotificationHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            var atPointCode = Context.Request.Headers.Get("AtPointCode");

            if (!string.IsNullOrWhiteSpace(atPointCode))
            {
                Groups.Add(Context.ConnectionId, atPointCode);
            }

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var atPointCode = Context.Request.Headers.Get("AtPointCode");

            if (!string.IsNullOrWhiteSpace(atPointCode))
            {
                Groups.Remove(Context.ConnectionId, atPointCode);
            }

            return base.OnDisconnected(stopCalled);
        }

        public void SendAtPointInfringement(string atPointCode, string model)
        {
            if (!string.IsNullOrWhiteSpace(atPointCode))
            {
                Clients.Group(atPointCode).onAtPointInfringementReceived(model);
            }
        }

        public void SendAtPointData(string atPointCode, string model)
        {
            if (!string.IsNullOrWhiteSpace(atPointCode))
            {
                Clients.Group(atPointCode).onAtPointDataReceived(model);
            }
        }
    }
}