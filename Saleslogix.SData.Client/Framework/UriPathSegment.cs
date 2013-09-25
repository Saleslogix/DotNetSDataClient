// Copyright (c) Sage (UK) Limited 2010. All rights reserved.
// This code may not be copied or used, except as set out in a written licence agreement
// between the user and Sage (UK) Limited, which specifically permits the user to use this
// code. Please contact Sage (UK) if you do not have such a licence. Sage will take
// appropriate legal action against those who make unauthorised use of this code.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Defines a path segment within a <see cref="Uri"/>
    /// </summary>
    [Serializable]
    public class UriPathSegment
    {
        #region Constants

        /// <summary>
        /// Returns the prefix to use for a selector.
        /// </summary>
        /// <value>The prefix to use for a selector.</value>
        public const string SelectorPrefix = "(";

        /// <summary>
        /// Returns the suffix to use for a selector.
        /// </summary>
        /// <value>The suffix to use for a selector.</value>
        public const string SelectorSuffix = ")";

        #endregion

        #region Fields

        private string _segment;
        private string _selector;
        private string _text;
        private bool _requiresParse;
        private bool _requiresRebuild;
        private WeakReference _formatter;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="UriPathSegment"/> class.
        /// </summary>
        public UriPathSegment()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriPathSegment"/> class with
        /// the specified text.
        /// </summary>
        /// <param name="segment">The text and selector for the segment.</param>
        public UriPathSegment(string segment)
        {
            Segment = segment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriPathSegment"/> class with
        /// the specified text and selector.
        /// </summary>
        /// <param name="text">The text for the segment.</param>
        /// <param name="selector">The selector for the segment.</param>
        public UriPathSegment(string text, string selector)
        {
            Text = text;
            Selector = selector;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UriPathSegment"/> class with
        /// the details of the specified segment.
        /// </summary>
        /// <param name="segment">The segment to copy the details from.</param>
        public UriPathSegment(UriPathSegment segment)
        {
            _segment = segment._segment;
            _selector = segment._selector;
            _text = segment._text;
            _requiresParse = segment._requiresParse;
            _requiresRebuild = segment._requiresRebuild;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the text for the segment.
        /// </summary>
        /// <value>The text for the segment.</value>
        public string Text
        {
            get
            {
                CheckParse();
                return _text;
            }
            set
            {
                CheckParse();
                _text = value;
                RequiresRebuild = true;
            }
        }

        /// <summary>
        /// Gets or sets the selector for the segment.
        /// </summary>
        /// <value>The selector for the segment.</value>
        public string Selector
        {
            get
            {
                CheckParse();
                return _selector;
            }
            set
            {
                CheckParse();
                _selector = value;
                RequiresRebuild = true;
            }
        }

        /// <summary>
        /// Returns a value indicating if this segment has a selector.
        /// </summary>
        /// <value><b>true</b> if this segment has a selector, otherwise <b>false</b>.</value>
        public bool HasSelector
        {
            get { return !string.IsNullOrEmpty(Selector); }
        }

        /// <summary>
        /// Gets or sets the text and selector for the segment.
        /// </summary>
        /// <value>The text and selector for the segment.</value>
        public string Segment
        {
            get
            {
                CheckRebuild();
                return _segment;
            }
            set
            {
                _segment = value;
                _requiresParse = true;
                RequiresRebuild = false;

                if (_formatter != null)
                {
                    var formatter = _formatter.Target as UriFormatter;

                    if (formatter != null)
                    {
                        formatter.RequiresRebuildPath = true;
                    }
                }
            }
        }

        private bool RequiresRebuild
        {
            get { return _requiresRebuild; }
            set
            {
                _requiresRebuild = value;

                if (value && _formatter != null)
                {
                    var formatter = _formatter.Target as UriFormatter;

                    if (formatter != null)
                    {
                        formatter.RequiresRebuildPath = true;
                    }
                }
            }
        }

        internal UriFormatter Formatter
        {
            set { _formatter = new WeakReference(value); }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a <see cref="String"/> representation of the segment.
        /// </summary>
        /// <returns>A <see cref="String"/> representation of the segment.</returns>
        public override string ToString()
        {
            return Segment;
        }

        /// <summary>
        /// Returns the hashcode for the <see cref="UriPathSegment"/>.
        /// </summary>
        /// <returns>The hashcode for the <see cref="UriPathSegment"/>.</returns>
        public override int GetHashCode()
        {
            return Segment.GetHashCode();
        }

        /// <summary>
        /// Compares the specified <see cref="object"/> with this <see cref="UriPathSegment"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare.</param>
        /// <returns><b>true</b> if <paramref name="obj"/> match this <see cref="UriPathSegment"/>, otherwise <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            return obj.ToString() == ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds an enumerable of <see cref="UriPathSegment"/> objects from an enumerable of <see cref="String"/>
        /// <see cref="Array"/>.
        /// </summary>
        /// <param name="segments"><see cref="Array"/> of strings.</param>
        /// <returns><see cref="Array"/> of <see cref="UriPathSegment"/> objects.</returns>
        public static IEnumerable<UriPathSegment> FromStrings(IEnumerable<string> segments)
        {
            return segments.SelectMany(UriPathParser.Parse).ToArray();
        }

        /// <summary>
        /// Returns the segments that make up the specified path.
        /// </summary>
        /// <param name="path">The path to return the segments for.</param>
        /// <returns>Array of segments that make up the specified path.</returns>
        public static IEnumerable<string> GetPathSegments(string path)
        {
            return UriPathParser.Parse(path).Select(segment => segment.Segment);
        }

        /// <summary>
        /// Appends a segment to a path.
        /// </summary>
        /// <param name="path">The path to append the segment to.</param>
        /// <param name="segment">The segment to append to the path.</param>
        /// <returns>A <see cref="String"/> containing the segment appended to the path.</returns>
        /// <remarks>If the <paramref name="segment"/> starts with a forward slash it is assumed that the segment specifies an absolute path.</remarks>
        public static string AppendPath(string path, string segment)
        {
            segment = ValidateSegment(segment);

            if (segment.StartsWith(UriFormatter.PathSegmentPrefix))
            {
                return segment.Substring(UriFormatter.PathSegmentPrefix.Length);
            }

            if (string.IsNullOrEmpty(path))
            {
                return segment;
            }

            return path + UriFormatter.PathSegmentPrefix + segment;
        }

        /// <summary>
        /// Appends a segment to a path.
        /// </summary>
        /// <param name="path">The path to append the segment to.</param>
        /// <param name="segment">The segment to append to the path.</param>
        /// <remarks>If the <paramref name="segment"/> starts with a forward slash it is assumed that the segment specifies an absolute path.</remarks>
        public static void AppendPath(StringBuilder path, string segment)
        {
            segment = ValidateSegment(segment);

            if (segment.StartsWith(UriFormatter.PathSegmentPrefix))
            {
                path.Length = 0;
                path.Append(segment.Substring(UriFormatter.PathSegmentPrefix.Length));
            }
            else
            {
                if (path.Length > 0)
                {
                    path.Append(UriFormatter.PathSegmentPrefix);
                }

                path.Append(segment);
            }
        }

        #endregion

        #region Local Methods

        private static string ValidateSegment(string segment)
        {
            return segment;
        }

        private void CheckRebuild()
        {
            if (!RequiresRebuild)
            {
                return;
            }

            RequiresRebuild = false;

            OnRebuild();
        }

        /// <summary>
        /// Called when the <see cref="Segment"/> needs rebuilding using the <see cref="Text"/>
        /// and <see cref="Selector"/> values.
        /// </summary>
        private void OnRebuild()
        {
            var segment = new StringBuilder(_text);

            if (HasSelector)
            {
                segment.AppendFormat("{0}{1}{2}", SelectorPrefix, _selector, SelectorSuffix);
            }

            _segment = segment.ToString();
        }

        private void CheckParse()
        {
            if (!_requiresParse)
            {
                return;
            }

            _requiresParse = false;

            OnParse();
        }

        /// <summary>
        /// Called when the <see cref="Segment"/> needs parsing to extract the
        /// <see cref="Text"/> and <see cref="Selector"/> values.
        /// </summary>
        private void OnParse()
        {
            _text = string.Empty;
            _selector = string.Empty;

            if (!string.IsNullOrEmpty(_segment))
            {
                var segments = UriPathParser.Parse(_segment);

                if (segments.Count > 0)
                {
                    var segment = segments[0];

                    _text = segment.Text;
                    _selector = segment.Selector;
                }
            }
        }

        #endregion
    }
}