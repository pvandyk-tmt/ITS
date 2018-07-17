using Kapsch.ITS.Gateway.Clients;
using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Portal.Controllers
{
    public class FineController : BaseController
    {
        // GET: Fine
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchFines(string sidx, string sord, int page, int rows, SearchCriteria searchCriteria, string searchValue)
        {
            var fineService = new FineService(AuthenticatedUser.SessionToken);
            var models = fineService.GetFines(searchCriteria, searchValue, false, true, false);
           
            var jsonData = new
            {
                total = 1,
                page,
                records = models.Count,
                rows = models
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);   
        }

        public ActionResult ViewFine(string referenceNumber)
        {
            var fineService = new FineService(AuthenticatedUser.SessionToken);
            var model = fineService.GetFines(SearchCriteria.ReferenceNumber, referenceNumber, true, true, false).FirstOrDefault();

            return View(model);
        }
    }
}