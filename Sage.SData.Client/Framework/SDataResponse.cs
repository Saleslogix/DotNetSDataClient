// Copyright (c) Sage (UK) Limited 2010. All rights reserved.
// This code may not be copied or used, except as set out in a written licence agreement
// between the user and Sage (UK) Limited, which specifically permits the user to use this
// code. Please contact Sage (UK) if you do not have such a licence. Sage will take
// appropriate legal action against those who make unauthorised use of this code.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Mime;
using Sage.SData.Client.Atom;
using Sage.SData.Client.Mime;

namespace Sage.SData.Client.Framework
{
    /// <summary>
    /// An interface which encapsulates interesting information returned
    /// from a request.
    /// </summary>
    public interface ISDataResponse
    {
        /// <summary>
        /// The response status code.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// The response content type.
        /// </summary>
        MediaType? ContentType { get; }

        /// <summary>
        /// The response ETag.
        /// </summary>
        string ETag { get; }

        /// <summary>
        /// The response location.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// The response content.
        /// </summary>
        object Content { get; }

        /// <summary>
        /// Gets the files attached to the response.
        /// </summary>
        IList<AttachedFile> Files { get; }
    }

    /// <summary>
    /// The response class which encapsulates interesting information returned
    /// from a request.
    /// </summary>
    public class SDataResponse : ISDataResponse
    {
        private readonly HttpStatusCode _statusCode;
        private readonly MediaType? _contentType;
        private readonly string _eTag;
        private readonly string _location;
        private readonly object _content;
        private readonly IList<AttachedFile> _files;

        internal SDataResponse(WebResponse response, string redirectLocation)
        {
            var httpResponse = response as HttpWebResponse;
            _statusCode = httpResponse != null ? httpResponse.StatusCode : 0;

            MediaType contentType;
            if (MediaTypeNames.TryGetMediaType(response.ContentType, out contentType))
            {
                _contentType = contentType;
            }

            _eTag = response.Headers[HttpResponseHeader.ETag];
            _location = response.Headers[HttpResponseHeader.Location] ?? redirectLocation;
            _files = new List<AttachedFile>();

            if (_statusCode != HttpStatusCode.NoContent)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    string boundary;

                    if (_contentType == MediaType.Multipart && TryGetMultipartBoundary(response.ContentType, out boundary))
                    {
                        var multipart = MimeMessage.Parse(responseStream, boundary);

                        foreach (var part in multipart)
                        {
                            if (_content == null && MediaTypeNames.TryGetMediaType(part.ContentType, out contentType))
                            {
                                _contentType = contentType;
                                _content = LoadContent(part.Content, null, _contentType.Value);
                            }
                            else
                            {
                                _files.Add(new AttachedFile(part));
                            }
                        }
                    }
                    else
                    {
                        _content = LoadContent(responseStream, _statusCode, _contentType);
                    }
                }
            }
        }

        private static bool TryGetMultipartBoundary(string contentType, out string boundary)
        {
            try
            {
                var type = new ContentType(contentType);
                boundary = type.Boundary;
                return !string.IsNullOrEmpty(boundary);
            }
            catch (FormatException)
            {
                boundary = null;
                return false;
            }
        }

        /// <summary>
        /// The response status code.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>
        /// The response content type.
        /// </summary>
        public MediaType? ContentType
        {
            get { return _contentType; }
        }

        /// <summary>
        /// The response ETag.
        /// </summary>
        public string ETag
        {
            get { return _eTag; }
        }

        /// <summary>
        /// The response location.
        /// </summary>
        public string Location
        {
            get { return _location; }
        }

        /// <summary>
        /// The response content.
        /// </summary>
        public object Content
        {
            get { return _content; }
        }

        /// <summary>
        /// Gets the files attached to the response.
        /// </summary>
        public IList<AttachedFile> Files
        {
            get { return _files; }
        }

        private static object LoadContent(Stream stream, HttpStatusCode? statusCode, MediaType? contentType)
        {
            switch (contentType)
            {
                case MediaType.Atom:
                    return LoadFeedContent(stream);
                case MediaType.AtomEntry:
                    return LoadEntryContent(stream);
                case MediaType.Xml:
                    return LoadXmlContent(stream, statusCode);
                default:
                    return LoadOtherContent(stream, contentType);
            }
        }

        private static AtomFeed LoadFeedContent(Stream stream)
        {
            var feed = new AtomFeed();
            feed.Load(stream);
            return feed;
        }

        private static AtomEntry LoadEntryContent(Stream stream)
        {
            var entry = new AtomEntry();
            entry.Load(stream);
            return entry;
        }

        private static object LoadXmlContent(Stream stream, HttpStatusCode? statusCode)
        {
            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);

                memory.Seek(0, SeekOrigin.Begin);
                var tracking = memory.DeserializeXml<Tracking>();
                if (tracking != null)
                {
                    return tracking;
                }

                memory.Seek(0, SeekOrigin.Begin);
                var diagnoses = memory.DeserializeXml<Diagnoses>();
                if (diagnoses != null)
                {
                    if (statusCode != null)
                    {
                        throw new SDataException(diagnoses, statusCode.Value);
                    }
                    return diagnoses;
                }

                memory.Seek(0, SeekOrigin.Begin);
                var diagnosis = memory.DeserializeXml<Diagnosis>();
                if (diagnosis != null)
                {
                    if (statusCode != null)
                    {
                        throw new SDataException(new Collection<Diagnosis> {diagnosis}, statusCode.Value);
                    }
                    return diagnosis;
                }

                memory.Seek(0, SeekOrigin.Begin);
                return LoadStringContent(memory);
            }
        }

        private static object LoadOtherContent(Stream stream, MediaType? contentType)
        {
            if (contentType != null)
            {
                return LoadStringContent(stream);
            }

            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }

        private static string LoadStringContent(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}