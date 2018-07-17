using System.Net;
using System.Linq;
using System;
using System.Data.Entity;
using System.Web.Http;
using System.Net.Http;
using Kapsch.Core.Data.Enums;
using Kapsch.Core.Caching;
using Kapsch.Core.Data;
using Kapsch.Core.Gateway.Models.Authenticate;

namespace Kapsch.Gateway.Shared.Filters
{
    public class SessionAuthorizeAttribute : AuthorizeAttribute
    {      
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var authorizationHeader = actionContext.Request.Headers.Authorization;
            if (authorizationHeader == null || authorizationHeader.Scheme != "SessionToken")
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                return;
            }

            var sessionStore = new MemoryCache<SessionModel>("USER_SESSION");
            var sessionModel = sessionStore.Get(authorizationHeader.Parameter);
            if (sessionModel == null)
            {
                using (var dbContext = new DataContext())
                {
                    var session = dbContext.Sessions.Include(f => f.Credential).SingleOrDefault(f => f.Token == authorizationHeader.Parameter);
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