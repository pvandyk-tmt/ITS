using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Payment
{
    
    public class PaymentTerminalModel
    {
        public long ID { get; set; }

        [Required]
        [Display(Name = "Terminal Type")]
        public TerminalType TerminalType { get; set; }

        [Required]
        [Display(Name = "UUID")]
        public string UUID { get; set; }

        public Status Status { get; set; }

        public DateTime? ModifiedTimestamp { get; set; }

        public string FormattedTerminalType 
        { 
            get { return TerminalType.ToString(); } 
        }

        public string FormattedStatus 
        { 
            get { return Status.ToString(); } 
        }
    }
}
