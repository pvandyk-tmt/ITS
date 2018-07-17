using Microsoft.AspNet.SignalR;

namespace Kapsch.RTE.Gateway.Hubs.CameraEvent
{
    public class DotNotificationHub : Hub
    {
        public override System.Threading.Tasks.Task OnConnected()
        {
            var sectionCode = Context.Request.Headers.Get("SectionCode");

            if (!string.IsNullOrWhiteSpace(sectionCode))
            {
                Groups.Add(Context.ConnectionId, sectionCode);
            }

            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var sectionCode = Context.Request.Headers.Get("SectionCode");

            if (!string.IsNullOrWhiteSpace(sectionCode))
            {
                Groups.Remove(Context.ConnectionId, sectionCode);
            }

            return base.OnDisconnected(stopCalled);
        }

        public void SendOverSectionInfringement(string sectionCode, string model)
        {
            if(!string.IsNullOrWhiteSpace(sectionCode))
            {
                Clients.Group(sectionCode).onOverSectionInfringementReceived(model);
            }
        }
    }
}