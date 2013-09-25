using System;

// ReSharper disable UnusedParameter.Global

namespace Saleslogix.SData.Client.Linq
{
    public static class SDataFunctionExtensions
    {
        /// <summary>
        /// Returns the leftmost <paramref name="length"/> characters from <paramref name="value"/>.
        /// Returns <paramref name="value"/> if <paramref name="value"/> has less than <paramref name="length"/> characters.
        /// </summary>
        public static string Left(this string value, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns rightmost <paramref name="length"/> characters from str.
        /// Returns <paramref name="value"/> if <paramref name="value"/> has less than <paramref name="length"/> characters.
        /// </summary>
        public static string Right(this string value, int length)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns the ASCII code of the leftmost character of <param name="value"/>. 
        /// </summary>
        public static int Ascii(this string value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts the ASCII code to a single character string.
        /// </summary>
        public static string Char(this int code)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts the ASCII code to a single character string.
        /// </summary>
        public static string Char(this int? code)
        {
            throw new NotSupportedException();
        }
    }
}