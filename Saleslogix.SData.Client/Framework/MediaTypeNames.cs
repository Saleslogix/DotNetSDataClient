// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// Handles the mapping of <see cref="MediaType"/> values to names and visa-versa.
    /// </summary>
    public static class MediaTypeNames
    {
        #region Constants

        /// <summary>
        /// ATOM feed content type.
        /// </summary>
        public const string AtomMediaType = "application/atom+xml";

        /// <summary>
        /// ATOM feed content type.
        /// </summary>
        public const string AtomFeedMediaType = "application/atom+xml;type=feed";

        /// <summary>
        /// ATOM entry content type.
        /// </summary>
        public const string AtomEntryMediaType = "application/atom+xml;type=entry";

        /// <summary>
        /// RSS content type.
        /// </summary>
        public const string RssMediaType = "application/rss+xml";

        /// <summary>
        /// XML content type.
        /// </summary>
        public const string XmlMediaType = "application/xml";

        /// <summary>
        /// Legacy XML content type.
        /// </summary>
        public const string LegacyXmlMediaType = "text/xml";

        /// <summary>
        /// HTML content type.
        /// </summary>
        public const string HtmlMediaType = "text/html";

        /// <summary>
        /// Javascript Object Notation (JSON) content type
        /// </summary>
        public const string JsonMediaType = "application/json";

        /// <summary>
        /// Binary Javascript Object Notation (BSON) content type
        /// </summary>
        public const string BsonMediaType = "application/bson";

        /// <summary>
        /// Form content type
        /// </summary>
        public const string FormMediaType = "application/x-www-form-urlencoded";

        /// <summary>
        /// Text content type.
        /// </summary>
        public const string TextMediaType = "text/plain";

        /// <summary>
        /// PNG Image content type.
        /// </summary>
        public const string ImagePngMediaType = "image/png";

        /// <summary>
        /// JPEG Image content type.
        /// </summary>
        public const string ImageJpegMediaType = "image/jpeg";

        /// <summary>
        /// GIF Image content type.
        /// </summary>
        public const string ImageGifMediaType = "image/gif";

        /// <summary>
        /// TIFF Image content type.
        /// </summary>
        public const string ImageTiffMediaType = "image/tiff";

        /// <summary>
        /// BMP Image content type.
        /// </summary>
        public const string ImageBmpMediaType = "image/bmp";

        /// <summary>
        /// XSLT content type.
        /// </summary>
        public const string XsltMediaType = "application/xsl";

        /// <summary>
        /// CSS content type.
        /// </summary>
        public const string CssMediaType = "text/css";

        /// <summary>
        /// Short Javascript Object Notation (JSON) content type
        /// </summary>
        public const string ShortJsonMediaType = "json";

        /// <summary>
        /// Short Binary Javascript Object Notation (BSON) content type
        /// </summary>
        public const string ShortBsonMediaType = "bson";

        /// <summary>
        /// Short ATOM feed content type.
        /// </summary>
        public const string ShortAtomMediaType = "atom";

        /// <summary>
        /// Short ATOM entry content type.
        /// </summary>
        public const string ShortAtomEntryMediaType = "atomentry";

        /// <summary>
        /// Short RSS content type.
        /// </summary>
        public const string ShortRssMediaType = "rss";

        /// <summary>
        /// XML content type.
        /// </summary>
        public const string ShortXmlMediaType = "xml";

        /// <summary>
        /// HTML content type.
        /// </summary>
        public const string ShortHtmlMediaType = "html";

        /// <summary>
        /// Text content type.
        /// </summary>
        public const string ShortTextMediaType = "text";

        /// <summary>
        /// Form content type
        /// </summary>
        public const string ShortFormMediaType = "form";

        /// <summary>
        /// Image PNG content type.
        /// </summary>
        public const string ShortImagePngMediaType = "png";

        /// <summary>
        /// Image JPEG content type.
        /// </summary>
        public const string ShortImageJpegMediaType = "jpeg";

        /// <summary>
        /// Image GIF content type.
        /// </summary>
        public const string ShortImageGifMediaType = "gif";

        /// <summary>
        /// Image TIFF content type.
        /// </summary>
        public const string ShortImageTiffMediaType = "tiff";

        /// <summary>
        /// Image BMP content type.
        /// </summary>
        public const string ShortImageBmpMediaType = "bmp";

        /// <summary>
        /// XSLT content type.
        /// </summary>
        public const string ShortXsltMediaType = "xslt";

        /// <summary>
        /// CSS content type.
        /// </summary>
        public const string ShortCssMediaType = "css";

        /// <summary>
        /// Returns the default <see cref="MediaType"/>.
        /// </summary>
        /// <value>The default <see cref="MediaType"/>, which is <b>Atom</b>.</value>
        public const MediaType DefaultMediaType = MediaType.Atom;

        #endregion

        #region Fields

        private static readonly IDictionary<MediaType, string> _mediaTypeToName;
        private static readonly IDictionary<ContentType, MediaType> _nameToMediaType;
        private static readonly IDictionary<MediaType, string> _mediaTypeToShortName;
        private static readonly IDictionary<string, MediaType> _shortNameToMediaType;

        #endregion

        static MediaTypeNames()
        {
            _mediaTypeToName = new Dictionary<MediaType, string>();
            _mediaTypeToName[MediaType.Text] = TextMediaType;
            _mediaTypeToName[MediaType.Html] = HtmlMediaType;
            _mediaTypeToName[MediaType.Atom] = AtomMediaType;
            _mediaTypeToName[MediaType.AtomEntry] = AtomEntryMediaType;
            _mediaTypeToName[MediaType.Rss] = RssMediaType;
            _mediaTypeToName[MediaType.Xml] = XmlMediaType;
            _mediaTypeToName[MediaType.ImagePng] = ImagePngMediaType;
            _mediaTypeToName[MediaType.ImageGif] = ImageGifMediaType;
            _mediaTypeToName[MediaType.ImageTiff] = ImageTiffMediaType;
            _mediaTypeToName[MediaType.ImageBmp] = ImageBmpMediaType;
            _mediaTypeToName[MediaType.ImageJpeg] = ImageJpegMediaType;
            _mediaTypeToName[MediaType.Xslt] = XsltMediaType;
            _mediaTypeToName[MediaType.Css] = CssMediaType;
            _mediaTypeToName[MediaType.Json] = JsonMediaType;
            _mediaTypeToName[MediaType.Bson] = BsonMediaType;
            _mediaTypeToName[MediaType.Form] = FormMediaType;

            _nameToMediaType = new Dictionary<ContentType, MediaType>(new ContentTypeComparer());
            _nameToMediaType[new ContentType(TextMediaType)] = MediaType.Text;
            _nameToMediaType[new ContentType(HtmlMediaType)] = MediaType.Html;
            _nameToMediaType[new ContentType(AtomMediaType)] = MediaType.Atom;
            _nameToMediaType[new ContentType(AtomFeedMediaType)] = MediaType.Atom;
            _nameToMediaType[new ContentType(AtomEntryMediaType)] = MediaType.AtomEntry;
            _nameToMediaType[new ContentType(RssMediaType)] = MediaType.Rss;
            _nameToMediaType[new ContentType(XmlMediaType)] = MediaType.Xml;
            _nameToMediaType[new ContentType(LegacyXmlMediaType)] = MediaType.Xml;
            _nameToMediaType[new ContentType(ImagePngMediaType)] = MediaType.ImagePng;
            _nameToMediaType[new ContentType(ImageJpegMediaType)] = MediaType.ImageJpeg;
            _nameToMediaType[new ContentType(ImageGifMediaType)] = MediaType.ImageGif;
            _nameToMediaType[new ContentType(ImageTiffMediaType)] = MediaType.ImageTiff;
            _nameToMediaType[new ContentType(ImageBmpMediaType)] = MediaType.ImageBmp;
            _nameToMediaType[new ContentType(XsltMediaType)] = MediaType.Xslt;
            _nameToMediaType[new ContentType(CssMediaType)] = MediaType.Css;
            _nameToMediaType[new ContentType(JsonMediaType)] = MediaType.Json;
            _nameToMediaType[new ContentType(BsonMediaType)] = MediaType.Bson;
            _nameToMediaType[new ContentType(FormMediaType)] = MediaType.Form;

            _mediaTypeToShortName = new Dictionary<MediaType, string>();
            _mediaTypeToShortName[MediaType.Text] = ShortTextMediaType;
            _mediaTypeToShortName[MediaType.Html] = ShortHtmlMediaType;
            _mediaTypeToShortName[MediaType.Atom] = ShortAtomMediaType;
            _mediaTypeToShortName[MediaType.AtomEntry] = ShortAtomEntryMediaType;
            _mediaTypeToShortName[MediaType.Rss] = ShortRssMediaType;
            _mediaTypeToShortName[MediaType.Xml] = ShortXmlMediaType;
            _mediaTypeToShortName[MediaType.ImagePng] = ShortImagePngMediaType;
            _mediaTypeToShortName[MediaType.ImageJpeg] = ShortImageJpegMediaType;
            _mediaTypeToShortName[MediaType.ImageGif] = ShortImageGifMediaType;
            _mediaTypeToShortName[MediaType.ImageTiff] = ShortImageTiffMediaType;
            _mediaTypeToShortName[MediaType.ImageBmp] = ShortImageBmpMediaType;
            _mediaTypeToShortName[MediaType.Xslt] = ShortXsltMediaType;
            _mediaTypeToShortName[MediaType.Css] = ShortCssMediaType;
            _mediaTypeToShortName[MediaType.Json] = ShortJsonMediaType;
            _mediaTypeToShortName[MediaType.Bson] = ShortBsonMediaType;
            _mediaTypeToShortName[MediaType.Form] = ShortFormMediaType;

            _shortNameToMediaType = new Dictionary<string, MediaType>(StringComparer.OrdinalIgnoreCase);
            _shortNameToMediaType[ShortTextMediaType] = MediaType.Text;
            _shortNameToMediaType[ShortHtmlMediaType] = MediaType.Html;
            _shortNameToMediaType[ShortAtomMediaType] = MediaType.Atom;
            _shortNameToMediaType[ShortAtomEntryMediaType] = MediaType.AtomEntry;
            _shortNameToMediaType[ShortRssMediaType] = MediaType.Rss;
            _shortNameToMediaType[ShortXmlMediaType] = MediaType.Xml;
            _shortNameToMediaType[ShortImagePngMediaType] = MediaType.ImagePng;
            _shortNameToMediaType[ShortImageJpegMediaType] = MediaType.ImageJpeg;
            _shortNameToMediaType[ShortImageGifMediaType] = MediaType.ImageGif;
            _shortNameToMediaType[ShortImageTiffMediaType] = MediaType.ImageTiff;
            _shortNameToMediaType[ShortImageBmpMediaType] = MediaType.ImageBmp;
            _shortNameToMediaType[ShortXsltMediaType] = MediaType.Xslt;
            _shortNameToMediaType[ShortCssMediaType] = MediaType.Css;
            _shortNameToMediaType[ShortJsonMediaType] = MediaType.Json;
            _shortNameToMediaType[ShortBsonMediaType] = MediaType.Bson;
            _shortNameToMediaType[ShortFormMediaType] = MediaType.Form;
        }

        /// <summary>
        /// Returns the name of the content type for the specified <see cref="MediaType"/>.
        /// </summary>
        /// <param name="type">One of the <see cref="MediaType"/> values.</param>
        /// <returns>A <see cref="string"/> containing the name of the specified <see cref="MediaType"/>.</returns>
        public static string GetMediaType(MediaType type)
        {
            return _mediaTypeToName[type];
        }

        /// <summary>
        /// Returns the <see cref="MediaType"/> for the specified name.
        /// </summary>
        /// <param name="name">The name of the content type.</param>
        /// <returns>The <see cref="MediaType"/> that matches the specified name.</returns>
        public static MediaType GetMediaType(string name)
        {
            Guard.ArgumentNotNullOrEmptyString(name, "name");

            if (name.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))
            {
                return MediaType.Multipart;
            }

            return _nameToMediaType[new ContentType(name)];
        }

        /// <summary>
        /// Returns the <see cref="MediaType"/> for the specified name.
        /// </summary>
        /// <param name="name">The name of the content type.</param>
        /// <param name="mediaType">On return contains the <see cref="MediaType"/> for the specified name.</param>
        /// <returns><b>true</b> if the content type was found, otherwise <b>false</b>.</returns>
        public static bool TryGetMediaType(string name, out MediaType mediaType)
        {
            if (name != null && name.StartsWith("multipart/", StringComparison.OrdinalIgnoreCase))
            {
                mediaType = MediaType.Multipart;
                return true;
            }

            if (string.IsNullOrEmpty(name))
            {
                mediaType = DefaultMediaType;
                return false;
            }

            ContentType key;
            try
            {
                key = new ContentType(name);
            }
            catch (FormatException)
            {
                mediaType = DefaultMediaType;
                return false;
            }

            if (!_nameToMediaType.TryGetValue(key, out mediaType))
            {
                mediaType = DefaultMediaType;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the short name of the content type for the specified <see cref="MediaType"/>.
        /// </summary>
        /// <param name="type">One of the <see cref="MediaType"/> values.</param>
        /// <returns>A <see cref="string"/> containing the short name of the specified <see cref="MediaType"/>.</returns>
        public static string GetShortMediaType(MediaType type)
        {
            return _mediaTypeToShortName[type];
        }

        /// <summary>
        /// Returns the <see cref="MediaType"/> for the specified short name.
        /// </summary>
        /// <param name="name">The short name of the content type.</param>
        /// <returns>The <see cref="MediaType"/> that matches the specified name.</returns>
        public static MediaType GetShortMediaType(string name)
        {
            return _shortNameToMediaType[name];
        }

        /// <summary>
        /// Returns the <see cref="MediaType"/> for the specified short name.
        /// </summary>
        /// <param name="name">The short name of the content type.</param>
        /// <param name="mediaType">On return contains the <see cref="MediaType"/> for the specified name.</param>
        /// <returns><b>true</b> if the content type was found, otherwise <b>false</b>.</returns>
        public static bool TryGetShortMediaType(string name, out MediaType mediaType)
        {
            if (_shortNameToMediaType.TryGetValue(name, out mediaType))
            {
                return true;
            }

            mediaType = DefaultMediaType;
            return false;
        }

        #region Nested type: ContentTypeComparer

        private class ContentTypeComparer : IEqualityComparer<ContentType>
        {
            #region IEqualityComparer<ContentType> Members

            public bool Equals(ContentType x, ContentType y)
            {
                return string.Equals(x.MediaType, y.MediaType, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x.SubType, y.SubType, StringComparison.OrdinalIgnoreCase) &&
                       string.Equals(x["type"], y["type"], StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(ContentType obj)
            {
                var code = obj.MediaType.ToLowerInvariant().GetHashCode() ^
                           obj.SubType.ToLowerInvariant().GetHashCode();

                var type = obj["type"];
                if (type != null)
                {
                    code ^= type.ToLowerInvariant().GetHashCode();
                }

                return code;
            }

            #endregion
        }

        #endregion
    }
}