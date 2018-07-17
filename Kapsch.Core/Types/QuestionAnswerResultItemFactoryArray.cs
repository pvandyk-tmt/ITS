using System;
using Oracle.DataAccess.Types;

namespace TMT.Build.OracleTableTypeClasses
{
    [OracleCustomTypeMappingAttribute("TIS.TABLE_VEHICLE_TEST_RESULT_TYPE")]
    public class QuestionAnswerResultItemFactoryArray : IOracleArrayTypeFactory
    {
        public Array CreateArray(int numElems)
        {
            return new QuestionAnswerResult[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return null;
        }
    }
}
