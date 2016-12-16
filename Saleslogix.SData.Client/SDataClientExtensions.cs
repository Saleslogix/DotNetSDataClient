// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Collections.Generic;
using Saleslogix.SData.Client.Content;
using Saleslogix.SData.Client.Framework;
using Saleslogix.SData.Client.Utilities;

#if !NET_2_0
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
#endif

#if !NET_2_0 && !NET_3_5
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Saleslogix.SData.Client
{
    public static class SDataClientExtensions
    {
#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static IEnumerable<SDataResource> Enumerate(this ISDataClient client, string path, SDataEnumerateOptions options = null)
        {
            return Enumerate<SDataResource>(client, path, options);
        }

        public static IEnumerable<T> Enumerate<T>(this ISDataClient client, string path, T prototype, SDataEnumerateOptions options = null)
        {
            return Enumerate<T>(client, path, options);
        }

        public static IEnumerable<T> Enumerate<T>(this ISDataClient client, string path = null, SDataEnumerateOptions options = null)
        {
            var parms = GetEnumerateParameters(client, GetPath<T>(path), options);

            while (true)
            {
                var collection = client.Execute<SDataCollection<T>>(parms).Content;
                if (collection.Count == 0)
                {
                    break;
                }

                foreach (var item in collection)
                {
                    yield return item;
                }

                parms.StartIndex = (parms.StartIndex ?? 1) + collection.Count;
                if (collection.TotalResults != null && parms.StartIndex > collection.TotalResults)
                {
                    break;
                }
            }
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task<IEnumerable<SDataResource>> EnumerateAsync(this ISDataClient client, string path = null, SDataEnumerateOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return EnumerateAsync<SDataResource>(client, path, options, cancel);
        }

        public static Task<IEnumerable<T>> EnumerateAsync<T>(this ISDataClient client, string path, T prototype, SDataEnumerateOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return EnumerateAsync<T>(client, path, options, cancel);
        }

        public static Task<IEnumerable<T>> EnumerateAsync<T>(this ISDataClient client, string path = null, SDataEnumerateOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            var parms = GetEnumerateParameters(client, GetPath<T>(path), options);
            var items = new List<T>();
            Func<Task<ISDataResults<SDataCollection<T>>>> loop = null;
            loop =
                () => client.ExecuteAsync<SDataCollection<T>>(parms, cancel)
                    .ContinueWith(
                        task =>
                        {
                            var collection = task.Result.Content;
                            if (collection.Count == 0)
                            {
                                return task;
                            }

                            items.AddRange(collection);
                            parms.StartIndex = (parms.StartIndex ?? 1) + collection.Count;
                            if (collection.TotalResults != null && parms.StartIndex > collection.TotalResults)
                            {
                                return task;
                            }

                            return loop();
                        }, cancel)
                    .Unwrap();
            return loop().ContinueWith(task => (IEnumerable<T>) items, cancel);
        }
#endif

        private static SDataParameters GetEnumerateParameters(ISDataClient client, string path, SDataEnumerateOptions options)
        {
            Guard.ArgumentNotNull(client, "client");
            if (options == null)
            {
                options = new SDataEnumerateOptions();
            }
            return new SDataParameters
                {
                    Path = path,
                    Where = options.Where,
                    OrderBy = options.OrderBy,
                    Include = options.Include,
                    Select = options.Select,
                    Precedence = options.Precedence
                };
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static SDataResource Get(this ISDataClient client, string key, string path, SDataPayloadOptions options = null)
        {
            return Get<SDataResource>(client, key, path, options);
        }

        public static T Get<T>(this ISDataClient client, string key, string path, T prototype, SDataPayloadOptions options = null)
        {
            return Get<T>(client, key, path, options);
        }

        public static T Get<T>(this ISDataClient client, string key, string path = null, SDataPayloadOptions options = null)
        {
            return client.Execute<T>(GetGetParameters(client, key, null, GetPath<T>(path), options)).Content;
        }

        public static T Get<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null)
        {
            Guard.ArgumentNotNull(content, "content");
            var results = client.Execute<T>(GetGetParameters(client, GetKey(content), GetETag(content), GetPath<T>(path), options));
            return !Equals(results.Content, default(T)) ? results.Content : content;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task<SDataResource> GetAsync(this ISDataClient client, string key, string path = null, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return GetAsync<SDataResource>(client, key, path, options, cancel);
        }

        public static Task<T> GetAsync<T>(this ISDataClient client, string key, string path, T prototype, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return GetAsync<T>(client, key, path, options, cancel);
        }

        public static Task<T> GetAsync<T>(this ISDataClient client, string key, string path = null, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync<T>(GetGetParameters(client, key, null, GetPath<T>(path), options), cancel)
                .ContinueWith(task => task.Result.Content, cancel);
        }

        public static Task<T> GetAsync<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(content, "content");
            return client.ExecuteAsync<T>(GetGetParameters(client, GetKey(content), GetETag(content), GetPath<T>(path), options), cancel)
                .ContinueWith(task => !Equals(task.Result.Content, default(T)) ? task.Result.Content : content, cancel);
        }
#endif

        private static SDataParameters GetGetParameters(ISDataClient client, string key, string etag, string path, SDataPayloadOptions options)
        {
            Guard.ArgumentNotNull(client, "client");
            Guard.ArgumentNotNullOrEmptyString(key, "key");
            Guard.ArgumentNotNullOrEmptyString(path, "path");
            if (options == null)
            {
                options = new SDataPayloadOptions();
            }
            return new SDataParameters
                {
                    Path = string.Format("{0}({1})", path, SDataUri.FormatConstant(key)),
                    ETag = etag,
                    Include = options.Include,
                    Select = options.Select,
                    Precedence = options.Precedence
                };
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static T Post<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null)
        {
            return client.Execute<T>(GetPostParameters(client, content, GetPath<T>(path), options)).Content;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task<T> PostAsync<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync<T>(GetPostParameters(client, content, GetPath<T>(path), options), cancel)
                .ContinueWith(task => task.Result.Content, cancel);
        }
#endif

        private static SDataParameters GetPostParameters<T>(ISDataClient client, T content, string path, SDataPayloadOptions options)
        {
            Guard.ArgumentNotNull(client, "client");
            Guard.ArgumentNotNull(content, "content");
            Guard.ArgumentNotNullOrEmptyString(path, "path");
            if (options == null)
            {
                options = new SDataPayloadOptions();
            }

            var resource = ContentHelper.Serialize(content, client.NamingScheme) as SDataResource;
            if (resource == null)
            {
                throw new SDataClientException("Only resources can be posted");
            }
            resource.HttpMethod = HttpMethod.Post;

            return new SDataParameters
                {
                    Method = HttpMethod.Post,
                    Path = path,
                    Content = resource,
                    Include = options.Include,
                    Select = options.Select,
                    Precedence = options.Precedence
                };
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static T Put<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null)
        {
            return client.Execute<T>(GetPutParameters(client, content, GetPath<T>(path), options)).Content;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task<T> PutAsync<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync<T>(GetPutParameters(client, content, GetPath<T>(path), options), cancel)
                .ContinueWith(task => task.Result.Content, cancel);
        }
#endif

        private static SDataParameters GetPutParameters<T>(ISDataClient client, T content, string path, SDataPayloadOptions options)
        {
            Guard.ArgumentNotNull(client, "client");
            Guard.ArgumentNotNull(content, "content");
            Guard.ArgumentNotNullOrEmptyString(path, "path");
            if (options == null)
            {
                options = new SDataPayloadOptions();
            }
            return new SDataParameters
                {
                    Method = HttpMethod.Put,
                    Path = string.Format("{0}({1})", path, SDataUri.FormatConstant(GetKey(content))),
                    Content = content,
                    ETag = GetETag(content),
                    Include = options.Include,
                    Select = options.Select,
                    Precedence = options.Precedence
                };
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static void Delete<T>(this ISDataClient client, T content, string path = null)
        {
            client.Execute(GetDeleteParameters(client, content, GetPath<T>(path)));
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task DeleteAsync<T>(this ISDataClient client, T content, string path = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync(GetDeleteParameters(client, content, GetPath<T>(path)), cancel);
        }
#endif

        private static SDataParameters GetDeleteParameters<T>(ISDataClient client, T content, string path)
        {
            Guard.ArgumentNotNull(client, "client");
            Guard.ArgumentNotNull(content, "content");
            Guard.ArgumentNotNullOrEmptyString(path, "path");
            return new SDataParameters
                {
                    Method = HttpMethod.Delete,
                    Path = string.Format("{0}({1})", path, SDataUri.FormatConstant(GetKey(content))),
                    ETag = GetETag(content)
                };
        }

        public static ISDataBatch<SDataResource> CreateBatch(this ISDataClient client, string path, SDataPayloadOptions options = null)
        {
            return CreateBatch<SDataResource>(client, path, options);
        }

        public static ISDataBatch<T> CreateBatch<T>(this ISDataClient client, string path = null, SDataPayloadOptions options = null)
        {
            Guard.ArgumentNotNull(client, "client");
            return new SDataBatch<T>(client, GetPath<T>(path), options);
        }

        private static string GetPath<T>(string path)
        {
            return path ?? SDataPathAttribute.GetPath(typeof (T));
        }

        private static string GetKey(object content)
        {
            var key = ContentHelper.GetProtocolValue<string>(content, SDataProtocolProperty.Key);
            if (string.IsNullOrEmpty(key))
            {
                throw new SDataClientException("Unable to extract resource key from content");
            }

            return key;
        }

        private static string GetETag(object content)
        {
            return ContentHelper.GetProtocolValue<string>(content, SDataProtocolProperty.ETag);
        }

#if !NET_2_0
#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static void CallService(this ISDataClient client, Expression<Action> methodCall, string path = null)
        {
            client.Execute(GetServiceParameters(client, methodCall.Body, path));
        }

        public static T CallService<T>(this ISDataClient client, Expression<Func<T>> methodCall, string path = null)
        {
            var content = client.Execute<SDataResource>(GetServiceParameters(client, methodCall.Body, path)).Content;
            return GetServiceResult<T>(client, methodCall.Body, content);
        }
#endif

#if !NET_3_5
        public static Task CallServiceAsync(this ISDataClient client, Expression<Action> methodCall, string path = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync(GetServiceParameters(client, methodCall.Body, path), cancel);
        }

        public static Task<T> CallServiceAsync<T>(this ISDataClient client, Expression<Func<T>> methodCall, string path = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync<SDataResource>(GetServiceParameters(client, methodCall.Body, path), cancel)
                .ContinueWith(task => GetServiceResult<T>(client, methodCall.Body, task.Result.Content), cancel);
        }
#endif

        private static SDataParameters GetServiceParameters(ISDataClient client, Expression bodyExpr, string path)
        {
            Guard.ArgumentNotNull(client, "client");

            var callExpr = bodyExpr as MethodCallExpression;
            if (callExpr == null)
            {
                throw new SDataClientException("Expression must be a method call");
            }

            var attr = callExpr.Method.GetCustomAttribute<SDataServiceOperationAttribute>();
            var namingScheme = client.NamingScheme ?? NamingScheme.Default;
            var request = new SDataResource();
            var instance = callExpr.Object != null ? Expression.Lambda(callExpr.Object).Compile().DynamicInvoke() : null;

            if (path == null)
            {
                path = SDataPathAttribute.GetPath(instance != null ? instance.GetType() : callExpr.Method.DeclaringType);
            }

            if (instance != null)
            {
                if (attr == null || attr.PassInstanceBy == InstancePassingConvention.Selector ||
                    (attr.PassInstanceBy == InstancePassingConvention.Default && string.IsNullOrEmpty(attr.InstancePropertyName)))
                {
                    if (path == null)
                    {
                        throw new SDataClientException("Path must be specified when passing instance context by selector");
                    }
                    var key = ContentHelper.GetProtocolValue<string>(instance, SDataProtocolProperty.Key);
                    if (string.IsNullOrEmpty(key))
                    {
                        throw new SDataClientException("Unable to extract resource key from instance");
                    }
                    path += string.Format("({0})", SDataUri.FormatConstant(key));
                }
                else if (attr.PassInstanceBy == InstancePassingConvention.Default)
                {
                    var key = ContentHelper.GetProtocolValue<string>(instance, SDataProtocolProperty.Key);
                    request[attr.InstancePropertyName] = !string.IsNullOrEmpty(key) ? key : instance;
                }
                else
                {
                    if (string.IsNullOrEmpty(attr.InstancePropertyName))
                    {
                        throw new SDataClientException("Instance property name must be specified when passing instance context by key property or object property");
                    }

                    if (attr.PassInstanceBy == InstancePassingConvention.KeyProperty)
                    {
                        var key = ContentHelper.GetProtocolValue<string>(instance, SDataProtocolProperty.Key);
                        if (string.IsNullOrEmpty(key))
                        {
                            throw new SDataClientException("Unable to extract resource key from instance");
                        }
                        request[attr.InstancePropertyName] = key;
                    }
                    else if (attr.PassInstanceBy == InstancePassingConvention.ObjectProperty)
                    {
                        request[attr.InstancePropertyName] = instance;
                    }
                }
            }

            if (path != null)
            {
                path += "/";
            }
            path += "$service/" + namingScheme.GetName(callExpr.Method);

            foreach (var pair in callExpr.Method.GetParameters().Zip(callExpr.Arguments, (param, arg) => new {param, arg}))
            {
                request[namingScheme.GetName(pair.param)] = Expression.Lambda(pair.arg).Compile().DynamicInvoke();
            }

            var xmlLocalName = attr != null ? attr.XmlLocalName : null;
            var xmlNamespace = attr != null ? attr.XmlNamespace : null;
            var content = new SDataResource(xmlLocalName, xmlNamespace) {{"request", request}};
            return new SDataParameters
                {
                    Method = HttpMethod.Post,
                    Path = path,
                    Content = content
                };
        }

        private static T GetServiceResult<T>(ISDataClient client, Expression bodyExpr, SDataResource content)
        {
            var value = content["response"];
            var response = (SDataResource) value;

            var attr = ((MethodCallExpression) bodyExpr).Method.GetCustomAttribute<SDataServiceOperationAttribute>();
            if (attr != null && !string.IsNullOrEmpty(attr.ResultPropertyName))
            {
                value = response[attr.ResultPropertyName];
            }
            else if (response.Count == 1)
            {
                var firstValue = response.Values.First();
                var type = typeof (T);
                type = Nullable.GetUnderlyingType(type) ?? type;
                if (ContentHelper.IsObject(type) == ContentHelper.IsObject(firstValue))
                {
                    value = firstValue;
                }
            }

            var result = ContentHelper.Deserialize<T>(value, client.NamingScheme);
            var tracking = result as IChangeTracking;
            if (tracking != null)
            {
                tracking.AcceptChanges();
            }
            return result;
        }
#endif
    }
}