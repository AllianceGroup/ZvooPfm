using System;
using System.Collections.Generic;

namespace mPower.Framework.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="T:IComparable"/> types.
    /// </summary>
    public static class ComparableExtensions
    {

        #region Methods

        #region In

        /// <summary>
        /// Determines whether this value exists in the <paramref name="set"/> passed.
        /// </summary>
        /// <typeparam name="T">The type of the value to compare and the values in the set.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <param name="set">The set of values to determine if <paramref name="value"/> is in.</param>
        /// <returns>True if <paramref name="value"/> exists in the <paramref name="set"/> otherwise, false.</returns>
        public static bool In<T>(this T value, IEqualityComparer<T> comparer, params T[] set)
        {

            // Use default comparer
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }   // if

            // Constant false
            if (set.Length == 0)
            {
                return false;
            }   // if

            for (int i = 0; i < set.Length; i++)
            {
                if (comparer.Equals(value, set[i]))
                {
                    return true;
                }   // if
            }   // for

            return false;

        }

        /// <summary>
        /// Determines whether this value exists in the <paramref name="set"/> passed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="set">The set of values to determine if <paramref name="value"/> is in.</param>
        /// <returns>True if <paramref name="value"/> exists in the <paramref name="set"/> otherwise, false.</returns>
        public static bool In(this byte value, params byte[] set)
        {
            for (int i = 0; i < set.Length; i++)
            {
                if (value == set[i])
                {
                    return true;
                }   // if
            }   // for
            return false;
        }

        /// <summary>
        /// Determines whether this value exists in the <paramref name="set"/> passed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="set">The set of values to determine if <paramref name="value"/> is in.</param>
        /// <returns>True if <paramref name="value"/> exists in the <paramref name="set"/> otherwise, false.</returns>
        public static bool In(this short value, params short[] set)
        {
            for (int i = 0; i < set.Length; i++)
            {
                if (value == set[i])
                {
                    return true;
                }   // if
            }   // for
            return false;
        }

        /// <summary>
        /// Determines whether this value exists in the <paramref name="set"/> passed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="set">The set of values to determine if <paramref name="value"/> is in.</param>
        /// <returns>True if <paramref name="value"/> exists in the <paramref name="set"/> otherwise, false.</returns>
        public static bool In(this int value, params int[] set)
        {
            for (int i = 0; i < set.Length; i++)
            {
                if (value == set[i])
                {
                    return true;
                }   // if
            }   // for
            return false;
        }

        /// <summary>
        /// Determines whether this value exists in the <paramref name="set"/> passed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="set">The set of values to determine if <paramref name="value"/> is in.</param>
        /// <returns>True if <paramref name="value"/> exists in the <paramref name="set"/> otherwise, false.</returns>
        public static bool In(this long value, params long[] set)
        {
            for (int i = 0; i < set.Length; i++)
            {
                if (value == set[i])
                {
                    return true;
                }   // if
            }   // for
            return false;
        }

        /// <summary>
        /// Determines whether this value exists in the <paramref name="set"/> passed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="set">The set of values to determine if <paramref name="value"/> is in.</param>
        /// <returns>True if <paramref name="value"/> exists in the <paramref name="set"/> otherwise, false.</returns>
        public static bool In(this string value, params string[] set)
        {
            return In<string>(value, StringComparer.OrdinalIgnoreCase, set);
        }

        /// <summary>
        /// Determines whether this value exists in the <paramref name="set"/> passed.
        /// </summary>
        /// <typeparam name="T">The type of the value to compare and the values in the set.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="set">The set of values to determine if <paramref name="value"/> is in.</param>
        /// <returns>True if <paramref name="value"/> exists in the <paramref name="set"/> otherwise, false.</returns>
        public static bool In<T>(this T value, params T[] set)
        {
            return In<T>(value, EqualityComparer<T>.Default, set);
        }

        #endregion

        #region Between

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <typeparam name="T">The type of values to compare.</typeparam>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between<T>(this T value, T minValueInclusive, T maxValueInclusive) where T : IComparable
        {
            if (!typeof(T).IsValueType && value == null)
            {
                throw new ArgumentNullException("value");
            }

            int o1 = value.CompareTo(minValueInclusive);
            int o2 = value.CompareTo(maxValueInclusive);

            return (o1 >= 0 && o2 <= 0) || (o1 <= 0 && o2 >= 0);

        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this byte value, byte minValueInclusive, byte maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this short value, short minValueInclusive, short maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this int value, int minValueInclusive, int maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this long value, long minValueInclusive, long maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this decimal value, decimal minValueInclusive, decimal maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this float value, float minValueInclusive, float maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this double value, double minValueInclusive, double maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this DateTime value, DateTime minValueInclusive, DateTime maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        /// <summary>
        /// Determines if the <paramref name="value"/> is between <paramref name="minValueInclusive"/> and
        /// <paramref name="maxValueInclusive"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="minValueInclusive">The min value inclusive.</param>
        /// <param name="maxValueInclusive">The max value inclusive.</param>
        /// <returns>True if <paramref name="value"/> is between <paramref name="minValueInclusive"/> and 
        /// <paramref name="maxValueInclusive"/>, otherwise false.</returns>
        public static bool Between(this TimeSpan value, TimeSpan minValueInclusive, TimeSpan maxValueInclusive)
        {
            if (minValueInclusive <= maxValueInclusive)
            {
                return (value >= minValueInclusive) && (value <= maxValueInclusive);
            }   // if
            else
            {
                return (value >= maxValueInclusive) && (value <= minValueInclusive);
            }   // else
        }

        #endregion

        #endregion

    }   // class

}   // namespace