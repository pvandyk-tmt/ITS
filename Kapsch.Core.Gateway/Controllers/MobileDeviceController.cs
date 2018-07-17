using Kapsch.Core.Data;
using Kapsch.Core.Filters;
using Kapsch.Core.Gateway.Helpers;
using Kapsch.Core.Gateway.Models.User;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using Kapsch.Core.Gateway.Models;
using Kapsch.Core.Extensions;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Shared.Helpers;
using Kapsch.Core.Gateway.Models.MobileDevice;
using System.IO;
using System.Net.Http.Headers;
using System.Configuration;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/MobileDevice")]
    [SessionAuthorize]
    [UsageLog]
    public class MobileDeviceController : BaseController
    {
        public readonly static string ApkExtention = ".apk";
        public readonly static string ApplicationUpdateFilePath = ConfigurationManager.AppSettings["ApplicationUpdateFilePath"];
        
        [HttpPost]
        [ValidationActionFilter]
        [ResponseType(typeof(MobileDeviceModel))]
        public IHttpActionResult Post([FromBody] MobileDeviceModel model)
        {
            using (var dbContext = new DataContext())
            {
                var mobileDevice = dbContext.MobileDevices.SingleOrDefault(f => f.DeviceID == model.DeviceID);
                if (mobileDevice != null)
                {
                    model.ID = mobileDevice.ID;
                    model.CreatedTimestamp = mobileDevice.CreatedTimestamp;
                    model.Status = (Models.Enums.MobileDeviceStatus) mobileDevice.Status;

                    return Ok(model);
                }
                
                mobileDevice = new MobileDevice();
                mobileDevice.CreatedTimestamp = DateTime.Now;
                mobileDevice.DeviceID = model.DeviceID;
                mobileDevice.DistrictID = model.DistrictID;
                mobileDevice.SerialNumber = model.SerialNumber;
                mobileDevice.Status = Data.Enums.Status.Active;

                dbContext.MobileDevices.Add(mobileDevice);
                dbContext.SaveChanges();

                model.ID = mobileDevice.ID;
                
                return Ok(model);
            }
        }

        [HttpPut]
        [ValidationActionFilter]
        public IHttpActionResult Put([FromBody] MobileDeviceModel model)
        {
            using (var dbContext = new DataContext())
            {
                var mobileDevice = dbContext.MobileDevices.Find(model.ID);
                if (mobileDevice == null)
                {
                    return this.BadRequestEx(Error.MobileDeviceDoesNotExist);
                }

                if (dbContext.MobileDevices.Any(f => f.DeviceID == model.DeviceID && f.ID != mobileDevice.ID))
                {
                    return this.BadRequestEx(Error.MobileDeviceAlreadyExist);
                }

                mobileDevice.DeviceID = model.DeviceID;
                mobileDevice.DistrictID = model.DistrictID;
                mobileDevice.SerialNumber = model.SerialNumber;
                mobileDevice.Status = (Data.Enums.Status)model.Status;

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [Route("PaginatedList")]
        [ResponseType(typeof(PaginationListModel<MobileDeviceModel>))]
        public IHttpActionResult GetPaginatedList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.MobileDevice>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .MobileDevices
                    .Include(f => f.District)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<MobileDeviceModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<MobileDeviceModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<MobileDevice>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);

                var paginationList = new PaginationListModel<MobileDeviceModel>();
                paginationList.Models = entities.Select(f =>
                    new MobileDeviceModel
                    {
                        ID = f.ID,
                        DeviceID = f.DeviceID,
                        CreatedTimestamp = f.CreatedTimestamp,
                        DistrictID = f.District == null ? default(long?) : f.District.ID,
                        DistrictName = f.District == null ? string.Empty : f.District.BranchName,
                        SerialNumber = f.SerialNumber,
                        Status = (Models.Enums.MobileDeviceStatus)f.Status
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpGet]
        [Route("MobileDeviceItem")]
        [ResponseType(typeof(IList<MobileDeviceItemModel>))]
        public IHttpActionResult GetMobileDeviceItems(string deviceID)
        {
            using (var dbContext = new DataContext())
            {
                var mobileDevice = dbContext.MobileDevices.FirstOrDefault(f => f.DeviceID == deviceID);
                if (mobileDevice == null)
                {
                    return this.BadRequestEx(Error.MobileDeviceDoesNotExist);
                }

                var mobileDeviceItems = dbContext.MobileDeviceItems.Select(f => new { Name = f.Name , Value = f.Value, MobileDeviceItemType = 0 });
                var mobileDeviceConfigItems = dbContext.MobileDeviceConfigItems.Where(f => f.MobileDeviceID == mobileDevice.ID).Select(f => new { Name = f.Name, Value = f.Value, MobileDeviceItemType = 1 });
                var entities = mobileDeviceItems.Union(mobileDeviceItems).OrderBy(f => f.Name).ToList();

                return Ok(entities.Select(f => 
                    new MobileDeviceItemModel 
                    { 
                        MobileDeviceItemType = (Models.Enums.MobileDeviceItemType)f.MobileDeviceItemType,
                        Name = f.Name,
                        Value = f.Value
                    }
                 ));
            }
        }

        [HttpGet]
        [Route("MobileDeviceItem")]
        [ResponseType(typeof(MobileDeviceItemModel))]
        public IHttpActionResult GetMobileDeviceItem(string deviceID, string name)
        {
            using (var dbContext = new DataContext())
            {
                var mobileDevice = dbContext.MobileDevices.FirstOrDefault(f => f.DeviceID == deviceID);
                if (mobileDevice == null)
                {
                    return this.BadRequestEx(Error.MobileDeviceDoesNotExist);
                }

                var mobileDeviceConfigItem = dbContext.MobileDeviceConfigItems.FirstOrDefault(f => f.MobileDeviceID == mobileDevice.ID && f.Name == name);
                if (mobileDeviceConfigItem == null)
                {
                    return this.BadRequestEx(Error.MobileDeviceItemDoesNotExist);
                }

                return Ok(
                    new MobileDeviceItemModel
                    {
                        MobileDeviceItemType = Models.Enums.MobileDeviceItemType.PerDevice,
                        Name = mobileDeviceConfigItem.Name,
                        Value = mobileDeviceConfigItem.Value
                    });
            }
        }

        [Route("UserMobileDeviceActivity")]
        [HttpPost]
        public IHttpActionResult AddUpdateMobileDeviceActivities([FromBody] IList<UserMobileDeviceActivityModel> models)
        {
            using (var dbContext = new DataContext())
            {
                foreach (var model in models)
                {
                    var userMobileDeviceActivity = new UserMobileDeviceActivity();
                    userMobileDeviceActivity.DeviceID = model.DeviceID;
                    userMobileDeviceActivity.CredentialID = model.CredentialID;
                    userMobileDeviceActivity.CreatedTimestamp = model.CreatedTimestamp;
                    userMobileDeviceActivity.Category = model.Category;
                    userMobileDeviceActivity.ActionDescription = model.ActionDescription;

                    dbContext.UserMobileDeviceActivities.Add(userMobileDeviceActivity);
                }

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [Route("UserMobileDeviceActivity/Category")]
        [HttpGet]
        public IHttpActionResult GetMobileDeviceActivityCatgories()
        {
            using (var dbContext = new DataContext())
            {
                var categories = dbContext.UserMobileDeviceActivities
                    .AsNoTracking()
                    .OrderBy(f => f.Category)
                    .Select(f => f.Category)
                    .Distinct()
                    .ToList();

                return Ok(categories);
            }
        }

        [HttpPost]
        [Route("MobileDeviceItem")]
        public IHttpActionResult AddUpdateMobileDeviceItems([FromBody] IList<MobileDeviceItemModel> models, string deviceID)
        {
            using (var dbContext = new DataContext())
            {
                var mobileDevice = dbContext.MobileDevices.FirstOrDefault(f => f.DeviceID == deviceID);
                if (mobileDevice == null)
                {
                    return this.BadRequestEx(Error.MobileDeviceDoesNotExist);
                }

                var mobileDeviceItemModels = models.Where(f => f.MobileDeviceItemType == Models.Enums.MobileDeviceItemType.All);
                foreach (var mobileDeviceItemModel in mobileDeviceItemModels)
                {
                    var mobileDeviceItem = dbContext.MobileDeviceItems.FirstOrDefault(f => f.Name == mobileDeviceItemModel.Name);
                    if (mobileDeviceItem == null)
                    {
                        mobileDeviceItem = new MobileDeviceItem();
                        mobileDeviceItem.Name = mobileDeviceItemModel.Name;
                        mobileDeviceItem.Value = mobileDeviceItemModel.Value;

                        dbContext.MobileDeviceItems.Add(mobileDeviceItem);
                    }
                    else
                    {
                        mobileDeviceItem.Value = mobileDeviceItemModel.Value;
                    }
                }

                var mobileDeviceConfigItemModels = models.Where(f => f.MobileDeviceItemType == Models.Enums.MobileDeviceItemType.PerDevice);
                foreach (var mobileDeviceConfigItemModel in mobileDeviceConfigItemModels)
                {
                    var mobileDeviceItem = dbContext.MobileDeviceConfigItems.FirstOrDefault(f => f.Name == mobileDeviceConfigItemModel.Name && f.MobileDeviceID == mobileDevice.ID);
                    if (mobileDeviceItem == null)
                    {
                        mobileDeviceItem = new MobileDeviceConfigItem();
                        mobileDeviceItem.MobileDeviceID = mobileDevice.ID;
                        mobileDeviceItem.Name = mobileDeviceConfigItemModel.Name;
                        mobileDeviceItem.Value = mobileDeviceConfigItemModel.Value;

                        dbContext.MobileDeviceConfigItems.Add(mobileDeviceItem);
                    }
                    else
                    {
                        mobileDeviceItem.Value = mobileDeviceConfigItemModel.Value;
                    }
                }

                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [Route("MobileDeviceLocation")]
        public IHttpActionResult AddMobileDeviceLocations([FromBody] IList<MobileDeviceLocationModel> models, string deviceID)
        {
            using (var dbContext = new DataContext())
            {
                var mobileDevice = dbContext.MobileDevices.FirstOrDefault(f => f.DeviceID == deviceID);
                if (mobileDevice == null)
                {
                    return this.BadRequestEx(Error.MobileDeviceDoesNotExist);
                }

                foreach (var mobileDeviceLocationModel in models)
                {
                    var mobileDeviceLocation = new MobileDeviceLocation();
                    mobileDeviceLocation.GpsLongitude = mobileDeviceLocationModel.GpsLongitude;
                    mobileDeviceLocation.GpsLatitude = mobileDeviceLocationModel.GpsLatitude;
                    mobileDeviceLocation.LocationTimestamp = mobileDeviceLocationModel.LocationTimestamp;
                    mobileDeviceLocation.MobileDeviceID = mobileDevice.ID;

                    dbContext.MobileDeviceLocations.Add(mobileDeviceLocation);
                
                }
                
                dbContext.SaveChanges();

                return Ok();
            }
        }

        [HttpPost]
        [Route("MobileDeviceApplication/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<MobileDeviceApplicationModel>))]
        public IHttpActionResult GetMobileDeviceApplicationPaginatedList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.MobileDeviceApplication>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .MobileDeviceApplications
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<MobileDeviceApplicationModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<MobileDeviceApplicationModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<MobileDeviceApplication>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);

                var paginationList = new PaginationListModel<MobileDeviceApplicationModel>();
                paginationList.Models = entities.Select(f =>
                    new MobileDeviceApplicationModel
                    {
                        ID = f.ID,
                        Name = f.Name,
                        SoftwareVersion = f.SoftwareVersion,
                        ModifiedTimestamp = f.ModifiedTimestamp,
                        ApplicationType = (Models.Enums.ApplicationType)f.ApplicationType,
                        Status = (Models.Enums.Status)f.Status
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [Route("MobileDeviceApplication")]
        [ResponseType(typeof(byte[]))]
        [HttpGet]
        public IHttpActionResult GetMobileDeviceApplication(string deviceID, string androidPackageName, int? version)
        {
            if (!version.HasValue)
            {
                version = 0;
            }

            var applicationFilePath = Path.Combine(ApplicationUpdateFilePath, androidPackageName);
            string mobileDeviceItemName = androidPackageName.Replace(ApkExtention, string.Empty);

            using (var dbContext = new DataContext())
            {
                var currentVersion = 0;
                var mobileDeviceApplication = dbContext.MobileDeviceApplications.SingleOrDefault(f => f.Name == mobileDeviceItemName);
                if (mobileDeviceApplication != null)
                {
                    currentVersion = int.Parse(mobileDeviceApplication.SoftwareVersion);
                }

                if (currentVersion <= version)
                {
                    return Ok();
                }

                using (var binaryReader = new BinaryReader(File.Open(applicationFilePath, FileMode.Open, FileAccess.Read)))
                {
                    binaryReader.BaseStream.Position = 0;

                    var bytes = binaryReader.ReadBytes(Convert.ToInt32(binaryReader.BaseStream.Length));
                    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    httpResponseMessage.Content = new ByteArrayContent(bytes);
                    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = androidPackageName;
                    httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    return ResponseMessage(httpResponseMessage);
                }
            }
        }
    }
}
