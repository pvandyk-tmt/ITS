using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Authenticate;
using Kapsch.ITS.Web.Helpers;
using Kapsch.ITS.Web.Models.Notification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Web.Controllers
{
    public class NotificationController : Controller
    {
        public readonly static string ITSGatewayEndpoint = ConfigurationManager.AppSettings["ITSGatewayEndpoint"];

        public ActionResult VerifyOwner(string referenceNumber)
        {
            return View(new VerifyOwnerModel { ReferenceNumber = referenceNumber });
        }

        [HttpPost]
        public ActionResult VerifyOwner(VerifyOwnerModel model)
        {
            if (model.VerificationCode != "0000")
            {
                ModelState.AddModelError("VerificationCode", "Invalid verification code for the notice.");
                return View(model);
            }

            var authenticationService = new AuthenticationService();
            var sessionModel = authenticationService.GetSession(new CredentialModel { UserName = "Super User", Password = "Q!w2e3r4t5" });

            var noticeURL = string.Format("{0}api/Fine/Notice?sessionToken={1}&referenceNumber={2}", ITSGatewayEndpoint, sessionModel.SessionToken, model.ReferenceNumber);

            return ExternalGet(noticeURL);
        }

        /// <summary>
        /// Makes a GET request to a URL and returns the relayed result.
        /// </summary>
        private HttpWebResponseResult ExternalGet(string url)
        {
            var getRequest = (HttpWebRequest)WebRequest.Create(url);
            var getResponse = (HttpWebResponse)getRequest.GetResponse();

            return new HttpWebResponseResult(getResponse);
        }
    }
}