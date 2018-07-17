using Kapsch.Core.Gateway.Clients;
using Kapsch.Core.Gateway.Models.Payment;
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

namespace Kapsch.ITS.Portal.Controllers
{
    public class PaymentController : BaseController
    {
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchFines(string sidx, string sord, int page, int rows, SearchCriteria searchCriteria, string searchValue)
        {
            Session.Remove("PaymentSummary");

            var fineService = new FineService(AuthenticatedUser.SessionToken);
            var models = fineService.GetFines(searchCriteria, searchValue, false, true, true);

            var jsonData = new
            {
                total = 1,
                page,
                records = models.Count,
                rows = models
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult GetCourtsByDistrict(long districtID)
        {
            var configurationService = new ConfigurationService(AuthenticatedUser.SessionToken);
            var filters = new List<FilterModel>();
            filters.Add(new FilterModel { PropertyName = "DistrictID", Operation = Operation.Equals, Value = districtID });
            return new JsonResult { Data = configurationService.GetCourtPaginatedList(filters, FilterJoin.And, true, "CourtName", 1, 10000000).Models, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult DoPayment(PaymentSummary paymentSummary)
        {
            if (string.IsNullOrWhiteSpace(paymentSummary.PaymentReference))
                paymentSummary.PaymentReference = Guid.NewGuid().ToString();

            Session["PaymentSummary"] = paymentSummary;

            return Json("OK");
        }

        public ActionResult CourtPayment()
        {
            PaymentSummary paymentSummary = Session["PaymentSummary"] as PaymentSummary;
            ViewBag.Districts = AuthenticatedUser.UserData.Districts;

            return View(paymentSummary);
        }

        [HttpPost]
        public ActionResult CourtPayment(PaymentSummary model)
        {
            ViewBag.Districts = AuthenticatedUser.UserData.Districts;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var paymentDate = DateTime.Now;
            if (!DateTime.TryParseExact(model.PaymentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out paymentDate))
            {

                ModelState.AddModelError("PaymentDate", "Invalid Payment Date.");

                return View(model);
            }

            try
            { 
                PaymentSummary cachedModel = Session["PaymentSummary"] as PaymentSummary;
                model.Fines = cachedModel.Fines;

                var paymentTransaction = new PaymentTransactionModel();
                paymentTransaction.CredentialID = AuthenticatedUser.SessionData.CredentialID;
                paymentTransaction.CustomerContactNumber = model.MobileNumber;
                paymentTransaction.CustomerIDNumber = string.Empty;
                paymentTransaction.CustomerFirstName = model.FirstName;
                paymentTransaction.CustomerLastName = model.LastName;
                paymentTransaction.PaymentSource = model.PaymentMethod;
                paymentTransaction.Receipt = model.PaymentReference;
                paymentTransaction.ReceiptTimestamp = paymentDate;
                paymentTransaction.CourtID = model.CourtID;
                paymentTransaction.TerminalType = Core.Gateway.Models.Enums.TerminalType.InternalServer;
                paymentTransaction.TerminalUUID = "IMS";
                paymentTransaction.Amount = model.Fines.Sum(f => f.OutstandingAmount).Value;
                paymentTransaction.PaymentTransactionItems =
                    model.Fines.Select(f =>
                        new PaymentTransactionItemModel
                        {
                            ReferenceNumber = f.ReferenceNumber,
                            Amount = f.OutstandingAmount.Value,
                            TransactionToken = string.Empty,
                            EntityReferenceTypeID = long.Parse(f.ReferenceNumber[1].ToString()),
                            Description = "Notice issued."
                        })
                        .ToList();

                var paymentService = new PaymentService(AuthenticatedUser.SessionToken);
                var transactionToken = paymentService.RegisterPayment(paymentTransaction);

                var confirmedPayment = new ConfirmedPaymentModel();
                confirmedPayment.CustomerContactNumber = model.MobileNumber;
                confirmedPayment.CustomerIDNumber = string.Empty;
                confirmedPayment.CustomerFirstName = model.FirstName;
                confirmedPayment.CustomerLastName = model.LastName;
                confirmedPayment.PaymentSource = model.PaymentMethod;
                confirmedPayment.Receipt = model.PaymentReference;
                confirmedPayment.TerminalType = Core.Gateway.Models.Enums.TerminalType.InternalServer;
                confirmedPayment.TerminalUUID = "IMS";
                confirmedPayment.Amount = paymentTransaction.Amount;
                confirmedPayment.TransactionToken = transactionToken;

                paymentService.SettlePayment(confirmedPayment);

                Session.Remove("PaymentSummary");

                return RedirectToAction("Index");
            }
            catch (GatewayException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            } 
        }

        [HttpPost]
        public ActionResult ChangeAmount(ChangeAmountModel model)
        {
            if ((model.AccountTransactionType == AccountTransactionType.Withdrawn || model.AccountTransactionType == AccountTransactionType.Reduction) && 
                model.NewAmount > model.CurrentAmount)
            {
                return Json(new { IsValid = false, ReferenceNumber = model.ReferenceNumber, Amount = model.NewAmount, ErrorMessage = "New Amount cannot be more than the Current Amount." });
            }

            model.TerminalName = "IMS";
            model.ApprovedDate = DateTime.Now;
            model.ReferenceTransactionType = ReferenceTransactionType.Representation;

            try
            {
                var fineService = new FineService(AuthenticatedUser.SessionToken);
                fineService.ChangeAmount(model);

                PaymentSummary paymentSummary = Session["PaymentSummary"] as PaymentSummary;
                paymentSummary.Fines.Single(f => f.ReferenceNumber == model.ReferenceNumber).OutstandingAmount = model.NewAmount;

                Session["PaymentSummary"] = paymentSummary;

                return Json(new { IsValid = true, ReferenceNumber = model.ReferenceNumber, Amount = model.NewAmount, ErrorMessage = string.Empty });
            }
            catch (GatewayException ex)
            {
                return Json(new { IsValid = false, ReferenceNumber = model.ReferenceNumber, Amount = model.NewAmount, ErrorMessage = ex.Message });
            }           
        }
    }
}