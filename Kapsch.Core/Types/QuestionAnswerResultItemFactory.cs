using Oracle.DataAccess.Types;


namespace TMT.Build.OracleTableTypeClasses
{
    [OracleCustomTypeMappingAttribute("TIS.VEHICLE_TEST_RESULT_TYPE")]
    public class QuestionAnswerResultItemFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new QuestionAnswerResult();
        }
    }
}
