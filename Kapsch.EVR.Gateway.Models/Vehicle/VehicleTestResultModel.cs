using System;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleTestResultModel
    {
        public int ID { get; set; }
        public int VehicleTestBookingID { get; set; }
        public int TestTypeID { get; set; }
        public int TestQuestionsID { get; set; }
        public string TextAnswer { get; set; }
        public Nullable<int> TestQuestionsAnswersID { get; set; }
        public int TestQuestionsAnswersRelID { get; set; }
        public string Comments { get; set; }

    }
}
