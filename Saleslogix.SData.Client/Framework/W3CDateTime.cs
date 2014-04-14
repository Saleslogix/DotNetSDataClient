// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Represents a W3C DateTime structure.
    /// </summary>
    /// <remarks>See http://www.w3.org/TR/NOTE-datetime for details on the W3C date time guidelines.</remarks>
    [Serializable]
    public class W3CDateTime
    {
        private readonly DateTime _utcDateTime;
        private readonly TimeSpan _utcOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="W3CDateTime"/> class.
        /// </summary>
        /// <param name="dateTime">The datetime to represent in a W3C format.</param>
        public W3CDateTime(DateTime dateTime)
        {
            _utcDateTime = dateTime;

            if (dateTime.Equals(DateTime.MinValue))
            {
                _utcOffset = TimeSpan.Zero;
            }
            else
            {
#if NET_2_0
                _utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
#else
                _utcOffset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
#endif
                _utcDateTime -= _utcOffset;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="W3CDateTime"/> class.
        /// </summary>
        /// <param name="dateTime">The datetime to represent in a W3C format.</param>
        /// <param name="offset">The UTC offset for the datetime.</param>
        public W3CDateTime(DateTime dateTime, TimeSpan offset)
        {
            _utcDateTime = dateTime;
            _utcOffset = offset;
        }

        /// <summary>
        /// Gets the W3C datetime.
        /// </summary>
        public DateTime DateTime
        {
            get { return _utcDateTime + _utcOffset; }
        }

        /// <summary>
        /// Gets the UTC offset.
        /// </summary>
        public TimeSpan UtcOffset
        {
            get { return _utcOffset; }
        }

        /// <summary>
        /// Gets the UTC datetime.
        /// </summary>
        public DateTime UtcTime
        {
            get { return _utcDateTime; }
        }

        private static readonly Regex _dateFormat = new Regex(
            @"^(?<year>\d\d\d\d)(-(?<month>\d\d)(-(?<day>\d\d)?)?)?(T(?<hour>\d\d)(:(?<min>\d\d)(:(?<sec>\d\d)(?<ms>\.\d+)?)?)?)?(?<ofs>(Z|[+\-]\d\d:\d\d))?$",
            RegexOptions.IgnoreCase |
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            | RegexOptions.Compiled
#endif
            );

        /// <summary>
        /// Converts the specified string representation of a W3C date and time to its <see cref="W3CDateTime"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing a date and time to convert.</param>
        /// <returns>A W3CDateTime equivalent to the date and time contained in s.</returns>
        public static W3CDateTime Parse(string s)
        {
            Guard.ArgumentNotNullOrEmptyString(s, "s");

            var match = _dateFormat.Match(s);
            if (!match.Success)
            {
                throw new FormatException("DateTime is not in a valid format");
            }

            try
            {
                var year = int.Parse(match.Groups["year"].Value, CultureInfo.InvariantCulture);
                if (year < 1000 && match.Groups["year"].Length < 3)
                {
                    year += 1999;
                    if (year < 50)
                    {
                        year++;
                    }
                }

                var month = match.Groups["month"].Success ? int.Parse(match.Groups["month"].Value, CultureInfo.InvariantCulture) : 1;
                var day = match.Groups["day"].Success ? int.Parse(match.Groups["day"].Value, CultureInfo.InvariantCulture) : 1;
                var hour = match.Groups["hour"].Success ? int.Parse(match.Groups["hour"].Value, CultureInfo.InvariantCulture) : 0;
                var minute = match.Groups["min"].Success ? int.Parse(match.Groups["min"].Value, CultureInfo.InvariantCulture) : 0;
                var second = match.Groups["sec"].Success ? int.Parse(match.Groups["sec"].Value, CultureInfo.InvariantCulture) : 0;
                var millisecond = match.Groups["ms"].Success ? (int) Math.Round((1000*double.Parse(match.Groups["ms"].Value, CultureInfo.InvariantCulture))) : 0;
                var offset = match.Groups["ofs"].Success ? ParseW3COffSet(match.Groups["ofs"].Value) : TimeSpan.Zero;
                return new W3CDateTime(new DateTime(year, month, day, hour, minute, second, millisecond) - offset, offset);
            }
            catch (Exception exception)
            {
                throw new FormatException("DateTime is not in a valid format", exception);
            }
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of value of this instance.</returns>
        public override string ToString()
        {
            return (_utcDateTime + _utcOffset).ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) + FormatOffset(_utcOffset, ":");
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>A string representation of value of this instance.</returns>
        public string ToDateString()
        {
            // Don't include the offset if we are not including the time otherwise the offset may be interpreted as a time
            // e.g. a date string 2009-10-09-7:00 would be interpreted as 7AM on 9th October instead of being seen as a -7 hour offset
            return (_utcDateTime + _utcOffset).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a value indicating if successfully able to parse W3C formatted datetime string.
        /// </summary>
        /// <param name="date">The W3C datetime formatted string to parse.</param>
        /// <param name="result">The <see cref="W3CDateTime"/> represented by the datetime string.</param>
        /// <returns><b>true</b> if able to parse string representation, otherwise returns false.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="date"/> is an empty string or is a null reference (Nothing in Visual Basic).</exception>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool TryParse(string date, out W3CDateTime result)
        {
            Guard.ArgumentNotNullOrEmptyString(date, "date");

            try
            {
                result = Parse(date);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Converts the value of the specified <see cref="TimeSpan"/> to its equivalent string representation.
        /// </summary>
        /// <param name="offset">The <see cref="TimeSpan"/> to convert.</param>
        /// <param name="separator">Separator used to delimit hours and minutes.</param>
        /// <returns>A string representation of the TimeSpan.</returns>
        private static string FormatOffset(TimeSpan offset, string separator)
        {
            var formattedOffset = string.Concat(offset.Hours.ToString("00", CultureInfo.InvariantCulture), separator, offset.Minutes.ToString("00", CultureInfo.InvariantCulture));
            return (offset >= TimeSpan.Zero ? "+" : null) + formattedOffset;
        }

        /// <summary>
        /// Converts the specified string representation of an offset to its <see cref="TimeSpan"/> equivalent.
        /// </summary>
        /// <param name="s">A string containing an offset to convert.</param>
        /// <returns>A TimeSpan equivalent to the offset contained in s.</returns>
        private static TimeSpan ParseW3COffSet(string s)
        {
            if (string.IsNullOrEmpty(s) || s == "Z")
            {
                return TimeSpan.Zero;
            }
            if (s[0] == '+')
            {
                s = s.Substring(1);
            }
            return TimeSpan.Parse(s);
        }

        /// <summary>
        /// Returns a value indicating whether the specified date represents a null date.
        /// </summary>
        /// <param name="dateTime">The <see cref="W3CDateTime"/> that needs to be checked.</param>
        /// <returns><b>true</b> if the value represents a null date, otherwise returns false.</returns>
        public static bool IsNull(W3CDateTime dateTime)
        {
            return dateTime == null || dateTime.DateTime == DateTime.MinValue;
        }
    }
}