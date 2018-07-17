using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace Kapsch.Core.Gateway.Helpers
{
    public class EmailHelper
    {
        public static readonly bool IsTestMode = bool.Parse(ConfigurationManager.AppSettings["IsTestMode"]);

        // Email Settings
        public static readonly string MailServer = ConfigurationManager.AppSettings["MailServer"];
        public static readonly int MailPort = int.Parse(ConfigurationManager.AppSettings["MailPort"]);
        public static readonly string ReportingFromEmail = ConfigurationManager.AppSettings["ReportingFromEmail"];
        public static readonly string ReportingCredential = ConfigurationManager.AppSettings["ReportingCredential"];
        //public static readonly string EmailAddressForAdmin = ConfigurationManager.AppSettings["EmailAddressForAdmin"];
        public static readonly int MaxEmailRetryAttemps = 3;
        public static readonly int WaitBetweenEmailRetryInMilliSeconds = 10 * 1000;

        public static void Send(string mailTemplatePath, IList<string> recipients, string subject, string templateName, IDictionary<string, string> personalizations)
        {

            //Read template file from the App_Data folder
            using (var sr = new StreamReader(Path.Combine(mailTemplatePath, templateName)))
            {
                string body = sr.ReadToEnd();

                foreach (var personalization in personalizations)
                {
                    body = body.Replace(string.Format("@{0}", personalization.Key), personalization.Value);
                }

                if (!IsTestMode)
                {
                    var email = new MailMessage();
                    var now = DateTime.Now;
                    email.From = new MailAddress(ReportingFromEmail);
                    foreach (string emailAddress in recipients)
                    {
                        email.To.Add(new MailAddress(emailAddress));
                    }

                    email.Subject = subject;
                    email.Body = body;
                    email.IsBodyHtml = true;

                    using (var client = new SmtpClient())
                    {
                        client.Host = MailServer;
                        client.Port = MailPort;
                        client.Credentials = new NetworkCredential(ReportingFromEmail.Split(new char[] { '@' })[0], ReportingCredential);
                        for (int i = 0; i < MaxEmailRetryAttemps; ++i)
                        {
                            try
                            {
                                client.Send(email);
                                break;
                            }
                            catch (Exception)
                            {
                                Thread.Sleep(WaitBetweenEmailRetryInMilliSeconds);
                            }
                        }
                    }
                }
            }
        }
    }
}