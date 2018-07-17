using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Correspondence
{
    public class MockSmsSettings : ISettings
    {
        public MockSmsSettings(string correspondenceSubtype, TimeSpan delay)
        {
            CorrespondenceSubType = correspondenceSubtype;
            Delay = delay;
        }

        public CorrespondenceType CorrespondenceType
        {
            get
            {
                return CorrespondenceType.Sms;
            }
        }

        public CorrespondenceProvider CorrespondenceProvider
        {
            get
            {
                return CorrespondenceProvider.MockSms;
            }
        }

        public string CorrespondenceSubType { get; private set; }
        public TimeSpan Delay { get; private set; }
    }
}
