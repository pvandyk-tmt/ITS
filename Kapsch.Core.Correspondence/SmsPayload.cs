using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Correspondence
{
    public class SmsPayload : IPayload
    {
        private string context;
        private string subType;
        private string message;

        public SmsPayload(string context, string message)
        {
            this.context = context;
            this.subType = null;
            this.message = message;
        }

        public SmsPayload(string context, string subType, string message)
        {
            this.context = context;
            this.subType = subType;
            this.message = message;
        }

        public CorrespondenceType CorrespondenceType
        {
            get
            {
                return CorrespondenceType.Sms;
            }
        }

        public string CorrespondenceContext
        {
            get
            {
                return this.context;
            }
        }

        public string SubType
        {
            get
            {
                return this.subType;
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }

        public string Serialize()
        {
            return message;
        }
    }
}
