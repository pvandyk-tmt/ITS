using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public class iCamClientAdapterOptions : BaseAdapterOptions
    {
        public iCamClientAdapterOptions()
        {
            ApplicationName = "Camera";
            AdapterType = "iCamClient";
        }
    }
}
