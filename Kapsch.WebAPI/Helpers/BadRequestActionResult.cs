using Kapsch.Core;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kapsch.Gateway.Shared.Helpers
{
    public class BadRequestActionResult : IHttpActionResult
    {
        public BadRequestActionResult(HttpRequestMessage request, ErrorBase error)
        {
            Request = request;
            Error = error;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ExecuteResult());
        }

        public HttpResponseMessage ExecuteResult()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
        }

        public ErrorBase Error { get; private set; }
        public HttpRequestMessage Request { get; private set; }
    }
}