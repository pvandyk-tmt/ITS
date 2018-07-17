using System;
using Kapsch.Device.Listener.Enums;
using Kapsch.Device.Listener.Events;
using Kapsch.Device.Listener.Interfaces;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces;
using NLog;

namespace Kapsch.Device.Listener.Listeners
{
    public class MockListener : IListener
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public MockListener(IListenerConfiguration configuration)
        {
            if (!(configuration is MockConfigurationModel))
            {
                throw new Exception(string.Format("The listener for device {0} is not a mock type listener!", configuration.DeviceName));
            }

            Configuration = configuration;
            MockConfigurationModel mc = (MockConfigurationModel)configuration;

            Logger.Info("Created file listener for device on {0}", mc.Seed);
        }

        public event EventHandler<ListenEvent> ListenEventReceived;
        public IListenerConfiguration Configuration { get; set; }
        public ConnectionStatus ConnectionStatus { get; set; }
        public bool IsConnected { get; private set; }
        public bool Connect()
        {
            IsConnected = true;
            ConnectionStatus = ConnectionStatus.Operational;
            return true;
        }

        public bool Disconnect()
        {
            IsConnected = false;
            return true;
        }

        public void Send(string data)
        {
            return;
        }
    }
}
