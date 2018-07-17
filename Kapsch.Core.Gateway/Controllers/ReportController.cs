using Kapsch.Core.Data;
using Kapsch.Core.Reports;
using Kapsch.Core.Reports.Enums;
using Kapsch.Core.Reports.Models;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.Gateway.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.Core.Gateway.Controllers
{
    [RoutePrefix("api/Report")]
    public class ReportController : BaseController
    {

        [HttpGet]
        [Route("MetaData")]
        //[SessionAuthorize]
        [UsageLog]
        [ResponseType(typeof(ReportMetaDataModel))]
        public IHttpActionResult MetaData()
        {
            Kapsch.Core.Reports.Models.ReportMetaDataModel x = new ReportMetaDataModel();

            var reportDefinitions = new List<ReportDefinitionModel>();
            var libraryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/bin/ReportDefinitions");

            foreach (string file in Directory.GetFiles(libraryPath, "*.dll"))
            {
                Assembly assembly = Assembly.LoadFile(file);
                foreach (Type ti in assembly.GetTypes().Where(f => typeof(IReportDefinition).IsAssignableFrom(f) && !f.IsInterface))
                {
                    var reportDefinition = Activator.CreateInstance(ti) as IReportDefinition;
                    reportDefinitions.Add(
                        new ReportDefinitionModel
                            {
                                CategoryName = reportDefinition.CategoryName,
                                SubCategoryName = reportDefinition.SubCategoryName,
                                ReportName = reportDefinition.ReportName,
                                ExportTypes = reportDefinition.ExportTypes,
                                ParameterTypes = reportDefinition.ParameterTypes,
                                RequiredAccessRole = reportDefinition.RequiredAccessRole
                            });                  
                }
            }

            var reportMetaDataModel = new ReportMetaDataModel();
            foreach (var reportCategoryGroup in reportDefinitions.GroupBy(f => f.CategoryName))
            {
                var reportCategoryModel = new ReportCategoryModel();
                reportCategoryModel.CategoryName = reportCategoryGroup.Key;

                foreach (var reportSubCategoryGroup in reportCategoryGroup.GroupBy(f => f.SubCategoryName))
                {
                    var reportSubCategoryModel = new ReportSubCategoryModel();
                    reportSubCategoryModel.SubCategoryName = reportSubCategoryGroup.Key;
                    reportSubCategoryModel.ReportDefinitions = reportSubCategoryGroup.ToList();

                    reportCategoryModel.ReportSubCategories.Add(reportSubCategoryModel);
                }

                reportMetaDataModel.ReportCategories.Add(reportCategoryModel);

            }

            return Ok(reportMetaDataModel);
        }

        [HttpGet]
        [Route("Export")]
        [SessionTokenActionFilter]
        [UsageLog]
        
        public HttpResponseMessage Export(string sessionToken, string reportName, ExportType exportType, [FromUri] string[] parameters = null)
        {
            try
            {
                var libraryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/bin/ReportDefinitions");

                foreach (string file in Directory.GetFiles(libraryPath, "*.dll"))
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    foreach (Type ti in assembly.GetTypes().Where(f => typeof(IReportDefinition).IsAssignableFrom(f) && !f.IsInterface))
                    {
                        var reportDefinition = Activator.CreateInstance(ti) as IReportDefinition;
                        if (reportDefinition.ReportName.Equals(reportName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var bytes = reportDefinition.Export(exportType, parameters);
                            var result =
                                new HttpResponseMessage(HttpStatusCode.OK)
                                {
                                    Content = new ByteArrayContent(bytes)
                                };

                            if (exportType == ExportType.PDF)
                                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                            else if (exportType == ExportType.Excel)
                            {
                                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.ms-excel");
                                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline") { FileName = "Test.xls" };
                            }
                            else if (exportType == ExportType.Html)
                                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");


                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            var errorMessage = "Report failed. Please check your parameters.";

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(errorMessage)
            };
                   
        }
    }
}
