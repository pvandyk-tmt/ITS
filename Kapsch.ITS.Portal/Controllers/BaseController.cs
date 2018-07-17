using Kapsch.ITS.Portal.Filters;
using Kapsch.ITS.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Kapsch.ITS.Portal.Controllers
{
    [FunctionAuthorize]
    [OutputCache(NoStore = true, Duration = 0, Location = OutputCacheLocation.None)]
    public class BaseController : Controller
    {
        public AuthenticatedUser AuthenticatedUser
        {
            get { return User as AuthenticatedUser; }
        }
    }
}