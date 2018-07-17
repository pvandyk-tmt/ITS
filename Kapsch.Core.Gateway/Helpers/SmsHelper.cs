using Kapsch.Core.Correspondence;
using Kapsch.Core.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kapsch.Core.Gateway.Helpers
{
    public class SmsHelper
    {
        private static readonly Data.Enums.Country Country = (Data.Enums.Country)Enum.Parse(typeof(Data.Enums.Country), System.Configuration.ConfigurationManager.AppSettings["Sms.Msisdn.Rules.Country"]);

        public static void Send(DataContext dbContext, string context, string subContext, Router router, Company company, User user, string smsTemplatePath, string templateName, IDictionary<string, string> personalizations)
        {
            //Read template file from the App_Data folder
            using (var sr = new StreamReader(Path.Combine(smsTemplatePath, templateName)))
            {
                string body = sr.ReadToEnd();

                foreach (var personalization in personalizations)
                {
                    body = body.Replace(string.Format("@{0}", personalization.Key), personalization.Value);
                }

                var payload = new SmsPayload(context, subContext, body);
                Item.Initiate(dbContext, Guid.NewGuid().ToString(), company, user, router, payload, true);
            }         
        }
    }
}