using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Reports.Models
{
    public class VehicleTestQuestionAnswerModel
    {
        public long ID { get; set; }
        public string Question { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
        public string Comments { get; set; }
    }
}
