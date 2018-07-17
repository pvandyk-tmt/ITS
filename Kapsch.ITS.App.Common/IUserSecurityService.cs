using Kapsch.ITS.App.Common.Models;

namespace Kapsch.ITS.App.Common
{
    public interface IUserSecurityService
    {
        AuthenticatedUser SignIn(string userName, string password);
        void SignOut(string sessionToken);
    }
}
