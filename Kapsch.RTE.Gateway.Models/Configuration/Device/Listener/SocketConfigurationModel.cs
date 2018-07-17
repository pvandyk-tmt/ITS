using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces;

namespace Kapsch.RTE.Gateway.Models.Configuration.Device.Listener
{
    public class SocketConfigurationModel : IListenerConfiguration
    {
        public SocketConfigurationModel()
        {
            TimeoutInMilliseconds = 60000;
        }

        public int TimeoutInMilliseconds { get; set; }

        /// <summary>
        /// IP Address of the camera where to connect to eg. 10.0.0.3
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// The camera port eg. 6001
        /// </summary>
        public int IpPort { get; set; }

        public int ListenEveryMilliseconds { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
    }
}
