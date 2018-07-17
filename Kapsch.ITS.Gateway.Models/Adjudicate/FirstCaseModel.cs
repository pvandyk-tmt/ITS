using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Adjudicate
{
    public class FirstCaseModel
    {
        public IList<RejectReasonModel> RejectReasons { get; set; }
        public int TicketCount { get; set; }
        public CaseModel Case { get; set; }
    }
}
