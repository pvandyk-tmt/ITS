using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Kapsch.Core.Gateway.Clients;
using Kapsch.Gateway.Models.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kapsch.ITS.Portal.Controllers
{
    public class DataController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Query(string query)
        {
            try
            {
                var dataService = new DataService(AuthenticatedUser.SessionToken);

                return PartialView("_QueryResults", dataService.Query(query));
            }
            catch (GatewayException ge)
            {
                ViewBag.ErrorMessage = ge.Message;

                return PartialView("_QueryResults", null);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                return PartialView("_QueryResults", null);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public FileResult ExportToPDF(string html)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(html);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();

                return File(stream.ToArray(), "application/pdf", "results.pdf");
            }
        }
    }
}