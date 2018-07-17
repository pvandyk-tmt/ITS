using Kapsch.Camera.Adapters.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public abstract class BaseTCPAdapter : BaseAdapter, IAdapter
    {
        protected BaseTCPAdapter()
        {
            
        }

        public IEnumerable Process()
        {
            try
            {
                PrepareDevices();
                Initialise();

                return null;
            }
            catch (Exception e)
            {
                Log.Error(e);
                return null;
            }
            finally
            {
                Cleanup();
            }
        }

        public abstract void Shutdown();

        protected abstract void PrepareDevices();
        protected abstract void Initialise();
        protected abstract void Cleanup();       
    }
}
