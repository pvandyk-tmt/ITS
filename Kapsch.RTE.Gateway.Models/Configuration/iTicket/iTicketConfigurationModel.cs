using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Enums;

namespace Kapsch.RTE.Gateway.Models.Configuration.iTicket
{
    public class iTicketConfigurationModel
    {

        //When using Socket Reader you need to enter the following
        public string PointAIpAddress { get; set; }
        public string PointBIpAddress { get; set; }
        public int PointAPort { get; set; }
        public int PointBPort { get; set; }

        public int PollMs { get; set; }
    }
}
