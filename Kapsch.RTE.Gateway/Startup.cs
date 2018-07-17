using System.Collections.Generic;
using Kapsch.Core.Caching;
using Kapsch.RTE.Gateway;
using Kapsch.RTE.Gateway.Models.Configuration.Dot;
using Microsoft.Owin;
using Owin;

namespace Kapsch.RTE.Gateway
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }

        private static MemoryCache<Dictionary<SectionConfigurationModel, long>> _registeredAdapters;
        
        public static MemoryCache<Dictionary<SectionConfigurationModel, long>> RegisteredAdapters()
        {
            if (_registeredAdapters == null)
            {
                _registeredAdapters = new MemoryCache<Dictionary<SectionConfigurationModel, long>>("RegisteredDotAdapters");
            }

            return _registeredAdapters;
        }
    }
}
