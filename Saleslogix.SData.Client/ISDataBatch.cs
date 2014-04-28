using System;
using System.Collections.Generic;

#if !NET_2_0 && !NET_3_5
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client
{
    public interface ISDataBatch<T>
    {
#if NET_2_0 || NET_3_5
        void Get(string key);
        void Post(T content);
        void Put(T content);
        void Delete(T content);
#else
        Lazy<T> Get(string key);
        Lazy<T> Post(T content);
        Lazy<T> Put(T content);
        Lazy<T> Delete(T content);
#endif

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        IList<T> Commit();
#endif

#if !NET_2_0 && !NET_3_5
        Task<IList<T>> CommitAsync(CancellationToken cancel = default(CancellationToken));
#endif
    }
}