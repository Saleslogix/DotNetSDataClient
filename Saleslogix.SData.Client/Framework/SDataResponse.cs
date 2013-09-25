// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

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
        private IDictionary<string, string> _form;
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
            _form = new Dictionary<string, string>();
            _files = new List<AttachedFile>();

            if (_statusCode != HttpStatusCode.NoContent)
            {
                using (var responseStream = response.GetResponseStream())
                {
                    string boundary;

                    if (_contentType == MediaType.Multipart && TryGetMultipartBoundary(response.ContentType, out boundary))
                    {
                        var multipart = MimeMessage.Parse(responseStream, boundary);
                        var isFormData = string.Equals(new ContentType(response.ContentType).SubType, "form-data", StringComparison.OrdinalIgnoreCase);

                        foreach (var part in multipart)
                        {
                            if (_content == null && MediaTypeNames.TryGetMediaType(part.ContentType, out contentType))
                            {
                                _contentType = contentType;
                                _content = LoadContent(part.Content, _contentType.Value);
                            }
                            else
                            {
                                if (isFormData)
                                {
                                    var name = ContentDisposition.Parse(part.ContentDisposition)["name"];
                                    if (name != null)
                                    {
                                        var value = new StreamReader(part.Content).ReadToEnd();
                                        _form.Add(name, value);
                                        continue;
                                    }
                                }

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
                        else if (_statusCode >= HttpStatusCode.BadRequest)
                        {
                            if (ContentHelper.IsDictionary(_content))
                            {
                                var diagnosis = ContentHelper.Deserialize<Diagnosis>(_content);
                                if (diagnosis != null)
                                {
                                    throw new SDataException(new Diagnoses {diagnosis}, _statusCode);
                                }
                            }
                            else if (ContentHelper.IsCollection(_content))
                            {
                                var diagnoses = ContentHelper.Deserialize<Diagnoses>(_content);
                                if (diagnoses != null)
                                {
                                    throw new SDataException(diagnoses, _statusCode);
                                }
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
        /// Gets the form data attached to the response.
        /// </summary>
        public IDictionary<string, string> Form
        {
            get { return _form ?? (_form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)); }
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
            }

            using (var memory = new MemoryStream())
            {
                stream.CopyTo(memory);
                return memory.ToArray();
            }
        }
    }
}