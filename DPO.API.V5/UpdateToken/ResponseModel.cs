using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.API.V5.UpdateToken
{
    public class ResponseModel
    {
        public API3G API { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class API3G
        {

            private string resultField;

            private string resultExplanationField;

            /// <remarks/>
            public string Result
            {
                get
                {
                    return this.resultField;
                }
                set
                {
                    this.resultField = value;
                }
            }

            /// <remarks/>
            public string ResultExplanation
            {
                get
                {
                    return this.resultExplanationField;
                }
                set
                {
                    this.resultExplanationField = value;
                }
            }
        }


    }
}
