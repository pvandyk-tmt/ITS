using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.API.V5.CreateToken
{
    public class RequestModel
    {
        public API3G API { get; set; }

        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class API3G
        {

            private string companyTokenField;

            private string requestField;

            private Transaction transactionField;

            private Service[] servicesField;

            private Allocations allocationsField;

            private Traveler[] travelersField;

            private string[] additionalField;

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
            public Transaction Transaction
            {
                get
                {
                    return this.transactionField;
                }
                set
                {
                    this.transactionField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute(typeof(Service), ElementName = "Service", IsNullable = false)]
            public Service[] Services
            {
                get
                {
                    return this.servicesField;
                }
                set
                {
                    this.servicesField = value;
                }
            }

            /// <remarks/>
            public Allocations Allocations
            {
                get
                {
                    return this.allocationsField;
                }
                set
                {
                    this.allocationsField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("Traveler", IsNullable = false)]
            public Traveler[] Travelers
            {
                get
                {
                    return this.travelersField;
                }
                set
                {
                    this.travelersField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayItemAttribute("BlockPayment", IsNullable = false)]
            public string[] Additional
            {
                get
                {
                    return this.additionalField;
                }
                set
                {
                    this.additionalField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class Transaction
        {

            private decimal paymentAmountField;

            private string paymentCurrencyField;

            private string companyRefField;

            private string redirectURLField;

            private string backURLField;

            private byte companyRefUniqueField;

            private int pTLField;

            private string customerEmailField;

            public string CustomerEmail
            {
                get { return customerEmailField; }
                set { customerEmailField = value; }
            }
            private string customerFirstNameField;

            public string CustomerFirstName
            {
                get { return customerFirstNameField; }
                set { customerFirstNameField = value; }
            }
            private string customerLastNameField;

            public string CustomerLastName
            {
                get { return customerLastNameField; }
                set { customerLastNameField = value; }
            }
            private string customerDialCodeField;

            public string CustomerDialCode
            {
                get { return customerDialCodeField; }
                set { customerDialCodeField = value; }
            }
            private string customerPhoneField;

            public string CustomerPhone
            {
                get { return customerPhoneField; }
                set { customerPhoneField = value; }
            }
            private string companyAccRef;

            public string CompanyAccRef
            {
                get { return companyAccRef; }
                set { companyAccRef = value; }
            }
            private string userTokenField;

            public string UserToken
            {
                get { return userTokenField; }
                set { userTokenField = value; }
            }

            /// <remarks/>
            public decimal PaymentAmount
            {
                get
                {
                    return this.paymentAmountField;
                }
                set
                {
                    this.paymentAmountField = value;
                }
            }

            /// <remarks/>
            public string PaymentCurrency
            {
                get
                {
                    return this.paymentCurrencyField;
                }
                set
                {
                    this.paymentCurrencyField = value;
                }
            }

            /// <remarks/>
            public string CompanyRef
            {
                get
                {
                    return this.companyRefField;
                }
                set
                {
                    this.companyRefField = value;
                }
            }

            /// <remarks/>
            public string RedirectURL
            {
                get
                {
                    return this.redirectURLField;
                }
                set
                {
                    this.redirectURLField = value;
                }
            }

            /// <remarks/>
            public string BackURL
            {
                get
                {
                    return this.backURLField;
                }
                set
                {
                    this.backURLField = value;
                }
            }

            /// <remarks/>
            public byte CompanyRefUnique
            {
                get
                {
                    return this.companyRefUniqueField;
                }
                set
                {
                    this.companyRefUniqueField = value;
                }
            }

            /// <remarks/>
            public int PTL
            {
                get
                {
                    return this.pTLField;
                }
                set
                {
                    this.pTLField = value;
                }
            }

            /// <remarks/>
            public string customerEmail
            {
                get
                {
                    return this.customerEmailField;
                }
                set
                {
                    this.customerEmailField = value;
                }
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true )]
        public partial class Service
        {

            private int serviceTypeField;

            private string serviceDescriptionField;

            private string serviceDateField;

            /// <remarks/>
            public int ServiceType
            {
                get
                {
                    return this.serviceTypeField;
                }
                set
                {
                    this.serviceTypeField = value;
                }
            }

            /// <remarks/>
            public string ServiceDescription
            {
                get
                {
                    return this.serviceDescriptionField;
                }
                set
                {
                    this.serviceDescriptionField = value;
                }
            }

            /// <remarks/>
            public string ServiceDate
            {
                get
                {
                    return this.serviceDateField;
                }
                set
                {
                    this.serviceDateField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class Allocations
        {

            private API3GAllocationsAllocation allocationField;

            /// <remarks/>
            public API3GAllocationsAllocation Allocation
            {
                get
                {
                    return this.allocationField;
                }
                set
                {
                    this.allocationField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class API3GAllocationsAllocation
        {

            private string allocationCodeField;

            private decimal allocationAmountField;

            private int allocationServiceTypeField;

            private string allocationServiceDescriptionField;

            /// <remarks/>
            public string AllocationCode
            {
                get
                {
                    return this.allocationCodeField;
                }
                set
                {
                    this.allocationCodeField = value;
                }
            }

            /// <remarks/>
            public decimal AllocationAmount
            {
                get
                {
                    return this.allocationAmountField;
                }
                set
                {
                    this.allocationAmountField = value;
                }
            }

            /// <remarks/>
            public int AllocationServiceType
            {
                get
                {
                    return this.allocationServiceTypeField;
                }
                set
                {
                    this.allocationServiceTypeField = value;
                }
            }

            /// <remarks/>
            public string AllocationServiceDescription
            {
                get
                {
                    return this.allocationServiceDescriptionField;
                }
                set
                {
                    this.allocationServiceDescriptionField = value;
                }
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        public partial class Traveler
        {

            private string travelerFirstNameField;

            private string travelerLastNameField;

            private string travelerPhoneField;

            private bool travelerPhoneFieldSpecified;

            private string travelerPhonePrefixField;

            private bool travelerPhonePrefixFieldSpecified;

            /// <remarks/>
            public string TravelerFirstName
            {
                get
                {
                    return this.travelerFirstNameField;
                }
                set
                {
                    this.travelerFirstNameField = value;
                }
            }

            /// <remarks/>
            public string TravelerLastName
            {
                get
                {
                    return this.travelerLastNameField;
                }
                set
                {
                    this.travelerLastNameField = value;
                }
            }

            /// <remarks/>
            public string TravelerPhone
            {
                get
                {
                    return this.travelerPhoneField;
                }
                set
                {
                    this.travelerPhoneField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool TravelerPhoneSpecified
            {
                get
                {
                    return this.travelerPhoneFieldSpecified;
                }
                set
                {
                    this.travelerPhoneFieldSpecified = value;
                }
            }

            /// <remarks/>
            public string TravelerPhonePrefix
            {
                get
                {
                    return this.travelerPhonePrefixField;
                }
                set
                {
                    this.travelerPhonePrefixField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlIgnoreAttribute()]
            public bool TravelerPhonePrefixSpecified
            {
                get
                {
                    return this.travelerPhonePrefixFieldSpecified;
                }
                set
                {
                    this.travelerPhonePrefixFieldSpecified = value;
                }
            }
        }
    }
}
