// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

#if !PCL && !NETFX_CORE && !SILVERLIGHT
using System.Runtime.Serialization;
#endif

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Helper class for building an SData compatible <see cref="Uri"/>.
    /// </summary>
    /// <remarks>
    /// The format of an SData <see cref="Uri"/> is as follows:
    /// http(s)://&gt;host&lt;:&gt;port&lt;/&gt;server&lt;/&gt;product&lt;/&gt;contract&lt;/&gt;company-dataset&lt;/&gt;collection&lt;(resource identifier)&gt;value&lt;
    /// </remarks>
    [Serializable]
    public class SDataUri : UriFormatter
    {
        #region QueryArgNames

        /// <summary>
        /// Specifies the names of the SData defined query arguments
        /// </summary>
        public static class QueryArgNames
        {
            /// <summary>
            /// Specifies the name of the 'orderBy' query argument.
            /// </summary>
            /// <value>The name of the 'orderBy' query argument</value>
            public const string OrderBy = "orderBy";

            /// <summary>
            /// Specifies the name of the 'where' query argument.
            /// </summary>
            /// <value>The name of the 'where' query argument</value>
            public const string Where = "where";

            /// <summary>
            /// Specifies the name of the 'count' query argument.
            /// </summary>
            /// <value>The name of the 'count' query argument.</value>
            public const string Count = "count";

            /// <summary>
            /// Specifies the name of the 'startIndex' query argument.
            /// </summary>
            /// <value>The name of the 'startIndex' query argument.</value>
            public const string StartIndex = "startIndex";

            /// <summary>
            /// Specifies the name of the 'language' query argument.
            /// </summary>
            /// <value>The name of the 'language' query argument.</value>
            public const string Language = "language";

            /// <summary>
            /// Specifies the name of the format type query argument.
            /// </summary>
            /// <value>The name of the format type query argument.</value>
            public const string Format = "format";

            /// <summary>
            /// Specifies the name of the precedence query argument.
            /// </summary>
            /// <value>The name of the precedence query argument.</value>
            public const string Precedence = "precedence";

            /// <summary>
            /// Specifies the name of the include query argument.
            /// </summary>
            /// <value>The name of the include query argument.</value>
            public const string Include = "include";

            /// <summary>
            /// Specifies the name of the includeSchema query argument.
            /// </summary>
            /// <value>The name of the includeSchema query argument.</value>
            public const string IncludeSchema = "includeSchema";

            /// <summary>
            /// Specifies the name of the TrackingID argument
            /// </summary>
            public const string TrackingId = "trackingId";

            /// <summary>
            /// Specifies the name of the search argument
            /// </summary>
            public const string Search = "search";

            /// <summary>
            /// Specifies the name of the return delta argument
            /// </summary>
            public const string ReturnDelta = "returnDelta";

            /// <summary>
            /// Specifies the name of the select argument
            /// </summary>
            public const string Select = "select";

            /// <summary>
            /// Specifies the name of the version argument
            /// </summary>
            public const string Version = "version";

            /// <summary>
            /// Specifies the name of the runName argument
            /// </summary>
            public const string RunName = "runName";

            /// <summary>
            /// Specifies the name of the runStamp argument
            /// </summary>
            public const string RunStamp = "runStamp";
        }

        #endregion

        #region PropertySort

        /// <summary>
        /// Order By details
        /// </summary>
        public class PropertySort
        {
            #region Fields

            private readonly string _property;
            private readonly bool _descending;

            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="PropertySort"/> class.
            /// </summary>
            /// <param name="property">The name of the property.</param>
            /// <param name="descending">The direction.</param>
            public PropertySort(string property, bool descending)
            {
                _property = property;
                _descending = descending;
            }

            #region Properties

            /// <summary>
            /// Returns the name of the property.
            /// </summary>
            /// <value>The name of the property.</value>
            public string Property
            {
                get { return _property; }
            }

            /// <summary>
            /// Returns a value indicating if the order is descending.
            /// </summary>
            /// <value><b>true</b> for a descending sort, otherwise <b>false</b> for ascending.</value>
            public bool Descending
            {
                get { return _descending; }
            }

            #endregion
        }

        #endregion

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="SDataUri"/> class.
        /// </summary>
        protected SDataUri(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataUri"/> class.
        /// </summary>
        public SDataUri()
            : this(default(Uri))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataUri"/> class with
        /// the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to assign.</param>
        public SDataUri(string uri)
            : this(!string.IsNullOrEmpty(uri) ? new Uri(uri) : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataUri"/> class with
        /// the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to assign.</param>
        public SDataUri(Uri uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataUri"/> class with
        /// the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to assign.</param>
        public SDataUri(SDataUri uri)
            : base(uri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataUri"/> class with
        /// the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to assign.</param>
        public SDataUri(UriFormatter uri)
            : base(uri)
        {
        }

        #region Properties

        /// <summary>
        /// Returns a value indicating if there is a selector associated with the last segment.
        /// </summary>
        /// <value><b>true</b> if there is a selector associated with the last segment; otherwise <b>false</b>.</value>
        public bool HasSelector
        {
            get
            {
                var segment = LastPathSegment;
                return segment != null && segment.HasSelector;
            }
        }

        /// <summary>
        /// Gets or sets the expression to use when sorting,
        /// </summary>
        /// <value>The expression to use when sorting.</value>
        public string OrderBy
        {
            get { return this[QueryArgNames.OrderBy]; }
            set { this[QueryArgNames.OrderBy] = value; }
        }

        private static readonly Regex _orderByFormat = new Regex(
            @"\s?(?<property>[^,\s]*)\s?(?<direction>[^,\s]*)",
            RegexOptions.IgnoreCase |
            RegexOptions.CultureInvariant |
            RegexOptions.IgnorePatternWhitespace
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            | RegexOptions.Compiled
#endif
            );

        /// <summary>
        /// Returns the sort details.
        /// </summary>
        /// <value>The details of the sort.</value>
        public PropertySort[] OrderByParsed
        {
            get
            {
                var orderBy = OrderBy;

                if (string.IsNullOrEmpty(orderBy))
                {
                    return null;
                }

                var properties = new List<PropertySort>();
                var match = _orderByFormat.Match(orderBy);

                while (match.Success)
                {
                    var property = match.Groups["property"];
                    if (property.Success)
                    {
                        var raw = match.Groups["direction"].Value;
                        var descending = string.Equals(raw, "desc", StringComparison.OrdinalIgnoreCase);
                        properties.Add(new PropertySort(property.Value, descending));
                    }

                    match = match.NextMatch();
                }

                return properties.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the expression to use when filtering a collection of resources.
        /// </summary>
        /// <value>The expression to use when filtering a collection of resources.</value>
        public string Where
        {
            get { return this[QueryArgNames.Where]; }
            set { this[QueryArgNames.Where] = value; }
        }

        /// <summary>
        /// Gets or sets the number of resources to return.
        /// </summary>
        /// <value>The number of resources to return. <b>0</b> to return all resources.  The default is value <b>0</b>.</value>
        public long? Count
        {
            get
            {
                var count = this[QueryArgNames.Count];
                if (count != null)
                {
                    long result;
                    if (long.TryParse(count, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                    {
                        return result;
                    }
                }

                return null;
            }
            set { this[QueryArgNames.Count] = value != null ? value.Value.ToString(CultureInfo.InvariantCulture) : null; }
        }

        /// <summary>
        /// Gets or sets the starting index to return resources from.
        /// </summary>
        /// <value>The starting index to return resources from. <b>0</b> specifies the first item.  The default value is <b>0</b>.</value>
        public long? StartIndex
        {
            get
            {
                var startIndex = this[QueryArgNames.StartIndex];
                if (startIndex != null)
                {
                    long result;
                    if (long.TryParse(startIndex, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                    {
                        return result;
                    }
                }

                return null;
            }
            set { this[QueryArgNames.StartIndex] = value != null ? value.Value.ToString(CultureInfo.InvariantCulture) : null; }
        }

        /// <summary>
        /// Gets or sets the language to use for resources.
        /// </summary>
        /// <value>The language to use for resources.</value>
        public string Language
        {
            get { return this[QueryArgNames.Language]; }
            set { this[QueryArgNames.Language] = value; }
        }

        /// <summary>
        /// Gets or sets the format type to return.
        /// </summary>
        /// <value>One of the <see cref="MediaType"/> values.</value>
        public MediaType? Format
        {
            get
            {
                var formatType = this[QueryArgNames.Format];
                if (formatType != null)
                {
                    MediaType value;
                    if (MediaTypeNames.TryGetShortMediaType(formatType, out value))
                    {
                        return value;
                    }
                }

                return null;
            }
            set { this[QueryArgNames.Format] = value != null ? MediaTypeNames.GetShortMediaType(value.Value) : null; }
        }

        /// <summary>
        /// Gets or sets the maximum precedence of the fields to return in the result.
        /// </summary>
        /// <value>0 to return no entity information (only name and id).
        /// Higher values will return entity information for the properties
        /// with an equal or lower precedence value.</value>
        public int? Precedence
        {
            get
            {
                var precedence = this[QueryArgNames.Precedence];
                if (!string.IsNullOrEmpty(precedence))
                {
                    int result;
                    if (int.TryParse(precedence, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                    {
                        return result;
                    }
                }

                return null;
            }
            set { this[QueryArgNames.Precedence] = value != null ? value.Value.ToString(CultureInfo.InvariantCulture) : null; }
        }

        /// <summary>
        /// Specifies which related objects should be included in the payload along
        /// with the main resource being requested.
        /// </summary>
        public string Include
        {
            get { return this[QueryArgNames.Include]; }
            set { this[QueryArgNames.Include] = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether or not the schema should be returned.
        /// </summary>
        /// <value><b>true</b> if the schema should be returned, otherwise <b>false</b>.  The default is <b>false</b>.</value>
        public bool? IncludeSchema
        {
            get
            {
                var includeSchema = this[QueryArgNames.IncludeSchema];
                if (!string.IsNullOrEmpty(includeSchema))
                {
                    bool result;
                    if (bool.TryParse(includeSchema, out result))
                    {
                        return result;
                    }
                }

                return false;
            }
            set { this[QueryArgNames.IncludeSchema] = value != null ? value.ToString().ToLowerInvariant() : null; }
        }

        /// <summary>
        /// The ID used to track asynchronous operations, and to ensure that
        /// duplicate operations are not processed.
        /// </summary>
        public string TrackingId
        {
            get { return this[QueryArgNames.TrackingId]; }
            set { this[QueryArgNames.TrackingId] = value; }
        }

        /// <summary>
        /// Specifies a full-text search criteria.
        /// </summary>
        /// <remarks>
        /// The syntax for the search string may vary depending on the search
        /// engine used by the provider. SData does not attempt to introduce
        /// a standard syntax for full text searches because the search text
        /// will usually be entered interactively through a search box.
        /// </remarks>
        public string Search
        {
            get { return this[QueryArgNames.Search]; }
            set { this[QueryArgNames.Search] = value; }
        }

        /// <summary>
        /// Specifies the properties to return .
        /// </summary>
        /// <remarks>
        /// The select parameter gives more control on the depth and breadth of information returned. It allows the consumer to specify the list of properties that he wants in the response, very much like a SQL select clause.
        /// </remarks>
        public string Select
        {
            get { return this[QueryArgNames.Select]; }
            set { this[QueryArgNames.Select] = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the server should only include properties that have been modified in its response.
        /// </summary>
        /// <value><b>true</b> if the server should only include properties that have been modified in its response, otherwise <b>false</b>.  The default is <b>false</b>.</value>
        public bool? ReturnDelta
        {
            get
            {
                var returnDelta = this[QueryArgNames.ReturnDelta];
                if (!string.IsNullOrEmpty(returnDelta))
                {
                    bool result;
                    if (bool.TryParse(returnDelta, out result))
                    {
                        return result;
                    }
                }

                return null;
            }
            set { this[QueryArgNames.ReturnDelta] = value != null ? value.ToString().ToLowerInvariant() : null; }
        }

        /// <summary>
        /// Gets or sets a specific version of the resource.
        /// </summary>
        /// <value>The version of the resources.</value>
        public string Version
        {
            get { return this[QueryArgNames.Version]; }
            set { this[QueryArgNames.Version] = value; }
        }

        /// <summary>
        /// A name that the synchronization engine assigned to the run.
        /// </summary>
        public string RunName
        {
            get { return this[QueryArgNames.RunName]; }
            set { this[QueryArgNames.RunName] = value; }
        }

        /// <summary>
        /// The timestamp at which the synchronization run was started.
        /// </summary>
        public DateTime? RunStamp
        {
            get
            {
                var dateTime = this[QueryArgNames.RunStamp];
                if (!string.IsNullOrEmpty(dateTime))
                {
                    W3CDateTime result;
                    if (W3CDateTime.TryParse(dateTime, out result))
                    {
                        return result.DateTime;
                    }
                }

                return null;
            }
            set { this[QueryArgNames.RunStamp] = value != null ? new W3CDateTime(value.Value).ToString() : null; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds a local path from the specified segments.
        /// </summary>
        /// <param name="segments">Array of path segments.</param>
        public SDataUri BuildLocalPath(params string[] segments)
        {
            return BuildLocalPath(UriPathSegment.FromStrings(segments));
        }

        /// <summary>
        /// Builds a local path from the specified segments.
        /// </summary>
        /// <param name="segments">Array of path segments.</param>
        public SDataUri BuildLocalPath(IEnumerable<UriPathSegment> segments)
        {
            Empty();

            Host = LocalHost;

            AppendPath(segments);

            return this;
        }

        public static string FormatConstant(object value)
        {
            if (value == null)
            {
                return "null";
            }
            if (value is bool)
            {
                return (bool) value ? "true" : "false";
            }
            if (value is string)
            {
                return string.Format(CultureInfo.InvariantCulture, "'{0}'", ((string) value).Replace("'", "''"));
            }
            if (value is DateTime || value is DateTimeOffset)
            {
                return string.Format(CultureInfo.InvariantCulture, "@{0:yyyy'-'MM'-'dd'T'HH':'mm':'ssK}@", value);
            }
            if (value is TimeSpan)
            {
                return string.Format(CultureInfo.InvariantCulture, "@{0:hh':'mm':'ss}@", value);
            }
            if (value is Guid || value is char || value is Enum || value is Version || value is Uri)
            {
                return string.Format(CultureInfo.InvariantCulture, "'{0}'", value);
            }
            if (value is IFormattable)
            {
                return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
            if (value is Array)
            {
                return string.Format(CultureInfo.InvariantCulture, "({0})", string.Join(",", ((Array) value).Cast<object>().Select(FormatConstant).ToArray()));
            }

            throw new NotSupportedException(string.Format("Selector '{0}' not supported", value));
        }

        #endregion
    }
}