using System;
using Oracle.DataAccess.Types;

namespace TMT.Build.OracleTableTypeClasses
{
    public class QuestionAnswerResult : INullable, IOracleCustomType
    {
        private readonly bool mIsNull;
        public long ID;

        public bool IsNull
        {
            get { return mIsNull; }
        }

        public QuestionAnswerResult()
        {
        }

        public QuestionAnswerResult(int vehicleTestBookingID,
            int testTypeID,
            int testQuestionsID,
            string textAnswer,
            int testQuestionsAnswersID,
            int testQuestionsAnswersRelID,
            string comments,
            int isPassed)
        {
        }


        [OracleObjectMappingAttribute("VEHICLE_TEST_BOOKING_ID")]
        public int VehicleTestBookingID { get; set; }

        [OracleObjectMappingAttribute("TEST_TYPE_ID")]
        public int TestTypeID { get; set; }

        [OracleObjectMappingAttribute("TEST_QUESTIONS_ID")]
        public int TestQuestionsID { get; set; }

        [OracleObjectMappingAttribute("TEXT_ANSWER")]
        public string TextAnswer { get; set; }

        [OracleObjectMappingAttribute("TEST_QUESTIONS_ANSWERS_ID")]
        public Nullable<int> TestQuestionsAnswersID { get; set; }

        [OracleObjectMappingAttribute("TEST_QUESTIONS_ANSWERS_REL_ID")]
        public int TestQuestionsAnswersIDRelID { get; set; }

        [OracleObjectMappingAttribute("COMMENTS")]
        public string Comments { get; set; }

        [OracleObjectMappingAttribute("IS_PASSED")]
        public int IsPassed { get; set; }

        public void ToCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            VehicleTestBookingID = (int)OracleUdt.GetValue(con, pUdt, "VEHICLE_TEST_BOOKING_ID");
            TestTypeID = (int)OracleUdt.GetValue(con, pUdt, "TEST_TYPE_ID");
            TestQuestionsID = (int)OracleUdt.GetValue(con, pUdt, "TEST_QUESTIONS_ID");
            TextAnswer = (string)OracleUdt.GetValue(con, pUdt, "TEXT_ANSWER");
            TestQuestionsAnswersID = (int)OracleUdt.GetValue(con, pUdt, "TEST_QUESTIONS_ANSWERS_ID");
            TestQuestionsAnswersIDRelID = (int)OracleUdt.GetValue(con, pUdt, "TEST_QUESTIONS_ANSWERS_REL_ID");
            Comments = (string)OracleUdt.GetValue(con, pUdt, "COMMENTS");
            IsPassed = (int)OracleUdt.GetValue(con, pUdt, "IS_PASSED");
        }

        public void FromCustomObject(Oracle.DataAccess.Client.OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, "VEHICLE_TEST_BOOKING_ID", VehicleTestBookingID);
            OracleUdt.SetValue(con, pUdt, "TEST_TYPE_ID", TestTypeID);
            OracleUdt.SetValue(con, pUdt, "TEST_QUESTIONS_ID", TestQuestionsID);
            OracleUdt.SetValue(con, pUdt, "TEXT_ANSWER", TextAnswer);
            OracleUdt.SetValue(con, pUdt, "TEST_QUESTIONS_ANSWERS_ID", TestQuestionsAnswersID);
            OracleUdt.SetValue(con, pUdt, "TEST_QUESTIONS_ANSWERS_REL_ID", TestQuestionsAnswersIDRelID);
            OracleUdt.SetValue(con, pUdt, "COMMENTS", Comments);
            OracleUdt.SetValue(con, pUdt, "IS_PASSED", IsPassed);
        }
    }
}
