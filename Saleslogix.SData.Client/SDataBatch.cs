using System;
using System.Collections.Generic;
using System.Threading;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Framework;

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

#if NET_2_0 || NET_3_5
        public void Get(string key)
        {
            Add(HttpMethod.Get, key, null);
        }

        public void Post(T content)
        {
            Add(HttpMethod.Post, null, content);
        }

        public void Put(T content)
        {
            Add(HttpMethod.Put, null, content);
        }

        public void Delete(T content)
        {
            Add(HttpMethod.Delete, null, content);
        }
#else
        public Lazy<T> Get(string key)
        {
            return Add(HttpMethod.Get, key, null);
        }

        public Lazy<T> Post(T content)
        {
            return Add(HttpMethod.Post, null, content);
        }

        public Lazy<T> Put(T content)
        {
            return Add(HttpMethod.Put, null, content);
        }

        public Lazy<T> Delete(T content)
        {
            return Add(HttpMethod.Delete, null, content);
        }
#endif

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public IList<T> Commit()
        {
            EnsureNotCommitted();

            _results = _client.ExecuteBatch<T>(_items).Content;
            Interlocked.Exchange(ref _state, 2);
            return _results;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public Task<IList<T>> CommitAsync(CancellationToken cancel = default(CancellationToken))
        {
            EnsureNotCommitted();

            return _client.ExecuteBatchAsync<T>(_items, cancel)
                .ContinueWith(task =>
                {
                    _results = task.Result.Content;
                    Interlocked.Exchange(ref _state, 2);
                    return _results;
                }, cancel);
        }
#endif

        private
#if NET_2_0 || NET_3_5
            void
#else
            Lazy<T>
#endif
            Add(HttpMethod method, string key, object content)
        {
            EnsureNotCommitted();

            var parms = new SDataParameters
                {
                    Method = method,
                    Path = _path,
                    Include = _options.Include,
                    Select = _options.Select,
                    Precedence = _options.Precedence
                };
            if (key != null)
            {
                parms.Selector = SDataUri.FormatConstant(key);
            }
            else if (content != null)
            {
                if (method != HttpMethod.Post)
                {
                    parms.Selector = GetSelector(content);
                    parms.ETag = GetETag(content);
                }

                if (method != HttpMethod.Delete)
                {
                    parms.Content = content;
                }
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

        private static string GetSelector(object content)
        {
            var key = ContentHelper.GetProtocolValue<string>(content, SDataProtocolProperty.Key);
            if (string.IsNullOrEmpty(key))
            {
                throw new SDataClientException("Unable to extract resource key from content");
            }

            return SDataUri.FormatConstant(key);
        }

        private static string GetETag(object content)
        {
            return ContentHelper.GetProtocolValue<string>(content, SDataProtocolProperty.ETag);
        }

        private void EnsureNotCommitted()
        {
            if (_state != 0)
            {
                throw new InvalidOperationException("Batch has already been committed");
            }
        }
    }
}