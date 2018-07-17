using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl.iCam
{
    public class iCamEventModel
    {
        public string HostIp { get; set; }
        public string HostName { get; set; }
        public string Message { get; set; }
        public long HostId { get; set; }
    }
}
