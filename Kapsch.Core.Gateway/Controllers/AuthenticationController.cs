using Kapsch.Core.Caching;
using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using Kapsch.Core.Gateway.Models.Authenticate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Core.Cryptography;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/Authentication")]
    [UsageLog]
    public class AuthenticationController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(SessionModel))]
        public IHttpActionResult Post([FromBody] CredentialModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                return this.BadRequestEx(Error.PopulateInvalidParameter("userName", "Can not be empty."));
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return this.BadRequestEx(Error.PopulateInvalidParameter("password", "Can not be empty."));
            }

            var userSession = new Session();

            using (var dbContext = new DataContext())
            {
                var credential = dbContext.Credentials.FirstOrDefault(f => f.UserName == model.UserName);
                if (credential == null)
                    return this.BadRequestEx(Error.CredentialNotFound);

                if ((credential.Password != model.Password) && (model.Password.ToUpper() != MessageDigest.HashSHA256(credential.Password)))
                    return this.BadRequestEx(Error.PasswordIncorrect);

                if (credential.Status != Status.Active)
                    return this.BadRequestEx(Error.CredentialNotActive);

                if (credential.ExpiryTimeStamp < DateTime.Now)
                    return this.BadRequestEx(Error.PasswordHasExpired);

                var session = new Session();
                session.Credential = credential;
                session.Token = Guid.NewGuid().ToString();
                session.CreatedTimestamp = DateTime.Now;
                session.ExpiryTimestamp = DateTime.Now.AddHours(8);

                dbContext.Sessions.Add(session);
                dbContext.SaveChanges();

                SessionModel sessionModel = new SessionModel();
                sessionModel.CredentialID = credential.ID;
                sessionModel.UserName = credential.UserName;
                sessionModel.EntityID = credential.EntityID;
                sessionModel.EntityType = (Models.Enums.EntityType)credential.EntityType;
                sessionModel.SessionToken = session.Token;
                sessionModel.ExpiryTimestamp = session.ExpiryTimestamp;
                sessionModel.CreatedTimestamp = session.CreatedTimestamp;

                var sessionStore = new MemoryCache<SessionModel>("USER_SESSION");
                sessionStore.Set(sessionModel.SessionToken, sessionModel, 8 * 60);
               
                return Ok(sessionModel);
            }
        }

        //[SessionAuthorize(RequiredEntityTypes = new [] { EntityType.InternalUser})]
        public IHttpActionResult Delete(string sessionToken)
        {
            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                return this.BadRequestEx(Error.PopulateInvalidParameter("sessionToken", "Can not be empty."));
            }

            using (var dbContext = new DataContext())
            {
                var userSession = dbContext.Sessions.SingleOrDefault(f => f.Token == sessionToken);
                if (userSession != null && userSession.ExpiryTimestamp < DateTime.Now)
                {
                    userSession.ExpiryTimestamp = DateTime.Now;
                    dbContext.SaveChanges();
                }

                var sessionStore = new MemoryCache<SessionModel>("USER_SESSION");
                sessionStore.Remove(sessionToken);

                return Ok();
            }
        }
    }
}
