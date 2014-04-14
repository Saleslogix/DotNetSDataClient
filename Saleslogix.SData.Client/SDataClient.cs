// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Net;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Framework;

#if !NET_2_0 && !NET_3_5
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client
{
    public class SDataClient : ISDataClient
    {
        private readonly CookieContainer _cookies = new CookieContainer();

        public SDataClient(string uri)
        {
            Uri = uri;
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

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public ISDataResults Execute(SDataParameters parms)
        {
            var request = CreateRequest(parms);
            return SDataResults.FromResponse(request.GetResponse());
        }

        public ISDataResults<T> Execute<T>(SDataParameters parms)
        {
            var request = CreateRequest(parms);
            return CreateResults<T>(request.GetResponse());
        }
#endif

#if !NET_2_0 && !NET_3_5
        public Task<ISDataResults> ExecuteAsync(SDataParameters parms)
        {
            var request = CreateRequest(parms);
            return Task.Factory
                       .FromAsync<SDataResponse>(request.BeginGetResponse, request.EndGetResponse, null)
                       .ContinueWith(task => SDataResults.FromResponse(task.Result));
        }

        public Task<ISDataResults<T>> ExecuteAsync<T>(SDataParameters parms)
        {
            var request = CreateRequest(parms);
            return Task.Factory
                       .FromAsync<SDataResponse>(request.BeginGetResponse, request.EndGetResponse, null)
                       .ContinueWith(task => CreateResults<T>(task.Result));
        }
#endif

        private SDataRequest CreateRequest(SDataParameters parms)
        {
            var uri = new SDataUri(Uri)
                          {
                              StartIndex = parms.StartIndex,
                              Count = parms.Count,
                              Where = parms.Where,
                              OrderBy = parms.OrderBy,
                              Search = parms.Search,
                              Include = parms.Include,
                              Select = parms.Select,
                              Precedence = parms.Precedence,
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
            foreach (var item in parms.ExtensionArgs)
            {
                uri["_" + item.Key] = item.Value;
            }
            var operation = new RequestOperation(parms.Method, parms.Content)
                                {
                                    Selector = parms.Selector,
                                    ContentType = parms.ContentType,
                                    ETag = parms.ETag
                                };
            foreach (var item in parms.Form)
            {
                operation.Form.Add(item.Key, item.Value);
            }
            foreach (var file in parms.Files)
            {
                operation.Files.Add(file);
            }
            var request = new SDataRequest(uri.ToString(), operation)
                              {
                                  UserName = UserName,
                                  Password = Password,
                                  Credentials = Credentials,
                                  NamingScheme = NamingScheme,
                                  Cookies = _cookies,
                                  Accept = parms.Accept,
                                  AcceptLanguage = parms.Language ?? Language,
                                  UseHttpMethodOverride = UseHttpMethodOverride
                              };
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
            var content = ContentHelper.Deserialize<T>(response.Content, NamingScheme);
            return SDataResults.FromResponse(response, content);
        }
    }
}