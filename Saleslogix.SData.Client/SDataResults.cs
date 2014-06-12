// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client
{
    public class SDataResults : ISDataResults
    {
        private readonly HttpStatusCode _statusCode;
        private readonly MediaType? _contentType;
        private readonly string _eTag;
        private readonly string _location;
        private readonly DateTimeOffset? _expires;
        private readonly DateTimeOffset? _retryAfter;
        private readonly IDictionary<string, string> _form;
        private readonly IList<AttachedFile> _files;

        public static ISDataResults FromResponse(SDataResponse response)
        {
            Guard.ArgumentNotNull(response, "response");
            return new SDataResults(response.StatusCode,
                                    response.ContentType,
                                    response.ETag,
                                    response.Location,
                                    response.Expires,
                                    response.RetryAfter,
                                    response.Form,
                                    response.Files);
        }

        public static ISDataResults<T> FromResponse<T>(SDataResponse response, T content)
        {
            Guard.ArgumentNotNull(response, "response");
            return new SDataResults<T>(response.StatusCode,
                                       response.ContentType,
                                       response.ETag,
                                       response.Location,
                                       response.Expires,
                                       response.RetryAfter,
                                       response.Form,
                                       response.Files,
                                       content);
        }

        internal SDataResults(HttpStatusCode statusCode,
                              MediaType? contentType,
                              string eTag,
                              string location,
                              DateTimeOffset? expires,
                              DateTimeOffset? retryAfter,
                              IDictionary<string, string> form,
                              IList<AttachedFile> files)
        {
            _statusCode = statusCode;
            _contentType = contentType;
            _eTag = eTag;
            _location = location;
            _expires = expires;
            _retryAfter = retryAfter;
            _form = form;
            _files = files;
        }

        public HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        public MediaType? ContentType
        {
            get { return _contentType; }
        }

        public string ETag
        {
            get { return _eTag; }
        }

        public string Location
        {
            get { return _location; }
        }

        public DateTimeOffset? Expires
        {
            get { return _expires; }
        }

        public DateTimeOffset? RetryAfter
        {
            get { return _retryAfter; }
        }

        public IDictionary<string, string> Form
        {
            get { return _form; }
        }

        public IList<AttachedFile> Files
        {
            get { return _files; }
        }
    }

    public class SDataResults<T> : SDataResults, ISDataResults<T>
    {
        private readonly T _content;

        internal SDataResults(HttpStatusCode statusCode,
                              MediaType? contentType,
                              string eTag,
                              string location,
                              DateTimeOffset? expires,
                              DateTimeOffset? retryAfter,
                              IDictionary<string, string> form,
                              IList<AttachedFile> files,
                              T content)
            : base(statusCode, contentType, eTag, location, expires, retryAfter, form, files)
        {
            _content = content;
        }

        public T Content
        {
            get { return _content; }
        }
    }
}