// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;

namespace Saleslogix.SData.Client.Utilities
{
    /// <summary>
    /// Provides common validation methods shared across the framework entities. This class cannot be inherited.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Validates that the supplied <paramref name="value"/> is not a null reference.
        /// </summary>
        /// <param name="value">The value of the method argument to validate.</param>
        /// <param name="name">The name of the method argument.</param>
        /// <remarks>
        ///     If the <paramref name="value"/> is a <b>null</b> reference, an <see cref="ArgumentNullException"/> is raised using the supplied <paramref name="name"/>.
        /// </remarks>
        public static void ArgumentNotNull(object value, string name)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Validates that the supplied <paramref name="value"/> is not a null reference or an empty string.
        /// </summary>
        /// <param name="value">The value of the method argument to validate.</param>
        /// <param name="name">The name of the method argument.</param>
        /// <remarks>
        ///     If the <paramref name="value"/> is an empty string, an <see cref="ArgumentException"/> is raised using the supplied <paramref name="name"/>.
        /// </remarks>
        public static void ArgumentNotNullOrEmptyString(string value, string name)
        {
            ArgumentNotNull(value, name);

            if (value.Length == 0)
            {
                throw new ArgumentException("Value cannot be empty.", name);
            }
        }

        /// <summary>
        /// Validates that the supplied <paramref name="value"/> is an instance of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The value of the method argument to validate.</param>
        /// <param name="name">The name of the method argument.</param>
        /// <remarks>
        ///     If the <paramref name="value"/> is not an instance of <typeparamref name="T"/>, an <see cref="ArgumentException"/> is raised using the supplied <paramref name="name"/>.
        /// </remarks>
        public static void ArgumentIsType<T>(object value, string name)
        {
            ArgumentNotNull(value, name);

            if (!(value is T))
            {
                throw new ArgumentException(string.Format("Value must be an instance of '{0}'.", typeof (T)), name);
            }
        }
    }
}