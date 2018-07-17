using System;
using System.Collections.Generic;

namespace Kapsch.ITS.Gateway.Models.Verify
{
    public class FirstCaseModel
    {
        public IList<RejectReasonModel> RejectReasons { get; set; }
        public IList<CaptureTypeModel> CaptureTypes { get; set; }
        public CaseModel Case { get; set; }
        public int Count { get; set; }
    }
}
