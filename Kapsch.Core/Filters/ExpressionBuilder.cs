using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Kapsch.Core.Filters
{
    public static class ExpressionBuilder
    {
        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        public static Expression<Func<T, bool>> GetExpression<T>(IList<Filter> filters, FilterJoin join)
        {
            if (filters.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]);
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1], join);
            else
            {
                while (filters.Count > 0)
                {
                    var f1 = filters[0];
                    var f2 = filters[1];

                    if (exp == null)
                        exp = GetExpression<T>(param, filters[0], filters[1], join);
                    else
                        exp = join == FilterJoin.And ?
                            Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1], join)) :
                            Expression.Or(exp, GetExpression<T>(param, filters[0], filters[1], join));

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = join == FilterJoin.And ?
                            Expression.AndAlso(exp, GetExpression<T>(param, filters[0])) :
                            Expression.Or(exp, GetExpression<T>(param, filters[0]));

                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static Expression GetExpression<T>(ParameterExpression param, Filter filter)
        {
            MemberExpression member = null;

            if (filter.PropertyName.Contains("."))
            {
                var properties = filter.PropertyName.Split('.');

                MemberExpression lastMember = Expression.Property(param, properties[0]);
                member = Expression.Property(lastMember, properties[1]);
                
            }
            else
            {
               member = Expression.Property(param, filter.PropertyName);
            }
            

            object value = filter.Value;
            if (filter.Value is long || member.Type == typeof(long?))
                value = ((long)filter.Value);
            else if (member.Type.IsEnum)
                value = Enum.Parse(member.Type, value.ToString());
            else if (member.Type == typeof(Char))
                value = char.Parse((string)filter.Value);
            else if (member.Type == typeof(bool) || member.Type == typeof(bool?))
                value = bool.Parse((string)filter.Value);        

            ConstantExpression constant = Expression.Constant(value);
            var converted = Expression.Convert(constant, member.Type);


            switch (filter.Operation)
            {
                case Op.Equals:
                    return Expression.Equal(member, converted);

                case Op.GreaterThan:
                    return Expression.GreaterThan(member, converted);

                case Op.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, converted);

                case Op.LessThan:
                    return Expression.LessThan(member, converted);

                case Op.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, converted);

                case Op.Contains:
                    return Expression.Call(member, containsMethod, constant);

                case Op.StartsWith:
                    return Expression.Call(member, startsWithMethod, constant);

                case Op.EndsWith:
                    return Expression.Call(member, endsWithMethod, constant);
            }


            return null;
        }

        private static BinaryExpression GetExpression<T>(ParameterExpression param, Filter filter1, Filter filter2, FilterJoin join)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            return join == FilterJoin.And ? Expression.AndAlso(bin1, bin2) : Expression.Or(bin1, bin2);
        }
    }
}
