using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.Capture
{
    public class FirstCaseModel
    {
        public IList<RejectReasonModel> RejectReasons { get; set; }
        public IList<CaptureTypeModel> CaptureTypes { get; set; }
        public IList<int> FileNumbers { get; set; }
        public IList<OfficerModel> Officers { get; set; }
        public CaseModel Case { get; set; }
        public int OffenceSet { get; set; }
        public int StartIndex { get; set; }
    }
}
