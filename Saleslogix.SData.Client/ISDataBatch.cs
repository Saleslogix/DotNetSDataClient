// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Collections.Generic;
using Saleslogix.SData.Client.Framework;

#if !NET_2_0 && !NET_3_5
using System;
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client
{
    public interface ISDataBatch<T>
    {
#if NET_2_0 || NET_3_5
        void Add(HttpMethod method, object content);
#else
        Lazy<T> Add(HttpMethod method, object content);
#endif

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        IList<T> Commit();
#endif

#if !NET_2_0 && !NET_3_5
        Task<IList<T>> CommitAsync(CancellationToken cancel = default(CancellationToken));
#endif
    }
}