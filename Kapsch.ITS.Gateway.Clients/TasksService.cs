using Kapsch.ITS.Gateway.Models.Adjudicate;
using Kapsch.ITS.Gateway.Models.Tasks;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;

namespace Kapsch.ITS.Gateway.Clients
{
    public class TasksService : BaseService
    {
        public TasksService(): base()
        {
        }

        public TasksService(string sessionToken)
            : base(sessionToken)
        {
        }

        public IList<TaskModel> GetTasks(bool refreshData)
        {
            var request = new RestRequest("/api/Task", Method.GET);
            request.AddQueryParameter("refreshData", refreshData.ToString());
            
            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<TaskModel>>(response.Content);
        }
    }
}
