using Atlas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Enforcement.iAutoLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = Host.Configure<MainService>()
                .Named("iAutoLog", "TMT Auto iLog", "Auto logging of infringement files")
                .WithArguments(args); // creates configuration with defaults

            // then just start the configuration and away you go
            Host.Start(configuration);
        }
    }
}
