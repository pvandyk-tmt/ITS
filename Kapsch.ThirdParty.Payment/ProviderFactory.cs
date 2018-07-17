using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ThirdParty.Payment
{
    public class ProviderFactory
    {
        public static readonly string ProviderName = ConfigurationManager.AppSettings["ProviderName"];

        public static IProvider Get()
        {
            if (ProviderName == "DPO")
            {
                return (IProvider)Activator.CreateInstance(Type.GetType("DPO.API.V5.Provider, DPO.API.V5", true));
            }

            if (ProviderName == "Mock")
            {
                return (IProvider)Activator.CreateInstance(Type.GetType("Kapsch.ThirdParty.Payment.Mock.Provider, Kapsch.ThirdParty.Payment.Mock", true));               
            }

            throw new Exception(string.Format("ProviderName {0} not supported.", ProviderName));
        }
    }
}
