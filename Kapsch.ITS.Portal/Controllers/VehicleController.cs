using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Payment;
using Kapsch.EVR.Gateway.Models.Vehicle;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Fine;
using Kapsch.ITS.Portal.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kapsch.EVR.Gateway.Clients;

namespace Kapsch.ITS.Portal.Controllers
{
    public class VehicleController : BaseController
    {
        //// GET: Vehicle
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VehicleDetails(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var model = vehicleService.GetTestResultsByBookingID(ID);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ListVehicles(FormCollection collection, string sidx, string sord, int page, int rows)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            
            
            var filters = new List<FilterModel>();
                       
            var model = new BookingSearchTypeModel();

            model.TestCategoryID = -1;
            model.IsPassed = -1;
            model.EngineNumber = collection["engineNumber"] == "" ? "NONE" : collection["engineNumber"];
            model.VehicleIdentificationNumber = collection["vehicleIDNumber"] == "" ? "NONE" : collection["vehicleIDNumber"];
            model.VLN = "NONE";
            model.BookingReference = "NONE";
            model.DateIndicator = 0;
            model.Quantity = 10;
            model.PageNumber = 1;

            if (model.DateIndicator == 1)
            {
                if (collection["testBookingDate"] != "")
                {
                    model.BookingDate = collection["testBookingDate"];
                }
            }

            var paginatedList = vehicleService.GetBookingTestResultsPaginatedList(model);
            //var paginatedList = vehicleService.GetBookingTestResultsPaginatedList(filters, FilterJoin.And, true, "ID", page, rows);
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


        public ActionResult ListBookings()
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            var filters = new List<FilterModel>();
            var vehicleModels = vehicleService.GetBookingsPaginatedList(filters, FilterJoin.And, true, "ID", 1, 10);
            return View(vehicleModels);
        }


        public ActionResult CreateVehicle()
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var model = new VehicleBookingModel();
            var userDistricts = AuthenticatedUser.UserData.Districts;


            model.Districts.Add(new Core.Gateway.Models.Configuration.DistrictModel { ID = 0, BranchName = "-- SELECT DISTRICT --" });
            foreach (var entity in userDistricts)
            {
                model.Districts.Add(entity);
            }


            model.Sites.Add(new Core.Gateway.Models.Configuration.SiteModel() { ID = 0, Name = "-- SELECT SITE --" });
            foreach (var entity in vehicleService.GetSites(0))
            {
                model.Sites.Add(entity);
            }


            model.TestCategories.Add(new TestCategoryModel() { ID = 0, Name = "-- SELECT TEST CATEGORY --" });
            foreach (var entity in vehicleService.GetTestCategories())
            {
                model.TestCategories.Add(entity);
            }

            //model.VehicleTypes.Add(new VehicleTypeModel() { ID = 0, Description = "-- SELECT VEHICLE TYPE --" });
            foreach (var entity in vehicleService.GetVehicleTypes())
            {
                model.VehicleTypes.Add(entity);
            }

            //model.VehicleMakes.Add(new VehicleMakeModel() { ID = 0, Description = "-- SELECT VEHICLE MAKE --" });
            foreach (var entity in vehicleService.GetMakes())
            {
                model.VehicleMakes.Add(entity);
            }

            //model.VehicleModels.Add(new VehicleModelModel() { ID = 0, Description = "-- SELECT VEHICLE MODEL --" });
            foreach (var entity in vehicleService.GetModels(0))
            {
                model.VehicleModels.Add(entity);
            }

            //model.VehicleModelNumbers.Add(new VehicleModelNumberModel() { ID = 0, Description = "-- SELECT MODEL NUMBER --" });
            foreach (var entity in vehicleService.GetModelNumbers(0))
            {
                model.VehicleModelNumbers.Add(entity);
            }

           // model.VehicleCategories.Add(new VehicleCategoryModel() { ID = 0, Description = "-- SELECT VEHICLE CATEGORY --" });
            foreach (var entity in vehicleService.GetCategories())
            {
                model.VehicleCategories.Add(entity);
            }

            //model.VehiclePropellers.Add(new VehiclePropellerModel() { ID = 0, Description = "-- SELECT PROPELLED BY --" });
            foreach (var entity in vehicleService.GetPropellers())
            {
                model.VehiclePropellers.Add(entity);
            }

            foreach (var entity in vehicleService.GetFuelType())
            {
                model.VehicleFuelType.Add(entity);
            }

            //model.VehicleColors.Add(new VehicleColorModel() { ID = 0, Description = "-- SELECT VEHICLE COLOR --" });
            foreach (var entity in vehicleService.GetVehicleColors())
            {
                model.VehicleColors.Add(entity);
            }

            return View(model);
        }


        public ActionResult Model()
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);            
            var vehicleMakeID = Convert.ToInt32(Request["vehicleMakeDropDownList"]);
            var model = new VehicleBookingModel();
            return View(model);
        }


        public ActionResult Booking(FormCollection collection)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var model = new VehicleBookingRecordModel();

            try
            {

                model.BookingReference = collection["BookingReference"].ToString();
                model.CredentialID = (int)AuthenticatedUser.SessionData.CredentialID;
                model.SiteId = Convert.ToInt32(collection["site"]);
                model.TestCategoryID = Convert.ToInt32(collection["testCategory"]);
                model.CapturedByCredentialId = 1;
                model.CapturedDate = DateTime.Now;

                model.VehicleDetails.ColourID = Convert.ToInt32(collection["Vehicle.VehicleColourId"]);
                model.VehicleDetails.EngineNumber = collection["Vehicle.EngineNumber"].ToString();
                model.VehicleDetails.GVM = Convert.ToInt32(collection["Vehicle.GVM"]);

                model.VehicleDetails.InsuranceExpiryDate = collection["InsuranceExpiryDate_"].ToString();
                model.VehicleDetails.LicenceExpiryDate = collection["LicenceExpiryDate_"].ToString();
                model.VehicleDetails.RoadworthyExpiryDate = collection["RoadworthyExpiryDate_"].ToString();

                model.VehicleDetails.NetWeight = Convert.ToInt32(collection["Vehicle.NetWeight"]);
                model.VehicleDetails.PropelledByID = Convert.ToInt32(collection["Vehicle.PropelledById"]);
                model.VehicleDetails.RegistrationStatusID = 1;

                model.VehicleDetails.VehicleCategoryID = Convert.ToInt32(collection["Vehicle.VehicleCategoryId"]);
                model.VehicleDetails.VehicleMakeID = Convert.ToInt32(collection["Vehicle.VehicleMakeId"]);
                model.VehicleDetails.VehicleModelID = Convert.ToInt32(collection["Vehicle.VehicleModelId"]);
                model.VehicleDetails.VehicleModelNumberID = Convert.ToInt32(collection["Vehicle.VehicleModelNumberId"]);
                model.VehicleDetails.VehicleTypeID = Convert.ToInt32(collection["Vehicle.VehicleTypeId"]);
                model.VehicleDetails.VIN = collection["Vehicle.VIN"].ToString();
                model.VehicleDetails.VLN = collection["Vehicle.VLN"].ToString();
                model.VehicleDetails.YearOfMake = Convert.ToInt32(collection["Vehicle.YearOfMake"]);
                model.VehicleDetails.SeatingCapacity = Convert.ToInt64(collection["Vehicle.SeatingCapacity"]);

                model = vehicleService.AddBooking(model);

                return Json(new { IsValid = true, model });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }

            //return RedirectToAction("Index");
        }


        public JsonResult GetModelsByMake(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            var model = new List<SelectListItem>();

            //model.Add(new SelectListItem() { Text = "--SELECT MODEL--", Value = "0" });

            foreach (var entity in vehicleService.GetModels(ID))
            {
                model.Add(new SelectListItem() { Text = entity.Description, Value = entity.ID.ToString()});
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSites(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var model = new List<SelectListItem>();

            model.Add(new SelectListItem() { Text = "--SELECT SITE--", Value = "0" });

            foreach (var entity in vehicleService.GetSites(ID))
            {
                model.Add(new SelectListItem() { Text = entity.Name, Value = entity.ID.ToString()});
            }

            HttpCookie districtID = new HttpCookie("DistrictID");
            districtID.Value = ID.ToString();
            this.ControllerContext.HttpContext.Response.Cookies.Add(districtID);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult GetModelNumberByModel(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            var model = new List<SelectListItem>();

            //model.Add(new SelectListItem() { Text = "--SELECT MODEL NUMBER--", Value = "0" });

            foreach (var entity in vehicleService.GetModelNumbers(ID))
            {
                model.Add(new SelectListItem() { Text = entity.Description, Value = entity.ID.ToString() });
            }
            
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetTestResultByID(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var model = vehicleService.GetTestResultsByBookingID(ID);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewResults()
        {   
            return View();
        }

        public ActionResult SetSite(int ID)
        {   
            HttpCookie siteID = new HttpCookie("SiteID");
            siteID.Value = ID.ToString();
            this.ControllerContext.HttpContext.Response.Cookies.Add(siteID);

            return View();
        }

        public ActionResult BookingResult(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var model = vehicleService.GetVehicleTest(ID);

            return View(model);
        }

        public ActionResult TestResults()
        {

            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var model = new TestResultsSearchModel();


            model.TestCategories.Add(new TestCategoryModel() { ID = -1, Name = "-- SELECT TEST CATEGORY --" });
            foreach (var entity in vehicleService.GetTestCategories())
            {
                model.TestCategories.Add(entity);
            }

            return View(model);
        }

        public ActionResult ListBookingResults(string sidx, string sord, int page, int rows)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            var filters = new List<FilterModel>();
            var paginatedList = vehicleService.GetBookingsPaginatedList(filters, FilterJoin.And, true, "ID", page, rows);
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

        public ActionResult ListTestResults(FormCollection collection)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);


            var model = new BookingSearchTypeModel();

            model.TestCategoryID = Convert.ToInt32(collection["testCategory"]);
            model.IsPassed = Convert.ToInt32(collection["result"]);
            model.EngineNumber = collection["engineNumber"] == "" ? "NONE" : collection["engineNumber"];
            model.VehicleIdentificationNumber = collection["VIN"] == "" ? "NONE" : collection["VIN"];
            model.VLN = collection["VLN"] == "" ? "NONE" : collection["VLN"];
            model.BookingReference = collection["bookingReference"] == "" ? "NONE" : collection["bookingReference"]; 
            model.DateIndicator = Convert.ToInt32(collection["dateIndicator"]);
            model.Quantity = 10;
            model.PageNumber = 1;

            if (model.DateIndicator == 1)
            {
                if (collection["testBookingDate"] != "")
                {
                    model.BookingDate = collection["testBookingDate"];
                }
            }

            var rows = 10;
            var page = 1;
            var filters = new List<FilterModel>();
            var paginatedList = vehicleService.GetBookingTestResultsPaginatedList(model);

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

        public ActionResult GetVehicleMake(long id)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var vehicleMake = vehicleService.GetMake(id);

            return Json(vehicleMake, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetVehicleTypes()
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            var model = new List<SelectListItem>();

            model.Add(new SelectListItem() { Text = "--SELECT VEHICLE TYPES--", Value = "0" });

            foreach (var entity in vehicleService.GetVehicleTypes())
            {
                model.Add(new SelectListItem() { Text = entity.Description, Value = entity.ID.ToString() });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVehicleModel(long id)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var vehicleMake = vehicleService.GetModel(id);

            return Json(vehicleMake, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVehicleModelNumber(long id)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var vehicleMake = vehicleService.GetModelNumber(id);

            return Json(vehicleMake, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SearchVehicles(string webVin, string webEngineNumber)  //, 
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            string engineNumber = webEngineNumber;
            string vin = webVin;

            var filters = new List<FilterModel>();

            if (!string.IsNullOrWhiteSpace(engineNumber))
            {
                var engineNumberFilterModel = new FilterModel();
                engineNumberFilterModel.PropertyName = "EngineNumber";
                engineNumberFilterModel.Value = engineNumber;
                filters.Add(engineNumberFilterModel);
            }

            if (!string.IsNullOrWhiteSpace(webVin))
            {
                var vehicleIdentificationNumberFilterModel = new FilterModel();
                vehicleIdentificationNumberFilterModel.PropertyName = "VehicleIDNumber";
                vehicleIdentificationNumberFilterModel.Value = vin;
                filters.Add(vehicleIdentificationNumberFilterModel);
            }

            var vehicleDetail = vehicleService.GetVehicleDetail(filters, FilterJoin.And);
            var jsonData = new
            {
                vehicleDetail
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}