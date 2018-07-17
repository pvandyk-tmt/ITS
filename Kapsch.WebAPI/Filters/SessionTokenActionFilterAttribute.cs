using System;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Data.Entity;
using Kapsch.Core.Caching;
using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.Core.Data.Enums;

namespace Kapsch.Gateway.Shared.Filters
{
    public class SessionTokenActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            if (!actionContext.ActionArguments.ContainsKey("sessionToken"))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            var sessionToken = actionContext.ActionArguments["sessionToken"] as string;
            var sessionStore = new MemoryCache<SessionModel>("USER_SESSION");
            var sessionModel = sessionStore.Get(sessionToken);
            if (sessionModel == null)
            {
                using (var dbContext = new DataContext())
                {
                    var session = dbContext.Sessions.Include(f => f.Credential).SingleOrDefault(f => f.Token == sessionToken);
                    if (session == null || session.ExpiryTimestamp < DateTime.UtcNow)
                    {
                        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                        return;
                    }

                    sessionModel = new SessionModel();
                    sessionModel.CredentialID = session.CredentialID;
                    sessionModel.UserName = session.Credential.UserName;
                    sessionModel.EntityID = session.Credential.EntityID;
                    sessionModel.EntityType = (Core.Gateway.Models.Enums.EntityType)session.Credential.EntityType;
                    sessionModel.SessionToken = session.Token;
                    sessionModel.ExpiryTimestamp = session.ExpiryTimestamp;
                    sessionModel.CreatedTimestamp = session.CreatedTimestamp;

                    sessionStore.Set(sessionModel.SessionToken, sessionModel, 8 * 60);
                }
            }

            if (RequiredEntityTypes != null && !RequiredEntityTypes.Contains((EntityType)sessionModel.EntityType))
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            var baseController = actionContext.ControllerContext.Controller as BaseController;
            if (baseController == null)
                return;

            baseController.SessionModel = sessionModel;
        }

        public EntityType[] RequiredEntityTypes { get; set; }
    }
}