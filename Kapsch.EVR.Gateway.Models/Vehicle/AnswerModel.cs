using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class AnswerModel
    {
        public int RelationshipID { get; set; }
        public int TestQuestionID { get; set; }
     
        public Nullable<int> TestQuestionAnswerID { get; set; }
        public string QuestionAnswerDescription { get; set; }
        public int NextQuestionID { get; set; }
        public string DisplayColour { get; set; }
        public bool IsCommentRequired { get; set; }
    }
}
