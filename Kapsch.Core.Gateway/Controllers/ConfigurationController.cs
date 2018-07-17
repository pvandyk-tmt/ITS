using Kapsch.Core.Data;
using Kapsch.Core.Extensions;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Data.Entity;
using System.Web.Http;
using Kapsch.Core.Gateway.Models.Configuration;
using System.Web.Http.Description;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Core.Filters;
using Kapsch.Core.Gateway.Models.User;
using Newtonsoft.Json;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/Configuration")]
    [SessionAuthorize]
    [UsageLog]
    public class ConfigurationController : BaseController
    {
        private readonly static string GoogleGeoSearchUrlByGps = "http://maps.googleapis.com/maps/api/geocode/json?latlng={0},{1}&sensor=false";
        private readonly static string GoogleGeoSearchUrlByAddress = "http://maps.googleapis.com/maps/api/geocode/json?address={0}&sensor=false";

       

        [SessionAuthorize]
        [HttpPost]
        [Route("Camera/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<CameraModel>))]
        public IHttpActionResult GetCameraPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.Camera>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .Cameras
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<CameraModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<CameraModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Camera>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<CameraModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new CameraModel
                    {
                        ID = f.ID,
                        AdapterType = (Models.Enums.CameraAdapterType)f.CameraAdapterType,
                        ConfigJson = f.ConfigJson,
                        CreatedTimeStamp = f.CreatedTimeStamp,
                        ConnectToHost = f.ConnectToHost == "1",
                        DeviceConnectionType = (Models.Enums.CameraConnectionType)f.CameraConnectionType,
                        DeviceStatus = (Models.Enums.CameraStatusType)f.CameraStatusType,
                        FriendlyName = f.FriendlyName,
                        GpsLatitude = f.GpsLatitude,
                        GpsLongitude = f.GpsLongitude,
                        IsEnabled = f.IsEnabled == "1",
                        ModifiedTimeStamp = f.ModifiedTimeStamp

                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [SessionAuthorize]
        [HttpPost]
        [Route("InfringementLocation/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<InfringementLocationModel>))]
        public IHttpActionResult GetInfringementLocationPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.InfringementLocation>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .InfringementLocations
                    .Include(f => f.Court)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<InfringementLocationModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<InfringementLocationModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<InfringementLocation>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<InfringementLocationModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new InfringementLocationModel
                    {
                        ID = f.ID,
                        Code = f.Code,
                        Description = f.Description,
                        CourtID = f.CourtID,
                        CourtName = f.Court == null ? string.Empty : f.Court.CourtName,
                        GpsLatitude = f.GpsLatitude,
                        GpsLongitude = f.GpsLongitude,
                        InfringementLocationType = (Models.Enums.InfringementLocationType)f.InfringementLocationType

                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("OffenceCode/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<OffenceCodeModel>))]
        public IHttpActionResult GetOffenceCodePaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.OffenceCode>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .OffenceCodes
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<OffenceCodeModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<OffenceCodeModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<OffenceCode>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<OffenceCodeModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new OffenceCodeModel
                    {

                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("Country/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<CountryModel>))]
        public IHttpActionResult GetCountryPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(orderPropertyName))
                orderPropertyName = "Description";

            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.Country>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .Countries
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<CountryModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<CountryModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Country>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<CountryModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new CountryModel
                    {
                        ID = f.ID,
                        Description = f.Description,
                        ModifiedTimestamp = f.ModifiedTimestamp
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("District/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<DistrictModel>))]
        public IHttpActionResult GetDistrictPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.District>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .Districts
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<DistrictModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<DistrictModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<District>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<DistrictModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new DistrictModel
                    {
                        ID = f.ID,
                        Province = f.Province,
                        Town = f.Town,
                        Suburb = f.Suburb,
                        Street = f.Street,
                        Code = f.Code,
                        POBox = f.POBox,
                        PostalCode = f.PostalCode,
                        PostalStreet = f.PostalStreet,
                        PostalSuburb = f.PostalSuburb,
                        PostalTown = f.PostalTown,
                        TicketPre = f.TicketPre,
                        TicketPost = f.TicketPost,
                        TicketSequenceName = f.TicketSequenceName,
                        CaseSequenceName = f.CaseSequenceName,
                        Section56TicketPre = f.Section56TicketPre,
                        Section56TicketPost = f.Section56TicketPost,
                        TrafficTicketPre = f.TrafficTicketPre,
                        TrafficTicketPost = f.TrafficTicketPost,
                        DepartmantName = f.DepartmantName,
                        Telephone = f.Telephone,
                        Faks = f.Faks,
                        CaseNoPre = f.CaseNoPre,
                        CaseNoPost = f.CaseNoPost,
                        DistrictAll = f.DistrictAll,
                        RegionID = f.RegionID,
                        DistrictTypeID = f.DistrictTypeID,
                        AccountNo = f.AccountNo,
                        Bank = f.Bank,
                        Branch = f.Branch,
                        BranchCode = f.BranchCode,
                        AccountType = f.AccountType,
                        BankDetails = f.BankDetails,
                        ExtTicketPost = f.ExtTicketPost,
                        IACode = f.IACode,
                        NatisNumber = f.NatisNumber,
                        NatisSequenceName = f.NatisSequenceName,
                        OfficeHours = f.OfficeHours,
                        Sectiontion67Pre = f.Sectiontion67Pre,
                        Section67Post = f.Section67Post,
                        J175Printing = f.J175Printing,
                        CentralCaptureIndicator = f.CentralCaptureIndicator,
                        ActiveIndicator = f.ActiveIndicator,
                        EmailAddress = f.EmailAddress,
                        ITicketPre56 = f.ITicketPre56,
                        ITicketSeqName56 = f.ITicketSeqName56,
                        ITicketPre341 = f.ITicketPre341,
                        ITicketSeqName341 = f.ITicketSeqName341,
                        BranchName = f.BranchName,
                        SiteLogo = f.SiteLogo,
                        PaymentOptions = f.PaymentOptions
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("District")]
        [ResponseType(typeof(DistrictModel))]
        public IHttpActionResult GetDistrict(long id)
        {
            using (var dbContext = new DataContext())
            {
                var entity = dbContext.Districts.Find(id);
                if (entity == null)
                    return this.BadRequestEx(Error.DistrictDoesNotExist);

                return Ok(
                    new DistrictModel
                    {
                        ID = entity.ID,
                        Province = entity.Province,
                        Town = entity.Town,
                        Suburb = entity.Suburb,
                        Street = entity.Street,
                        Code = entity.Code,
                        POBox = entity.POBox,
                        PostalCode = entity.PostalCode,
                        PostalStreet = entity.PostalStreet,
                        PostalSuburb = entity.PostalSuburb,
                        PostalTown = entity.PostalTown,
                        TicketPre = entity.TicketPre,
                        TicketPost = entity.TicketPost,
                        TicketSequenceName = entity.TicketSequenceName,
                        CaseSequenceName = entity.CaseSequenceName,
                        Section56TicketPre = entity.Section56TicketPre,
                        Section56TicketPost = entity.Section56TicketPost,
                        TrafficTicketPre = entity.TrafficTicketPre,
                        TrafficTicketPost = entity.TrafficTicketPost,
                        DepartmantName = entity.DepartmantName,
                        Telephone = entity.Telephone,
                        Faks = entity.Faks,
                        CaseNoPre = entity.CaseNoPre,
                        CaseNoPost = entity.CaseNoPost,
                        DistrictAll = entity.DistrictAll,
                        RegionID = entity.RegionID,
                        DistrictTypeID = entity.DistrictTypeID,
                        AccountNo = entity.AccountNo,
                        Bank = entity.Bank,
                        Branch = entity.Branch,
                        BranchCode = entity.BranchCode,
                        AccountType = entity.AccountType,
                        BankDetails = entity.BankDetails,
                        ExtTicketPost = entity.ExtTicketPost,
                        IACode = entity.IACode,
                        NatisNumber = entity.NatisNumber,
                        NatisSequenceName = entity.NatisSequenceName,
                        OfficeHours = entity.OfficeHours,
                        Sectiontion67Pre = entity.Sectiontion67Pre,
                        Section67Post = entity.Section67Post,
                        J175Printing = entity.J175Printing,
                        CentralCaptureIndicator = entity.CentralCaptureIndicator,
                        ActiveIndicator = entity.ActiveIndicator,
                        EmailAddress = entity.EmailAddress,
                        ITicketPre56 = entity.ITicketPre56,
                        ITicketSeqName56 = entity.ITicketSeqName56,
                        ITicketPre341 = entity.ITicketPre341,
                        ITicketSeqName341 = entity.ITicketSeqName341,
                        BranchName = entity.BranchName,
                        SiteLogo = entity.SiteLogo,
                        PaymentOptions = entity.PaymentOptions
                    });
            }
        }

        [HttpPost]
        [Route("Court/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<CourtModel>))]
        public IHttpActionResult GetCourtPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.Court>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .Courts
                    .Include(f => f.AddressInfo)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<CourtModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<CourtModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Court>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<CourtModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new CourtModel
                    {
                        ID = f.ID,
                        CourtName = f.CourtName,
                        CourtTime = f.CourtTime,
                        PersonInfoID = f.PersonInfoID,
                        ContemptAmount = f.ContemptAmount,
                        ContemptDays = f.ContemptDays,
                        BankingInfoID = f.BankingInfoID,
                        DistrictID = f.DistrictID,
                        CasePost = f.CasePost,
                        CasePre = f.CasePre,
                        SequenceName = f.SequenceName,
                        Status = (Models.Enums.Status) f.Status,
                        WarrantExpireDays = f.WarrantExpireDays,
                        WarrantLetterGrace = f.WarrantLetterGrace,
                        WarrantPost = f.WarrantPost,
                        WarrantPre = f.WarrantPre,
                        CaptureDate = f.CaptureDate,
                        DaysToCourtDate = f.DaysToCourtDate,
                        OverAllocation = f.OverAllocation,
                        ReIssueInvalidServing = f.ReIssueInvalidServing,
                        SummonsExpireDays = f.SummonsExpireDays,
                        TypeOfServiceAllowed = f.TypeOfServiceAllowed,
                        UserId = f.CredentialID,
                        Address = f.AddressInfo == null ? null : 
                            new AddressInfoModel
                            {
                                ID = f.AddressInfo.ID,
                                AddressTypeID = (long)f.AddressInfo.AddressTypeID,
                                SourceID = f.AddressInfo.SourceID,
                                PersonInfoID = f.AddressInfo.PersonInfoID,
                                Line1 = f.AddressInfo.Line1,
                                Line2 = f.AddressInfo.Line2,
                                Suburb = f.AddressInfo.Suburb,
                                Town = f.AddressInfo.Town,
                                Country = f.AddressInfo.Country,
                                Code = f.AddressInfo.Code,
                                CreatedDate = f.AddressInfo.CreatedDate,
                                CreatedUserDetailID = f.AddressInfo.CreatedCredentialID,
                                IsPrefferedIndicator = f.AddressInfo.IsPrefferedIndicator,
                                Latitude = f.AddressInfo.Latitude,
                                Longitude = f.AddressInfo.Longitude
                            }

                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("PublicHoliday/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<PublicHolidayModel>))]
        public IHttpActionResult GetPublicHolidayPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.PublicHoliday>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .PublicHolidays
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<PublicHolidayModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<PublicHolidayModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<PublicHoliday>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<PublicHolidayModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new PublicHolidayModel
                    {
                        ID = f.ID,
                        HolidayDate = f.HolidayDate,
                        HolidayDescription = f.HolidayDescription,
                        IsActive = f.IsActive == "1"
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("Region/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<RegionModel>))]
        public IHttpActionResult GetRegionPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.Region>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .Regions
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<RegionModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<RegionModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Region>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<RegionModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new RegionModel
                    {
                        ID = f.ID,
                        Name = f.Name
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("Site/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<RegionModel>))]
        public IHttpActionResult GetSitePaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.Site>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .Sites
                    .Include(f => f.District)
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<SiteModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<SiteModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<Site>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<SiteModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new SiteModel
                    {
                        ID = f.ID,
                        Name = f.Name,
                        SiteTypeID = f.SiteTypeID,
                        DistrictID = f.DistrictID,
                        DistrictName = f.District == null ? string.Empty : f.District.BranchName
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("SystemFunction/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<SystemFunctionModel>))]
        public IHttpActionResult GetSystemFunctionPaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                //dbContext.Database.Log = (s) => Debug.WriteLine(s);

                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.SystemFunction>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);
                var query = dbContext
                    .SystemFunctions
                    .AsNoTracking();
                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<SystemFunctionModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<SystemFunctionModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .OrderByMember(PropertyHelper.GetSortingValue<SystemFunctionModel>(orderPropertyName))
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<SystemFunction>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities =  (asc ? pageResults.OrderBy(orderPropertyName) : pageResults.OrderByDescending(orderPropertyName)).ToList();
                }

                var paginationList = new PaginationListModel<SystemFunctionModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new SystemFunctionModel
                    {
                        ID = f.ID,
                        Name = f.Name,
                        Description = f.Description
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpPost]
        [Route("SystemRole/PaginatedList")]
        [ResponseType(typeof(PaginationListModel<SystemRoleModel>))]
        public IHttpActionResult GetSystemRolePaginatedList([FromBody] IList<FilterModel> filters, Kapsch.Gateway.Models.Shared.Enums.FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize, bool includeFunctions)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<Data.SystemRole>(filter, (Kapsch.Core.Filters.FilterJoin)filterJoin);

                var query = includeFunctions ?
                    dbContext.SystemRoles.Include(f => f.SystemRoleFunctions).AsNoTracking() :
                    dbContext.SystemRoles.AsNoTracking();

                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<SystemRoleModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<SystemRoleModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<SystemRole>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<SystemRoleModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;
                paginationList.Models = entities.Select(f =>
                    new SystemRoleModel
                    {
                        ID = f.ID,
                        Name = f.Name,
                        Description = f.Description,
                        SystemFunctions = 
                            includeFunctions ? 
                                f.SystemRoleFunctions.Select(g =>
                                    new SystemFunctionModel
                                    {
                                        ID = g.SystemFunction.ID,
                                        Name = g.SystemFunction.Name,
                                        Description = g.SystemFunction.Description
                                    })
                                    .ToList() : 
                                    null
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        [HttpGet]
        [Route("GeoLocation")]
        [ValidationActionFilter]
        [ResponseType(typeof(GoogleGeoCodeResponse))]
        public IHttpActionResult GetGeoLocation(string partialAddress)
        {
            if (partialAddress == null)
            {
                return this.BadRequestEx(Error.PopulateInvalidParameter("partialAddress", "Can not be empty."));
            }

            string url = String.Format(GoogleGeoSearchUrlByAddress, partialAddress);
            string responseString = string.Empty;
            GoogleGeoCodeResponse googleGeoCodeResponse = null;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                if (webResponse != null)
                {
                    using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responseString = streamReader.ReadToEnd();
                        googleGeoCodeResponse = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(responseString);
                    }

                    return Ok(googleGeoCodeResponse);
                }

                return this.BadRequestEx(Error.AddressInformationNotFound);
            }
            catch (WebException e)
            {
                return this.BadRequestEx(Error.PopulateUnexpectedException(e));
            }
        }

        [HttpGet]
        [Route("GeoLocation")]
        [ValidationActionFilter]
        [ResponseType(typeof(GoogleGeoCodeResponse))]
        public IHttpActionResult GetGeoLocation(string latitude, string longitude)
        {
            if ((latitude == null) || (longitude == null))
            {
                return this.BadRequestEx(Error.PopulateInvalidParameter("latitude and/or longitude", "Can not be empty."));
            }

            string url = string.Format(GoogleGeoSearchUrlByGps, latitude, longitude);
            string responseString = string.Empty;
            GoogleGeoCodeResponse googleGeoCodeResponse = null;

            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                if (webResponse != null)
                {
                    using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responseString = streamReader.ReadToEnd();
                        googleGeoCodeResponse = JsonConvert.DeserializeObject<GoogleGeoCodeResponse>(responseString);
                    }

                    return Ok(googleGeoCodeResponse);
                }

                return this.BadRequestEx(Error.AddressInformationNotFound);
            }
            catch (WebException e)
            {
                return this.BadRequest(Error.PopulateUnexpectedException(e));
            }
        }

        [HttpGet]
        [Route("IdentificationType")]
        [ResponseType(typeof(IList<IdentificationTypeModel>))]
        public IHttpActionResult GetIdentificationTypes()
        {
            using (var dbContext = new DataContext())
            {
                return Ok(
                    dbContext.IdentificationTypes
                        .AsNoTracking()
                        .ToList()
                        .Select(f => 
                            new IdentificationTypeModel 
                            { 
                                ID = f.ID,
                                Description = f.Description,
                                ModifiedDate = f.ModifiedDate
                            })
                        .ToList());
            }
        }
    }
}
