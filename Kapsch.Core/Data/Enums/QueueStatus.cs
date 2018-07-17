using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data.Enums
{
    public enum QueueStatus
    {
        None = 0,
        Queued = 1,
        InProcess = 2,
        Remove = 3
    }
}
