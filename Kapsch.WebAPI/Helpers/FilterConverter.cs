using Kapsch.Core.Filters;
using Kapsch.Gateway.Models.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kapsch.Gateway.Shared.Helpers
{
    public class FilterConverter
    {
        public static Filter Convert(FilterModel fromFilter)
        {
            if (fromFilter == null)
                return null;

            var toFilter = new Filter();
            toFilter.Operation = (Op)fromFilter.Operation;
            toFilter.PropertyName = fromFilter.PropertyName;
            toFilter.Value = fromFilter.Value;

            return toFilter;
        }
    }
}