// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

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
        public static SDataResource Get(this ISDataClient client, string key, string path, SDataPayloadOptions options = null)
        {
            Guard.ArgumentNotNull(client, "client");
            return client.Execute<SDataResource>(GetGetParameters(key, path, options)).Content;
        }

        public static T Get<T>(this ISDataClient client, string key, string path = null, SDataPayloadOptions options = null)
        {
            Guard.ArgumentNotNull(client, "client");
            return client.Execute<T>(GetGetParameters(key, GetPath<T>(path), options)).Content;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task<SDataResource> GetAsync(this ISDataClient client, string key, string path, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(client, "client");
            return client.ExecuteAsync<SDataResource>(GetGetParameters(key, path, options), cancel)
                .ContinueWith(task => task.Result.Content, cancel);
        }

        public static Task<T> GetAsync<T>(this ISDataClient client, string key, string path = null, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(client, "client");
            return client.ExecuteAsync<T>(GetGetParameters(key, GetPath<T>(path), options), cancel)
                .ContinueWith(task => task.Result.Content, cancel);
        }
#endif

        private static SDataParameters GetGetParameters(string key, string path, SDataPayloadOptions options)
        {
            Guard.ArgumentNotNullOrEmptyString(path, "path");
            if (options == null)
            {
                options = new SDataPayloadOptions();
            }
            return new SDataParameters
                {
                    Path = path,
                    Selector = key != null ? SDataUri.FormatConstant(key) : null,
                    Include = options.Include,
                    Select = options.Select,
                    Precedence = options.Precedence
                };
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static SDataResource Post(this ISDataClient client, SDataResource content, string path, SDataPayloadOptions options = null)
        {
            return client.Execute<SDataResource>(GetPostParameters(client, content, path, options)).Content;
        }

        public static T Post<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null)
        {
            return client.Execute<T>(GetPostParameters(client, content, GetPath<T>(path), options)).Content;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task<SDataResource> PostAsync(this ISDataClient client, SDataResource content, string path, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync<SDataResource>(GetPostParameters(client, content, path, options), cancel)
                .ContinueWith(task => task.Result.Content, cancel);
        }

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
            return new SDataParameters
                {
                    Method = HttpMethod.Post,
                    Path = path,
                    Content = content,
                    Include = options.Include,
                    Select = options.Select,
                    Precedence = options.Precedence
                };
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static SDataResource Put(this ISDataClient client, SDataResource content, string path = null, SDataPayloadOptions options = null)
        {
            return client.Execute<SDataResource>(GetPutParameters(client, content, path, options)).Content;
        }

        public static T Put<T>(this ISDataClient client, T content, string path = null, SDataPayloadOptions options = null)
        {
            return client.Execute<T>(GetPutParameters(client, content, GetPath<T>(path), options)).Content;
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task<SDataResource> PutAsync(this ISDataClient client, SDataResource content, string path, SDataPayloadOptions options = null, CancellationToken cancel = default(CancellationToken))
        {
            return client.ExecuteAsync<SDataResource>(GetPutParameters(client, content, path, options), cancel)
                .ContinueWith(task => task.Result.Content, cancel);
        }

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
                    Path = path,
                    Selector = GetSelector(content),
                    Content = content,
                    ETag = GetETag(content),
                    Include = options.Include,
                    Select = options.Select,
                    Precedence = options.Precedence
                };
        }

#if !PCL && !NETFX_CORE && !SILVERLIGHT
        public static void Delete(this ISDataClient client, SDataResource content, string path = null)
        {
            Guard.ArgumentNotNull(client, "client");
            client.Execute(GetDeleteParameters(content, path));
        }

        public static void Delete<T>(this ISDataClient client, T content, string path = null)
        {
            Guard.ArgumentNotNull(client, "client");
            client.Execute(GetDeleteParameters(content, GetPath<T>(path)));
        }
#endif

#if !NET_2_0 && !NET_3_5
        public static Task DeleteAsync(this ISDataClient client, SDataResource content, string path = null, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(client, "client");
            return client.ExecuteAsync(GetDeleteParameters(content, path), cancel);
        }

        public static Task DeleteAsync<T>(this ISDataClient client, T content, string path = null, CancellationToken cancel = default(CancellationToken))
        {
            Guard.ArgumentNotNull(client, "client");
            return client.ExecuteAsync(GetDeleteParameters(content, GetPath<T>(path)), cancel);
        }
#endif

        private static SDataParameters GetDeleteParameters<T>(T content, string path)
        {
            Guard.ArgumentNotNull(content, "content");
            Guard.ArgumentNotNullOrEmptyString(path, "path");
            return new SDataParameters
                {
                    Method = HttpMethod.Delete,
                    Path = path,
                    Selector = GetSelector(content),
                    ETag = GetETag(content)
                };
        }

        public static ISDataBatch<SDataResource> CreateBatch(this ISDataClient client, string path, SDataPayloadOptions options = null)
        {
            Guard.ArgumentNotNull(client, "client");
            return new SDataBatch<SDataResource>(client, path, options);
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
            object instance;

            if (callExpr.Object != null)
            {
                if (attr == null || string.IsNullOrEmpty(attr.InstancePropertyName))
                {
                    throw new SDataClientException("Instance methods must be decorated with SDataServiceOperation attribute with InstancePropertyName specified");
                }

                instance = Expression.Lambda(callExpr.Object).Compile().DynamicInvoke();
                request[attr.InstancePropertyName] = instance;
            }
            else
            {
                instance = null;
            }
            foreach (var pair in callExpr.Method.GetParameters().Zip(callExpr.Arguments, (param, arg) => new {param, arg}))
            {
                request[namingScheme.GetName(pair.param)] = Expression.Lambda(pair.arg).Compile().DynamicInvoke();
            }

            if (path == null)
            {
                path = SDataPathAttribute.GetPath(callExpr.Method) ??
                       SDataPathAttribute.GetPath(instance != null ? instance.GetType() : callExpr.Method.DeclaringType);
            }
            if (path != null)
            {
                path += "/";
            }
            path += "$service/" + namingScheme.GetName(callExpr.Method);

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
                if (type.IsInstanceOfType(firstValue) ||
#if NETFX_CORE
                    type.GetTypeInfo().IsEnum || type == typeof (byte) || type == typeof (short) || type == typeof (int) || type == typeof (long) ||
                    type == typeof (float) || type == typeof (double) || type == typeof (decimal) || type == typeof (bool) ||
                    type == typeof (DateTime) || type == typeof (DateTimeOffset) || type == typeof (char) || type == typeof (string) ||
                    !(firstValue is Enum || firstValue is byte || firstValue is short || firstValue is int || firstValue is long ||
                      firstValue is float || firstValue is double || firstValue is decimal || firstValue is bool ||
                      firstValue is DateTime || firstValue is DateTimeOffset || firstValue is char || firstValue is string))
#else
                    typeof (IConvertible).IsAssignableFrom(type) ||
                    !(firstValue is IConvertible))
#endif
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