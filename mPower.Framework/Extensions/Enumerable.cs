using System;
using System.Collections.Generic;
using System.Linq;

namespace mPower.Framework.Extensions
{
    public static class Enumerable
    {
        /// <summary>
        /// Enumerates a sequence in chunks, yielding batches of a certain size to the enumerator.
        /// </summary>
        /// <typeparam name="T">The type of item in the batch.</typeparam>
        /// <param name="sequence">The sequence of items to be enumerated.</param>
        /// <param name="batchSize">The maximum number of items to include in a batch.</param>
        /// <returns>A sequence of arrays, with each array containing at most
        /// <paramref name="batchSize"/> elements.</returns>
        public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> sequence, int batchSize)
        {

            var batch = new List<T>(batchSize);

            foreach (var item in sequence)
            {

                batch.Add(item);

                // when we've accumulated enough in the
                // batch, send it out
                if (batch.Count >= batchSize)
                {
                    yield return batch.ToArray();
                    batch.Clear();
                }   // if

            }   // foreach

            // send out any leftovers
            if (batch.Count > 0)
            {
                yield return batch.ToArray();
                batch.Clear();
            }   // if

        }

        public static TOut Min<T,TOut>(this IEnumerable<T> source, Func<T, double> valueSelector, Func<T,double,TOut>  resultSelector)
        {
            double min = double.MaxValue;
            T value = default(T);
            foreach (var item in source)
            {
                var current = valueSelector(item);
                if (current < min)
                {
                    value = item;
                    min = current;
                }
            }
            return resultSelector(value, min);
        }

        public static IEnumerable<T> Distinct<T, TProperty>(this IEnumerable<T> source, Func<T, TProperty> selector)
        {
            return source.Distinct(new ExpressionEqualityComparer<T, TProperty>(selector));
        }
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, T, bool> predicate)
        {
            return source.Distinct(new PredicateEqualityComparer<T>(predicate));
        }

        public class ExpressionEqualityComparer<T,TProperty> : EqualityComparer<T>
        {
            private readonly Func<T, TProperty> _selector;

            public ExpressionEqualityComparer(Func<T, TProperty> selector)
            {
                _selector = selector;
            }

            public override bool Equals(T x, T y)
            {
                return _selector(x).Equals(_selector(y));
            }

            public override int GetHashCode(T obj)
            {
                return _selector(obj).GetHashCode();
            }
        }
        public class PredicateEqualityComparer<T> : EqualityComparer<T>
        {
            private readonly Func<T, T, bool> _predicate;

            public PredicateEqualityComparer(Func<T, T, bool> predicate)
            {
                _predicate = predicate;
            }

            public override bool Equals(T x, T y)
            {
                return _predicate(x, y);
            }

            public override int GetHashCode(T obj)
            {
                // Always return the same value to force the call to IEqualityComparer<T>.Equals
                return 0;
            }
        }
    }
}