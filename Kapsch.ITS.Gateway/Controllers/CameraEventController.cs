using Kapsch.ITS.Gateway.Models.CameraEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/CameraEvent")]
    public class CameraEventController : ApiController
    {
        [HttpPost]
        [Route("Event")]
        [ResponseType(typeof(EventModel))]
        public IHttpActionResult Post([FromBody] EventModel model)
        {
            return Ok();
        }

        [HttpPost]
        [Route("EncStatistic")]
        [ResponseType(typeof(EncStatisticModel))]
        public IHttpActionResult PostEncStatistic([FromBody] EncStatisticModel model)
        {
            return Ok();
        }

        [HttpPost]
        [Route("EventMedia")]
        [ResponseType(typeof(EventMediaModel))]
        public IHttpActionResult PostMedia([FromBody] EventMediaModel model)
        {
            return Ok();
        }
    }
}
