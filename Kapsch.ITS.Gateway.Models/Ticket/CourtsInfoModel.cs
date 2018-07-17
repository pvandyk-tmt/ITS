using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Ticket
{
    public class CourtsInfoModel
    {
        public IList<CourtModel> Courts { get; set; }
        public IList<CourtRoomModel> CourtRooms { get; set; }
        public IList<CourtDateModel> CourtDates { get; set; }
    }
}
