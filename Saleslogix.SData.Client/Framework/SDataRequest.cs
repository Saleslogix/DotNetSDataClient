// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

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

#if !PCL && !NETFX_CORE && !SILVERLIGHT
using System.Diagnostics;
#endif

namespace Saleslogix.SData.Client.Framework
{
    /// <summary>
    /// The request class which is responsible for sending and
    /// receiving data over HTTP with the server.
    /// </summary>
    public class SDataRequest
    {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public readonly TraceSource Trace = new TraceSource("SDataRequest");
#endif

        private IDictionary<string, string> _form;
        private IList<AttachedFile> _files;
#if !PCL && !SILVERLIGHT
        private bool _proxySet;
        private IWebProxy _proxy;
#endif
        private int _state;
        private WebRequest _request;

        /// <summary>
        /// Initializes a new instance of the <see cref="SDataRequest"/> class.
        /// </summary>
        public SDataRequest(string uri = null, HttpMethod method = HttpMethod.Get, object content = null)
        {
            Uri = uri;
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            UserAgent = "DotNetSDataClient";
            Timeout = 120000;
#endif
            TimeoutRetryAttempts = 1;
            Method = method;
            Content = content;
            MaxGetUriLength = 2000;
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
        /// Gets or sets the method for the request.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the input resource for the request.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// Gets or sets the input content type for the request.
        /// </summary>
        public MediaType? ContentType { get; set; }

        /// <summary>
        /// Gets or sets the ETag value for the request.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets the form data associated with the request.
        /// </summary>
        public IDictionary<string, string> Form
        {
            get { return _form ?? (_form = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)); }
        }

        /// <summary>
        /// Gets the files that will be attached to the request content.
        /// </summary>
        public IList<AttachedFile> Files
        {
            get { return _files ?? (_files = new List<AttachedFile>()); }
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        /// <summary>
        /// Gets or sets the user agent passed during requests.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the timeout in milliseconds used during requests.
        /// </summary>
        public int Timeout { get; set; }
#endif

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
        /// Gets or sets the media types accepted by requests.
        /// </summary>
        public MediaType[] Accept { get; set; }

        /// <summary>
        /// Gets or sets the language accepted by requests.
        /// </summary>
        public string AcceptLanguage { get; set; }

        /// <summary>
        /// Gets or sets whether HTTP method override should be used by requests.
        /// </summary>
        public bool UseHttpMethodOverride { get; set; }

        /// <summary>
        /// Gets or sets the maximum length for a GET request URI before
        /// HTTP method override should be used.
        /// </summary>
        public int MaxGetUriLength { get; set; }

        /// <summary>
        /// Gets or sets the cookies associated with this request.
        /// </summary>
        public CookieContainer Cookies { get; set; }

        public IAuthenticator Authenticator { get; set; }

        /// <summary>
        /// Gets of sets the credentials associated with this request.
        /// </summary>
        public ICredentials Credentials { get; set; }

        public INamingScheme NamingScheme { get; set; }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        /// <summary>
        /// Execute the request and return a response object.
        /// </summary>
        public virtual SDataResponse GetResponse()
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) != 0)
            {
                throw new InvalidOperationException("Existing request in progress");
            }

            var uri = Uri;
            string location = null;
            var attempts = TimeoutRetryAttempts;

            try
            {
                while (true)
                {
                    MediaType? contentType;
                    object content;
                    _request = CreateRequest(uri, out contentType, out content);
                    if (content != null || Form.Count > 0 || Files.Count > 0)
                    {
                        using (var stream = _request.GetRequestStream())
                        {
                            WriteRequestContent(contentType, content, stream);
                        }
                    }

                    WebResponse response;
                    try
                    {
                        TraceRequest(_request);
                        response = _request.GetResponse();
                        TraceResponse(response);
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
            finally
            {
                _request = null;
                Interlocked.Exchange(ref _state, 0);
            }
        }
#endif

        public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) != 0)
            {
                throw new InvalidOperationException("Existing request in progress");
            }

            var uri = Uri;
            string location = null;
            var attempts = TimeoutRetryAttempts;
            var result = new AsyncResult<SDataResponse>(callback, state);
            Action getResponse = null;
            Action loop =
                () =>
                    {
                        MediaType? contentType;
                        object content;
                        _request = CreateRequest(uri, out contentType, out content);
                        if (content != null || Form.Count > 0 || Files.Count > 0)
                        {
                            _request.BeginGetRequestStream(
                                async =>
                                    {
                                        try
                                        {
                                            using (var stream = _request.EndGetRequestStream(async))
                                            {
                                                WriteRequestContent(contentType, content, stream);
                                            }
                                            TraceRequest(_request);
                                            getResponse();
                                        }
                                        catch (Exception ex)
                                        {
                                            result.Failure(ex, async.CompletedSynchronously);
                                            _request = null;
                                            Interlocked.Exchange(ref _state, 0);
                                        }
                                    }, null);
                        }
                        else
                        {
                            TraceRequest(_request);
                            getResponse();
                        }
                    };
            getResponse =
                () => _request.BeginGetResponse(
                    async =>
                        {
                            try
                            {
                                WebResponse response;
                                try
                                {
                                    response = _request.EndGetResponse(async);
                                    TraceResponse(response);
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
                                    _request = null;
                                    Interlocked.Exchange(ref _state, 0);
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
                                _request = null;
                                Interlocked.Exchange(ref _state, 0);
                            }
                        }, null);
            try
            {
                loop();
            }
            catch
            {
                _request = null;
                Interlocked.Exchange(ref _state, 0);
                throw;
            }
            return result;
        }

        public virtual SDataResponse EndGetResponse(IAsyncResult asyncResult)
        {
            Guard.ArgumentIsType<AsyncResult<SDataResponse>>(asyncResult, "asyncResult");
            try
            {
                return ((AsyncResult<SDataResponse>) asyncResult).End();
            }
            finally
            {
                _request = null;
                Interlocked.Exchange(ref _state, 0);
            }
        }

        public void Abort()
        {
            if (Interlocked.CompareExchange(ref _state, 2, 1) == 1)
            {
                _request.Abort();
            }
        }

        private WebRequest CreateRequest(string uri, out MediaType? contentType, out object content)
        {
            bool methodOverride;
            if (Method == HttpMethod.Get && uri.Length > MaxGetUriLength)
            {
                methodOverride = true;
                contentType = MediaType.Text;
                content = uri;
                uri = new SDataUri(uri) {Query = null}.ToString();
            }
            else
            {
                methodOverride = (UseHttpMethodOverride && Method != HttpMethod.Get && Method != HttpMethod.Post);
                contentType = ContentType;
                content = Content;
            }

            var request = WebRequest.Create(uri);
            if (methodOverride)
            {
                request.Method = "POST";
                request.Headers["X-HTTP-Method-Override"] = Method.ToString().ToUpperInvariant();
            }
            else
            {
                request.Method = Method.ToString().ToUpperInvariant();
            }

#if PCL
            try
            {
                var preAuthProp = request.GetType().GetProperty("PreAuthenticate");
                if (preAuthProp != null && preAuthProp.CanWrite)
                {
                    preAuthProp.SetValue(request, true, null);
                }
            }
            catch
            {
            }
#endif
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            request.Timeout = Timeout;
            request.PreAuthenticate = true;
#endif
#if !PCL && !SILVERLIGHT
            if (_proxySet)
            {
                request.Proxy = Proxy;
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

                if (UserAgent != null)
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

            if (AcceptLanguage != null)
            {
                request.Headers[HttpRequestHeader.AcceptLanguage] = AcceptLanguage;
            }

            if (Authenticator != null)
            {
                Authenticator.Authenticate(request);
            }
            else if (Credentials != null)
            {
                request.Credentials = Credentials;
            }
            else if (UserName != null || Password != null)
            {
                request.Credentials = new NetworkCredential(UserName, Password);
            }
            else
            {
                request.UseDefaultCredentials = true;
            }

            if (ETag != null)
            {
                var header = Method == HttpMethod.Get
                                 ? HttpRequestHeader.IfNoneMatch
                                 : HttpRequestHeader.IfMatch;
                request.Headers[header] = ETag;
            }

            return request;
        }

        private void WriteRequestContent(MediaType? contentType, object content, Stream stream)
        {
            var isMultipart = Form.Count > 0 || Files.Count > 0;
            var requestStream = isMultipart ? new MemoryStream() : stream;

            if (contentType == null)
            {
                if (ContentHelper.IsDictionary(content))
                {
                    contentType = MediaType.AtomEntry;
                }
                else if (ContentHelper.IsCollection(content))
                {
                    contentType = MediaType.Atom;
                }
                else if (content is IXmlSerializable)
                {
                    contentType = MediaType.Xml;
                }
                else if (content is string)
                {
                    contentType = MediaType.Text;
                }
                else if (ContentHelper.IsObject(content))
                {
                    contentType = MediaType.AtomEntry;
                }
            }

            if (contentType != null && content != null)
            {
                var handler = ContentManager.GetHandler(contentType.Value);
                if (handler == null)
                {
                    throw new NotSupportedException(string.Format("Content type '{0}' not supported", contentType));
                }

                handler.WriteTo(content, requestStream, NamingScheme);
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

                    foreach (var data in Form)
                    {
                        var part = new MimePart(new MemoryStream(Encoding.UTF8.GetBytes(data.Value)))
                                       {
                                           ContentType = MediaTypeNames.TextMediaType,
                                           ContentTransferEncoding = "binary",
                                           ContentDisposition = "inline; name=" + data.Key
                                       };
                        multipart.Add(part);
                    }

                    foreach (var file in Files)
                    {
                        var contentDisposition = "attachment";
                        if (file.FileName != null)
                        {
                            contentDisposition += Encoding.UTF8.GetByteCount(file.FileName) == file.FileName.Length
                                ? "; filename=" + string.Format("\"{0}\"", file.FileName.Replace("\"", "\\\""))
                                : "; filename*=" + string.Format("{0}''{1}", Encoding.UTF8.WebName, System.Uri.EscapeDataString(file.FileName));
                        }

                        var part = new MimePart(file.Stream)
                                       {
                                           ContentType = file.ContentType ?? "application/octet-stream",
                                           ContentTransferEncoding = "binary",
                                           ContentDisposition = contentDisposition
                                       };
                        multipart.Add(part);
                    }

                    multipart.WriteTo(stream);
                    _request.ContentType = string.Format("multipart/{0}; boundary={1}", (Files.Count > 0 ? "related" : "form-data"), multipart.Boundary);
                }
            }
            else if (contentType != null)
            {
                _request.ContentType = MediaTypeNames.GetMediaType(contentType.Value);
            }
        }

        private void TraceRequest(WebRequest request)
        {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            Trace.TraceData(TraceEventType.Information, 0, request);
#endif
        }

        private void TraceResponse(WebResponse response)
        {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            Trace.TraceData(TraceEventType.Information, 1, response);
#endif
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