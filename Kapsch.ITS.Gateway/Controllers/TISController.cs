using System;
using System.Collections.Generic;
using System.Data;
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
using Kapsch.ITS.Gateway.Models.TISCapture;
using Kapsch.ITS.Repositories;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/TIS")]
    public class TISController : BaseController
    {
        [HttpGet]
        [SessionAuthorize]
        [Route("Export")]
        [ResponseType(typeof(IList<NatisExportModel>))]
        public IHttpActionResult Exports (long numberToExport, long districtID)
        {
            var models = new List<NatisExportModel>();

            if (numberToExport <= 0)
                return Ok(models);

            using (var dbContext = new DataContext())
            {
                var exports = new List<TISRepository.NatisExport>();
                var tisRepository = new TISRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!tisRepository.GetNatisExports(numberToExport, districtID, out exports))
                {
                    return this.BadRequestEx(Error.PopulateMethodFailed(tisRepository.Error));
                }

                foreach (var export in exports)
                {
                    var model = new NatisExportModel();
                    model.ReferenceNumber = export.ReferenceNumber;
                    model.InfringementDate = export.InfringementDate;
                    model.ExportDate = export.ExportDate;
                    model.VehicleRegistration = export.VehicleRegistration;
                    model.LockedByCredentialID = export.LockedByCredentialID;
                    model.DistrictID = export.DistrictID;
                    models.Add(model);
                }
            }
            return Ok(models);
            
                //As EF alleenlik gebruik word(???):
                //var query = dbContext.NatisExports
                //    .AsNoTracking()
                //    .Take(numberToExport)
                //    .ToList();

                //var offences = query.OrderByDescending(f => f.InfringementDate).ToList();     
        }
        
        [HttpPut]
        [SessionAuthorize]
        [Route("LockCapture")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult LockCapture(string referenceNumber, string vehicleRegistration)
        {
            bool isSuccessful;
            using (var dbContext = new DataContext())
            {
                var tisRepository = new TISRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!tisRepository.CaptureLock(referenceNumber, vehicleRegistration, out isSuccessful))
                {
                    return this.BadRequest(Error.PopulateMethodFailed(tisRepository.Error));
                }

                return Ok(isSuccessful);
            }
        }

        [Route("CaptureTISData")]
        [HttpPost]
        [SessionAuthorize]
        //[ResponseType(typeof(bool))]
        public IHttpActionResult TISDataCapture(IList<TISDataModel> models)
        {
            using (var dbContext = new DataContext())
            {
                var tisRepository = new TISRepository(dbContext, SessionModel.CredentialID, SessionModel.UserName);

                if (!tisRepository.CaptureTIS(models))
                {
                    return this.BadRequest(Error.PopulateMethodFailed(tisRepository.Error));
                }

                return Ok();
            }
        }

        [HttpPost]
        [SessionAuthorize]
        [UsageLog]
        [Route("TISPaginatedList")]
        [ResponseType(typeof(PaginationListModel<NatisExportModel>))]
        public IHttpActionResult TISPaginatedList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        {
            using (var dbContext = new DataContext())
            {
                var totalCount = 0;
                var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
                var func = ExpressionBuilder.GetExpression<NatisExport>(filter, (FilterJoin)filterJoin);
                var query = dbContext
                    .NatisExports
                    .AsNoTracking();

                if (func != null)
                    query = query.Where(func);

                var orderedQuery = asc ?
                    query.OrderByMember(PropertyHelper.GetSortingValue<NatisExportModel>(orderPropertyName)) :
                    query.OrderByMemberDescending(PropertyHelper.GetSortingValue<NatisExportModel>(orderPropertyName));

                var resultsToSkip = (pageIndex - 1) * pageSize;
                var pageResults = orderedQuery
                    .Skip(resultsToSkip)
                    .Take(pageSize)
                    .GroupBy(f => new { Total = query.Count() })
                    .FirstOrDefault();

                var entities = new List<NatisExport>();

                if (pageResults != null)
                {
                    totalCount = pageResults.Key.Total;
                    entities = pageResults.ToList();
                }

                var paginationList = new PaginationListModel<NatisExportModel>();
                paginationList.PageIndex = pageIndex;
                paginationList.PageSize = pageSize;

                var totalPages = Math.Ceiling(totalCount / (float)pageSize);               

                paginationList.Models = entities.Select(f =>
                    new Models.TISCapture.NatisExportModel
                    {
                        //ID = f.ID,
                        VehicleRegistration = f.VehicleRegistration,
                        InfringementDate = f.InfringementDate,
                        ReferenceNumber = f.ReferenceNumber,
                        ExportDate = f.ExportDate,
                        DistrictID = f.DistrictID,
                        LockedByCredentialID = f.LockedByCredentialID,
                        LockedByName = f.LockedByCredentialID.HasValue ? string.Format("{0}, {1}", f.User.LastName, f.User.FirstName) : string.Empty,
                        DistrictName = f.District.BranchName,
                       
                    })
                    .ToList();
                paginationList.TotalCount = totalCount;

                return Ok(paginationList);
            }
        }

        //[HttpPost]
        //[SessionAuthorize]
        //[UsageLog]
        //[Route("TISPaginatedList")]
        //[ResponseType(typeof(PaginationListModel<TISDataModel>))]
        //public IHttpActionResult TISPaginatedList([FromBody] IList<FilterModel> filters, FilterJoin filterJoin, bool asc, string orderPropertyName, int pageIndex, int pageSize)
        //{
        //    using (var dbContext = new DataContext())
        //    {
        //        var totalCount = 0;
        //        var filter = filters.Select(f => FilterConverter.Convert(f)).ToList();
        //        var func = ExpressionBuilder.GetExpression<TISData>(filter, (FilterJoin)filterJoin);
        //        var query = dbContext
        //            .TISData
        //            .AsNoTracking();

        //        if (func != null)
        //            query = query.Where(func);

        //        var orderedQuery = asc ?
        //            query.OrderByMember(PropertyHelper.GetSortingValue<TISDataModel>(orderPropertyName)) :
        //            query.OrderByMemberDescending(PropertyHelper.GetSortingValue<TISDataModel>(orderPropertyName));

        //        var resultsToSkip = (pageIndex - 1) * pageSize;
        //        var pageResults = orderedQuery
        //            .Skip(resultsToSkip)
        //            .Take(pageSize)
        //            .GroupBy(f => new { Total = query.Count() })
        //            .FirstOrDefault();

        //        var entities = new List<TISData>();

        //        if (pageResults != null)
        //        {
        //            totalCount = pageResults.Key.Total;
        //            entities = pageResults.ToList();
        //        }

        //        var paginationList = new PaginationListModel<TISDataModel>();
        //        paginationList.PageIndex = pageIndex;
        //        paginationList.PageSize = pageSize;

        //        var ownerIDs = entities.Select(f => f.OwnerID).ToList();
        //        var credentials = dbContext.Credentials
        //            .Where(f => /*f.EntityType == Kapsch.Core.Data.Enums.EntityType.TISData &&*/ ownerIDs.Contains(f.EntityID.ToString()))
        //            .ToList();

        //        var models = new List<TISDataModel>();
        //        foreach (var entity in entities)
        //        {
        //            var ownerModel =
        //                new TISDataModel
        //                {
        //                    ReferenceNumber = entity.ReferenceNumber,
        //                    OwnerID = entity.OwnerID,
        //                    OwnerIDType = entity.OwnerIDType,
        //                    Surname = entity.Surname,
        //                    OwnerTelephone = entity.OwnerTelephone,
        //                    VehicleModel = entity.VehicleModel,
        //                    VehicleMake = entity.VehicleMake,
        //                    VehicleModelID = entity.VehicleModelID,
        //                    VehicleType = entity.VehicleType,
        //                    VehicleUsageID = entity.VehicleUsageID,
        //                    VehicleColourID = entity.VehicleColourID,
        //                    LicenseExpireDate = entity.LicenseExpireDate,
        //                    YearOfMake = entity.YearOfMake,
        //                    ClearanceCertificateNumber = entity.ClearanceCertificateNumber,
        //                    OwnerInitials = entity.OwnerInitials,
        //                    OwnerGender = entity.OwnerGender,
        //                    OwnerCompany = entity.OwnerCompany,
        //                    DateOfOwner = entity.DateOfOwner,
        //                    ImportFileName = entity.ImportFileName,
        //                    NatureOfOwnership = entity.NatureOfOwnership,
        //                    VehicleRegistrationNumber = entity.VehicleRegistrationNumber,
        //                    OwnerName = entity.OwnerName,
        //                    //Email = entity.Email,
        //                    OwnerCellphone = entity.OwnerCellphone,
        //                    OwnerPostal = entity.OwnerPostal,
        //                    OwnerPostalStreet = entity.OwnerPostalStreet,
        //                    OwnerPostalSuburb = entity.OwnerPostalSuburb,
        //                    OwnerPostalTown = entity.OwnerPostalTown,
        //                    PostalCode = entity.PostalCode,
        //                    OwnerPhysical = entity.OwnerPhysical,
        //                    OwnerPhysicalStreet = entity.OwnerPhysicalStreet,
        //                    OwnerPhysicalSuburb = entity.OwnerPhysicalSuburb,
        //                    OwnerPhysicalTown = entity.OwnerPhysicalTown,
        //                    PhysicalCode = entity.PhysicalCode,
        //                    ProxyIndicator = entity.ProxyIndicator
        //                };

        //            var credential = credentials.FirstOrDefault(f => f.EntityID.ToString() == ownerModel.OwnerID);

        //            //if (credential != null)
        //            //{
        //            //    userModel.UserName = credential.UserName;
        //            //}

        //            models.Add(ownerModel);
        //        }
        //        paginationList.Models = models;
        //        paginationList.TotalCount = totalCount;

        //        return Ok(paginationList);
        //    }
        //}
    }
}

//// GET: TISData
//[HttpGet]
//[SessionAuthorize]
//[Route("TISData")]
//[ResponseType(typeof(IList<TISDataModel>))]
//public IHttpActionResult Index()
//{
//    using (var db = new DataContext())
//    {
//        return Ok(db.TISData.ToList());
//    }
//}

//[HttpGet]
//[SessionAuthorize]
//[UsageLog]
//[ResponseType(typeof(TISDataModel))]
//// GET: Owners/Details/5
//public IHttpActionResult Details(long? id)
//{
//    using (var db = new DataContext())
//    {
//        if (id == null)
//        {
//            return this.BadRequestEx(Error.CredentialNotFound);
//        }
//        TISData owner = db.TISData.Find(id);
//        if (owner == null)
//        {
//            return this.BadRequestEx(Error.CredentialNotActive);
//        }
//        return Ok(owner);
//    }
//}

//// GET: Owners/Create
////public IHttpActionResult Create()
////{
////    return View();
////}

//// POST: Owners/Create
//// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//[HttpPost]
//[ValidationActionFilter]
//[SessionAuthorize]
//[UsageLog]
//[ResponseType(typeof(TISDataModel))]
//public IHttpActionResult Post([FromBody] TISData owner)
//{
//    using (var db = new DataContext())
//    {
//        if (ModelState.IsValid)
//        {
//            db.TISData.Add(owner);
//            db.SaveChanges();
//        }

//        return Ok(owner);
//    }
//}

// [HttpPut]
// [ValidationActionFilter]
// [SessionAuthorize]
// [UsageLog]
// [ResponseType(typeof(TISDataModel))]
// // GET: Owners/Edit/5
// public IHttpActionResult Edit([FromBody] TISDataModel model)
//{
//     using (var db = new DataContext())
//     {
//         if (model == null)
//         {
//             return this.BadRequestEx(Error.CredentialNotFound);
//         }
//         TISData owner = db.Owners.Find(model.ID);
//         if (owner == null)
//         {
//             return this.BadRequestEx(Error.CredentialNotActive);
//         }

//         db.Owners.Remove(owner);

//         var updatedOwnerDetails = new TISData();
//         updatedOwnerDetails.ID = model.ID;
//         updatedOwnerDetails.IDNumber = model.IDNumber;
//         updatedOwnerDetails.IDType = model.IDType;
//         updatedOwnerDetails.LastName = model.LastName;
//         updatedOwnerDetails.MobileNumber = model.MobileNumber;
//         updatedOwnerDetails.PostalCode = model.PostalCode;
//         updatedOwnerDetails.VehicleModel = model.VehicleModel;
//         updatedOwnerDetails.VehicleMake = model.VehicleMake;
//         updatedOwnerDetails.VLN = model.VLN;
//         updatedOwnerDetails.FullNames = model.FullNames;
//         updatedOwnerDetails.Email = model.Email;
//         updatedOwnerDetails.AltPhoneNumber = model.AltPhoneNumber;
//         updatedOwnerDetails.Address1 = model.Address1;
//         updatedOwnerDetails.Address2 = model.Address2;

//         db.Owners.Add(updatedOwnerDetails);

//         db.SaveChanges();

//         return Ok();
//     }
// }

//// POST: Owners/Edit/5
//// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//[HttpPut]
//[ValidationActionFilter]
//[SessionAuthorize]
//[UsageLog]
//public IHttpActionResult Update([FromBody] TISData owner)
//{
//    using (var db = new DataContext())
//    {
//        if (ModelState.IsValid)
//        {
//            db.Entry(owner).State = EntityState.Modified;
//            db.SaveChanges();
//            //return RedirectToAction("Index");
//        }
//        return Ok(owner);
//    }
//}

//// GET: Owners/Delete/5
//public IHttpActionResult Delete(long? id)
//{
//    using (var db = new DataContext())
//    {
//        if (id == null)
//        {
//            return this.BadRequestEx(Error.CredentialNotFound);
//        }

//        TISData owner = db.TISData.Find(id);
//        if (owner == null)
//        {
//            return this.BadRequestEx(Error.CredentialNotFound);
//        }

//        return Ok(owner);
//    }
//}

//// POST: Owners/Delete/5
//[HttpPost, ActionName("Delete")]

//public IHttpActionResult DeleteConfirmed(long id)
//{
//    using (var db = new DataContext())
//    {
//        TISData owner = db.TISData.Find(id);
//        db.TISData.Remove(owner);
//        db.SaveChanges();
//        return Ok();
//    }
//}

//protected override void Dispose(bool disposing)
//{
//    using (var db = new DataContext())
//    {
//        if (disposing)
//        {
//            db.Dispose();
//        }
//        base.Dispose(disposing);
//    }
//}