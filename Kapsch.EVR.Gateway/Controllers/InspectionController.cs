using Kapsch.Core.Data;
using Kapsch.EVR.Gateway.Models.Inspection;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.EVR.Gateway.Controllers
{
    [RoutePrefix("api/Inspection")]
    [SessionAuthorize]
    [UsageLog]
    public class InspectionController : BaseController
    {
        [SessionAuthorize]
        [HttpGet]
        [Route("QuestionSet")]
        [ResponseType(typeof(QuestionSetModel))]
        public IHttpActionResult GetQuestionSet(long setID)
        {
            using (var dbContext = new DataContext())
            {
                // TODO: Query db and build model

                return Ok(new QuestionSetModel());
            }
        }
    }
}
