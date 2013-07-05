// Copyright (c) Sage (UK) Limited 2007. All rights reserved.
// This code may not be copied or used, except as set out in a written licence agreement
// between the user and Sage (UK) Limited, which specifically permits the user to use
// this code. Please contact [email@sage.com] if you do not have such a licence.
// Sage will take appropriate legal action against those who make unauthorised use of this
// code.

namespace Sage.SData.Client.Framework
{
    /// <summary>
    /// Provides the common elements for Syndication
    /// </summary>
    internal static class Common
    {
        /// <summary>
        /// Prefix for xml namespace
        /// </summary>
        public const string XmlNs = "xmlns";

        /// <summary>
        /// XS namespace
        /// </summary>
        public static class Xs
        {
            /// <summary>
            /// Xml Schema namespace.
            /// </summary>
            public const string Namespace = "http://www.w3.org/2001/XMLSchema";

            /// <summary>
            /// Prefix for XmlSchema namespace
            /// </summary>
            public const string Prefix = "xs";
        }

        /// <summary>
        /// XSI namespace
        /// </summary>
        public static class Xsi
        {
            /// <summary>
            /// Namespace for XSI 
            /// </summary>
            public const string Namespace = "http://www.w3.org/2001/XMLSchema-instance";

            /// <summary>
            /// Prefix for XSI namespace
            /// </summary>
            public const string Prefix = "xsi";

            /// <summary>
            /// Nil
            /// </summary>
            public const string Nil = "nil";
        }

        /// <summary>
        /// ATOM namespace
        /// </summary>
        public static class Atom
        {
            /// <summary>
            /// URI for Atom namespace
            /// </summary>
            public const string Namespace = "http://www.w3.org/2005/Atom";

            /// <summary>
            /// Prefix for Atom namespace
            /// </summary>
            public const string Prefix = "atom";
        }

        /// <summary>
        /// SData namespace
        /// </summary>
        public static class SData
        {
            /// <summary>
            /// URI for SData namespace
            /// </summary>
            public const string Namespace = "http://schemas.sage.com/sdata/2008/1";

            /// <summary>
            /// Prefix for SData namespace
            /// </summary>
            public const string Prefix = "sdata";
        }

        /// <summary>
        /// SME namespace
        /// </summary>
        public static class Sme
        {
            /// <summary>
            /// URI for SME namespace
            /// </summary>
            public const string Namespace = "http://schemas.sage.com/sdata/sme/2007";

            /// <summary>
            /// Prefix for SME namespace
            /// </summary>
            public const string Prefix = "sme";
        }

        /// <summary>
        /// HTTP namespace
        /// </summary>
        public static class Http
        {
            /// <summary>
            /// Namespace for SData HTTP elements 
            /// </summary>
            public const string Namespace = "http://schemas.sage.com/sdata/http/2008/1";

            /// <summary>
            /// Prefix for SData Http header elements.
            /// </summary>
            public const string Prefix = "http";
        }

        /// <summary>
        /// Sync namespace
        /// </summary>
        public static class Sync
        {
            /// <summary>
            /// Namespace for SData Sync elements 
            /// </summary>
            public const string Namespace = "http://schemas.sage.com/sdata/sync/2008/1";

            /// <summary>
            /// Prefix for SData Http header elements.
            /// </summary>
            public const string Prefix = "sync";
        }

        /// <summary>
        /// SLE namespace
        /// </summary>
        public static class Sle
        {
            /// <summary>
            /// URI for SLE namespace
            /// </summary>
            public const string Namespace = "http://www.microsoft.com/schemas/rss/core/2005";

            /// <summary>
            /// Prefix for SLE namespace
            /// </summary>
            public const string Prefix = "cf";
        }

        /// <summary>
        /// OpenSearch namespace
        /// </summary>
        public static class OpenSearch
        {
            /// <summary>
            /// URI for OpenSearch namespace
            /// </summary>
            public const string Namespace = "http://a9.com/-/spec/opensearch/1.1/";

            /// <summary>
            /// Prefix for OpenSearch namespace
            /// </summary>
            public const string Prefix = "opensearch";
        }
    }
}