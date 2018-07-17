using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Authenticate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Kapsch.ITS.Webhooks
{
    public class CoreGatewaySession
    {
        private static readonly string UserName = ConfigurationManager.AppSettings["CoreGatewayUserName"];
        private static readonly string Password = ConfigurationManager.AppSettings["CoreGatewayPassword"];

        private static object _lock = new object();
        private static string _sessionToken;
        private static DateTime? _expiryTimestamp;

        public static string SessionToken
        {
            get
            {
                if (!_expiryTimestamp.HasValue || _expiryTimestamp.Value.AddHours(1) > DateTime.Now)
                {
                    lock(_lock)
                    {
                        var authenticationService = new AuthenticationService();
                        var sessionModel = authenticationService.GetSession(new CredentialModel { UserName = UserName, Password = Password });

                        _sessionToken = sessionModel.SessionToken;
                        _expiryTimestamp = sessionModel.ExpiryTimestamp;
                    }
                }

                return _sessionToken;
            }
        }
    }
}