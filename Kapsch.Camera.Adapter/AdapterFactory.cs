using Kapsch.Camera.Adapters.Impl;
using Kapsch.Camera.Adapters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters
{
    public class AdapterFactory
    {
        public static IAdapter GetAdapter(string typeName)
        {
            if (typeName == "Kapsch.Camera.Adapters.Impl.iCamClientAdapter")
                return new iCamClientAdapter();

            return null;
        }
    }
}
