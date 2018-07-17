using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Interfaces;

namespace Kapsch.RTE.Gateway.Models.Configuration.Device.Listener
{
    public class MockConfigurationModel : IListenerConfiguration
    {
        public MockConfigurationModel()
        {
            TimeOffsetSecondsStart = 0;
            TimeOffsetSecondsEnd = 5;
            Seed = 1000;
        }
        
        /// <summary>
        /// The Time Offset in Seconds from DateTime.Now - This will allow the creation of DataPoints that has an offset in time to allow for a start and end scenario.
        /// Typically the offset will be 0 for one Mock Run and n for another
        /// </summary>
        public int TimeOffsetSecondsStart { get; set; }

        public int TimeOffsetSecondsEnd { get; set; }

        public int Seed { get; set; }
        public int ListenEveryMilliseconds { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string LocationCode { get; set;}
    }
}
