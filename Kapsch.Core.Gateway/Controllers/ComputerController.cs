using Kapsch.Core.Data;
using Kapsch.Core.Extensions;
using Kapsch.Core.Filters;
using Kapsch.Core.Gateway.Models.Computer;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/Computer")]
    [SessionAuthorize]
    [UsageLog]
    public class ComputerController : BaseController
    {
        [HttpPost]
        [ValidationActionFilter]
        [ResponseType(typeof(ComputerModel))]
        public IHttpActionResult Post([FromBody] ComputerModel model)
        {
            using (var dbContext = new DataContext())
            {
                var computer = dbContext.Computers.SingleOrDefault(f => f.Name.ToUpper() == model.Name.ToUpper());
                if (computer != null)
                {
                    model.ID = computer.ID;
                    model.IPAddress = computer.IPAddress;
                    model.DistrictID = computer.DistrictID;

                    return Ok(model);
                }

                computer = new Computer();
                computer.Name = model.Name;
                computer.IPAddress = model.IPAddress;
                computer.DistrictID = model.DistrictID;

                dbContext.Computers.Add(computer);
                dbContext.SaveChanges();

                model.ID = computer.ID;

                return Ok(model);
            }
        }

        [HttpPut]
        [ValidationActionFilter]
        public IHttpActionResult Put([FromBody] ComputerModel model)
        {
            using (var dbContext = new DataContext())
            {
                var computer = dbContext.Computers.Find(model.ID);
                if (computer == null)
                {
                    return this.BadRequestEx(Error.ComputerDoesNotExist);
                }

                if (dbContext.Computers.Any(f => f.Name == model.Name && f.ID != computer.ID))
                {
                    return this.BadRequestEx(Error.ComputerAlreadyExist);
                }

                computer.Name = model.Name;
                computer.IPAddress = model.IPAddress;
                computer.DistrictID = model.DistrictID;

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [Route("PaginatedList")]
        [ResponseType(typeof(PaginationListModel<ComputerModel>))]
        public IHttpActionResult GetPaginatedList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.Computer>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .Computers
                    .Include(f => f.District)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<ComputerModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<ComputerModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Computer>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);

                var paginationList = new PaginationListModel<ComputerModel>();
                paginationList.Models = entities.Select(f =>
                    new ComputerModel
                    {
                        ID = f.ID,
                        IPAddress = f.IPAddress,
                        Name = f.Name,
                        DistrictID = f.District == null ? default(long?) : f.District.ID,
                        DistrictName = f.District == null ? string.Empty : f.District.BranchName
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpGet]
        [ResponseType(typeof(ComputerModel))]
        public IHttpActionResult Get(long id)
        {
            using (var dbContext = new DataContext())
            {
                var computer = dbContext.Computers
                    .AsNoTracking()
                    .Include(f => f.District)
                    .Include(f => f.ComputerConfigSettings)
                    .SingleOrDefault(f => f.ID == id);
                if (computer == null)
                {
                    return this.BadRequestEx(Error.ComputerDoesNotExist);
                }

                var model =
                    new ComputerModel
                    {
                        ID = computer.ID,
                        IPAddress = computer.IPAddress,
                        Name = computer.Name,
                        DistrictID = computer.District == null ? default(long?) : computer.District.ID,
                        DistrictName = computer.District == null ? string.Empty : computer.District.BranchName,
                        ComputerConfigSettings = 
                            computer.ComputerConfigSettings
                                .Select(f =>
                                    new ComputerConfigSettingModel()
                                    {
                                        ID = f.ID,
                                        Value = f.Value,
                                        ComputerItemType = (Models.Enums.ComputerItemType)f.ComputerItemType
                                    })
                                .ToList()
                    };

                return Ok(model);
            }
        }

        ////
        [HttpPost]
        [ValidationActionFilter]
        [Route("Setting")]
        [ResponseType(typeof(ComputerConfigSettingModel))]
        public IHttpActionResult PostComputerConfigSetting([FromBody] ComputerConfigSettingModel model)
        {
            using (var dbContext = new DataContext())
            {
                var computerConfigSetting = dbContext.ComputerConfigSettings
                    .FirstOrDefault(f => f.ComputerItemType == (Data.Enums.ComputerItemType) model.ComputerItemType && f.ComputerID == model.ComputerID);
                if (computerConfigSetting != null)
                {
                    return this.BadRequestEx(Error.ComputerSettingAlreadyExist);
                }

                computerConfigSetting = new ComputerConfigSetting();
                computerConfigSetting.ComputerID = model.ComputerID;
                computerConfigSetting.ComputerItemType = (Data.Enums.ComputerItemType)model.ComputerItemType;
                computerConfigSetting.Value = model.Value;

                dbContext.ComputerConfigSettings.Add(computerConfigSetting);
                dbContext.SaveChanges();

                model.ID = computerConfigSetting.ID;

                return Ok(model);
            }
        }

        [HttpPut]
        [ValidationActionFilter]
        [Route("Setting")]
        public IHttpActionResult PutComputerConfigSettingModel([FromBody] ComputerConfigSettingModel model)
        {
            using (var dbContext = new DataContext())
            {
                var computerConfigSetting = dbContext.ComputerConfigSettings.Find(model.ID);
                if (computerConfigSetting == null)
                {
                    return this.BadRequestEx(Error.ComputerSettingDoesNotExist);
                }

                if (dbContext.ComputerConfigSettings.Any(f => f.ComputerItemType == (Data.Enums.ComputerItemType)model.ComputerItemType && f.ComputerID == model.ComputerID && f.ID != model.ID))
                {
                    return this.BadRequestEx(Error.ComputerSettingAlreadyExist);
                }

                computerConfigSetting.ComputerID = model.ComputerID;
                computerConfigSetting.ComputerItemType = (Data.Enums.ComputerItemType)model.ComputerItemType;
                computerConfigSetting.Value = model.Value;

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpGet]
        [ValidationActionFilter]
        [Route("Setting")]
        [ResponseType(typeof(ComputerConfigSettingModel))]
        public IHttpActionResult GetComputerConfigSettingModel(long id)
        {
            using (var dbContext = new DataContext())
            {
                var computerConfigSetting = dbContext.ComputerConfigSettings.Find(id);
                if (computerConfigSetting == null)
                {
                    return this.BadRequestEx(Error.ComputerSettingDoesNotExist);
                }

                var model = new ComputerConfigSettingModel();
                model.ID = computerConfigSetting.ID;
                model.ComputerID = computerConfigSetting.ComputerID;
                model.ComputerItemType = (Models.Enums.ComputerItemType)computerConfigSetting.ComputerItemType;
                model.Value = computerConfigSetting.Value;
               
                return Ok(model);
            }
        }
    }
}
