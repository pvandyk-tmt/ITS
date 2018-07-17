using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Kapsch.Core.Data;
using Kapsch.Core.Extensions;
using Kapsch.Core.Filters;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.RTE.Data;
using Kapsch.RTE.Gateway.Hubs;
using Kapsch.RTE.Gateway.Models.Camera;
using Newtonsoft.Json;
using DataContext = Kapsch.RTE.Data.DataContext;
using FilterJoin = Kapsch.Gateway.Models.Shared.Enums.FilterJoin;
using Microsoft.AspNet.SignalR.Client;

namespace Kapsch.RTE.Gateway.Controllers
{
    [RoutePrefix("api/OverSection")]
    public class OverSectionController : BaseController
    {
        public static readonly string SignalRHubUrl = ConfigurationManager.AppSettings["SignalRHubUrl"];

        /// <summary>
        /// Posts the data logged for an infringement logged over a section of road
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult Post([FromBody] OverSectionModel model)
        {
            try
            {
                PushSignalR(model);

                return Ok(true);
            }
            catch (Exception ex)
            {
                //return this.TMTBadRequest(TMTErrorBase.PopulateUnexpectedException(ex));
                return Ok(false);
            }
        }

        private static void PushSignalR(OverSectionModel model)
        {
            if (!model.IsOffence)
            {
                return;
            }

            string json = JsonConvert.SerializeObject(model);

            using (var hubConnection = new HubConnection(SignalRHubUrl))
            {
                var hubProxy = hubConnection.CreateHubProxy("DotNotificationHub");

                hubConnection.Start().Wait();

                hubProxy.Invoke("SendOverSectionInfringement", model.SectionCode, json);
            }
        }   
    }
}