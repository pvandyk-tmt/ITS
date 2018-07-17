using Microsoft.Owin;
using Owin;
using Hangfire;
using System.Configuration;
using Hangfire.MemoryStorage;
using Kapsch.ITS.Gateway.Jobs;


[assembly: OwinStartup(typeof(Kapsch.ITS.Gateway.Startup))]
namespace Kapsch.ITS.Gateway
{
    public class Startup
    {
        public static string CheckPaymentProviderQueueTime = ConfigurationManager.AppSettings.Get("CheckPaymentProviderQueueTime");

        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseMemoryStorage();

            app.UseHangfireServer();

            RecurringJob.AddOrUpdate("CheckPaymentProviderQueueTime", () => CheckPaymentProviderQueue.Execute(), CheckPaymentProviderQueueTime);
        }
    }
}