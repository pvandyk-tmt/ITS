using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kapsch.Core.Data.Enums;

namespace Kapsch.Core.Correspondence
{
    public class CMSmsSettings : ISettings
    {
        public CMSmsSettings(string correspondenceSubtype, string baseUrl, Guid productToken, int cost)
        {
            CorrespondenceSubType = correspondenceSubtype;
            BaseUrl = baseUrl;
            ProductToken = productToken;
            Cost = cost;
        }

        public string CorrespondenceSubType
        {
            get; private set;
        }

        public string BaseUrl
        {
            get; private set;
        }

        public Guid ProductToken
        {
            get; private set;
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
                return CorrespondenceProvider.CMSms;
            }
        }

        public int Cost
        {
            get; private set;
        }
    }
}
