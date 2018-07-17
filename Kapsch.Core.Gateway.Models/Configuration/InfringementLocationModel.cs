using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Configuration
{
    public class InfringementLocationModel
    {

        public long ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public long CourtID { get; set; }
        public decimal? GpsLatitude { get; set; }
        public decimal? GpsLongitude { get; set; }
        public InfringementLocationType InfringementLocationType { get; set; }        
        public string CourtName { get; set; }
    }
}
