using Kapsch.Core.Data.Enums;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Correspondence
{
    public class ConfigurationManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ISettings GetSettings(CorrespondenceType correspondenceType, string correspondenceSubtype)
        {
            try
            {
                string provider;

                switch (correspondenceType)
                {
                    case CorrespondenceType.Email:
                        if (correspondenceSubtype.ToLower() == "mock")
                            provider = "mock";
                        else

                            provider = System.Configuration.ConfigurationManager.AppSettings["EmailProvider"];

                        switch (provider.ToLowerInvariant())
                        {
                            case "mock":
                                return new MockEmailSettings(correspondenceSubtype, TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["Email.Mock.Delay"]));
                            default:
                                Logger.Error("Unsupported email provider in configuration.");
                                return null;
                        }
                    case CorrespondenceType.Sms:

                        if (!string.IsNullOrWhiteSpace(correspondenceSubtype) && (correspondenceSubtype.ToLower() == "mock"))
                            provider = "mock";
                        else
                            provider = System.Configuration.ConfigurationManager.AppSettings["SmsProvider"];

                        switch (provider.ToLowerInvariant())
                        {
                            case "cm":
                                switch (correspondenceSubtype)
                                {
                                    default:
                                        return new CMSmsSettings(
                                            correspondenceSubtype,
                                            System.Configuration.ConfigurationManager.AppSettings["Sms.CM.BaseUrl"],
                                            Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["Sms.CM.ProductToken"]),
                                            1);
                                }
                            case "mock":
                                return new MockSmsSettings(correspondenceSubtype, TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["Sms.Mock.Delay"]));
                            default:
                                Logger.Error("Unsupported sms provider in configuration.");
                                return null;
                        }
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }

        }
    }
}
