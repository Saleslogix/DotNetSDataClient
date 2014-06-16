// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Utilities;

#if !NET_2_0 && !NET_3_5
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client
{
    public class SDataBatch<T> : ISDataBatch<T>
    {
        private readonly ISDataClient _client;
        private readonly string _path;
        private readonly SDataPayloadOptions _options;
        private readonly IList<SDataParameters> _items = new List<SDataParameters>();
        private IList<T> _results;
        private int _state;

        public SDataBatch(ISDataClient client, string path, SDataPayloadOptions options)
        {
            _client = client;
            _path = path;
            _options = options ?? new SDataPayloadOptions();
        }

        public
#if NET_2_0 || NET_3_5
            void
#else
            Lazy<T>
#endif
            Add(HttpMethod method, object content)
        {
            Guard.ArgumentNotNull(content, "content");
            if (_state == 1)
            {
                throw new InvalidOperationException("Commit in progress");
            }
            if (_state == 2)
            {
                throw new InvalidOperationException("Batch has been committed");
            }

            var parms = new SDataParameters
                {
                    Method = method,
                    Path = _path,
                    Content = content,
                    Include = _options.Include,
                    Select = _options.Select,
                    Precedence = _options.Precedence
                };
            if (method == HttpMethod.Put || method == HttpMethod.Delete)
            {
                parms.ETag = ContentHelper.GetProtocolValue<string>(content, SDataProtocolProperty.ETag);
            }

#if !NET_2_0 && !NET_3_5
            var index = _items.Count;
#endif
            _items.Add(parms);

#if !NET_2_0 && !NET_3_5
            return new Lazy<T>(() =>
            {
                if (_state == 0)
                {
                    throw new InvalidOperationException("Value cannot be fetched before committing the batch");
                }
                if (_state == 1)
                {
                    throw new InvalidOperationException("Value cannot be fetched until the commit has completed");
                }
                return _results[index];
            });
#endif
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public IList<T> Commit()
        {
            var state = Interlocked.CompareExchange(ref _state, 1, 0);
            if (state == 1)
            {
                throw new InvalidOperationException("Commit already in progress");
            }
            if (state == 2)
            {
                throw new InvalidOperationException("Batch has already been committed");
            }

            _results = _client.ExecuteBatch<T>(_items).Content;
            Interlocked.Exchange(ref _state, 2);
            return _results;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public Task<IList<T>> CommitAsync(CancellationToken cancel = default(CancellationToken))
        {
            var state = Interlocked.CompareExchange(ref _state, 1, 0);
            if (state == 1)
            {
                throw new InvalidOperationException("Commit already in progress");
            }
            if (state == 2)
            {
                throw new InvalidOperationException("Batch has already been committed");
            }

            return _client.ExecuteBatchAsync<T>(_items, cancel)
                .ContinueWith(task =>
                {
                    _results = task.Result.Content;
                    Interlocked.Exchange(ref _state, 2);
                    return _results;
                }, cancel);
        }
#endif
    }
}