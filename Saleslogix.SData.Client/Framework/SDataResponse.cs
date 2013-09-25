// Copyright (c) Sage (UK) Limited 2010. All rights reserved.
// This code may not be copied or used, except as set out in a written licence agreement
// between the user and Sage (UK) Limited, which specifically permits the user to use this
// code. Please contact Sage (UK) if you do not have such a licence. Sage will take
// appropriate legal action against those who make unauthorised use of this code.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Mime;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// The response class which encapsulates interesting information returned
    /// from a request.
    /// </summary>
    public class SDataResponse
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

            _eTag = response.Headers["ETag"];
            _location = response.Headers["Location"] ?? redirectLocation;
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
                                _content = LoadContent(part.Content, _contentType.Value);
                            }
                            else
                            {
                                _files.Add(new AttachedFile(part));
                            }
                        }
                    }
                    else
                    {
                        _content = LoadContent(responseStream, _contentType);

                        if (_statusCode == HttpStatusCode.Accepted)
                        {
                            var tracking = ContentHelper.Deserialize<Tracking>(_content);
                            if (tracking != null)
                            {
                                _content = tracking;
                            }
                        }

                        if (_statusCode >= HttpStatusCode.BadRequest)
                        {
                            var diagnoses = ContentHelper.Deserialize<Diagnoses>(_content);
                            if (diagnoses != null)
                            {
                                throw new SDataException(diagnoses, _statusCode);
                            }

                            var diagnosis = ContentHelper.Deserialize<Diagnosis>(_content);
                            if (diagnosis != null)
                            {
                                throw new SDataException(new Diagnoses {diagnosis}, _statusCode);
                            }
                        }
                    }
                }
            }
        }

        private static bool TryGetMultipartBoundary(string contentType, out string boundary)
        {
            try
            {
                var type = new ContentType(contentType);
                boundary = type["boundary"];
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

        private static object LoadContent(Stream stream, MediaType? contentType)
        {
            if (contentType != null)
            {
                var handler = ContentManager.GetHandler(contentType.Value);
                if (handler != null)
                {
                    return handler.ReadFrom(stream);
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }

            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
    }
}