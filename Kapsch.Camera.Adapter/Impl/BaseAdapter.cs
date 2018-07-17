using Kapsch.Camera.Adapters.Interfaces;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kapsch.Camera.Adapters.Impl
{
    public abstract class BaseAdapter
    {
        public readonly static ILog Log = LogManager.GetLogger(typeof(BaseAdapter));

        protected static readonly string CoreGatewayEndpoint = ConfigurationManager.AppSettings["CoreGatewayEndpoint"];
        protected static readonly string ITSGatewayEndpoint = ConfigurationManager.AppSettings["ITSGatewayEndPoint"];
        protected static readonly string CredentialUsername = ConfigurationManager.AppSettings["CredentialUsername"];
        protected static readonly string CredentialPassword = ConfigurationManager.AppSettings["CredentialPassword"]; 

        protected BaseAdapter()
        {
         
        }

        public string Name
        {
            get { return GetType().Name.Replace("Adapter", string.Empty); }
            set { }
        }
    }
}
