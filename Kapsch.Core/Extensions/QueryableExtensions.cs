using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Extensions
{
    // Code by https://stackoverflow.com/questions/39908403/how-to-use-a-string-to-create-a-ef-order-by-expression
    public static partial class QueryableExtensions
    {
        public static IOrderedQueryable<T> OrderByMember<T>(this IQueryable<T> source, string memberPath)
        {
            return source.OrderByMemberUsing(memberPath, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByMemberDescending<T>(this IQueryable<T> source, string memberPath)
        {
            return source.OrderByMemberUsing(memberPath, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenByMember<T>(this IOrderedQueryable<T> source, string memberPath)
        {
            return source.OrderByMemberUsing(memberPath, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByMemberDescending<T>(this IOrderedQueryable<T> source, string memberPath)
        {
            return source.OrderByMemberUsing(memberPath, "ThenByDescending");
        }

        private static IOrderedQueryable<T> OrderByMemberUsing<T>(this IQueryable<T> source, string memberPath, string method)
        {
            var parameter = Expression.Parameter(typeof(T), "item");
            var member = memberPath.Split('.')
                .Aggregate((Expression)parameter, Expression.PropertyOrField);
            var keySelector = Expression.Lambda(member, parameter);
            var methodCall = Expression.Call(
                typeof(Queryable), method, new[] { parameter.Type, member.Type },
                source.Expression, Expression.Quote(keySelector));
            return (IOrderedQueryable<T>)source.Provider.CreateQuery(methodCall);
        }
    }
}
