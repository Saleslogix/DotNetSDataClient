using System.Collections.Generic;
using System.Net;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client
{
    public class SDataResults : ISDataResults
    {
        private readonly HttpStatusCode _statusCode;
        private readonly MediaType? _contentType;
        private readonly string _eTag;
        private readonly string _location;
        private readonly IList<AttachedFile> _files;

        public static ISDataResults FromResponse(SDataResponse response)
        {
            return new SDataResults(response.StatusCode,
                                    response.ContentType,
                                    response.ETag,
                                    response.Location,
                                    response.Files);
        }

        public static ISDataResults<T> FromResponse<T>(SDataResponse response, T content)
        {
            return new SDataResults<T>(response.StatusCode,
                                       response.ContentType,
                                       response.ETag,
                                       response.Location,
                                       response.Files,
                                       content);
        }

        internal SDataResults(HttpStatusCode statusCode,
                              MediaType? contentType,
                              string eTag,
                              string location,
                              IList<AttachedFile> files)
        {
            _statusCode = statusCode;
            _contentType = contentType;
            _eTag = eTag;
            _location = location;
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
                              IList<AttachedFile> files,
                              T content)
            : base(statusCode, contentType, eTag, location, files)
        {
            _content = content;
        }

        public T Content
        {
            get { return _content; }
        }
    }
}