using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Cors;
using System.Web.Http;

[assembly: OwinStartup(typeof(Kapsch.EVR.Gateway.Startup))]

namespace Kapsch.EVR.Gateway
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);

        }
    }
}