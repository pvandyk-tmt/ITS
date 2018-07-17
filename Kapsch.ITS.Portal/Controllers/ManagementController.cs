using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Computer;
using Kapsch.Core.Gateway.Models.Configuration;
using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.Core.Gateway.Models.Payment;
using Kapsch.Core.Gateway.Models.User;
using Kapsch.EVR.Gateway.Clients;
using Kapsch.EVR.Gateway.Models.Vehicle;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Portal.Controllers
{
    public class ManagementController : BaseController
    {
        // GET: Management
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lookups()
        {
            return RedirectToAction("VehicleLookups");
        }

        public ActionResult VehicleLookups()
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var vehicleMakes = new List<VehicleMakeModel>();
            vehicleMakes.Add(new VehicleMakeModel() { ID = 0, Description = "Select One" });
            vehicleMakes.AddRange(vehicleService.GetMakes());

            ViewBag.VehicleMakes = vehicleMakes;

            return View();
        }

        public ActionResult GetVehicleMake(long id)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
            var vehicleMake = vehicleService.GetMake(id);

            return Json(vehicleMake, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetModelsByMake(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            var model = new List<SelectListItem>();

            model.Add(new SelectListItem() { Text = "Select One", Value = "0" });

            foreach (var entity in vehicleService.GetModels(ID))
            {
                model.Add(new SelectListItem() { Text = entity.Description, Value = entity.ID.ToString() });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetModelNumberByModel(int ID)
        {
            var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);

            var model = new List<SelectListItem>();

            model.Add(new SelectListItem() { Text = "Select One", Value = "0" });

            foreach (var entity in vehicleService.GetModelNumbers(ID))
            {
                model.Add(new SelectListItem() { Text = entity.Description, Value = entity.ID.ToString() });
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddVehicleMake(VehicleMakeModel model)
        {
            try
            {
                var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
                vehicleService.AddMake(model);

                var models = vehicleService.GetMakes();
                models.Insert(0, new VehicleMakeModel() { ID = 0, Description = "Select One" });

                return Json(new { IsValid = true, Data = models.Select(f => new SelectListItem { Text = f.Description, Value = f.ID.ToString() }).ToList() });
            }
            catch (GatewayException gex)
            {
                return Json(new { IsValid = false, ErrorMessage = gex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateVehicleMake(VehicleMakeModel model)
        {
            try
            {
                var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
                vehicleService.UpdateMake(model);

                var models = vehicleService.GetMakes();
                models.Insert(0, new VehicleMakeModel() { ID = 0, Description = "Select One" });

                return Json(new { IsValid = true, Data = models.Select(f => new SelectListItem { Text = f.Description, Value = f.ID.ToString() }).ToList() });
            }
            catch (GatewayException gex)
            {
                return Json(new { IsValid = false, ErrorMessage = gex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult AddVehicleModel(VehicleModelModel model)
        {
            try
            {
                var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
                vehicleService.AddModel(model);

                var models = vehicleService.GetModels(model.VehicleMakeID);
                models.Insert(0, new VehicleModelModel() { ID = 0, Description = "Select One" });

                return Json(new { IsValid = true, Data = models.Select(f => new SelectListItem { Text = f.Description, Value = f.ID.ToString() }).ToList() });
            }
            catch (GatewayException gex)
            {
                return Json(new { IsValid = false, ErrorMessage = gex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateVehicleModel(VehicleModelModel model)
        {
            try
            {
                var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
                vehicleService.UpdateModel(model);

                var models = vehicleService.GetModels(model.VehicleMakeID);
                models.Insert(0, new VehicleModelModel() { ID = 0, Description = "Select One" });

                return Json(new { IsValid = true, Data = models.Select(f => new SelectListItem { Text = f.Description, Value = f.ID.ToString() }).ToList() });
            }
            catch (GatewayException gex)
            {
                return Json(new { IsValid = false, ErrorMessage = gex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult AddVehicleModelNumber(VehicleModelNumberModel model)
        {
            try
            {
                var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
                vehicleService.AddModelNumber(model);

                var models = vehicleService.GetModelNumbers(model.VehicleModelID);
                models.Insert(0, new VehicleModelNumberModel() { ID = 0, Description = "Select One" });

                return Json(new { IsValid = true, Data = models.Select(f => new SelectListItem { Text = f.Description, Value = f.ID.ToString() }).ToList() });
            }
            catch (GatewayException gex)
            {
                return Json(new { IsValid = false, ErrorMessage = gex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult UpdateVehicleModelNumber(VehicleModelNumberModel model)
        {
            try
            {
                var vehicleService = new VehicleService(AuthenticatedUser.SessionToken);
                vehicleService.UpdateModelNumber(model);

                var models = vehicleService.GetModelNumbers(model.VehicleModelID);
                models.Insert(0, new VehicleModelNumberModel() { ID = 0, Description = "Select One" });

                return Json(new { IsValid = true, Data = models.Select(f => new SelectListItem { Text = f.Description, Value = f.ID.ToString() }).ToList() });
            }
            catch (GatewayException gex)
            {
                return Json(new { IsValid = false, ErrorMessage = gex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsValid = false, ErrorMessage = ex.Message });
            }
        }

        public ActionResult Computers()
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "BranchName", 1, 1000000).Models;

            return View(new PaginationListModel<ComputerModel> { TotalCount = 0, Models = new List<ComputerModel>() });
        }

        public ActionResult SearchComputers(string sidx, string sord, int page, int rows, string name, long? districtID)
        {
            var computerService = new ComputerService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();
            if (!string.IsNullOrWhiteSpace(name))
                filters.Add(new FilterModel { PropertyName = "Name", Operation = Operation.StartsWith, Value = name });
            if (districtID.HasValue)
                filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = districtID.Value });

            var paginatedList = computerService.GetPaginatedList(filters, FilterJoin.And, true, "Name", page, rows);
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

        public ActionResult ViewComputer(long id)
        {
            var computerService = new ComputerService(AuthenticatedUser.SessionToken);
            var model = computerService.GetComputer(id);

            return View(model);
        }

        public ActionResult EditComputer(long id)
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "BranchName", 1, 1000000).Models;
            
            var computerService = new ComputerService(AuthenticatedUser.SessionToken);
            var model = computerService.GetComputer(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditComputer(ComputerModel model)
        {
            if (!ModelState.IsValid)
            {
                var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
                var filters = new List<FilterModel>();

                ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "BranchName", 1, 1000000).Models;
                
                return View(model);
            }

            var computerService = new ComputerService(AuthenticatedUser.SessionToken);
            computerService.UpdateComputer(model);

            return RedirectToAction("Computers");
        }

        public ActionResult CreateComputerSetting(long computerID)
        {
            return View(new ComputerConfigSettingModel { ComputerID = computerID });
        }

        [HttpPost]
        public ActionResult CreateComputerSetting(ComputerConfigSettingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var computerService = new ComputerService(AuthenticatedUser.SessionToken);
                computerService.CreateComputerSetting(model);
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }

            return RedirectToAction("ViewComputer", new { id = model.ComputerID });
        }

        public ActionResult EditComputerSetting(long id)
        {
            var computerService = new ComputerService(AuthenticatedUser.SessionToken);
            var model = computerService.GetComputerSetting(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditComputerSetting(ComputerConfigSettingModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var computerService = new ComputerService(AuthenticatedUser.SessionToken);
                computerService.UpdateComputerSetting(model);
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }

            return RedirectToAction("ViewComputer", new { id = model.ComputerID });
        }

        public ActionResult CreateComputer()
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "BranchName", 1, 1000000).Models;
            
            return View(new ComputerModel());
        }

        [HttpPost]
        public ActionResult CreateComputer(ComputerModel model)
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "BranchName", 1, 1000000).Models;
          
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var computerService = new ComputerService(AuthenticatedUser.SessionToken);
                computerService.CreateComputer(model);
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }

            return RedirectToAction("Computers");
        }

        public ActionResult Users()
        {
            return View(new PaginationListModel<UserModel> { TotalCount = 0, Models = new List<UserModel>() });           
        }

        public ActionResult SearchUsers(string sidx, string sord, int page, int rows, string lastName, string externalID)
        {
            var userService = new UserService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();
            if (!string.IsNullOrWhiteSpace(lastName))
                filters.Add(new FilterModel { PropertyName = "LastName", Operation = Operation.StartsWith, Value = lastName });
            if (!string.IsNullOrWhiteSpace(externalID))
                filters.Add(new FilterModel { PropertyName = "ExternalID", Operation = Operation.StartsWith, Value = externalID });

            var paginatedList = userService.GetPaginatedList(filters, FilterJoin.And, true, "LastName", page, rows);         
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

        
        public ActionResult CreateUser()
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "ID", 1, 1000000).Models;
            ViewBag.SystemFunctions = configurationService.GetSystemFunctionPaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000).Models;
            ViewBag.SystemRoles = configurationService.GetSystemRolePaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000, false).Models;

            return View(new UserModel());
        }

        [HttpPost]
        public ActionResult CreateUser(UserModel model, FormCollection formCollection)
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "ID", 1, 1000000).Models;
            ViewBag.SystemFunctions = configurationService.GetSystemFunctionPaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000).Models;
            ViewBag.SystemRoles = configurationService.GetSystemRolePaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000, false).Models;

            if (formCollection.AllKeys.Contains("districts_"))
                model.Districts = formCollection["districts_"].Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries).Select(f => new DistrictModel { ID = long.Parse(f) }).ToList();

            model.SystemFunctions = formCollection.AllKeys.Where(f => f.Contains("systemFunction_")).Select(f => new SystemFunctionModel { ID = long.Parse(f.Replace("systemFunction_", string.Empty)) }).ToList();
            
            if (model.IsOfficer && string.IsNullOrWhiteSpace(model.ExternalID))
                ModelState.AddModelError("ExternalID", "The External ID filed required when user is an officer.");

            if (!ModelState.IsValid)
            {                
                return View(model);
            }

            try
            {
                var userService = new UserService(AuthenticatedUser.SessionToken);
                var userID = userService.CreateUser(model).ID;
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }
            
            return RedirectToAction("Users");
            
        }

        public ActionResult GetFunctionsByRole(long roleID)
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();
            filters.Add(new FilterModel { Operation = Operation.Equals, PropertyName = "ID", Value = roleID });

            var role = configurationService
                .GetSystemRolePaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000, true)
                .Models
                .FirstOrDefault();

            return new JsonResult { Data = role == null? new List<SystemFunctionModel>() : role.SystemFunctions, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult EditUser(long id)
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();

            ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "ID", 1, 1000000).Models;
            ViewBag.SystemFunctions = configurationService.GetSystemFunctionPaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000).Models;

            var userService = new UserService(AuthenticatedUser.SessionToken);
            var model = userService.GetUser(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(UserModel model, FormCollection formCollection)
        {
            model.Districts = formCollection["districts_"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(f => new DistrictModel { ID = long.Parse(f) }).ToList();
            model.SystemFunctions = formCollection.AllKeys.Where(f => f.Contains("systemFunction_")).Select(f => new SystemFunctionModel { ID = long.Parse(f.Replace("systemFunction_", string.Empty)) }).ToList();

            if (model.IsOfficer && string.IsNullOrWhiteSpace(model.ExternalID))
                ModelState.AddModelError("ExternalID", "The External ID filed required when user is an officer.");

            if (!ModelState.IsValid)
            {
                var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
                var filters = new List<FilterModel>();

                ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "ID", 1, 1000000).Models;
                ViewBag.SystemFunctions = configurationService.GetSystemFunctionPaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000).Models;

                return View(model);
            }

            try
            {
                var userService = new UserService(AuthenticatedUser.SessionToken);
                userService.UpdateUser(model);
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
                var filters = new List<FilterModel>();

                ViewBag.Districts = configurationService.GetDistrictPaginatedList(filters, FilterJoin.And, true, "ID", 1, 1000000).Models;
                ViewBag.SystemFunctions = configurationService.GetSystemFunctionPaginatedList(filters, FilterJoin.And, true, "Description", 1, 1000000).Models;

                return View(model);
            }          

            return RedirectToAction("Users");

        }

        public ActionResult ViewUser(long id)
        {
            var userService = new UserService(AuthenticatedUser.SessionToken);
            var model = userService.GetUser(id);

            return View(model);
        }

        public ActionResult PaymentTerminals()
        {
            return View(new PaginationListModel<PaymentTerminalModel> { TotalCount = 0, Models = new List<PaymentTerminalModel>() });
        }

        public ActionResult SearchPaymentTerminals(string sidx, string sord, int page, int rows, string uuid, TerminalType terminalType)
        {
            var paymentService = new PaymentService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();
            if (!string.IsNullOrWhiteSpace(uuid))
                filters.Add(new FilterModel { PropertyName = "UUID", Operation = Operation.StartsWith, Value = uuid });
            if (terminalType != TerminalType.None)
                filters.Add(new FilterModel { PropertyName = "TerminalType", Operation = Operation.Equals, Value = terminalType.ToString() });

            var paginatedList = paymentService.GetPaginatedList(filters, FilterJoin.And, true, "ModifiedTimestamp", page, rows);
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


        public ActionResult CreatePaymentTerminal()
        {
            return View(new PaymentTerminalModel());
        }

        [HttpPost]
        public ActionResult CreatePaymentTerminal(PaymentTerminalModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var paymentService = new PaymentService(AuthenticatedUser.SessionToken);
                var paymentTerminalID = paymentService.CreatePaymentTerminal(model).ID;
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }

            return RedirectToAction("PaymentTerminals");

        }

        public ActionResult EditPaymentTerminal(long id)
        {
            var paymentService = new PaymentService(AuthenticatedUser.SessionToken);
            var model = paymentService.GetPaymentTerminal(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditPaymentTerminal(PaymentTerminalModel model)
        {          
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var paymentService = new PaymentService(AuthenticatedUser.SessionToken);
            paymentService.UpdatePaymentTerminal(model);

            return RedirectToAction("PaymentTerminals");
        }
    }
}