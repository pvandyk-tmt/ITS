using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.API.V5.CreateToken
{
    public class ResponseModel
    {
        public API3G API { get; set; }

        /// <remarks/>
        //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        //[System.Xml.Serialization.XmlRootAttribute(IsNullable = false)]
        public partial class API3G
        {

            private string resultField;

            private string resultExplanationField;

            private string transTokenField;

            private string transRefField;

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

            /// <remarks/>
            public string TransToken
            {
                get
                {
                    return this.transTokenField;
                }
                set
                {
                    this.transTokenField = value;
                }
            }

            /// <remarks/>
            public string TransRef
            {
                get
                {
                    return this.transRefField;
                }
                set
                {
                    this.transRefField = value;
                }
            }
        }

    }
}
