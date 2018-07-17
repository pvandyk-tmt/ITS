using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.API.V5.PushPayment
{
    public class PushReceivedModel
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class API3G
        {

            private string resultField;

            private string resultExplanationField;

            private string transactionTokenField;

            private string customerNameField;

            private string customerCreditField;

            private string transactionRefField;

            private string transactionApprovalField;

            private string transactionCurrencyField;

            private decimal transactionAmountField;

            private string fraudAlertField;

            private string fraudExplnationField;

            private decimal transactionNetAmountField;

            private string transactionSettlementDateField;

            private decimal transactionRollingReserveAmountField;

            private string transactionRollingReserveDateField;

            private string customerPhoneField;

            private string customerCountryField;

            private string customerAddressField;

            private string customerCityField;

            private string customerZipField;

            private string mobilePaymentRequestField;

            private string accRefField;

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

            /// <remarks/>
            public string CustomerName
            {
                get
                {
                    return this.customerNameField;
                }
                set
                {
                    this.customerNameField = value;
                }
            }

            /// <remarks/>
            public string CustomerCredit
            {
                get
                {
                    return this.customerCreditField;
                }
                set
                {
                    this.customerCreditField = value;
                }
            }

            /// <remarks/>
            public string TransactionRef
            {
                get
                {
                    return this.transactionRefField;
                }
                set
                {
                    this.transactionRefField = value;
                }
            }

            /// <remarks/>
            public string TransactionApproval
            {
                get
                {
                    return this.transactionApprovalField;
                }
                set
                {
                    this.transactionApprovalField = value;
                }
            }

            /// <remarks/>
            public string TransactionCurrency
            {
                get
                {
                    return this.transactionCurrencyField;
                }
                set
                {
                    this.transactionCurrencyField = value;
                }
            }

            /// <remarks/>
            public decimal TransactionAmount
            {
                get
                {
                    return this.transactionAmountField;
                }
                set
                {
                    this.transactionAmountField = value;
                }
            }

            /// <remarks/>
            public string FraudAlert
            {
                get
                {
                    return this.fraudAlertField;
                }
                set
                {
                    this.fraudAlertField = value;
                }
            }

            /// <remarks/>
            public string FraudExplnation
            {
                get
                {
                    return this.fraudExplnationField;
                }
                set
                {
                    this.fraudExplnationField = value;
                }
            }

            /// <remarks/>
            public decimal TransactionNetAmount
            {
                get
                {
                    return this.transactionNetAmountField;
                }
                set
                {
                    this.transactionNetAmountField = value;
                }
            }

            /// <remarks/>
            public string TransactionSettlementDate
            {
                get
                {
                    return this.transactionSettlementDateField;
                }
                set
                {
                    this.transactionSettlementDateField = value;
                }
            }

            /// <remarks/>
            public decimal TransactionRollingReserveAmount
            {
                get
                {
                    return this.transactionRollingReserveAmountField;
                }
                set
                {
                    this.transactionRollingReserveAmountField = value;
                }
            }

            /// <remarks/>
            public string TransactionRollingReserveDate
            {
                get
                {
                    return this.transactionRollingReserveDateField;
                }
                set
                {
                    this.transactionRollingReserveDateField = value;
                }
            }

            /// <remarks/>
            public string CustomerPhone
            {
                get
                {
                    return this.customerPhoneField;
                }
                set
                {
                    this.customerPhoneField = value;
                }
            }

            /// <remarks/>
            public string CustomerCountry
            {
                get
                {
                    return this.customerCountryField;
                }
                set
                {
                    this.customerCountryField = value;
                }
            }

            /// <remarks/>
            public string CustomerAddress
            {
                get
                {
                    return this.customerAddressField;
                }
                set
                {
                    this.customerAddressField = value;
                }
            }

            /// <remarks/>
            public string CustomerCity
            {
                get
                {
                    return this.customerCityField;
                }
                set
                {
                    this.customerCityField = value;
                }
            }

            /// <remarks/>
            public string CustomerZip
            {
                get
                {
                    return this.customerZipField;
                }
                set
                {
                    this.customerZipField = value;
                }
            }

            /// <remarks/>
            public string MobilePaymentRequest
            {
                get
                {
                    return this.mobilePaymentRequestField;
                }
                set
                {
                    this.mobilePaymentRequestField = value;
                }
            }

            /// <remarks/>
            public string AccRef
            {
                get
                {
                    return this.accRefField;
                }
                set
                {
                    this.accRefField = value;
                }
            }
        }
    }
}
