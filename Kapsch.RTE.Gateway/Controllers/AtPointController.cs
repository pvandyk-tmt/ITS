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
using Microsoft.AspNet.SignalR.Client;

namespace Kapsch.RTE.Gateway.Controllers
{
    [RoutePrefix("api/AtPoint")]
    public class AtPointController : BaseController
    {
        public static readonly string SignalRHubUrl = ConfigurationManager.AppSettings["SignalRHubUrl"];

        [SessionAuthorize]
        [HttpPost]
        [Route("PaginatedList")]
        [ResponseType(typeof(PaginationListModel<AtPointModel>))]
        public IHttpActionResult GetPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new Kapsch.RTE.Data.DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<CameraPointData>(filter, (Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .CameraPointsData
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = 
                    asc ? 
                        query.OrderByMember(PropertyHelper.GetSortingValue<CameraPointData>(orderPropertyName)) : 
                        query.OrderByMemberDescending(PropertyHelper.GetSortingValue<CameraPointData>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<CameraPointData>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);

                var paginationList = new PaginationListModel<AtPointModel>();

                foreach (CameraPointData cameraPointData in entities)
                {
                    AtPointModel model = JsonConvert.DeserializeObject<AtPointModel>(cameraPointData.Json);
                    paginationList.Models.Add(model);
                }

                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        /// <summary>
        ///     Posts the data logged for an infringement logged at a Point
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult Post([FromBody] AtPointModel model)
        {
            PushSignalR(model);

            using (var dbContext = new RTE.Data.DataContext())
            {
                var pointData = 
                    new CameraPointData
                    {
                        Json = JsonConvert.SerializeObject(model),
                        TimeStamp = DateTime.Now
                    };

                dbContext.CameraPointsData.Add(pointData);
                dbContext.SaveChanges();

                return Ok(true);
            }
        }

        private static void PushSignalR(AtPointModel model)
        {
            string json = JsonConvert.SerializeObject(model);

            using (var hubConnection = new HubConnection(SignalRHubUrl))
            {
                var hubProxy = hubConnection.CreateHubProxy("AtPointNotificationHub");

                hubConnection.Start().Wait();

                if (model.IsOffence)
                {
                    hubProxy.Invoke("SendAtPointInfringement", model.SectionPointCode, json).Wait();
                }
                else
                {
                    hubProxy.Invoke("SendAtPointData", model.SectionPointCode, json).Wait();
                }

            }
        }
    }
}
