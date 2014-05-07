// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Utilities;

#if !NET_2_0 && !NET_3_5
using System.Threading;
using System.Threading.Tasks;
#endif

#if !PCL && !NETFX_CORE && !SILVERLIGHT
using System.IO;
using System.Text;
using Saleslogix.SData.Client.Metadata;
#endif

namespace Saleslogix.SData.Client
{
    public class SDataClient : ISDataClient
    {
        private readonly Func<string, SDataRequest> _requestFactory;
        private readonly CookieContainer _cookies = new CookieContainer();

        public SDataClient(string uri)
            : this(uri, targetUri => new SDataRequest(targetUri))
        {
        }

        internal SDataClient(string uri, Func<string, SDataRequest> requestFactory)
        {
            Uri = uri;
            _requestFactory = requestFactory;
            Format = MediaType.Json;
            DifferentialUpdate = true;
        }

        public string Uri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserAgent { get; set; }
        public int? Timeout { get; set; }
        public int? TimeoutRetryAttempts { get; set; }
        public bool UseHttpMethodOverride { get; set; }
#if !PCL && !SILVERLIGHT
        public IWebProxy Proxy { get; set; }
#endif
        public ICredentials Credentials { get; set; }
        public INamingScheme NamingScheme { get; set; }
        public MediaType? Format { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public bool DifferentialUpdate { get; set; }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public ISDataResults Execute(SDataParameters parms)
        {
            var request = CreateRequest(parms, true);
            return SDataResults.FromResponse(request.GetResponse());
        }

        public ISDataResults<T> Execute<T>(SDataParameters parms)
        {
            var request = CreateRequest(parms, false);
            return CreateResults<T>(request.GetResponse());
        }

        public ISDataResults<IList<T>> ExecuteBatch<T>(IList<SDataParameters> items)
        {
            var request = CreateBatchRequest(items);
            return CreateResults<IList<T>>(request.GetResponse());
        }
#endif

#if !NET_2_0 && !NET_3_5
        public Task<ISDataResults> ExecuteAsync(SDataParameters parms, CancellationToken cancel = default(CancellationToken))
        {
            var request = CreateRequest(parms, true);
            return ExecuteAsync(request, SDataResults.FromResponse, cancel);
        }

        public Task<ISDataResults<T>> ExecuteAsync<T>(SDataParameters parms, CancellationToken cancel = default(CancellationToken))
        {
            var request = CreateRequest(parms, false);
            return ExecuteAsync(request, CreateResults<T>, cancel);
        }

        public Task<ISDataResults<IList<T>>> ExecuteBatchAsync<T>(IList<SDataParameters> items, CancellationToken cancel = new CancellationToken())
        {
            var request = CreateBatchRequest(items);
            return ExecuteAsync(request, CreateResults<IList<T>>, cancel);
        }

        private static Task<T> ExecuteAsync<T>(SDataRequest request, Func<SDataResponse, T> createResults, CancellationToken cancel)
        {
            var cancelScope = cancel.Register(request.Abort);
            return Task.Factory
                       .FromAsync<SDataResponse>(request.BeginGetResponse, request.EndGetResponse, null)
                       .ContinueWith(task =>
                       {
                           try
                           {
                               return createResults(task.Result);
                           }
                           catch (SDataException ex)
                           {
                               if (ex.Status == WebExceptionStatus.RequestCanceled)
                               {
                                   cancel.ThrowIfCancellationRequested();
                               }
                               if (cancel.IsCancellationRequested)
                               {
                                   throw new TaskCanceledException(ex.Message, ex);
                               }
                               throw;
                           }
                           finally
                           {
                               cancelScope.Dispose();
                           }
                       }, cancel);
        }
#endif

        private SDataRequest CreateRequest(SDataParameters parms, bool responseContentIgnored)
        {
            Guard.ArgumentNotNull(parms, "parms");

            var uri = new SDataUri(Uri)
                          {
                              StartIndex = parms.StartIndex,
                              Count = parms.Count,
                              Where = parms.Where,
                              OrderBy = parms.OrderBy,
                              Search = parms.Search,
                              Include = parms.Include,
                              Select = parms.Select,
                              Precedence = parms.Precedence ?? (responseContentIgnored ? 0 : (int?) null),
                              IncludeSchema = parms.IncludeSchema,
                              ReturnDelta = parms.ReturnDelta,
                              TrackingId = parms.TrackingId,
                              Format = parms.Format ?? Format,
                              Language = parms.Language ?? Language,
                              Version = parms.Version ?? Version
                          };
            if (parms.Path != null)
            {
                uri.AppendPath(parms.Path);
            }
            foreach (var arg in parms.ExtensionArgs)
            {
                uri["_" + arg.Key] = arg.Value;
            }

            var content = parms.Content;
            if (parms.Method == HttpMethod.Put && DifferentialUpdate)
            {
                var tracking = content as IChangeTracking;
                if (tracking != null)
                {
                    content = tracking.GetChanges();
                    if (content == null)
                    {
                        throw new SDataClientException("Content doesn't have any changes");
                    }
                }
            }

            var request = CreateRequest(uri, parms.Method, content);
            request.Selector = parms.Selector;
            request.ContentType = parms.ContentType ?? Format;
            request.ETag = parms.ETag;
            foreach (var item in parms.Form)
            {
                request.Form.Add(item.Key, item.Value);
            }
            foreach (var file in parms.Files)
            {
                request.Files.Add(file);
            }
            request.Accept = parms.Accept;
            request.AcceptLanguage = parms.Language ?? Language;
            return request;
        }

        private SDataRequest CreateBatchRequest(ICollection<SDataParameters> items)
        {
            Guard.ArgumentNotNull(items, "items");

            var resources = new SDataCollection<SDataResource>(items.Count);
            string path = null;
            MediaType? contentType = null;
            var include = new List<string>();
            var select = new List<string>();
            int? precedence = null;
            MediaType? format = null;
            string language = null;
            string version = null;
            var accept = new List<MediaType>();
            var form = new Dictionary<string, string>();
            var files = new List<AttachedFile>();
            var extensionArgs = new Dictionary<string, string>();

            foreach (var parms in items)
            {
                if (parms.StartIndex != null)
                {
                    throw new SDataClientException("StartIndex not supported in batch requests");
                }
                if (parms.Count != null)
                {
                    throw new SDataClientException("Count not supported in batch requests");
                }
                if (parms.Where != null)
                {
                    throw new SDataClientException("Where not supported in batch requests");
                }
                if (parms.OrderBy != null)
                {
                    throw new SDataClientException("OrderBy not supported in batch requests");
                }
                if (parms.Search != null)
                {
                    throw new SDataClientException("Search not supported in batch requests");
                }
                if (parms.TrackingId != null)
                {
                    throw new SDataClientException("TrackingId not supported in batch requests");
                }
                if (parms.IncludeSchema != null)
                {
                    throw new SDataClientException("IncludeSchema not supported in batch requests");
                }
                if (parms.ReturnDelta != null)
                {
                    throw new SDataClientException("ReturnDelta not supported in batch requests");
                }
                if (parms.Path != path)
                {
                    if (path != null)
                    {
                        throw new SDataClientException("All non-null path values must be the same");
                    }
                    path = parms.Path;
                }
                if (parms.ContentType != contentType)
                {
                    if (contentType != null)
                    {
                        throw new SDataClientException("All non-null content type values must be the same");
                    }
                    contentType = parms.ContentType;
                }
                if (parms.Include != null)
                {
                    include.AddRange(parms.Include.Split(','));
                }
                if (parms.Select != null)
                {
                    select.AddRange(parms.Select.Split(','));
                }
                if (parms.Precedence != null && parms.Precedence != precedence)
                {
                    precedence = Math.Max(precedence ?? 0, parms.Precedence.Value);
                }
                if (parms.Format != format)
                {
                    if (format != null)
                    {
                        throw new SDataClientException("All non-null format values must be the same");
                    }
                    format = parms.Format;
                }
                if (parms.Language != language)
                {
                    if (language != null)
                    {
                        throw new SDataClientException("All non-null language values must be the same");
                    }
                    language = parms.Language;
                }
                if (parms.Version != version)
                {
                    if (version != null)
                    {
                        throw new SDataClientException("All non-null version values must be the same");
                    }
                    version = parms.Version;
                }
                if (parms.Accept != null)
                {
                    accept.AddRange(parms.Accept);
                }
                foreach (var data in parms.Form)
                {
                    form.Add(data.Key, data.Value);
                }
                files.AddRange(parms.Files);
                foreach (var arg in parms.ExtensionArgs)
                {
                    extensionArgs[arg.Key] = arg.Value;
                }

                var selector = parms.Selector;

                SDataResource resource;
                if (parms.Content == null)
                {
                    resource = new SDataResource();
                }
                else
                {
                    var content = parms.Content;
                    if (parms.Method == HttpMethod.Put && DifferentialUpdate)
                    {
                        var tracking = content as IChangeTracking;
                        if (tracking != null)
                        {
                            content = tracking.GetChanges();
                            if (content == null)
                            {
                                throw new SDataClientException("Content doesn't have any changes");
                            }
                        }
                    }

                    resource = ContentHelper.Serialize(content, NamingScheme) as SDataResource;
                    if (resource == null)
                    {
                        throw new SDataClientException("Only resources can be submitted in batch requests");
                    }

                    if (selector == null && resource.Key != null)
                    {
                        selector = SDataUri.FormatConstant(resource.Key);
                    }
                }

                resource.HttpMethod = parms.Method;
                if (parms.Method != HttpMethod.Post)
                {
                    if (selector == null)
                    {
                        throw new SDataClientException("A selector must be specified for GET, PUT and DELETE batch requests");
                    }

                    var itemUri = new SDataUri(Uri);
                    if (path != null)
                    {
                        itemUri.AppendPath(path);
                    }
                    itemUri.LastPathSegment.Selector = selector;
                    resource.Id = itemUri.ToString();
                    resource.Url = itemUri.Uri;
                }

                if (parms.ETag != null)
                {
                    resource.ETag = parms.ETag;
                }

                resources.Add(resource);
            }

            var uri = new SDataUri(Uri)
                {
                    Precedence = precedence,
                    Format = format ?? Format,
                    Language = language ?? Language,
                    Version = version ?? Version
                };
            if (path != null)
            {
                uri.AppendPath(path);
            }
            uri.AppendPath("$batch");
            if (include.Count > 0)
            {
                uri.Include = string.Join(",", include.Distinct().ToArray());
            }
            if (select.Count > 0)
            {
                uri.Select = string.Join(",", select.Distinct().ToArray());
            }
            foreach (var arg in extensionArgs)
            {
                uri["_" + arg.Key] = arg.Value;
            }

            var request = CreateRequest(uri, HttpMethod.Post, resources);
            request.ContentType = contentType ?? Format;

            if (accept.Count > 0)
            {
                request.Accept = accept.Distinct().ToArray();
            }
            foreach (var data in form)
            {
                request.Form.Add(data.Key, data.Value);
            }
            foreach (var file in files)
            {
                request.Files.Add(file);
            }
            if (language != null)
            {
                request.AcceptLanguage = language ?? Language;
            }

            return request;
        }

        private SDataRequest CreateRequest(SDataUri uri, HttpMethod method, object content)
        {
            var request = _requestFactory(uri.ToString());
            request.Method = method;
            request.Content = content;
            request.UserName = UserName;
            request.Password = Password;
            request.Credentials = Credentials;
            request.NamingScheme = NamingScheme;
            request.Cookies = _cookies;
            request.UseHttpMethodOverride = UseHttpMethodOverride;
#if !PCL && !SILVERLIGHT
            request.Proxy = Proxy;
#endif
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            request.UserAgent = UserAgent;
            if (Timeout != null)
            {
                request.Timeout = Timeout.Value;
            }
#endif
            if (TimeoutRetryAttempts != null)
            {
                request.TimeoutRetryAttempts = TimeoutRetryAttempts.Value;
            }
            return request;
        }

        private ISDataResults<T> CreateResults<T>(SDataResponse response)
        {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
            T content;
            if (typeof (T) == typeof (SDataSchema) && response.Content is string)
            {
                using (var memory = new MemoryStream(Encoding.UTF8.GetBytes((string) response.Content)))
                {
                    content = (T) (object) SDataSchema.Read(memory);
                }
            }
            else
            {
                content = ContentHelper.Deserialize<T>(response.Content, NamingScheme);
            }
#else
            var content = ContentHelper.Deserialize<T>(response.Content, NamingScheme);
#endif
            var tracking = content as IChangeTracking;
            if (tracking != null)
            {
                tracking.AcceptChanges();
            }
            return SDataResults.FromResponse(response, content);
        }
    }
}