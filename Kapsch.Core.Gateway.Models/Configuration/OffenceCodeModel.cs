using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Configuration
{
    public class OffenceCodeModel
    {
        public long ID { get; set; }
        public long OffenceSetID { get; set; }
        public string Code { get; set; }
        public decimal FineAmount { get; set; }
        public string VehicleType { get; set; }
        public int? Zone { get; set; }
        public int? MinSpeed { get; set; }
        public int? MaxSpeed { get; set; }
        public int? WimVehicleTypeID { get; set; }
        public string WimOffenceDescription { get; set; }
        public long? MinOverweightPercentage { get; set; }
        public long? MaxOverweightPercentage { get; set; }
        public int CaseTypeID { get; set; }
        public string Description { get; set; }
        public string PrintDescription { get; set; }
        public string RegulationDescription { get; set; }
    }
}
