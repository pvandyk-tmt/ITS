using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.Enforcement.iLog.Persistence.Models.Enc
{
    public class EncStatisticModel
    {
        public Guid ID { get; set; }
        public string StatsFileId { get; set; }
        public string MachineId { get; set; }
        public string LocationCode { get; set; }
        public string RunDate { get; set; }
        public string Time { get; set; }
        public int Speed { get; set; }
        public int Zone { get; set; }
        public int Lane { get; set; }
        public string Type { get; set; }
        public int Distance { get; set; }
        public string Direction { get; set; }
        public string Classification { get; set; }
        public string Captured { get; set; }
        public string File { get; set; }
        public string Error { get; set; }
        public string SmdString { get; set; }
        public string Plates { get; set; }
        public DateTime CreatedTimestamp { get; set; }
    }
}
