using System.Linq;
using System.Linq.Expressions;
using System.Web.Helpers;
using mPower.Environment.MultiTenancy;

namespace System.Collections.Generic
{
    
    /// <summary>
    /// Extention methods for <see cref="System.Collections.Generic.IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExt
    {
        /// <summary>
        /// Performs an action on each value of the enumerable
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="enumerable">Sequence on which to perform action</param>
        /// <param name="action">Action to perform on every item</param>
        /// <exception cref="System.ArgumentNullException">Thrown when given null <paramref name="enumerable"/> or <paramref name="action"/></exception>
        public static void Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            Ensure.Argument.NotNull(enumerable, "enumerable");
            Ensure.Argument.NotNull(action, "action");

            foreach (T value in enumerable)
            {
                action(value);
            }
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, String expression, SortDirection direction)
        {
            var sortExpression = GetSortExpression<T>(expression);
            if (direction == SortDirection.Descending)
            {
                return source.OrderByDescending(sortExpression.Compile());
            }
            return source.OrderBy(sortExpression.Compile());
        }

        private static Expression<Func<T, object>> GetSortExpression<T>(string expression)
        {
            var param = Expression.Parameter(typeof (T), "item");
            var sortExpression = Expression.Lambda<Func<T, object>>
                (Expression.Convert(Expression.Property(param, expression), typeof (object)), param);
            return sortExpression;
        }

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, String expression, SortDirection direction)
        {
            var sortExpression = GetSortExpression<T>(expression);
            if (direction == SortDirection.Descending)
            {
                return source.ThenByDescending(sortExpression.Compile());
            }
            return source.ThenBy(sortExpression.Compile());
        }
    }
}