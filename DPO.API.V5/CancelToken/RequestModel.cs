﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.API.V5.CancelToken
{
    public class RequestModel
    {
        public API3G API { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class API3G
        {

            private string companyTokenField;

            private string requestField;

            private string transactionTokenField;

            /// <remarks/>
            public string CompanyToken
            {
                get
                {
                    return this.companyTokenField;
                }
                set
                {
                    this.companyTokenField = value;
                }
            }

            /// <remarks/>
            public string Request
            {
                get
                {
                    return this.requestField;
                }
                set
                {
                    this.requestField = value;
                }
            }

            /// <remarks/>
            public string TransactionToken
            {
                get
                {
                    return this.transactionTokenField;
                }
                set
                {
                    this.transactionTokenField = value;
                }
            }
        }

    }
}