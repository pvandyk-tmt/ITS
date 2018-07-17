using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Gateway.Models.Shared
{
    public class GatewayException : Exception
    {
        public GatewayException() : base()
        {

        }

        public GatewayException(string message) : base(message)
        {

        }

        public GatewayException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected GatewayException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }


        public GatewayException(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public int Code { get; private set; }
        public string Message { get; private set; }
    }
}
