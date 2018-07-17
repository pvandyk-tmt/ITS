using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class TestResultRecordModel
    {
        public string TestTypeDescription { get; set; }
        public string TestQuestionDescription { get; set; }
        public string TextAnswer { get; set; }
        public string TestQuestionResult { get; set; }
        public string Comments { get; set; }
    }
}
