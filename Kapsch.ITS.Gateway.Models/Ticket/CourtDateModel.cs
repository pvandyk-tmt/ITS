using System;

namespace Kapsch.ITS.Gateway.Models.Ticket
{
    public class CourtDateModel
    {
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public long? CourtRoomID { get; set; }
        public long CourtID { get; set; }
    }
}
