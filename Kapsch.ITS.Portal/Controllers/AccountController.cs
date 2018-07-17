using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.Core.Gateway.Models.User;
using Kapsch.Gateway.Models.Shared;
using Kapsch.ITS.Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Kapsch.ITS.Portal.Controllers
{
    public class AccountController : Controller
    {
        private const int UserCacheTimeMinutes = 8 * 60;

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            //return View(new CredentialModel { UserName = "Super User", Password = "Q!w2e3r4t5" });
            return View(new CredentialModel());
        }

        [HttpPost]
        public ActionResult Login(CredentialModel model, string returnUrl)
        {
            if (TempData.ContainsKey("searchFinesModel"))
                TempData.Remove("searchFinesModel");

            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var authenticationService = new AuthenticationService();
                var sessionModel = authenticationService.GetSession(model);
                var userService = new UserService(sessionModel.SessionToken);
                var userModel = userService.GetUser(sessionModel.EntityID);
                
                var authenticatedUser = new AuthenticatedUser(model.UserName);
                authenticatedUser.SessionData = sessionModel;
                authenticatedUser.UserData = userModel;

                var reportService = new ReportService(authenticatedUser.SessionToken);
                var reportMetData = reportService.GetMetaData();
                reportMetData.ApplyFilterAndOrder(authenticatedUser.UserData.SystemFunctions.Select(f => f.Name).ToArray());
                authenticatedUser.ReportData = reportMetData;

                var ticket = new FormsAuthenticationTicket(1, authenticatedUser.SessionToken, DateTime.Now, DateTime.Now.AddMinutes(UserCacheTimeMinutes), false, authenticatedUser.SessionToken);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                var cookie = FormsAuthentication.GetAuthCookie(FormsAuthentication.FormsCookieName, true);
                cookie.Value = encTicket;
                HttpContext.Response.Cookies.Add(cookie);

                if (MvcApplication.UserCache.Get(authenticatedUser.SessionToken) == null)
                {
                    MvcApplication.UserCache.Set(
                        authenticatedUser.SessionToken,
                        authenticatedUser,
                        new CacheItemPolicy
                        {
                            AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(UserCacheTimeMinutes)
                        });
                }

                return RedirectToLocal(authenticatedUser, returnUrl);              
            }
            catch (GatewayException ge)
            {
                ModelState.AddModelError(string.Empty, ge.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            var authenticatedUser = User as AuthenticatedUser;

            if (authenticatedUser != null)
            {
                try
                {
                    var authenticationService = new AuthenticationService();
                    authenticationService.RemoveSession(authenticatedUser.SessionToken);
                }
                catch
                {
                    // Empty on purpose
                }

                MvcApplication.UserCache.Remove(authenticatedUser.SessionToken);
                FormsAuthentication.SignOut();
            }

            return RedirectToAction("Login");
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userService = new UserService();
                userService.ResetPassword(model);

                return RedirectToAction("ConfirmResetPassword");             
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public ActionResult ConfirmResetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            try
            {
                var user = User as AuthenticatedUser;
                var userService = new UserService(user.SessionData.SessionToken);
                userService.ChangePassword(model);

                return Json(new { IsValid = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ChangePasswordWithToken(string token)
        {
            return View(new ChangePasswordWithTokenModel { Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePasswordWithToken(ChangePasswordWithTokenModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userService = new UserService();
                userService.ChangePasswordWithToken(model);

                return RedirectToAction("ConfirmChangePasswordWithToken");               
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public ActionResult ConfirmChangePasswordWithToken()
        {
            return View();
        }

        private ActionResult RedirectToLocal(AuthenticatedUser authenticatedUser, string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }
    }
}