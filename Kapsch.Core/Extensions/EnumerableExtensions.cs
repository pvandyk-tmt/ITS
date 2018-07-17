using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Extensions
{
    public static partial class EnumerableExtensions
    {
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> entities, string propertyName)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            var propertyInfo = entities.First().GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return entities.OrderBy(e => propertyInfo.GetValue(e, null));
        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> entities, string propertyName)
        {
            if (!entities.Any() || string.IsNullOrEmpty(propertyName))
                return entities;

            var propertyInfo = entities.First().GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return entities.OrderByDescending(e => propertyInfo.GetValue(e, null));
        }
    }
}
