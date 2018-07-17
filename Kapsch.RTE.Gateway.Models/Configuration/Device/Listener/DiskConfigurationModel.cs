using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces;

namespace Kapsch.RTE.Gateway.Models.Configuration.Device.Listener
{
    public class DiskConfigurationModel : IListenerConfiguration
    {
        /// <summary>
        /// The Path where to look for files
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The Search Filter eg. *.enc
        /// </summary>
        public string SearchPattern { get; set; }
        public int ListenEveryMilliseconds { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
    }
}
