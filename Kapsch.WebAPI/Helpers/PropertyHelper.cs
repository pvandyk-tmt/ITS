using Kapsch.Gateway.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Kapsch.Gateway.Shared.Helpers
{
    public class PropertyHelper
    {
        public static Func<TIn, TOut> CreatePropertyAccessor<TIn, TOut>(string propertyName)
        {
            var param = Expression.Parameter(typeof(TIn));

            if (!propertyName.Contains("."))
            {
                var body = Expression.Property(param, propertyName);
                var type = body.Member.ReflectedType;

                return Expression.Lambda<Func<TIn, TOut>>(body, param).Compile();
            }

            var property = propertyName.Split('.').Aggregate<string, Expression>(param, (c, m) => Expression.Property(c, m));

            return Expression.Lambda<Func<TIn, TOut>>(Expression.Convert(property, typeof(TOut)), param).Compile();
        }

        public static string GetSortingValue<TIn>(string propertyName)
        {
            var memberInfo = (typeof(TIn)).GetProperty(propertyName);

            var sortAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(SortAttribute)) as SortAttribute;
            if (sortAttribute != null)
                return sortAttribute.DatabasePropertyName;

            return propertyName;
        }
    }
}