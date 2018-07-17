using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class SessionModel
    {
        public string LocationCode { get; set; }
        public string CameraDate { get; set; }
        public string CameraSessionID { get; set; }
        public string MachineID { get; set; }
        public int NothingDoneCol { get; set; }
        public int NothingDone { get; set; }
        public int CamDateCol { get; set; }
        public object[] Columns { get; set; }     
    }
}
