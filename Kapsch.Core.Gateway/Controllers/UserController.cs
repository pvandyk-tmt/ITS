using Kapsch.Core.Data;
using Kapsch.Core.Filters;
using Kapsch.Core.Gateway.Helpers;
using Kapsch.Core.Gateway.Models.User;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using Kapsch.Core.Gateway.Models;
using Kapsch.Core.Extensions;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.Core.Gateway.Models.Configuration;
using System.Configuration;
using System.Diagnostics;
using Kapsch.Core.Correspondence;
using System.Text.RegularExpressions;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/User")]
    public class UserController : BaseController
    {
        private static readonly string UserManagementPortal = System.Configuration.ConfigurationManager.AppSettings["UserManagementPortal"];
        private static readonly Data.Enums.Country Country = (Data.Enums.Country)Enum.Parse(typeof(Data.Enums.Country), System.Configuration.ConfigurationManager.AppSettings["Sms.Msisdn.Rules.Country"]);

        [HttpPost]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Post([FromBody] UserModel model)
        {
            if (!Msisdn.IsValid(model.MobileNumber, Country))
            {
                return this.BadRequestEx(Error.MobileNumberInvalid);
            }

            using (var dbContext = new DataContext())
            {
                var user = new User();               
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.MobileNumber = model.MobileNumber;
                user.Status = Data.Enums.Status.Active;
                user.CreatedTimestamp = DateTime.Now;
                user.IsOfficer = model.IsOfficer ? "1" : "0";
                user.ExternalID = model.ExternalID;

                dbContext.Users.Add(user);

                if (model.Districts != null)
                {
                    foreach (var districtModel in model.Districts)
                    {
                        var district = dbContext.Districts.Find(districtModel.ID);
                        if (district == null)
                            continue;

                        var userDistrict = new UserDistrict();
                        userDistrict.District = district;
                        userDistrict.User = user;

                        dbContext.UserDistricts.Add(userDistrict);
                    }
                }
                
                var userName = Kapsch.Core.Cryptography.Random.GenerateConcatenatedString(model.FirstName.Substring(0, 1), model.LastName);
                while (true)
                {
                    if (!dbContext.Credentials.Any(f => f.UserName == userName))
                        break;

                    userName = Kapsch.Core.Cryptography.Random.GenerateConcatenatedString(model.FirstName.Substring(0, 1), model.LastName);
                }

                Random random = new Random();

                var credential = new Credential();
                credential.CreatedTimeStamp = DateTime.Now;
                credential.EntityID = user.ID;
                credential.EntityType = Data.Enums.EntityType.User;
                credential.ExpiryTimeStamp = DateTime.Now.AddYears(20);
                credential.Status = Data.Enums.Status.Active;
                credential.UserName = userName;

                credential.Password = Membership.GeneratePassword(8, 0);
                credential.Password = Regex.Replace(credential.Password, @"[^a-zA-Z0-9]", m => random.Next(0, 9).ToString());

                dbContext.Credentials.Add(credential);

                if (model.SystemFunctions != null)
                {
                    foreach (var systemFunctionModel in model.SystemFunctions)
                    {
                        var systemFunction = dbContext.SystemFunctions.Find(systemFunctionModel.ID);
                        if (systemFunction == null)
                            continue;

                        var userSystemFunction = new CredentialSystemFunction();
                        userSystemFunction.SystemFunction = systemFunction;
                        userSystemFunction.Credential = credential;
                        userSystemFunction.Status = Data.Enums.Status.Active;

                        dbContext.CredentialSystemFunctions.Add(userSystemFunction);
                    }
                }

                dbContext.SaveChanges();

                var logo = string.Format("{0}/Images/IMS-logo-180x66-1color.png", UserManagementPortal);
                
                var personalizations = new Dictionary<string, string>();
                personalizations.Add("website", UserManagementPortal);
                personalizations.Add("logo", logo);
                personalizations.Add("fullName", string.Format("{0} {1}", user.FirstName, user.LastName));
                personalizations.Add("userName", credential.UserName);
                personalizations.Add("password", credential.Password);

                //EmailHelper.Send(
                //    HttpContext.Current.Server.MapPath("~/MailTemplates"),
                //    new[] { model.Email },
                //    "Account Created",
                //    "AccountCreated.txt",
                //    personalizations);
                var company = dbContext.Companies.FirstOrDefault(f => f.Name == "Intelligent Mobility Solutions"); // IMS
                if (company == null)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Unable to get company, Intelligent Mobility Solutions, from database."));
                }
                else
                {
                    SmsHelper.Send(
                        dbContext,
                        "User Management",
                        "Create User",
                        new Router() { Source = "IMS", Target = new Msisdn(user.MobileNumber, Country).ToString(Msisdn.Format.International) },
                        company,
                        user,
                        HttpContext.Current.Server.MapPath("~/MailTemplates"),
                        "SmsAccountCreated.txt",
                        personalizations);
                }

                model.ID = user.ID;
                model.UserName = credential.UserName;
                model.Status = (Models.Enums.UserStatus) user.Status;
                model.CreatedTimestamp = user.CreatedTimestamp;

                return Ok(model);
            }           
        }

        [HttpPut]
        [ValidationActionFilter]
        [SessionAuthorize]
        [UsageLog]
        public IHttpActionResult Put([FromBody] UserModel model)
        {
            if (!Msisdn.IsValid(model.MobileNumber, Country))
            {
                return this.BadRequestEx(Error.MobileNumberInvalid);
            }

            using (var dbContext = new DataContext())
            {
                var credential = dbContext.Credentials
                    .Include(f => f.User)
                    .Include(f => f.CredentialSystemFunctions)
                    .SingleOrDefault(f => f.EntityID == model.ID && f.EntityType == Data.Enums.EntityType.User);
                if (credential == null)
                    return this.BadRequestEx(Error.UserDoesNotExist);

                User user = credential.User;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.MobileNumber = model.MobileNumber;
                user.Status = (Data.Enums.Status)model.Status;
                user.IsOfficer = model.IsOfficer ? "1" : "0";
                user.ExternalID = model.ExternalID;

                credential.CredentialSystemFunctions.ToList().ForEach(f => dbContext.CredentialSystemFunctions.Remove(f));
                foreach (var systemFunctionModel in model.SystemFunctions)
                {
                    var systemFunction = dbContext.SystemFunctions.Find(systemFunctionModel.ID);
                    if (systemFunction == null)
                        continue;

                    var userSystemFunction = new CredentialSystemFunction();
                    userSystemFunction.SystemFunction = systemFunction;
                    userSystemFunction.Credential = credential;
                    userSystemFunction.Status = Data.Enums.Status.Active;

                    dbContext.CredentialSystemFunctions.Add(userSystemFunction);
                }

                user.UserDistricts.ToList().ForEach(f => dbContext.UserDistricts.Remove(f));
                foreach (var districtModel in model.Districts)
                {
                    var district = dbContext.Districts.Find(districtModel.ID);
                    if (district == null)
                        continue;

                    var userDistrict = new UserDistrict();
                    userDistrict.District = district;
                    userDistrict.User = user;

                    dbContext.UserDistricts.Add(userDistrict);
                }

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [SessionAuthorize]
        [UsageLog]
        [Route("PaginatedList")]
        [ResponseType(typeof(PaginationListModel<UserModel>))]
        public IHttpActionResult GetPaginatedList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {         
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.User>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .Users
                    .AsNoTracking();
                
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ? 
                    query.OrderByMember(PropertyHelper.GetSortingValue<UserModel>(orderPropertyName)) : 
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<UserModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<User>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<UserModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                
                var userIDs = entities.Select(f => f.ID).ToList();
                var credentials = dbContext.Credentials
                    .Where(f => f.EntityType == Data.Enums.EntityType.User && userIDs.Contains( f.EntityID))
                    .ToList();

                var models = new List<UserModel>(); 
                foreach (var entity in entities)
                {
                    var userModel = 
                        new UserModel
                        {                       
                            ID = entity.ID,
                            FirstName = entity.FirstName,
                            LastName = entity.LastName,
                            MobileNumber = entity.MobileNumber,
                            CreatedTimestamp = entity.CreatedTimestamp,
                            Status = (Models.Enums.UserStatus)entity.Status,
                            Email = entity.Email,
                            IsOfficer = entity.IsOfficer == "1",
                            ExternalID = entity.ExternalID
                        };

                    var credential = credentials.FirstOrDefault(f => f.EntityID == userModel.ID);
                    if (credential != null)
                    {
                        userModel.UserName = credential.UserName;
                    }

                    models.Add(userModel);
                }
                paginationList.Models = models;
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpGet]
        [SessionAuthorize]
        [Route("District")]
        [ResponseType(typeof(IList<UserModel>))]
        public IHttpActionResult GetUserByDistricts(long districtID)
        {
            using (var dbContext = new DataContext())
            {
                var userInfos = (
                                from user in dbContext.Users                        
                                join credential in dbContext.Credentials on user.ID equals credential.EntityID
                                join userDistrict in dbContext.UserDistricts on user.ID equals userDistrict.UserID
                                where
                                    user.Status == Data.Enums.Status.Active &&
                                    credential.EntityType == Core.Data.Enums.EntityType.User &&
                                    credential.Status == Data.Enums.Status.Active &&
                                    userDistrict.DistrictID == districtID
                                select new { User = user, Credential = credential }
                            )
                            .GroupBy(f => new { User = f.User, Credential = f.Credential })
                            .ToList();

                var models = userInfos.Select(f =>
                    new UserModel
                    {
                        ID = f.Key.User.ID,
                        FirstName = f.Key.User.FirstName,
                        LastName = f.Key.User.LastName,
                        MobileNumber = f.Key.User.MobileNumber,
                        CreatedTimestamp = f.Key.User.CreatedTimestamp,
                        Status = (Core.Gateway.Models.Enums.UserStatus)f.Key.User.Status,
                        Email = f.Key.User.Email,
                        IsOfficer = f.Key.User.IsOfficer == "1",
                        ExternalID = f.Key.User.ExternalID,
                        UserName = f.Key.Credential.UserName,
                        CredentialID = f.Key.Credential.ID
                    });

                return Ok(models);
            }
        }

        [HttpGet]
        [SessionAuthorize]
        [UsageLog]     
        [ResponseType(typeof(UserModel))]
        public IHttpActionResult Get(long id)
        {
            using (var dbContext = new DataContext())
            {
                var credential = dbContext.Credentials
                    .AsNoTracking()
                    .Include(f => f.User)
                    .Include(f => f.User.UserDistricts)
                    .Include(f => f.CredentialSystemFunctions)
                    .SingleOrDefault(f => f.EntityID == id && f.EntityType == Data.Enums.EntityType.User);
                if (credential == null)
                    return this.BadRequestEx(Error.UserDoesNotExist);

                var user = credential.User;

                if (user == null)
                    return this.BadRequestEx(Error.UserDoesNotExist);
             
                var model = new UserModel();
                model.UserName = credential == null ? string.Empty : credential.UserName;
                model.ID = user.ID;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Email = user.Email;
                model.MobileNumber = user.MobileNumber;
                model.CreatedTimestamp = user.CreatedTimestamp;
                model.Status = (Models.Enums.UserStatus)user.Status;
                model.IsOfficer = user.IsOfficer == "1";
                model.ExternalID = user.ExternalID;
                model.SystemFunctions = credential.CredentialSystemFunctions
                    .Where(f => f.Status == Data.Enums.Status.Active)
                    .Select(f => new SystemFunctionModel { ID = f.SystemFunction.ID, Name = f.SystemFunction.Name, Description = f.SystemFunction.Description })
                    .ToList();
                model.Districts = user.UserDistricts
                    .Select(f => new DistrictModel { ID = f.District.ID, BranchName = f.District.BranchName })
                    .ToList();

                return Ok(model);
            }
        }

        [ValidationActionFilter]
        [SessionAuthorize]
        [HttpPut]
        [Route("Password")]
        public IHttpActionResult ChangePassword([FromBody] ChangePasswordModel model)
        {
            using (var dbContext = new DataContext())
            {
                var credential = dbContext.Credentials.SingleOrDefault(f => f.UserName == SessionModel.UserName);
                if (credential == null)
                    return this.BadRequestEx(Error.CredentialNotFound);

                if (credential.EntityType != Data.Enums.EntityType.Company && credential.EntityType != Data.Enums.EntityType.User)
                    return this.BadRequestEx(Error.CredentialInvalidEntityType);

                if (model.CurrentPassword != credential.Password)
                    return BadRequest(Error.PasswordIncorrect);

                credential.Password = model.NewPassword;
                dbContext.SaveChanges();

                return Ok();
            }
        }

        [ValidationActionFilter]
        [UsageLog]
        [HttpPut]
        [Route("PasswordWithToken")]
        public IHttpActionResult ChangePasswordWithToken([FromBody] ChangePasswordWithTokenModel model)
        {
            using (var dbContext = new DataContext())
            {
                var credentialResetToken = dbContext.CredentialResetTokens.SingleOrDefault(f => f.Token == model.Token);
                if (credentialResetToken == null)
                    return this.BadRequestEx(Error.TokenDoesNotExist);

                if (credentialResetToken.ExpiryTimestamp <= DateTime.Now)
                    return this.BadRequestEx(Error.TokenExpired);

                var credential = credentialResetToken.Credential;
                if (credential.EntityType != Data.Enums.EntityType.Company && credential.EntityType != Data.Enums.EntityType.User)
                    return this.BadRequestEx(Error.CredentialInvalidEntityType);

                credential.Password = model.NewPassword;

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [ValidationActionFilter]
        [UsageLog]
        [HttpPut]
        [Route("ResetPassword")]
        public IHttpActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            using (var dbContext = new DataContext())
            {
                var credential = dbContext.Credentials.SingleOrDefault(f => f.UserName == model.UserName);
                if (credential == null)
                    return this.BadRequestEx(Error.CredentialNotFound);

                if (credential.EntityType != Data.Enums.EntityType.Company && credential.EntityType != Data.Enums.EntityType.User)
                    return this.BadRequestEx(Error.CredentialInvalidEntityType);

                var credentialResetToken = new CredentialResetToken();
                credentialResetToken.CredentialID = credential.ID;
                credentialResetToken.Token = Guid.NewGuid().ToString();
                credentialResetToken.ExpiryTimestamp = DateTime.Now.AddHours(24);

                dbContext.CredentialResetTokens.Add(credentialResetToken);
                dbContext.SaveChanges();

                model.Token = credentialResetToken.Token;
                model.UserName = credential.UserName;

                var email = string.Empty;
                if (credential.EntityType == Data.Enums.EntityType.User)
                {
                    var user = dbContext.Users.Find(credential.EntityID);
                    email = user.Email;
                } 
                else if (credential.EntityType == Data.Enums.EntityType.Company)
                {

                }

                var logo = string.Format("{0}/Images/IMS-logo-180x66-1color.png", UserManagementPortal);
                var link = string.Format("{0}/Account/ChangePasswordWithToken?token={1}", UserManagementPortal, credentialResetToken.Token);

                var personalizations = new Dictionary<string, string>();
                personalizations.Add("website", UserManagementPortal);
                personalizations.Add("logo", logo);
                personalizations.Add("link", link);

                EmailHelper.Send(
                    HttpContext.Current.Server.MapPath("~/MailTemplates"), 
                    new[] { email }, 
                    "Reset Password", 
                    "ResetPassword.txt", 
                    personalizations);
                
                return Ok();
            }


        }
    }
}
