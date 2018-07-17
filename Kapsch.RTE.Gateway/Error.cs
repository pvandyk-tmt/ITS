using Kapsch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapsch.RTE.Gateway
{
    public class Error : ErrorBase
    {
        public Error()
            : base()
        {
        }

        protected internal Error(int code, string message)
            : base(code, message)
        {

        }

        public static Error RegisterMobileDeviceTimeout
        {
            get
            {
                return new Error(20001, "Mobile Device registration has timed out.");
            }
        }

        public static Error SectionConfigurationDoesNotExist
        {
            get
            {
                return new Error(20101, "Section Configuration Doe sNot Exist.");
            }
        }
    }
}