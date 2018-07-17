using System;

namespace Kapsch.ThirdParty.Payment
{
    public class ProviderException : Exception
    {
        public ProviderException()
        {
        }

        public ProviderException(string message, string code)
            : base(message)
        {
            Code = code;
        }

        public ProviderException(string message, string code, Exception inner)
            : base(message, inner)
        {
            Code = code;
        }

        public string Code { get; private set; }
    }
}
