using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Authenticate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters
{
    public static class Authorize
    {
        private readonly static string UserName = ConfigurationManager.AppSettings["CredentialUsername"];
        private readonly static string Password = ConfigurationManager.AppSettings["CredentialPassword"];
        private static object _singletonLock = new object();
        
        public static string SessionToken
        {
            get
            {
                if (LastSession == null || LastSession.ExpiryTimestamp < DateTime.Now.AddMinutes(-60))
                {
                    lock (_singletonLock)
                    {
                        if (LastSession == null || LastSession.ExpiryTimestamp < DateTime.Now.AddMinutes(-60))
                        {
                            var authenticationService = new AuthenticationService();
                            var completed = false;

                            while (!completed)
                            {
                                try
                                {
                                    LastSession = authenticationService.GetSession(new CredentialModel { UserName = UserName, Password = Password });
                                    completed = true;
                                }
                                catch
                                {
                                    // Wait and try again
                                    Thread.Sleep(10 * 1000);
                                }
                            }                         
                        }
                    }
                }

                return LastSession.SessionToken;
            }
            
        }

        private static SessionModel LastSession { get; set; }
    }
}
