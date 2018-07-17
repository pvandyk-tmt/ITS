using Atlas;
using Kapsch.Camera.Adapters;
using Kapsch.Camera.Adapters.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Win32Service
{
    public class MainService : IAmAHostedProcess
    {
        public readonly static string AdaptorTypeName = ConfigurationManager.AppSettings["AdaptorTypeName"];
        private IAdapter _adapter;

        public void Pause()
        {
            
        }

        public void Resume()
        {
            
        }

        public void Start()
        {
            _adapter = AdapterFactory.GetAdapter(AdaptorTypeName);
            _adapter.Process();
        }

        public void Stop()
        {
            if (_adapter != null)
                _adapter.Shutdown();
        }
    }
}
