using Kapsch.Core;
using Kapsch.Core.Correspondence;
using Kapsch.Core.Data;
using log4net;
using System;
using System.Data.Entity;
using System.Linq;

namespace Kapsch.ITS.Correspondence.Console
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Core.Data.Enums.Country Country = (Core.Data.Enums.Country)Enum.Parse(typeof(Core.Data.Enums.Country), System.Configuration.ConfigurationManager.AppSettings["Sms.Msisdn.Rules.Country"]);

        static void Main(string[] args)
        {
            using (var dataContext = new DataContext())
            {
                var t = dataContext.EvidenceLogs
                    .Include(f => f.InfringementEvidences)
                    .ToList();

                //dataContext.Database.Log = (s) => System.Diagnostics.Debug.WriteLine(s);
                Company company = dataContext.Companies.Find(1);
                User user = dataContext.Users.Find(2);

                //var targetMobileNumber = "095 080 5791";
                //var targetMobileNumber = "096 213 8481";
                var targetMobileNumber = "0845766144";
                var isValid = Msisdn.IsValid(targetMobileNumber, Country);
                   
                var msisdn = new Msisdn(targetMobileNumber, Country);

                Router router = new Router() { Source = "IMS", Target = msisdn.ToString(Msisdn.Format.International) };
                SmsPayload payload = new SmsPayload("FirtNotice", "CM", "Traffic Offence. Download at http://www.ims.africa/Notification/Verify?refNo=123456789");
                Item.Initiate(dataContext, "internal reference", company, user, router, payload, false);
            }

            System.Console.ReadKey();
        }
    }
}
