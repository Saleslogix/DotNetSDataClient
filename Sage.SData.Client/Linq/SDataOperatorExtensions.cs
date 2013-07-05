using System;

// ReSharper disable UnusedParameter.Global

namespace Sage.SData.Client.Linq
{
    public static class SDataOperatorExtensions
    {
        public static bool Between<T>(this T value, T low, T high)
            where T : IComparable
        {
            throw new NotSupportedException();
        }

        public static bool Between<T>(this T? value, T? low, T? high)
            where T : struct, IComparable
        {
            throw new NotSupportedException();
        }

        public static bool In<T>(this T value, params T[] values)
            where T : IComparable
        {
            throw new NotSupportedException();
        }

        public static bool In<T>(this T? value, params T?[] values)
            where T : struct, IComparable
        {
            throw new NotSupportedException();
        }

        public static bool Like(this string value, string pattern)
        {
            throw new NotSupportedException();
        }
    }
}