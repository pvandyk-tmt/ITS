using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class SessionsModel
    {
        public IList<SessionModel> Sessions { get; set; }
        public IList<string> Headings { get; set; }
    }
}
