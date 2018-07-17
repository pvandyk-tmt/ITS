using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.MobileDevice
{
   
    public class MobileDeviceApplicationModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string SoftwareVersion { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public Status Status { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }       
    }
}
