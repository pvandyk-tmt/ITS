using System.Collections.Generic;

namespace Kapsch.ITS.Gateway.Models.Adjudicate
{
    public class FirstFishpondCaseModel
    {
        public IList<RejectReasonModel> RejectReasons { get; set; }
        public IList<FishpondCaseModel> Cases { get; set; }
    }
}
