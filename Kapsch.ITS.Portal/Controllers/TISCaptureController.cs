
using Kapsch.ITS.Portal.Models;
using System.Web.Mvc;
using Kapsch.ITS.Gateway.Clients;
using System.Collections.Generic;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.Gateway.Models.Shared.Enums;
using System;
using Kapsch.ITS.Gateway.Models.TISCapture;
using System.Text;
using Kapsch.Core.Gateway.Models.Configuration;

namespace Kapsch.ITS.Portal.Controllers
{
    public class TISCaptureController : BaseController
    {
        //// GET: TISData
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateInput(false)]
        public FileResult ExportToCSV(long numberToExport, long districtID) //check of html string ook ingestuur moet word (soos by DataController)
        {
            var exports = new List<NatisExportModel>();
            try
            {
                var tisService = new TISService(AuthenticatedUser.SessionToken);
                exports = (List<NatisExportModel>)tisService.GetExports(numberToExport, districtID);
            }
            catch (GatewayException gex)
            {
                // Probeer dalk viewbag error message (en return dalk iets) wat van toepassing is:
                 ViewBag.ErrorMessage = gex.Message;
                RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Probeer dalk viewbag error message (en return dalk iets) wat van toepassing is:
                ViewBag.ErrorMessage = ex.Message;
                RedirectToAction("Index");
            }

            StringBuilder sbRtn = new StringBuilder();
            try
            {
                // As headers benodig word in die file:
                var header = string.Format("\"{0}\",{1},{2},{3},{4},{5}",
                                           "Reference Number",
                                           "Infringement Date",
                                           "Export Date",
                                           "Vehicle Registration",
                                           "Locked By Credential ID",
                                           "District ID"
                                          );
                sbRtn.AppendLine(header);

                foreach (var export in exports)
                {
                    var listResults = string.Format("\"{0}\",{1},{2},{3},{4},{5}",
                                                      makeCsvFriendly(export.ReferenceNumber),
                                                      makeCsvFriendly(export.InfringementDate),
                                                      makeCsvFriendly(export.ExportDate),
                                                      makeCsvFriendly(export.VehicleRegistration),
                                                      makeCsvFriendly(export.LockedByCredentialID),
                                                      makeCsvFriendly(export.DistrictID)
                                                     );
                    sbRtn.AppendLine(listResults);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                RedirectToAction("Index");
            }
            //File.AppendAllText("c:/Temp/CSVExport", sbRtn.ToString());
            return File(new System.Text.UTF8Encoding().GetBytes(sbRtn.ToString()), "text/csv", "CSVExport.csv");
        }

        private string makeCsvFriendly(object value)
        {
            if (value == null)
                return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }

            string output = value.ToString();
            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';
            return output;
        }

        public ActionResult TISCaptureList()
        {
            var userDistricts = AuthenticatedUser.UserData.Districts;
            var model = new DistrictSelectionModel();

            try
            {
                model.Districts.Add(new DistrictModel { ID = 0, BranchName = "-- SELECT DISTRICT --" });
                foreach (var entity in userDistricts)
                {
                    model.Districts.Add(entity);
                }
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        public ActionResult CaptureTISDetails(string referenceNumber, string vehicleRegistration, long lockedByCredentialID)
        {
            var currentUserID = AuthenticatedUser.UserData.ID;
            var tisService = new TISService(AuthenticatedUser.SessionToken);
            if (string.IsNullOrEmpty(lockedByCredentialID.ToString()) || lockedByCredentialID.Equals(currentUserID))
            {
                try
                {
                    if (string.IsNullOrEmpty(lockedByCredentialID.ToString()))                   
                    {
                        if (tisService.CaptureLock(referenceNumber, vehicleRegistration))
                        {
                            var initialTISList = new List<TISDataModel>();
                            var initialTISModel = new TISDataModel { ReferenceNumber = referenceNumber, VehicleRegistrationNumber = vehicleRegistration };
                            initialTISList.Add(initialTISModel);

                            return View(initialTISList);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Could not lock TIS instance.";
                            return RedirectToAction("TISCaptureList");
                        }
                    }
                    else
                    {
                        var initialTISList = new List<TISDataModel>();
                        var initialTISModel = new TISDataModel { ReferenceNumber = referenceNumber, VehicleRegistrationNumber = vehicleRegistration };
                        initialTISList.Add(initialTISModel);

                        return View(initialTISList);
                    }
                }
                catch (GatewayException gex)
                {
                    ViewBag.ErrorMessage = gex.Message;
                    return RedirectToAction("TISCaptureList");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return RedirectToAction("TISCaptureList");
                }
            }
            else
            return RedirectToAction("TISCaptureList");
        }

        [HttpPost]
        public ActionResult CaptureTISDetails (IList<TISDataModel> models)
        {
            var tisService = new TISService(AuthenticatedUser.SessionToken);

            if (!ModelState.IsValid)
            {
                return View(models);
            }

            try
            {
                tisService.CaptureTISData(models);

                return RedirectToAction("TISCaptureList");
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(models);
            }
  
        }

        public ActionResult SearchNatisExports(string sidx, string sord, int page, int rows, string vehicleRegistration, string referenceNumber, long districtID) //DateTime exportDate
        {
            var tisService = new TISService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            if (!string.IsNullOrWhiteSpace(vehicleRegistration))
                filters.Add(new FilterModel { PropertyName = "VehicleRegistration", Operation = Operation.StartsWith, Value = vehicleRegistration });

            if (!string.IsNullOrWhiteSpace(referenceNumber))
                filters.Add(new FilterModel { PropertyName = "ReferenceNumber", Operation = Operation.StartsWith, Value = referenceNumber });

            if (!string.IsNullOrWhiteSpace(districtID.ToString()))
                filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = districtID });

            //if (!string.IsNullOrWhiteSpace(exportDate.ToString()))
            //    filters.Add(new FilterModel { PropertyName = "ExportDate", Operation = Operation.Contains, Value = exportDate });

           var paginatedList = tisService.GetNatisExportPaginatedList(filters, FilterJoin.And, false, "InfringementDate", page, rows);
            var totalPages = Math.Ceiling((float)paginatedList.TotalCount / (float)rows);
            var jsonData = new
            {
                total = totalPages,
                page,
                records = paginatedList.TotalCount,
                rows = paginatedList.Models
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportSelection()
        {
            var userDistricts = AuthenticatedUser.UserData.Districts;
            var model = new DistrictSelectionModel();
            try
            {
                model.Districts.Add(new DistrictModel { ID = 0, BranchName = "-- SELECT DISTRICT --" });
                foreach (var entity in userDistricts)
                {
                    model.Districts.Add(entity);
                }
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

//        public ActionResult SearchTISData (string sidx, string sord, int page, int rows, string lastName, string IDNumber, string VLN)
//        {
//            var ownerService = new TISService(AuthenticatedUser.SessionToken);

////sorteer filters uit:
//            var filters = new List<FilterModel>();
//            if (!string.IsNullOrWhiteSpace(lastName))
//                filters.Add(new FilterModel { PropertyName = "LastName", Operation = Operation.StartsWith, Value = lastName });
//            if (!string.IsNullOrWhiteSpace(IDNumber))
//                filters.Add(new FilterModel { PropertyName = "IDNumber", Operation = Operation.StartsWith, Value = IDNumber });
//            if (!string.IsNullOrWhiteSpace(VLN))
//                filters.Add(new FilterModel { PropertyName = "VLN", Operation = Operation.StartsWith, Value = VLN });

//            var paginatedList = ownerService.GetTISDataPaginatedList(filters, FilterJoin.And, true, "LastName", page, rows);
//            var totalPages = Math.Ceiling((float)paginatedList.TotalCount / (float)rows);
//            var jsonData = new
//            {
//                total = totalPages,
//                page,
//                records = paginatedList.TotalCount,
//                rows = paginatedList.Models
//            };

//            return Json(jsonData, JsonRequestBehavior.AllowGet);
//        } 

//        public ActionResult ViewTISData (long id)
//        {
//            var ownerService = new TISService(AuthenticatedUser.SessionToken);
//            var model = ownerService.GetTISData(id);

//            return View(model);
//        }

//        public ActionResult UpdateOwnerDetails(TISDataModel updatedOwnerDetails)
//        {
//            var ownerService = new TISService(AuthenticatedUser.SessionToken);
//            ownerService.UpdateTISData(updatedOwnerDetails);

//            return View();
//        }

//Skep IMPORT service vir TIS details:

        //public ActionResult ImportDetails(IList<TISDataModel> tisDetailsImports)
        //{
        //    var ownerService = new OwnerService(AuthenticatedUser.SessionToken);
        //    var model = ownerService.GetImportedOwnersList;

        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        
//Sit soortgelyke Actions vir TISDETAILS by:

        //public ActionResult CreateUser()
        //{
        //    var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
        //    var filters = new List<FilterModel>();

        //    ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "ID", 1, 1000000).Models;
        //    ViewBag.SystemFunctions = configurationService.GetSystemFunctionPaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000).Models;
        //    ViewBag.SystemRoles = configurationService.GetSystemRolePaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000, false).Models;

        //    return View(new UserModel());
        //}

        //[HttpPost]
        //public ActionResult CreateUser(UserModel model, FormCollection formCollection)
        //{
        //    var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
        //    var filters = new List<FilterModel>();

        //    ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "ID", 1, 1000000).Models;
        //    ViewBag.SystemFunctions = configurationService.GetSystemFunctionPaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000).Models;
        //    ViewBag.SystemRoles = configurationService.GetSystemRolePaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000, false).Models;

        //    if (formCollection.AllKeys.Contains("districts_"))
        //        model.Districts = formCollection["districts_"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(f => new DistrictModel { ID = long.Parse(f) }).ToList();

        //    model.SystemFunctions = formCollection.AllKeys.Where(f => f.Contains("systemFunction_")).Select(f => new SystemFunctionModel { ID = long.Parse(f.Replace("systemFunction_", string.Empty)) }).ToList();

        //    if (model.IsOfficer && string.IsNullOrWhiteSpace(model.ExternalID))
        //        ModelState.AddModelError("ExternalID", "The External ID filed required when user is an officer.");

        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    try
        //    {
        //        var userService = new UserService(AuthenticatedUser.SessionToken);
        //        var userID = userService.CreateUser(model).ID;
        //    }
        //    catch (GatewayException ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);

        //        return View(model);
        //    }

        //    return RedirectToAction("Users");

        //}
    }
}