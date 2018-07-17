using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public abstract class BaseAdapterOptions
    {
        protected BaseAdapterOptions()
        {
            ApplicationName = "TMT.iTrac";
            AdapterType = "Tcp";
            Enabled = true;
            Diagnostics = false;
            AuditTrail = true;
        }

        public string AdapterType { get; set; }
        public string ApplicationName { get; set; }
        public bool Diagnostics { get; set; }
        public bool AuditTrail { get; set; }
        public bool Enabled { get; set; }
    }
}
