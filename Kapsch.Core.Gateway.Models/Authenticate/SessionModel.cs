using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Authenticate
{
    public class SessionModel
    {
        public SessionModel()
        {
            TimeZoneID = "South Africa Standard Time";
        }

        public long CredentialID { get; set; }
        public string UserName { get; set; }
        public string SessionToken { get; set; }
        public long EntityID { get; set; }
        public EntityType EntityType { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime ExpiryTimestamp { get; set; }
        public string TimeZoneID { get; set; }
    }
}
