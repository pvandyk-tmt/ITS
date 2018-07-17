using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ThirdParty.Payment.Models
{
    public class TransactionModel
    {
        public string CompanyRef { get; set; }
        public string CompanyAccRef { get; set; }
        public decimal Amount { get; set; }

        public long ServiceType { get; set; }
        public string ServiceDescription { get; set; }

        public string CustomerEmail { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerISOCountry { get; set; }
        public string CustomerISODialCode { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerZipCode { get; set; }


        public string UserID { get; set; }
    }
}
