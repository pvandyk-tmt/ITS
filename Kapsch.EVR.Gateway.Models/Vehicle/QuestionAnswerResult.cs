using System;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class QuestionAnswerResult
    {
        public long ID;
        
        public int VehicleTestBookingID { get; set; }
        public int TestTypeID { get; set; }

        public int TestQuestionsID { get; set; }

        public string TextAnswer { get; set; }

        public Nullable<int> TestQuestionsAnswersID { get; set; }

        public int TestQuestionsAnswersIDRelID { get; set; }

        public string Comments { get; set; }

        public int IsPassed { get; set; }
    }
}
