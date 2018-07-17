using System;
using System.Collections.Generic;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class QuestionAnswerResultModel
    {
        public long ID { get; set; }
        public int inspectorID { get; set; }
        public DateTime TestStartTime { get; set; }
        public DateTime TestEndTime { get; set; }
        public List<QuestionAnswerResult> questionAnswerResults { get; set; }
        public bool isPassed { get; set; }
        public bool isSaved { get; set; }
    }
}
