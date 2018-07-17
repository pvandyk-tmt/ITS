using System.Collections.Generic;

namespace Kapsch.ITS.Gateway.Models.Verify
{
    public class FirstFishpondCaseModel
    {
        public IList<RejectReasonModel> RejectReasons { get; set; }
        public IList<FishpondInfoModel> Cases { get; set; }
    }
}
