
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class QuestionModel
    {
        public int TestQuestionID { get; set; }
        public string TestQuestionDescription { get; set; }
        public int QuestionTypeID { get; set; }        
        public string ValuePattern { get; set; }
        public bool HasComment { get; set; }
        public bool IsDoubleEntry { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsCompared { get; set; }
        public int Weight { get; set; }
        public string Criteria { get; set; }
        
        public string ComparedValue = string.Empty;

        public string CorrectQuestionAnswerID { get; set; }
                
        public List<AnswerModel> Answers = new List<AnswerModel>();
    }

}
