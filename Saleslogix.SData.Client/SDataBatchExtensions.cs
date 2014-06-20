// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Collections.Generic;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Utilities;

#if !NET_2_0 && !NET_3_5
using System;
using System.Linq;
#endif

namespace Saleslogix.SData.Client
{
    public static class SDataBatchExtensions
    {
#if NET_2_0 || NET_3_5
        public static void Get<T>(this ISDataBatch<T> batch, string key)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(key, "key");
            batch.Add(HttpMethod.Get, new SDataResource {Key = key});
        }

        public static void Post<T>(this ISDataBatch<T> batch, T content)
        {
            Guard.ArgumentNotNull(batch, "batch");
            batch.Add(HttpMethod.Post, content);
        }

        public static void Put<T>(this ISDataBatch<T> batch, T content)
        {
            Guard.ArgumentNotNull(batch, "batch");
            batch.Add(HttpMethod.Put, content);
        }

        public static void Delete<T>(this ISDataBatch<T> batch, T content)
        {
            Guard.ArgumentNotNull(batch, "batch");
            batch.Add(HttpMethod.Delete, content);
        }

        public static void GetAll<T>(this ISDataBatch<T> batch, params string[] keys)
        {
            GetAll(batch, (IEnumerable<string>) keys);
        }

        public static void PostAll<T>(this ISDataBatch<T> batch, params T[] contents)
        {
            PostAll(batch, (IEnumerable<T>) contents);
        }

        public static void PutAll<T>(this ISDataBatch<T> batch, params T[] contents)
        {
            PutAll(batch, (IEnumerable<T>) contents);
        }

        public static void DeleteAll<T>(this ISDataBatch<T> batch, params T[] contents)
        {
            DeleteAll(batch, (IEnumerable<T>) contents);
        }

        public static void GetAll<T>(this ISDataBatch<T> batch, IEnumerable<string> keys)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(keys, "keys");
            foreach (var key in keys)
            {
                batch.Add(HttpMethod.Get, new SDataResource {Key = key});
            }
        }

        public static void PostAll<T>(this ISDataBatch<T> batch, IEnumerable<T> contents)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(contents, "contents");
            foreach (var content in contents)
            {
                batch.Add(HttpMethod.Post, content);
            }
        }

        public static void PutAll<T>(this ISDataBatch<T> batch, IEnumerable<T> contents)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(contents, "contents");
            foreach (var content in contents)
            {
                batch.Add(HttpMethod.Put, content);
            }
        }

        public static void DeleteAll<T>(this ISDataBatch<T> batch, IEnumerable<T> contents)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(contents, "contents");
            foreach (var content in contents)
            {
                batch.Add(HttpMethod.Delete, content);
            }
        }
#else
        public static Lazy<T> Get<T>(this ISDataBatch<T> batch, string key)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(key, "key");
            return batch.Add(HttpMethod.Get, new SDataResource {Key = key});
        }

        public static Lazy<T> Post<T>(this ISDataBatch<T> batch, T content)
        {
            Guard.ArgumentNotNull(batch, "batch");
            return batch.Add(HttpMethod.Post, content);
        }

        public static Lazy<T> Put<T>(this ISDataBatch<T> batch, T content)
        {
            Guard.ArgumentNotNull(batch, "batch");
            return batch.Add(HttpMethod.Put, content);
        }

        public static Lazy<T> Delete<T>(this ISDataBatch<T> batch, T content)
        {
            Guard.ArgumentNotNull(batch, "batch");
            return batch.Add(HttpMethod.Delete, content);
        }

        public static Lazy<IList<T>> GetAll<T>(this ISDataBatch<T> batch, params string[] keys)
        {
            return GetAll(batch, (IEnumerable<string>) keys);
        }

        public static Lazy<IList<T>> PostAll<T>(this ISDataBatch<T> batch, params T[] contents)
        {
            return PostAll(batch, (IEnumerable<T>) contents);
        }

        public static Lazy<IList<T>> PutAll<T>(this ISDataBatch<T> batch, params T[] contents)
        {
            return PutAll(batch, (IEnumerable<T>) contents);
        }

        public static Lazy<IList<T>> DeleteAll<T>(this ISDataBatch<T> batch, params T[] contents)
        {
            return DeleteAll(batch, (IEnumerable<T>) contents);
        }

        public static Lazy<IList<T>> GetAll<T>(this ISDataBatch<T> batch, IEnumerable<string> keys)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(keys, "keys");
            var lazies = keys.Select(key => batch.Add(HttpMethod.Get, new SDataResource {Key = key})).ToList();
            return new Lazy<IList<T>>(() => lazies.Select(lazy => lazy.Value).ToList());
        }

        public static Lazy<IList<T>> PostAll<T>(this ISDataBatch<T> batch, IEnumerable<T> contents)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(contents, "contents");
            var lazies = contents.Select(content => batch.Add(HttpMethod.Post, content)).ToList();
            return new Lazy<IList<T>>(() => lazies.Select(lazy => lazy.Value).ToList());
        }

        public static Lazy<IList<T>> PutAll<T>(this ISDataBatch<T> batch, IEnumerable<T> contents)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(contents, "contents");
            var lazies = contents.Select(content => batch.Add(HttpMethod.Put, content)).ToList();
            return new Lazy<IList<T>>(() => lazies.Select(lazy => lazy.Value).ToList());
        }

        public static Lazy<IList<T>> DeleteAll<T>(this ISDataBatch<T> batch, IEnumerable<T> contents)
        {
            Guard.ArgumentNotNull(batch, "batch");
            Guard.ArgumentNotNull(contents, "contents");
            var lazies = contents.Select(content => batch.Add(HttpMethod.Delete, content)).ToList();
            return new Lazy<IList<T>>(() => lazies.Select(lazy => lazy.Value).ToList());
        }
#endif
    }
}