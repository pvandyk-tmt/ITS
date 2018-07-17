using System;
using System.Collections.Generic;
using System.Globalization;

namespace Kapsch.ITS.Gateway.Models.Fine
{
    public class AccountTransactionModel
    {
        public DateTime CreatedTimestamp { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public string FormattedCreatedTimestamp
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:dd MMM yyyy HH:mm}", CreatedTimestamp); }
        }

        public string FormattedAmount
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:0.00}", Amount); }
        }
    }
}