using Kapsch.Core.Data;
using Kapsch.Core.Data.Enums;
using System;

namespace Kapsch.Core.Correspondence
{
    public class Router
    {
        public static Data.Enums.Country Country = (Data.Enums.Country)Enum.Parse(typeof(Data.Enums.Country), System.Configuration.ConfigurationManager.AppSettings["Sms.Msisdn.Rules.Country"]);

        public static bool CanRoute(ICorrespondent source, ICorrespondent target, IPayload payload)
        {
            if (source == null)
                throw new Exception("source argument can not be null.");

            if (target == null)
                throw new Exception("target argument can not be null.");

            if (payload == null)
                throw new Exception("payload argument can not be null.");

            switch (payload.CorrespondenceType)
            {
                case CorrespondenceType.Email:
                    if (!(payload is EmailPayload))
                        throw new Exception("Payload is not of type EmailPayload.");

                    return GetEmailRoute(source, target) != null;
                case CorrespondenceType.Sms:
                    if (!(payload is SmsPayload))
                        throw new Exception("Payload is not of type SmsPayload.");

                    return GetSmsRoute(source, target) != null;
                default:
                    throw new NotImplementedException();
            }
        }

        public static Router GetRoute(ICorrespondent source, ICorrespondent target, IPayload payload)
        {
            if (source == null)
                throw new Exception("source argument can not be null.");

            if (target == null)
                throw new Exception("target argument can not be null.");

            if (payload == null)
                throw new Exception("payload argument can not be null.");

            switch (payload.CorrespondenceType)
            {
                case CorrespondenceType.Email:
                    if (!(payload is EmailPayload))
                        throw new Exception("Payload is not of type EmailPayload.");

                    return GetEmailRoute(source, target);
                case CorrespondenceType.Sms:
                    if (!(payload is SmsPayload))
                        throw new Exception("Payload is not of type SmsPayload.");

                    return GetSmsRoute(source, target);
                default:
                    throw new NotImplementedException();
            }
        }

        public static Router GetRoute(ICorrespondent source, ICorrespondent target, CorrespondenceType correspondenceType)
        {
            if (source == null)
                throw new Exception("source argument can not be null.");

            if (target == null)
                throw new Exception("target argument can not be null.");

            switch (correspondenceType)
            {
                case CorrespondenceType.Email:
                    return GetEmailRoute(source, target);

                case CorrespondenceType.Sms:
                    return GetSmsRoute(source, target);

                default:
                    throw new NotImplementedException();
            }
        }

        private static Router GetEmailRoute(ICorrespondent source, ICorrespondent target)
        {
            var sourceEmailAddress = source.Email;
            if (string.IsNullOrWhiteSpace(sourceEmailAddress))
                return null;

            var targetEmailAddress = target.Email;
            if (string.IsNullOrWhiteSpace(targetEmailAddress))
                return null;

            return new Router { Source = sourceEmailAddress, Target = targetEmailAddress };
        }

        private static Router GetSmsRoute(ICorrespondent source, ICorrespondent target)
        {
            var sourceMobileNumber = source.MobileNumber;
            if (string.IsNullOrWhiteSpace(sourceMobileNumber))
            {
                // For sms the source is not that important
                sourceMobileNumber = "0";
            }
                
            var targetMobileNumber = target.MobileNumber;
            if (!Msisdn.IsValid(targetMobileNumber, Country))
                return null;

            var msisdn = new Msisdn(targetMobileNumber, Country);          

            return new Router { Source = sourceMobileNumber, Target = msisdn.ToString(Msisdn.Format.International) };
        }

        public string Source { get; set; }
        public string Target { get; set; }
    }
}
