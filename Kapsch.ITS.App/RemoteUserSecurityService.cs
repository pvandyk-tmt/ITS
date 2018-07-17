using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.ITS.App.Common;
using Kapsch.ITS.App.Common.Models;

namespace Kapsch.ITS.App
{
    public class RemoteUserSecurityService : IUserSecurityService
    {
        public AuthenticatedUser SignIn(string userName, string password)
        {
            var authenticationService = new AuthenticationService();
            var sessionModel = authenticationService.GetSession(new CredentialModel { UserName = userName, Password = password });
            var userService = new UserService(sessionModel.SessionToken);
            var userModel = userService.GetUser(sessionModel.EntityID);

            var authenticatedUser = new AuthenticatedUser(userName);
            authenticatedUser.SessionData = sessionModel;
            authenticatedUser.UserData = userModel;

            return authenticatedUser;
        }

        public void SignOut(string sessionToken)
        {
            try
            {
                var authenticationService = new AuthenticationService();
                authenticationService.RemoveSession(sessionToken);
            }
            catch
            {
                // Empty on purpose
            }
        }
    }
}
