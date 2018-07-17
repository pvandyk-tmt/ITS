using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public class iCamEventArgs : EventArgs
    {
        public string IpAddress { get; set; }
        public long Timestamp { get; set; }
        public string FileName { get; set; }
        public string OriginalMessage { get; set; }
    }
}
