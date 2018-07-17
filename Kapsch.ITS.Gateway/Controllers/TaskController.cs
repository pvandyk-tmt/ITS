using Kapsch.Core.Data;
using Kapsch.Gateway.Shared;
using Kapsch.Gateway.Shared.Filters;
using Kapsch.ITS.Gateway.Models.Tasks;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace Kapsch.ITS.Gateway.Controllers
{
    [RoutePrefix("api/Task")]
    [UsageLog]
    public class TaskController : BaseController
    {
        [HttpGet]
        [SessionAuthorize]
        [ResponseType(typeof(IList<TaskModel>))]
        public IHttpActionResult Get(bool refreshData)
        {
            using (var dbContext = new DataContext())
            {
                var list = new List<TaskModel>();
                list.Add(new TaskModel { Name = "Capture", Low = 1000, Medium = 200, Critical = 50 });
                list.Add(new TaskModel { Name = "Adjudicate", Low = 100, Medium = 400, Critical = 100 });

                return Ok(list);
            }
        }
    }
}
