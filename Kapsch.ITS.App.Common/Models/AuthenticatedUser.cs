using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.Core.Gateway.Models.User;
using Kapsch.Core.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.App.Common.Models
{
    public class AuthenticatedUser : IPrincipal
    {
        public AuthenticatedUser(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            if (UserData == null || UserData.SystemFunctions == null)
                return false;

            return UserData.SystemFunctions.Any(f => f.Description == role);
        }

        public SessionModel SessionData { get; set; }
        public UserModel UserData { get; set; }
        public string SessionToken { get { return SessionData.SessionToken; } }
        public IIdentity Identity { get; set; }
        public IList<ReportMetaDataModel> ReportData { get; set; }
    }
}
