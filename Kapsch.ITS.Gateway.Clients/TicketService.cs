using Kapsch.Core.Gateway.Models.User;
using Kapsch.Gateway.Models.Shared;
using Kapsch.Gateway.Models.Shared.Enums;
using Kapsch.Gateway.Models.Shared.Models;
using Kapsch.ITS.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Fine;
using Kapsch.ITS.Gateway.Models.Monitor;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Clients
{
    public class TicketService : BaseService
    {
        public TicketService(): base()
        {
        }

        public TicketService(string sessionToken)
            : base(sessionToken)
        {
        }

        public IList<UserModel> GetOfficers(long districtID)
        {
            var request = new RestRequest("/api/Ticket/Officer", Method.GET);
            request.AddQueryParameter("districtID", districtID.ToString());

            var response = RestClient.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
                throw CreateException(response);

            return JsonConvert.DeserializeObject<IList<UserModel>>(response.Content);
        }
    }
}
