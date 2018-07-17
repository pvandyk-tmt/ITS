using Atlas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Camera.Win32Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = Host.Configure<MainService>()
                .Named("Kapsch.Camera.Win32Service", "Kapsch Camera Win32Service", "Application that connects to the configured cameras for monitoring purposes.")
                .WithArguments(args); // creates configuration with defaults

            // then just start the configuration and away you go
            Host.Start(configuration);
        }
    }
}
