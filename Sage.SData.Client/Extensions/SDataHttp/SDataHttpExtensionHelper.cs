using System;
using System.Linq;
using System.Net;
using Sage.SData.Client.Atom;
using Sage.SData.Client.Common;
using Sage.SData.Client.Framework;

namespace Sage.SData.Client.Extensions
{
    /// <summary>
    /// Helper class for accessing AtomEntry SDataHttpExtensions
    /// </summary>
    public static class SDataHttpExtensionHelper
    {
        /// <summary>
        /// Extension method to retrieve SData HTTP method
        /// </summary>
        public static HttpMethod? GetSDataHttpMethod(this AtomEntry entry)
        {
            var context = GetContext(entry, false);
            return context != null ? context.HttpMethod : null;
        }

        /// <summary>
        /// Extension method to retrieve SData HTTP status
        /// </summary>
        public static HttpStatusCode? GetSDataHttpStatus(this AtomEntry entry)
        {
            var context = GetContext(entry, false);
            return context != null ? context.HttpStatus : null;
        }

        /// <summary>
        /// Extension method to retrieve SData HTTP message
        /// </summary>
        public static string GetSDataHttpMessage(this AtomEntry entry)
        {
            var context = GetContext(entry, false);
            return context != null ? context.HttpMessage : null;
        }

        /// <summary>
        /// Extension method to retrieve SData HTTP location
        /// </summary>
        public static Uri GetSDataHttpLocation(this AtomEntry entry)
        {
            var context = GetContext(entry, false);
            return context != null ? context.Location : null;
        }

        /// <summary>
        /// Extension method to retrieve SData HTTP etag
        /// </summary>
        public static string GetSDataHttpETag(this AtomEntry entry)
        {
            var context = GetContext(entry, false);
            return context != null ? context.ETag : null;
        }

        /// <summary>
        /// Extension method to retrieve SData HTTP if match
        /// </summary>
        public static string GetSDataHttpIfMatch(this AtomEntry entry)
        {
            var context = GetContext(entry, false);
            return context != null ? context.IfMatch : null;
        }

        /// <summary>
        /// Extension method to set SData HTTP method
        /// </summary>
        public static void SetSDataHttpMethod(this AtomEntry entry, HttpMethod? value)
        {
            GetContext(entry, true).HttpMethod = value;
        }

        /// <summary>
        /// Extension method to set SData HTTP status
        /// </summary>
        public static void SetSDataHttpStatus(this AtomEntry entry, HttpStatusCode? value)
        {
            GetContext(entry, true).HttpStatus = value;
        }

        /// <summary>
        /// Extension method to set SData HTTP message
        /// </summary>
        public static void SetSDataHttpMessage(this AtomEntry entry, string value)
        {
            GetContext(entry, true).HttpMessage = value;
        }

        /// <summary>
        /// Extension method to set SData HTTP location
        /// </summary>
        public static void SetSDataHttpLocation(this AtomEntry entry, Uri value)
        {
            GetContext(entry, true).Location = value;
        }

        /// <summary>
        /// Extension method to set SData HTTP etag
        /// </summary>
        public static void SetSDataHttpETag(this AtomEntry entry, string value)
        {
            GetContext(entry, true).ETag = value;
        }

        /// <summary>
        /// Extension method to set SData HTTP if method
        /// </summary>
        public static void SetSDataHttpIfMatch(this AtomEntry entry, string value)
        {
            GetContext(entry, true).IfMatch = value;
        }

        private static SDataHttpExtensionContext GetContext(IExtensibleSyndicationObject entry, bool createIfMissing)
        {
            Guard.ArgumentNotNull(entry, "entry");
            var extension = entry.Extensions.OfType<SDataHttpExtension>().FirstOrDefault();

            if (extension == null)
            {
                if (!createIfMissing)
                {
                    return null;
                }

                extension = new SDataHttpExtension();
                entry.AddExtension(extension);
            }

            return extension.Context;
        }
    }
}