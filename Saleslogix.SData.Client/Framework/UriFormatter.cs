// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using Saleslogix.SData.Client.Utilities;

#if !PCL && !NETFX_CORE && !SILVERLIGHT
using System.Net;
using System.Security.Permissions;
#endif

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Helper class for building a <see cref="Uri"/>.
    /// </summary>
    [Serializable]
    public class UriFormatter : ISerializable
    {
        #region Constants

        /// <summary>
        /// Returns the <see cref="string"/> used as the name of the uri property during serialization.
        /// </summary>
        public const string UriName = "uri";

        /// <summary>
        /// Returns the <see cref="string"/> used as the suffix for the scheme part of a <see cref="Uri"/>.
        /// </summary>
        /// <value>A <see cref="string"/> used as the suffix for the scheme part of a <see cref="Uri"/>.</value>
        public const string SchemeSuffix = ":";

        /// <summary>
        /// Returns the <see cref="string"/> used as the prefix for the port part of a <see cref="Uri"/>.
        /// </summary>
        /// <value>A <see cref="string"/> used as the prefix for the port part of a <see cref="Uri"/>.</value>
        public const string PortPrefix = ":";

        /// <summary>
        /// Returns the <see cref="string"/> used as the prefix for the query part of a <see cref="Uri"/>.
        /// </summary>
        /// <value>A <see cref="string"/> used as the prefix for the query part of a <see cref="Uri"/>.</value>
        public const string QueryPrefix = "?";

        /// <summary>
        /// Returns the <see cref="string"/> used as the argument separator for the query part of a <see cref="Uri"/>.
        /// </summary>
        /// <value>A <see cref="string"/> used as the argument separator for the query part of a <see cref="Uri"/>.</value>
        public const string QueryArgPrefix = "&";

        /// <summary>
        /// Returns the <see cref="string"/> used as the query argument and value separator.
        /// </summary>
        /// <value>A <see cref="string"/> used as the query argument and value separator.</value>
        public const string QueryArgValuePrefix = "=";

        /// <summary>
        /// Returns the <see cref="string"/> to use for separating the path parts of a <see cref="Uri"/>.
        /// </summary>
        /// <value>A <see cref="string"/> to use for separating the path parts of a <see cref="Uri"/>.</value>
        public const string PathSegmentPrefix = "/";

        /// <summary>
        /// Returns the <see cref="string"/> used to prefix the fragment part of a <see cref="Uri"/>.
        /// </summary>
        /// <value>A <see cref="string"/> used to prefix the fragment part of a <see cref="Uri"/>.</value>
        public const string FragmentPrefix = "#";

        /// <summary>
        /// Returns the Http scheme.
        /// </summary>
        /// <value>A <see cref="string"/> containing the Http scheme.</value>
        public const string Http = "http";

        /// <summary>
        /// Returns the Https scheme.
        /// </summary>
        /// <value>A <see cref="string"/> containing the Https scheme.</value>
        public const string Https = "https";

        /// <summary>
        /// Defines that a port has not been specified.
        /// </summary>
        /// <value>A value defining that a port has not been specified.</value>
        public const int UnspecifiedPort = -1;

        #endregion

        #region Fields

        /// <summary>
        /// Gets the identifier/IPAddress to use for the Local Host.
        /// </summary>
#if PCL || NETFX_CORE || SILVERLIGHT
        public const string LocalHost = "localhost";
#else
        public static readonly string LocalHost = Dns.GetHostName();
#endif

        private Uri _uri;
        private bool _requiresRebuildUri;
        private bool _requiresParseUri;

        private string _scheme;
        private int _port;
        private string _host;
        private string _fragment;

        private string _pathInternal;
        private string _directPath;
        private bool _requiresRebuildPath;
        private bool _requiresParsePath;
        private List<UriPathSegment> _pathSegments;

        private string _query;
        private bool _requiresParseQuery;
        private bool _requiresRebuildQuery;
        private QueryArgsDictionary _queryArgs;

        #endregion

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="UriFormatter"/> class.
        /// </summary>
        protected UriFormatter(SerializationInfo info, StreamingContext context)
            : this()
        {
            string uri = null;

            if (info.MemberCount > 0)
            {
                try
                {
                    uri = info.GetString(UriName);
                }
                catch
                {
                    // Ignore any exceptions, as the member will not be present
                }
            }

            if (uri != null)
            {
                Uri = new Uri(uri);
            }
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="UriFormatter"/> class.
        /// </summary>
        public UriFormatter()
            : this(default(Uri))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriFormatter"/> class with
        /// the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to assign.</param>
        public UriFormatter(string uri)
            : this(!string.IsNullOrEmpty(uri) ? new Uri(uri) : null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriFormatter"/> class with
        /// the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to assign.</param>
        public UriFormatter(Uri uri)
        {
            Uri = uri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriFormatter"/> class with
        /// the specified <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to assign.</param>
        public UriFormatter(UriFormatter uri)
        {
            Guard.ArgumentNotNull(uri, "uri");

            _uri = uri._uri;
            _requiresParseUri = uri._requiresParseUri;
            _requiresRebuildUri = uri._requiresRebuildUri;

            _scheme = uri._scheme;
            _port = uri._port;
            _host = uri._host;
            _fragment = uri._fragment;

            _pathInternal = uri._pathInternal;
            _directPath = uri._directPath;
            _requiresParsePath = uri._requiresParsePath;
            _requiresRebuildPath = uri._requiresRebuildPath;

            if (uri._pathSegments != null)
            {
                _pathSegments = new List<UriPathSegment>(uri._pathSegments.Count);

                foreach (var segment in uri._pathSegments)
                {
                    var clone = new UriPathSegment(segment) {Formatter = this};
                    _pathSegments.Add(clone);
                }
            }

            _query = uri._query;
            _requiresParseQuery = uri._requiresParseQuery;
            _requiresRebuildQuery = uri._requiresRebuildQuery;

            if (uri._queryArgs != null)
            {
                _queryArgs = new QueryArgsDictionary(this, uri._queryArgs);
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Uri"/>.
        /// </summary>
        /// <value>The <see cref="Uri"/>.</value>
        public Uri Uri
        {
            get
            {
                CheckRebuildUri();
                return _uri;
            }
            set
            {
                _uri = value;
                _requiresParseUri = true;
                _requiresRebuildUri = false;
            }
        }

        /// <summary>
        /// Gets or sets the scheme for the <see cref="Uri"/>.
        /// </summary>
        /// <value>The scheme for the <see cref="Uri"/>.</value>
        public string Scheme
        {
            get
            {
                CheckParseUri();
                return _scheme;
            }
            set
            {
                CheckParseUri();
                _scheme = value;
                _requiresRebuildUri = true;
            }
        }

        /// <summary>
        /// Gets or sets the port for the <see cref="Uri"/>.
        /// </summary>
        /// <value>The port for the <see cref="Uri"/> if one exists, otherwise <b>-1</b>.</value>
        public int Port
        {
            get
            {
                CheckParseUri();
                return _port;
            }
            set
            {
                CheckParseUri();
                _port = value;
                _requiresRebuildUri = true;
            }
        }

        /// <summary>
        /// Gets or sets the host for the <see cref="Uri"/>.
        /// </summary>
        /// <value>The host for the <see cref="Uri"/>.</value>
        public string Host
        {
            get
            {
                CheckParseUri();
                return _host;
            }
            set
            {
                CheckParseUri();
                _host = value;
                _requiresRebuildUri = true;
            }
        }

        /// <summary>
        /// Gets or sets the fragment for the <see cref="Uri"/>.
        /// </summary>
        /// <value>The fragment for the <see cref="Uri"/>.</value>
        public string Fragment
        {
            get
            {
                CheckParseUri();
                return _fragment;
            }
            set
            {
                CheckParseUri();
                _fragment = value;
                _requiresRebuildUri = true;
            }
        }

        /// <summary>
        /// Gets or sets the path of the <see cref="Uri"/>.
        /// </summary>
        /// <value>The path for the <see cref="Uri"/>.</value>
        public string Path
        {
            get
            {
                CheckParsePath();

                CheckRebuildPath();

                return _pathInternal;
            }
            set
            {
                CheckParsePath();

                if (value != null && value.StartsWith(PathSegmentPrefix, StringComparison.Ordinal))
                {
                    _pathInternal = value.Substring(PathSegmentPrefix.Length);
                }
                else
                {
                    _pathInternal = value;
                }

                _requiresParsePath = true;
                _requiresRebuildUri = true;
                _directPath = null;
            }
        }

        /// <summary>
        /// Returns the path of the <see cref="Uri"/> with all selectors removed.
        /// </summary>
        /// <value>The path for the <see cref="Uri"/> with all selectors removed</value>
        public string DirectPath
        {
            get
            {
                if (_requiresRebuildPath || _directPath == null)
                {
                    CheckRebuildPath();

                    var path = new StringBuilder();

                    foreach (var segment in PathSegments)
                    {
                        if (segment == null)
                        {
                            continue;
                        }

                        if (path.Length > 0)
                        {
                            path.Append(PathSegmentPrefix);
                        }

                        path.Append(segment.Text);
                    }

                    _directPath = path.ToString();
                }

                return _directPath;
            }
        }

        /// <summary>
        /// Gets or sets the query for the <see cref="Uri"/>
        /// </summary>
        /// <value>The query for the query.</value>
        public string Query
        {
            get
            {
                CheckParseQuery();
                CheckRebuildQuery();
                return _query;
            }
            set
            {
                CheckParseQuery();
                _query = value;
                _requiresParseQuery = true;
                _requiresRebuildUri = true;
            }
        }

        /// <summary>
        /// Returns the query arguments for the <see cref="Uri"/>.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey, TValue}"/> containing the query arguments for the <see cref="Uri"/>.</value>
        public IDictionary<string, string> QueryArgs
        {
            get { return _queryArgs ?? (_queryArgs = new QueryArgsDictionary(this)); }
        }

        /// <summary>
        /// Gets or sets the path and query of the <see cref="Uri"/>.
        /// </summary>
        /// <value>The path and query for the <see cref="Uri"/>.</value>
        public string PathQuery
        {
            get
            {
                var path = Path;
                var query = Query;

                if (string.IsNullOrEmpty(query))
                {
                    return path;
                }
                return string.Concat(path, QueryPrefix, query);
            }
            set
            {
                var pos = value.IndexOf(QueryPrefix, StringComparison.Ordinal);
                if (pos < 0)
                {
                    Path = Path;
                    Query = null;
                }
                else
                {
                    Path = value.Substring(0, pos);
                    Query = value.Substring(pos + 1);
                }
            }
        }

        private List<UriPathSegment> InternalPathSegments
        {
            get { return _pathSegments ?? (_pathSegments = new List<UriPathSegment>()); }
        }

        /// <summary>
        /// Returns the components that make up the path.
        /// </summary>
        /// <value>Array of components that make up the path.</value>
        public IList<UriPathSegment> PathSegments
        {
            get
            {
                CheckParsePath();

                return InternalPathSegments.ToArray();
            }
            set
            {
                CheckParsePath();

                InternalPathSegments.Clear();
                if (value != null)
                {
                    AddPathSegments(value);
                }

                RequiresRebuildPath = true;
            }
        }

        /// <summary>
        /// Returns the last path segment.
        /// </summary>
        /// <value>The last path segment.</value>
        public UriPathSegment LastPathSegment
        {
            get
            {
                var segments = PathSegments;
                return segments.Count > 0 ? segments[segments.Count - 1] : null;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating if the <see cref="Uri"/> uses SSL.
        /// </summary>
        /// <value><b>true</b> if the <see cref="Uri"/> uses SSL, otherwise <b>false</b>.</value>
        public bool UseSsl
        {
            get
            {
                CheckParseUri();
                return string.Equals(_scheme, Https, StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                CheckParseUri();

                _scheme = value ? Https : Http;
                _requiresRebuildUri = true;
            }
        }

        /// <summary>
        /// Returns a flag indicating if the <see cref="Uri"/> is empty.
        /// </summary>
        /// <value><b>true</b> if the <see cref="Uri"/> is empty, otherwise <b>false</b>.</value>
        public bool IsEmpty
        {
            get { return _uri == null; }
        }

        /// <summary>
        /// Gets or sets a query argument.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        public string this[string name]
        {
            get
            {
                CheckParseQuery();

                string value;
                QueryArgs.TryGetValue(name, out value);
                return value;
            }
            set
            {
                CheckParseQuery();

                if (value == null)
                {
                    if (_queryArgs != null)
                    {
                        _queryArgs.Remove(name);
                    }
                }
                else
                {
                    QueryArgs[name] = value;
                }

                _requiresRebuildUri = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="Path"/> needs to be rebuilt.
        /// </summary>
        /// <value><b>true</b> if the <see cref="Path"/> needs to be rebuilt, otherwise <b>false</b>.</value>
        internal bool RequiresRebuildPath
        {
            get { return _requiresRebuildPath; }
            set
            {
                _requiresRebuildPath = value;
                if (value)
                {
                    _requiresRebuildUri = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the <see cref="Query"/> needs to be rebuilt.
        /// </summary>
        /// <value><b>true</b> if the <see cref="Query"/> needs to be rebuilt, otherwise <b>false</b>.</value>
        internal bool RequiresRebuildQuery
        {
            get { return _requiresRebuildQuery; }
            set
            {
                _requiresRebuildQuery = value;
                if (value)
                {
                    _requiresRebuildUri = true;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the <see cref="Uri"/> back to an empty <see cref="Uri"/>.
        /// </summary>
        public UriFormatter Empty()
        {
            Uri = null;
            return this;
        }

        /// <summary>
        /// Adds the specified path segments to the <see cref="Uri"/>.
        /// </summary>
        /// <param name="segments">The path segments to add to the <see cref="Uri"/>.</param>
        public UriFormatter AppendPath(params string[] segments)
        {
            return AppendPath(UriPathSegment.FromStrings(segments));
        }

        /// <summary>
        /// Adds the specified paths to the <see cref="Uri"/>.
        /// </summary>
        /// <param name="segment">The path segment to add to the <see cref="Uri"/>.</param>
        public UriFormatter AppendPath(UriPathSegment segment)
        {
            Guard.ArgumentNotNull(segment, "segment");

            CheckParsePath();

            AddPathSegments(new[] {segment});

            RequiresRebuildPath = true;

            return this;
        }

        /// <summary>
        /// Adds the specified paths to the <see cref="Uri"/>.
        /// </summary>
        /// <param name="segments">The path segments to add to the <see cref="Uri"/>.</param>
        public UriFormatter AppendPath(IEnumerable<UriPathSegment> segments)
        {
            Guard.ArgumentNotNull(segments, "segments");

            CheckParsePath();
            AddPathSegments(segments);

            RequiresRebuildPath = true;

            return this;
        }

        /// <summary>
        /// Sets the path for the <see cref="Uri"/>.
        /// </summary>
        /// <param name="segments">The path segments for the <see cref="Uri"/>.</param>
        public UriFormatter SetPath(params string[] segments)
        {
            return SetPath(UriPathSegment.FromStrings(segments));
        }

        /// <summary>
        /// Sets the path for the <see cref="Uri"/>.
        /// </summary>
        /// <param name="segments">The path segments for the <see cref="Uri"/>.</param>
        public UriFormatter SetPath(IEnumerable<UriPathSegment> segments)
        {
            Guard.ArgumentNotNull(segments, "segments");

            CheckParsePath();

            InternalPathSegments.Clear();
            AddPathSegments(segments);

            _requiresParsePath = false;
            RequiresRebuildPath = true;

            return this;
        }

        /// <summary>
        /// Removes first path segment of the <see cref="Uri"/>.
        /// </summary>
        public void TrimStart()
        {
            if (PathSegments.Count > 0)
            {
                _pathSegments.RemoveAt(0);
                RequiresRebuildPath = true;
            }
        }

        /// <summary>
        /// Removes last path segment of the <see cref="Uri"/>.
        /// </summary>
        public void TrimEnd()
        {
            if (PathSegments.Count > 0)
            {
                _pathSegments.RemoveAt(_pathSegments.Count - 1);
                RequiresRebuildPath = true;
            }
        }

        /// <summary>
        /// Removes a range of path segments from the <see cref="Uri"/>.
        /// </summary>
        ///<param name="pathSegmentIndex">The zero-based starting index of the range of path segments to remove</param>
        ///<param name="pathSegmentCount">The number of path segments to remove from the end of the <see cref="Uri"/>.</param>
        public void TrimRange(int pathSegmentIndex, int pathSegmentCount)
        {
            if (PathSegments.Count >= (pathSegmentIndex + pathSegmentCount))
            {
                _pathSegments.RemoveRange(pathSegmentIndex, pathSegmentCount);
                RequiresRebuildPath = true;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a <see cref="string"/> representing the <see cref="Uri"/>.
        /// </summary>
        public override string ToString()
        {
            return Uri != null ? Uri.ToString() : base.ToString();
        }

        /// <summary>
        /// Returns the hashcode for the <see cref="UriPathSegment"/>.
        /// </summary>
        /// <returns>The hashcode for the <see cref="UriPathSegment"/>.</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Compares the specified <see cref="object"/> with this <see cref="UriPathSegment"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare.</param>
        /// <returns><b>true</b> if <paramref name="obj"/> match this <see cref="UriPathSegment"/>, otherwise <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj.ToString() == ToString();
        }

        #endregion

        #region Local Methods

        private void AddPathSegments(IEnumerable<UriPathSegment> segments)
        {
            var pathSegments = InternalPathSegments;
            foreach (var segment in segments)
            {
                pathSegments.Add(segment);
                if (segment != null)
                {
                    segment.Formatter = this;
                }
            }
        }

        /// <summary>
        /// Checks if the <see cref="Uri"/> needs rebuilding.
        /// </summary>
        private void CheckRebuildUri()
        {
            if (!_requiresRebuildPath && !_requiresRebuildQuery && !_requiresRebuildUri)
            {
                return;
            }

            _requiresRebuildUri = false;

            OnBuildUri();
        }

        /// <summary>
        /// Called when the <see cref="Uri"/> needs to be rebuilt.
        /// </summary>
        protected virtual void OnBuildUri()
        {
            var uri = new StringBuilder();

            // http
            uri.Append(string.IsNullOrEmpty(_scheme) ? Http : _scheme);

            // http:
            uri.Append(SchemeSuffix);

            // http://
            uri.Append(PathSegmentPrefix);
            uri.Append(PathSegmentPrefix);

            // http://host
            uri.Append(string.IsNullOrEmpty(_host) ? LocalHost : _host);

            // http://<host><:port>
            if (_port != UnspecifiedPort)
            {
                uri.Append(PortPrefix);
                uri.Append(_port.ToString(CultureInfo.InvariantCulture));
            }

            // http://<host><:port>/<path>
            CheckRebuildPath();
            if (!string.IsNullOrEmpty(_pathInternal))
            {
                if (!_pathInternal.StartsWith(PathSegmentPrefix, StringComparison.Ordinal))
                {
                    uri.Append(PathSegmentPrefix);
                }
                uri.Append(_pathInternal);
            }

            // http://<host><:port>/<path><?query>
            CheckRebuildQuery();
            if (!string.IsNullOrEmpty(_query))
            {
                uri.Append(QueryPrefix);
                uri.Append(_query);
            }

            // http://<host><:port>/<path><?query><#fragment>
            if (!string.IsNullOrEmpty(_fragment))
            {
                if (!_fragment.StartsWith(FragmentPrefix, StringComparison.Ordinal))
                {
                    uri.Append(FragmentPrefix);
                }
                uri.Append(_fragment);
            }

            _uri = new Uri(uri.ToString());
        }

        /// <summary>
        /// Checks if the <see cref="Uri"/> needs parsing.
        /// </summary>
        private void CheckParseUri()
        {
            if (!_requiresParseUri)
            {
                return;
            }

            _requiresParseUri = false;

            OnParseUri();
        }

        /// <summary>
        /// Called when the <see cref="Uri"/> needs to be parsed.
        /// </summary>
        protected virtual void OnParseUri()
        {
            _requiresParsePath = true;
            _requiresParseQuery = true;
            _directPath = null;

            if (_uri == null)
            {
                _port = UnspecifiedPort;
                _scheme = Http;
                _host = null;
                _fragment = null;
                _pathInternal = null;
                _query = null;
            }
            else
            {
                _scheme = _uri.Scheme;
                _port = _uri.Port;
                _host = _uri.Host;

                var path = Uri.UnescapeDataString(_uri.AbsolutePath);
                if (path.StartsWith(PathSegmentPrefix, StringComparison.Ordinal))
                {
                    path = path.Substring(PathSegmentPrefix.Length);
                }
                _pathInternal = path;

                _fragment = _uri.Fragment;
                if (_fragment.StartsWith(FragmentPrefix, StringComparison.Ordinal))
                {
                    _fragment = _fragment.Substring(FragmentPrefix.Length);
                }

                _query = _uri.Query;
                if (_query.StartsWith(QueryPrefix, StringComparison.Ordinal))
                {
                    _query = _query.Substring(QueryPrefix.Length);
                }
            }
        }

        /// <summary>
        /// Returns the segment at the specified index.
        /// </summary>
        /// <param name="index">The index of the segment.</param>
        /// <returns>The segment at the specified index.</returns>
        /// <remarks>If the <paramref name="index"/> is past the end of the current array the length is increased.</remarks>
        public UriPathSegment GetPathSegment(int index)
        {
            var segments = PathSegments;

            if (segments.Count < index + 1)
            {
                var copySegments = new UriPathSegment[index + 1];
                segments.CopyTo(copySegments, 0);
                segments = copySegments;
            }

            if (segments[index] == null)
            {
                segments[index] = new UriPathSegment {Formatter = this};
                PathSegments = segments;
            }

            return segments[index];
        }

        /// <summary>
        /// Checks if the path part of the <see cref="Uri"/> needs rebuilding.
        /// </summary>
        private void CheckRebuildPath()
        {
            if (!_requiresRebuildPath)
            {
                return;
            }

            _requiresRebuildPath = false;

            OnBuildPath();
        }

        /// <summary>
        /// Called when the <see cref="Path"/> needs to be rebuilt.
        /// </summary>
        protected virtual void OnBuildPath()
        {
            var path = new StringBuilder();

            foreach (var segment in _pathSegments)
            {
                if (segment != null)
                {
                    UriPathSegment.AppendPath(path, segment.Segment);
                }
            }

            _pathInternal = path.ToString();
        }

        /// <summary>
        /// Checks if the path part of the <see cref="Uri"/> needs parsing.
        /// </summary>
        protected void CheckParsePath()
        {
            CheckParseUri();

            if (!_requiresParsePath)
            {
                return;
            }

            _requiresParsePath = false;

            OnParsePath();
        }

        /// <summary>
        /// Called when the <see cref="Path"/> needs to be parsed.
        /// </summary>
        protected virtual void OnParsePath()
        {
            var segments = InternalPathSegments;
            segments.Clear();

            if (!string.IsNullOrEmpty(_pathInternal))
            {
                foreach (var segment in UriPathSegment.FromStrings(UriPathSegment.GetPathSegments(_pathInternal)))
                {
                    if (segment != null)
                    {
                        segment.Formatter = this;
                        segments.Add(segment);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the query part of the <see cref="Uri"/> needs rebuilding.
        /// </summary>
        private void CheckRebuildQuery()
        {
            if (!_requiresRebuildQuery)
            {
                return;
            }

            _requiresRebuildQuery = false;
            _requiresRebuildUri = true;

            OnBuildQuery();
        }

        /// <summary>
        /// Called when the <see cref="Query"/> needs to be rebuilt.
        /// </summary>
        protected virtual void OnBuildQuery()
        {
            var query = new StringBuilder();

            foreach (var pair in _queryArgs)
            {
                if (query.Length > 0)
                {
                    query.Append(QueryArgPrefix);
                }

                query.Append(Uri.EscapeDataString(pair.Key));
                query.Append(QueryArgValuePrefix);
                query.Append(Uri.EscapeDataString(pair.Value));
            }

            _query = query.ToString();
        }

        /// <summary>
        /// Checks if the query part of the <see cref="Uri"/> needs parsing.
        /// </summary>
        protected void CheckParseQuery()
        {
            CheckParseUri();

            if (!_requiresParseQuery)
            {
                return;
            }

            _requiresParseQuery = false;

            OnParseQuery();
        }

        /// <summary>
        /// Called when the <see cref="Query"/> needs to be parsed.
        /// </summary>
        protected virtual void OnParseQuery()
        {
            var queryArgs = (Dictionary<string, string>) QueryArgs;
            queryArgs.Clear();

            if (!string.IsNullOrEmpty(_query))
            {
                if (_query.StartsWith(QueryPrefix, StringComparison.Ordinal))
                {
                    _query = _query.Substring(QueryPrefix.Length);
                }

                foreach (var arg in _query.Split(new[] {QueryArgPrefix}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var pos = arg.IndexOf(QueryArgValuePrefix, StringComparison.Ordinal);
                    var key = Uri.UnescapeDataString(pos >= 0 ? arg.Substring(0, pos) : arg);
                    var value = pos >= 0 ? Uri.UnescapeDataString(arg.Substring(pos + 1)) : null;
                    queryArgs[key] = value;
                }
            }
        }

        #endregion

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        #region ISerializable Members

        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var uri = Uri;

            if (uri != null)
            {
                info.AddValue(UriName, uri.ToString());
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        #endregion
#endif

        #region Nested type: QueryArgsDictionary

        private class QueryArgsDictionary : Dictionary<string, string>, IDictionary<string, string>
        {
            private readonly UriFormatter _uri;

            public QueryArgsDictionary(UriFormatter uri)
                : base(StringComparer.OrdinalIgnoreCase)
            {
                _uri = uri;
            }

            public QueryArgsDictionary(UriFormatter uri, IDictionary<string, string> items)
                : base(items, StringComparer.OrdinalIgnoreCase)
            {
                _uri = uri;
            }

            #region IDictionary Members

            string IDictionary<string, string>.this[string key]
            {
                get
                {
                    _uri.CheckParseQuery();
                    return this[key];
                }
                set
                {
                    _uri.CheckParseQuery();
                    this[key] = value;
                    _uri.RequiresRebuildQuery = true;
                }
            }

            void IDictionary<string, string>.Add(string key, string value)
            {
                _uri.CheckParseQuery();
                Add(key, value);
                _uri.RequiresRebuildQuery = true;
            }

            bool IDictionary<string, string>.Remove(string key)
            {
                _uri.CheckParseQuery();

                if (Remove(key))
                {
                    _uri.RequiresRebuildQuery = true;
                    return true;
                }

                return false;
            }

            #endregion

            #region ICollection Members

            void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
            {
                _uri.CheckParseQuery();
                Add(item.Key, item.Value);
                _uri.RequiresRebuildQuery = true;
            }

            bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
            {
                _uri.CheckParseQuery();

                if (Remove(item.Key))
                {
                    _uri.RequiresRebuildQuery = true;
                    return true;
                }

                return false;
            }

            void ICollection<KeyValuePair<string, string>>.Clear()
            {
                _uri.CheckParseQuery();
                Clear();
                _uri.RequiresRebuildQuery = true;
            }

            #endregion
        }

        #endregion
    }
}