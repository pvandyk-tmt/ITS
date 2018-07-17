using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMT.Core.Culture
{
    public static class Extensions
    {
        public static string ToISO8601(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ssK");
        }

        public static string ToISO8601(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToString("yyyy-MM-ddTHH:mm:ssK") : null;
        }
    }
}
