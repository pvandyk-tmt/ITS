using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data.Enums
{
    public enum Status
    {
        Inactive = 0,
        Active = 1,
        Deleted = 2,
        SuspendedLocked = 3,
        Logged = 4,
        Rejected = 5,
        Captured = 6,
        HandwrittenIssued = 7
    }
}
