// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Mime;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// The request class which is responsible for sending and
    /// receiving data over HTTP with the server.
    /// </summary>
    public class SDataRequest
    {
        private readonly IList<RequestOperation> _operations;
#if !PCL && !SILVERLIGHT
        private bool _proxySet;
        private IWebProxy _proxy;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataRequest"/> class.
        /// </summary>
        public SDataRequest(string uri)
            : this(uri, new RequestOperation())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataRequest"/> class.
        /// </summary>
        public SDataRequest(string uri, HttpMethod method)
            : this(uri, new RequestOperation(method))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataRequest"/> class.
        /// </summary>
        public SDataRequest(string uri, HttpMethod method, object content)
            : this(uri, new RequestOperation(method, content))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataRequest"/> class.
        /// </summary>
        public SDataRequest(string uri, params RequestOperation[] operations)
        {
            Uri = uri;
            UserAgent = "Sage";
            Timeout = 120000;
            TimeoutRetryAttempts = 1;
            _operations = new List<RequestOperation>(operations);
        }

        /// <summary>
        /// Gets or sets the target uri used by requests.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Gets or sets the user name used by requests.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password used by requests.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user agent passed during requests.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the timeout in milliseconds used during requests.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets the number of timeout retry attempts that should be made before giving up.
        /// </summary>
        public int TimeoutRetryAttempts { get; set; }

#if !PCL && !SILVERLIGHT
        /// <summary>
        /// Gets or sets the proxy used by requests.
        /// </summary>
        public IWebProxy Proxy
        {
            get { return _proxy; }
            set
            {
                _proxySet = true;
                _proxy = value;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the accept media types accepted by requests.
        /// </summary>
        public MediaType[] Accept { get; set; }

        /// <summary>
        /// Gets or sets the cookies associated with this request.
        /// </summary>
        public CookieContainer Cookies { get; set; }

        /// <summary>
        /// Gets of sets the credentials associated with this request.
        /// </summary>
        public ICredentials Credentials { get; set; }

        public INamingScheme NamingScheme { get; set; }

        /// <summary>
        /// Lists the operations associated with this request.
        /// </summary>
        public IList<RequestOperation> Operations
        {
            get { return _operations; }
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        /// <summary>
        /// Execute the request and return a response object.
        /// </summary>
        public SDataResponse GetResponse()
        {
            var uri = Uri;
            RequestOperation operation;

            if (_operations.Count == 1)
            {
                operation = _operations[0];
            }
            else
            {
                operation = CreateBatchOperation();
                uri = new SDataUri(uri).AppendPath("$batch").ToString();
            }

            string location = null;
            var attempts = TimeoutRetryAttempts;
            var hasContent = operation.Content != null || operation.Form.Count > 0 || operation.Files.Count > 0;

            while (true)
            {
                var request = CreateRequest(uri, operation);
                if (hasContent)
                {
                    using (var stream = request.GetRequestStream())
                    {
                        WriteRequestContent(operation, request, stream);
                    }
                }

                WebResponse response;
                try
                {
                    response = request.GetResponse();
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Timeout && attempts > 0)
                    {
                        attempts--;
                        continue;
                    }
                    throw new SDataException(ex);
                }

                var httpResponse = response as HttpWebResponse;
                var statusCode = httpResponse != null ? httpResponse.StatusCode : 0;

                if (statusCode != HttpStatusCode.Found)
                {
                    return new SDataResponse(response, location);
                }

                uri = location = response.Headers["Location"];
            }
        }
#endif

        public IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            var uri = Uri;
            RequestOperation operation;

            if (_operations.Count == 1)
            {
                operation = _operations[0];
            }
            else
            {
                operation = CreateBatchOperation();
                uri = new SDataUri(uri).AppendPath("$batch").ToString();
            }

            string location = null;
            var attempts = TimeoutRetryAttempts;
            var hasContent = operation.Content != null || operation.Form.Count > 0 || operation.Files.Count > 0;
            var result = new AsyncResult<SDataResponse>(callback, state);
            Action<WebRequest> getResponse = null;
            Action loop =
                () =>
                    {
                        var request = CreateRequest(uri, operation);
                        if (hasContent)
                        {
                            request.BeginGetRequestStream(
                                async =>
                                    {
                                        try
                                        {
                                            using (var stream = request.EndGetRequestStream(async))
                                            {
                                                WriteRequestContent(operation, request, stream);
                                            }
                                            getResponse(request);
                                        }
                                        catch (Exception ex)
                                        {
                                            result.Failure(ex, async.CompletedSynchronously);
                                        }
                                    }, null);
                        }
                        else
                        {
                            getResponse(request);
                        }
                    };
            getResponse =
                request => request.BeginGetResponse(
                    async =>
                        {
                            try
                            {
                                WebResponse response;
                                try
                                {
                                    response = request.EndGetResponse(async);
                                }
                                catch (WebException webEx)
                                {
#if PCL || NETFX_CORE || SILVERLIGHT
                                    if (attempts > 0)
#else
                                    if (webEx.Status == WebExceptionStatus.Timeout && attempts > 0)
#endif
                                    {
                                        attempts--;
                                        loop();
                                        return;
                                    }
                                    throw new SDataException(webEx);
                                }

                                var httpResponse = response as HttpWebResponse;
                                var statusCode = httpResponse != null ? httpResponse.StatusCode : 0;

                                if (statusCode != HttpStatusCode.Found)
                                {
                                    result.Success(new SDataResponse(response, location), async.CompletedSynchronously);
                                }
                                else
                                {
                                    uri = location = response.Headers["Location"];
                                    loop();
                                }
                            }
                            catch (Exception ex)
                            {
                                result.Failure(ex, async.CompletedSynchronously);
                            }
                        }, null);
            loop();
            return result;
        }

        public SDataResponse EndGetResponse(IAsyncResult asyncResult)
        {
            Guard.ArgumentIsType<AsyncResult<SDataResponse>>(asyncResult, "asyncResult");
            return ((AsyncResult<SDataResponse>) asyncResult).End();
        }

        private RequestOperation CreateBatchOperation()
        {
            var resources = new SDataCollection<SDataResource>(_operations.Count);
            var batchOp = new RequestOperation(HttpMethod.Post, resources);

            foreach (var op in _operations)
            {
                //TODO: be more permissive
                SDataResource resource;

                if (op.Content == null)
                {
                    if (op.Method != HttpMethod.Post && string.IsNullOrEmpty(op.Selector))
                    {
                        throw new InvalidOperationException("A selector must be specified for GET, PUT and DELETE batch requests");
                    }

                    var resourceUri = new SDataUri(Uri) {LastPathSegment = {Selector = op.Selector}};
                    resource = new SDataResource {Id = resourceUri.Uri.ToString()};
                }
                else
                {
                    resource = op.Content as SDataResource;

                    //TODO: could be a POCO, will need to be wrapped in a surrogate
                    if (resource == null)
                    {
                        throw new InvalidOperationException("Only resources can be submitted in batch requests");
                    }
                }

                resource.HttpMethod = op.Method;
                resource.ETag = op.ETag;
                resources.Add(resource);

                foreach (var data in op.Form)
                {
                    batchOp.Form.Add(data.Key, data.Value);
                }

                foreach (var file in op.Files)
                {
                    batchOp.Files.Add(file);
                }
            }

            return batchOp;
        }

        private WebRequest CreateRequest(string uri, RequestOperation op)
        {
            var request = WebRequest.Create(uri);
            request.Method = op.Method.ToString().ToUpperInvariant();
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            request.Timeout = Timeout;
            request.PreAuthenticate = true;

            if (_proxySet)
            {
                request.Proxy = _proxy;
            }
#endif

            var httpRequest = request as HttpWebRequest;
            if (httpRequest != null)
            {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
                httpRequest.AllowAutoRedirect = false;
                httpRequest.ReadWriteTimeout = Timeout;
                httpRequest.KeepAlive = false;
                httpRequest.ProtocolVersion = HttpVersion.Version10;

                if (!string.IsNullOrEmpty(UserAgent))
                {
                    httpRequest.UserAgent = UserAgent;
                }
#endif
                if (Accept != null)
                {
                    httpRequest.Accept = string.Join(",", Accept.Select(MediaTypeNames.GetMediaType).ToArray());
                }

                if (Cookies != null)
                {
                    httpRequest.CookieContainer = Cookies;
                }
            }

            if (Credentials != null)
            {
                request.Credentials = Credentials;
            }
            else if (!string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(Password))
            {
                request.Credentials = new NetworkCredential(UserName, Password);
            }
#if !PCL && !SILVERLIGHT
            else
            {
                request.Credentials = CredentialCache.DefaultCredentials;
            }
#endif

            if (!string.IsNullOrEmpty(op.ETag))
            {
                var header = op.Method == HttpMethod.Get
                                 ? HttpRequestHeader.IfNoneMatch
                                 : HttpRequestHeader.IfMatch;
                request.Headers[header] = op.ETag;
            }

            return request;
        }

        private void WriteRequestContent(RequestOperation op, WebRequest request, Stream stream)
        {
            var isMultipart = op.Form.Count > 0 || op.Files.Count > 0;
            var requestStream = isMultipart ? new MemoryStream() : stream;
            var contentType = op.ContentType;

            if (contentType == null)
            {
                if (ContentHelper.IsDictionary(op.Content))
                {
                    contentType = MediaType.AtomEntry;
                }
                else if (ContentHelper.IsCollection(op.Content))
                {
                    contentType = MediaType.Atom;
                }
                else if (op.Content is IXmlSerializable)
                {
                    contentType = MediaType.Xml;
                }
                else if (op.Content is string)
                {
                    contentType = MediaType.Text;
                }
            }

            if (contentType != null && op.Content != null)
            {
                var handler = ContentManager.GetHandler(contentType.Value);
                if (handler == null)
                {
                    throw new InvalidOperationException(string.Format("Content type '{0}' not supported", contentType));
                }

                handler.WriteTo(op.Content, requestStream, NamingScheme);
            }

            if (isMultipart)
            {
                requestStream.Seek(0, SeekOrigin.Begin);

                using (var multipart = new MimeMessage())
                {
                    if (contentType != null)
                    {
                        var part = new MimePart(requestStream) {ContentType = MediaTypeNames.GetMediaType(contentType.Value)};
                        multipart.Add(part);
                    }

                    foreach (var data in op.Form)
                    {
                        var part = new MimePart(new MemoryStream(Encoding.UTF8.GetBytes(data.Value)))
                                       {
                                           ContentType = MediaTypeNames.TextMediaType,
                                           ContentTransferEncoding = "binary",
                                           ContentDisposition = "inline; name=" + data.Key
                                       };
                        multipart.Add(part);
                    }

                    foreach (var file in op.Files)
                    {
                        var contentDisposition = "attachment";
                        if (file.FileName != null)
                        {
                            if (file.FileName == Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(file.FileName)))
                            {
                                contentDisposition += "; filename=" + file.FileName;
                            }
                            else
                            {
                                contentDisposition += "; filename*=" + string.Format("{0}''{1}", Encoding.UTF8.WebName, System.Uri.EscapeDataString(file.FileName));
                            }
                        }

                        var part = new MimePart(file.Stream)
                                       {
                                           ContentType = !string.IsNullOrEmpty(file.ContentType) ? file.ContentType : "application/octet-stream",
                                           ContentTransferEncoding = "binary",
                                           ContentDisposition = contentDisposition
                                       };
                        multipart.Add(part);
                    }

                    multipart.WriteTo(stream);
                    request.ContentType = string.Format("multipart/{0}; boundary={1}", (op.Files.Count > 0 ? "related" : "form-data"), multipart.Boundary);
                }
            }
            else if (contentType != null)
            {
                request.ContentType = MediaTypeNames.GetMediaType(contentType.Value);
            }
        }

        #region Nested type: AsyncResult

        private class AsyncResult<T> : IAsyncResult
        {
            private readonly AsyncCallback _callback;
            private readonly object _state;
            private ManualResetEvent _waitHandle;
            private T _result;
            private Exception _exception;
            private volatile bool _isCompleted;
            private volatile bool _completedSynchronously;

            public AsyncResult(AsyncCallback callback, object state)
            {
                _callback = callback;
                _state = state;
            }

            public object AsyncState
            {
                get { return _state; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (_waitHandle == null)
                    {
                        var isCompleted = _isCompleted;
                        var waitHandle = new ManualResetEvent(isCompleted);
                        if (Interlocked.Exchange(ref _waitHandle, waitHandle) != null)
                        {
#if NET_2_0 || NET_3_5
                            waitHandle.Close();
#else
                            waitHandle.Dispose();
#endif
                        }
                        else if (!isCompleted && _isCompleted)
                        {
                            waitHandle.Set();
                        }
                    }
                    return _waitHandle;
                }
            }

            public bool IsCompleted
            {
                get { return _isCompleted; }
            }

            public bool CompletedSynchronously
            {
                get { return _completedSynchronously; }
            }

            public void Success(T result, bool completedSynchronously)
            {
                Complete(result, null, completedSynchronously);
            }

            public void Failure(Exception exception, bool completedSynchronously)
            {
                Complete(default(T), exception, completedSynchronously);
            }

            private void Complete(T result, Exception exception, bool completedSynchronously)
            {
                _isCompleted = true;
                _completedSynchronously = completedSynchronously;
                _result = result;
                _exception = exception;

                if (_waitHandle != null)
                {
                    _waitHandle.Set();
                }

                if (_callback != null)
                {
                    _callback(this);
                }
            }

            public T End()
            {
                if (!_isCompleted)
                {
                    AsyncWaitHandle.WaitOne();
                }

                if (_waitHandle != null)
                {
#if NET_2_0 || NET_3_5
                    _waitHandle.Close();
#else
                    _waitHandle.Dispose();
#endif
                    _waitHandle = null;
                }

                if (_exception != null)
                {
                    throw _exception;
                }

                return _result;
            }
        }

        #endregion
    }
}