using Foolproof;
using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Fine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.ITS.Portal.Models
{
    public class PaymentSummary
    {
        [Required]
        [DisplayName("District")]
        public long? DistrictID { get; set; }

        [RequiredIf("PaymentMethod", Operator.EqualTo, PaymentMethod.Court)]
        [DisplayName("Court")]
        public long? CourtID { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Mobile Number")]
        public string MobileNumber { get; set; }

        //[Required]
        [DisplayName("Payment Date")]
        public string PaymentDate  { get; set; }

        [Required]
        [DisplayName("Payment Reference")]
        public string PaymentReference { get; set; }

        public IList<FineModel> Fines { get; set; }

        [DisplayName("Payment Method")]
        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}