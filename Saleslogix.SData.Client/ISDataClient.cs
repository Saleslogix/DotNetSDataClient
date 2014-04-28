// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Collections.Generic;
using System.Net;
using Saleslogix.SData.Client.Framework;

#if !NET_2_0 && !NET_3_5
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client
{
    public interface ISDataClient
    {
        string Uri { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string UserAgent { get; set; }
        int? Timeout { get; set; }
        int? TimeoutRetryAttempts { get; set; }
        bool UseHttpMethodOverride { get; set; }
#if !PCL && !SILVERLIGHT
        IWebProxy Proxy { get; set; }
#endif
        ICredentials Credentials { get; set; }
        INamingScheme NamingScheme { get; set; }
        MediaType? Format { get; set; }
        string Language { get; set; }
        string Version { get; set; }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        ISDataResults Execute(SDataParameters parms);
        ISDataResults<T> Execute<T>(SDataParameters parms);
        ISDataResults<IList<T>> ExecuteBatch<T>(IList<SDataParameters> items);
#endif

#if !NET_2_0 && !NET_3_5
        Task<ISDataResults> ExecuteAsync(SDataParameters parms, CancellationToken cancel = default(CancellationToken));
        Task<ISDataResults<T>> ExecuteAsync<T>(SDataParameters parms, CancellationToken cancel = default(CancellationToken));
        Task<ISDataResults<IList<T>>> ExecuteBatchAsync<T>(IList<SDataParameters> items, CancellationToken cancel = default(CancellationToken));
#endif
    }
}