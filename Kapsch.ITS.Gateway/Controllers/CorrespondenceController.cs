using Kapsch.Core;
using Kapsch.Core.Correspondence;
using Kapsch.Core.Data;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.ITS.Gateway.Models.Correspondence;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.ITS.Gateway.Controllers
{

    [RoutePrefix("api/Correspondence")]
    public class CorrespondenceController : BaseController
    {
        private static readonly Core.Data.Enums.Country Country = (Core.Data.Enums.Country)Enum.Parse(typeof(Core.Data.Enums.Country), System.Configuration.ConfigurationManager.AppSettings["Sms.Msisdn.Rules.Country"]);

        [HttpPost]
        [SessionAuthorize]
        [Route("NoticeSms")]
        [ResponseType(typeof(IList<SendResponseModel>))]
        public IHttpActionResult SendNoticeSms(IList<string> referenceNumbers)
        {
            using (var dataContext = new DataContext())
            {
                var response = new List<SendResponseModel>();
                var company = dataContext.Companies.FirstOrDefault(f => f.Name == "Intelligent Mobility Solutions");
                var corresponedenceTemplate = dataContext.CorrespondenceTemplates
                    .AsNoTracking()
                    .FirstOrDefault(f => f.Key == "FirstNotice" && f.CorrespondenceType == Core.Data.Enums.CorrespondenceType.Sms);

                foreach (var referenceNumber in referenceNumbers)
                {
                    var register = dataContext.Registers
                        .Include(f => f.Person)
                        .FirstOrDefault(f => f.ReferenceNumber == referenceNumber);

                    var person = register.Person;
                    if (person == null)
                    {
                        response.Add(new SendResponseModel { ReferenceNumber = referenceNumber, IsError = true, Error = "No person associated with the offence." });
                        continue;
                    }

                    var isValid = Msisdn.IsValid(person.MobileNumber, Country);
                    if (!isValid)
                    {
                        // TODO: feedback that Msisdn is invalid and cannot be trusted
                        response.Add(new SendResponseModel { ReferenceNumber = referenceNumber, IsError = true, Error = "Invalid Msisdn." });
                        continue;
                    }

                    var message = corresponedenceTemplate.Generate(new Dictionary<string, string> { { "referenceNumber", referenceNumber } });
                    Router router = new Router() { Source = "IMS", Target = new Msisdn(person.MobileNumber, Country).ToString(Msisdn.Format.International) };
                    SmsPayload payload = new SmsPayload("FirstNoticeSms", "CM", message);
                    Item.Initiate(dataContext, referenceNumber, company, person, router, payload, false);

                    dataContext.SaveChanges();

                    response.Add(new SendResponseModel { ReferenceNumber = referenceNumber, IsError = false });
                }

                return Ok(response);
            }
        }
    }
}
