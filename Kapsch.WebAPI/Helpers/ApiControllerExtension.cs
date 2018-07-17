using Kapsch.Core;
using System.Web.Http;

namespace Kapsch.Gateway.Shared.Helpers
{
    public static class ApiControllerExtension
    {
        public static BadRequestActionResult BadRequestEx(this ApiController controller, ErrorBase error)
        {
            return new BadRequestActionResult(controller.Request, error);
        }
    }
}