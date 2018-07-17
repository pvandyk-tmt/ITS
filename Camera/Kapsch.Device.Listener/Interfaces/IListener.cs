using System;
using Kapsch.Device.Listener.Enums;
using Kapsch.Device.Listener.Events;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces;

namespace Kapsch.Device.Listener.Interfaces
{
    public interface IListener
    {
        event EventHandler<ListenEvent> ListenEventReceived;

        IListenerConfiguration Configuration { get; set; }

        ConnectionStatus ConnectionStatus { get; set; }

        bool IsConnected { get; }
        bool Connect();
        bool Disconnect();
        void Send(string data);
    }
}