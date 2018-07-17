using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Models.Authenticate;
using System.Web.Http;

namespace Kapsch.Gateway.Shared
{
    public class BaseController : ApiController
    {
        public SessionModel SessionModel { get; set; }
    }
}