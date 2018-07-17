using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kapsch.Core.Data.Enums;

namespace Kapsch.Core.Correspondence
{
    public class MockEmailSettings : ISettings
    {
        public MockEmailSettings(string correspondenceSubtype, TimeSpan delay)
        {
            CorrespondenceSubType = correspondenceSubtype;
            Delay = delay;
        }

        public CorrespondenceType CorrespondenceType
        {
            get
            {
                return CorrespondenceType.Email;
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
