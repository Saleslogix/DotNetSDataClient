// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Parses a Uri path into segments
    /// </summary>
    public static class UriPathParser
    {
        #region Constants

        private const string Pattern = @"(?<segment>
                                           [^/(]*                       # anything other than slash or open paren
                                         )
                                         (
                                           \(
                                             (?<selector>
                                               (
                                                 ('([^']|(''))*')       # single quoted literal string
                                                 |
                                                 (""([^""]|(""""))*"")  # double quoted literal string
                                                 |
                                                 ([^'"")]*)             # anything other than quote or close paren
                                               )*
                                             )
                                           \)
                                         )?";

        private static readonly Regex _segmentFormat = new Regex(
            Pattern,
            RegexOptions.IgnoreCase |
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            | RegexOptions.Compiled
#endif
            );
        private static readonly IList<UriPathSegment> _emptyPath = new UriPathSegment[0];

        #endregion

        /// <summary>
        /// Parses the path of the specified <see cref="Uri"/> into segments.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> containing the path to parse into segments.</param>
        /// <returns>An array of segments that form the path for the specified <see cref="Uri"/>.</returns>
        public static IList<UriPathSegment> Parse(Uri uri)
        {
            return Parse(uri.AbsolutePath);
        }

        /// <summary>
        /// Parses the specified path into segments.
        /// </summary>
        /// <param name="path">The path to parse into segments.</param>
        /// <returns>An array of segments that form the specified path.</returns>
        public static IList<UriPathSegment> Parse(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return _emptyPath;
            }

            var segments = new List<UriPathSegment>();
            var match = _segmentFormat.Match(path);

            while (match.Success)
            {
                var segment = match.Groups["segment"].Value;

                if (segment.Length > 0)
                {
                    segments.Add(new UriPathSegment(segment, match.Groups["selector"].Value));
                }

                match = match.NextMatch();
            }

            return segments.ToArray();
        }
    }
}