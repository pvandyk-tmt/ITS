using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Description;
using Kapsch.Core.Caching;
using Kapsch.Core.Data;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.Gateway.Shared;
using Kapsch.RTE.Gateway.Clients;
using Kapsch.RTE.Gateway.Hubs;
using Kapsch.RTE.Gateway.Models.Camera;
using Kapsch.RTE.Gateway.Models.Configuration.Device.Listener.Enums;
using Kapsch.RTE.Gateway.Models.Configuration.Dot;
using Kapsch.RTE.Gateway.Models.Configuration.iTicket;
using Newtonsoft.Json;

namespace Kapsch.RTE.Gateway.Controllers
{
    [RoutePrefix("api/Configuration/DOT")]
    public class ConfigurationDotController : BaseController
    {
        [Route("SectionConfiguration")]
        [HttpGet]
        [ResponseType(typeof(SectionConfigurationModel))]
        public IHttpActionResult GetSection(string sectionCodeAtPointA, string sectionCodeAtPointB)
        {
            using (var dbContext = new Kapsch.RTE.Data.DataContext())
            {
                var item = dbContext.DotSectionConfigurations.FirstOrDefault(c => c.SectionCodePointA == sectionCodeAtPointA && c.SectionCodePointB == sectionCodeAtPointB);
                if (item == null)
                    return this.BadRequestEx(Error.SectionConfigurationDoesNotExist);

                var model =
                    new SectionConfigurationModel
                    {
                        LevenshteinMatchDistance = item.LevenshteinMatchDistance,
                        CreatePhysicalInfringement = item.CreatePhysicalInfringement == 1,
                        SectionCodePointA = item.SectionCodePointA,
                        SectionCodePointB = item.SectionCodePointB,
                        SectionCode = item.SectionCode,
                        SectionDescription = item.SectionDescription,
                        SectionDistanceInMeter = item.SectionDistance
                    };

                return Ok(model);
            }          
        }

        /// <summary>
        /// Register an Adapter. Send true when it can be registered and false if it cannot
        /// </summary>
        /// <param name="section"></param>
        /// <param name="listener"></param>
        /// <param name="heartBeatSeconds"></param>
        /// <returns></returns>
        [Route("Registration/Adapter")]
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult RegisterAdapter([FromBody]SectionConfigurationModel section, string listener, long heartBeatSeconds)
        {
            var key = listener + "|" + section.SectionCode;

            var dictionary = Startup.RegisteredAdapters().Get(key, false);
            if (dictionary == null)
            {
                dictionary = new Dictionary<SectionConfigurationModel, long> {{section, heartBeatSeconds}};

                Startup.RegisteredAdapters().Set(key, dictionary, (heartBeatSeconds/60) + 10); //Grace of 10 seconds

                return Ok(true);
            }

            return Ok(false);
        }

        /// <summary>
        /// Register an Adapter. Send true when it can be registered and false if it cannot
        /// </summary>
        /// <param name="section"></param>
        /// <param name="listener"></param>
        /// <param name="heartBeatSeconds"></param>
        /// <returns></returns>
        [Route("Heartbeat/Adapter")]
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SendHeartbeatToAdapter([FromBody]SectionConfigurationModel section, string listener, long heartBeatSeconds)
        {
            var key = listener + "|" + section.SectionCode;

            var dictionary = Startup.RegisteredAdapters().Get(key, false);
            if (dictionary != null)
            {
                dictionary = new Dictionary<SectionConfigurationModel, long> {{section, heartBeatSeconds}};

                Startup.RegisteredAdapters().Remove(key);
                Startup.RegisteredAdapters().Set(key, dictionary, heartBeatSeconds + 10); //Grace of 10 seconds
            }
            else
            {
                return RegisterAdapter(section, listener, heartBeatSeconds);
            }

            return Ok(true);
        }

        // GET: Ticketing
        /// <summary>
        ///     Posts the data logged for a section to database - This will typically be infringments
        /// </summary>
        /// <returns></returns>
        [Route("Registration/MobileDevice")]
        [HttpPost]
        [ResponseType(typeof(SectionConfigurationModel))]
        public IHttpActionResult RegisterMobileDevice([FromBody] iTicketConfigurationModel model, DateTime eventsAfter)
        {
            try
            {
                string pointA = model.PointAIpAddress + ":" + model.PointAPort;
                string pointB = model.PointBIpAddress + ":" + model.PointBPort;

                SectionConfigurationModel sectionModel = GetSectionByIP(pointA, pointB, eventsAfter);
                if (sectionModel == null)
                {
                    string processToStart = ConfigurationManager.AppSettings["DistanceOverTimeAdapter"].ToString();

                    Process process = new Process();

                    if (!string.IsNullOrEmpty(model.PointAIpAddress) && model.PointAPort > 0 && !string.IsNullOrEmpty(model.PointBIpAddress) && model.PointBPort > 0)
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = processToStart,
                            Arguments = string.Format("-listenerType=S -listenMs={0} -ipA={1}:{2} -ipB={3}:{4}", model.PollMs,model.PointAIpAddress,model.PointAPort,model.PointBIpAddress,model.PointBPort)

                        };
                    }

                    process.Start();
                }

                DateTime timeout = DateTime.Now.AddMinutes(2);

                while (true)
                {
                    if (DateTime.Now >= timeout)
                        return this.BadRequestEx(Error.RegisterMobileDeviceTimeout);
                    
                    sectionModel = GetSectionByIP(pointA, pointB, eventsAfter);

                    if (sectionModel != null)
                    {
                        break;
                    }
                    
                    Thread.Sleep(1000);
                }

                return Ok(sectionModel);
            }
            catch (Exception ex)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(ex));
            }
        }

        private SectionConfigurationModel GetSectionByIP(string iPAddressPointA, string iPAddressPointB, DateTime eventsAfter)
        {
            AtPointService cpds = new AtPointService();

            FilterModel filterSourceA = new FilterModel
            {
                Operation = Operation.Equals,
                PropertyName = "ListenerSource",
                Value = iPAddressPointA
            };

            FilterModel filterEvent = new FilterModel
            {
                Operation = Operation.GreaterThan,
                PropertyName = "EventDateTime",
                Value = eventsAfter
            };

            IList<FilterModel> filters = new List<FilterModel>
            {
                filterEvent,
                filterSourceA
            };

            PaginationListModel<AtPointModel> itemsPointA = cpds.GetPaginatedList(filters, FilterJoin.And, true, "ListenerSource", 0, 1);

            FilterModel filterSourceB = new FilterModel
            {
                Operation = Operation.Equals,
                PropertyName = "ListenerSource",
                Value = iPAddressPointB
            };

            filters = new List<FilterModel>
            {
                filterEvent,
                filterSourceB
            };

            PaginationListModel<AtPointModel> itemsPointB = cpds.GetPaginatedList(filters, FilterJoin.And, true, "ListenerSource", 0, 1);

            if (itemsPointA.TotalCount > 0 && itemsPointB.TotalCount > 0)
            {
                AtPointModel sectionCodeAtPointA = itemsPointA.Models.FirstOrDefault();
                AtPointModel sectionCodeAtPointB = itemsPointB.Models.FirstOrDefault();

                if (sectionCodeAtPointA != null && sectionCodeAtPointB != null)
                {
                    SectionConfigurationModel section = GetSectionByCode(sectionCodeAtPointA.SectionPointCode, sectionCodeAtPointB.SectionPointCode);
                    return section;
                }
            }

            return null;
        }

        private SectionConfigurationModel GetSectionByCode(string sectionCodeAtPointA, string sectionCodeAtPointB)
        {
            using (var dbContext = new Kapsch.RTE.Data.DataContext())
            {
                var item = dbContext.DotSectionConfigurations.FirstOrDefault(c => c.SectionCodePointA == sectionCodeAtPointA && c.SectionCodePointB == sectionCodeAtPointB);

                if (item != null)
                {
                    SectionConfigurationModel model = new SectionConfigurationModel
                    {
                        LevenshteinMatchDistance = item.LevenshteinMatchDistance,
                        CreatePhysicalInfringement = bool.Parse(item.CreatePhysicalInfringement.ToString()),
                        SectionCodePointA = item.SectionCodePointA,
                        SectionCodePointB = item.SectionCodePointB,
                        SectionCode = item.SectionCode,
                        SectionDescription = item.SectionDescription,
                        SectionDistanceInMeter = item.SectionDistance
                    };

                    return model;
                }
            }

            return null;
        }
    }
}