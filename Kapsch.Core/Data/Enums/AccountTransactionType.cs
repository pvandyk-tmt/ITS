using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data.Enums
{
    public enum AccountTransactionType
    {
        Undefined = 0,
        Issued = 1,
        Payment = 2,
        Reduction = 3,
        Withdrawn = 4,
        WrittenOff = 5
    }
}
