// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Saleslogix.SData.Client.Utilities;
using SimpleJson;
using SimpleJson.Reflection;

#if !NET_2_0
using System.Runtime.Serialization;
#endif

namespace Saleslogix.SData.Client.Content
{
    internal static class ContentHelper
    {
        private static readonly IDictionary<Type, IDictionary<SDataProtocolProperty, ReflectionUtils.GetDelegate>> GetProtocolValueCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<SDataProtocolProperty, ReflectionUtils.GetDelegate>>(GetterValueFactory);
        private static readonly IDictionary<Type, IDictionary<SDataProtocolProperty, ReflectionUtils.SetDelegate>> SetProtocolValueCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<SDataProtocolProperty, ReflectionUtils.SetDelegate>>(SetterValueFactory);

        public static object Serialize(object value, INamingScheme namingScheme = null)
        {
            object result;
            new Serializer(namingScheme ?? NamingScheme.Default).TrySerializeNonPrimitiveObject(value, out result);
            return result;
        }

        public static T Deserialize<T>(object value, INamingScheme namingScheme = null)
        {
            return (T) new Serializer(namingScheme ?? NamingScheme.Default).DeserializeObject(value, typeof (T));
        }

        public static IEnumerable<IDictionary<string, object>> AsDictionaries(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var generic = obj as IEnumerable<IDictionary<string, object>>;
            if (generic != null)
            {
                return generic;
            }

            var nonGeneric = obj as IEnumerable;
            if (nonGeneric != null &&
                obj.GetType()
                    .GetInterfaces()
                    .Any(item => item.GetTypeInfo().IsGenericType &&
                                 item.GetGenericTypeDefinition() == typeof (IEnumerable<>) &&
                                 typeof (IDictionary<string, object>).IsAssignableFrom(item.GetGenericArguments()[0])))
            {
                generic = nonGeneric.Cast<IDictionary<string, object>>();
            }

            var items = AsCollection(obj);
            if (items != null)
            {
                var dictionaries = items.Select(AsDictionary).ToList();
                if (dictionaries.Count > 0 && dictionaries.All(item => item != null))
                {
                    generic = dictionaries;
                }
            }

            if (generic != null)
            {
                var prot = obj as ISDataProtocolAware;
                if (prot != null)
                {
                    generic = new SDataCollection<IDictionary<string, object>>(generic);
                    ((ISDataProtocolAware) generic).Info = prot.Info;
                }

                return generic;
            }

            return null;
        }

        public static IDictionary<string, object> AsDictionary(object obj)
        {
            if (obj != null)
            {
                var generic = obj as IDictionary<string, object>;
                if (generic != null)
                {
                    return generic;
                }

                var nonGeneric = obj as IDictionary;
                if (nonGeneric != null)
                {
                    generic = nonGeneric.Cast<DictionaryEntry>()
                        .ToDictionary(entry => entry.Key.ToString(), entry => entry.Value);
                }
                else
                {
                    var entries = obj as IEnumerable;
                    if (entries != null)
                    {
                        var iface = obj.GetType()
                            .GetInterfaces()
                            .FirstOrDefault(item => item.GetTypeInfo().IsGenericType && item.GetGenericTypeDefinition() == typeof (IDictionary<,>));
                        if (iface != null)
                        {
                            var keyValueType = iface.GetInterfaces()
                                .First(a => a.GetTypeInfo().IsGenericType && a.GetGenericTypeDefinition() == typeof (IEnumerable<>))
                                .GetGenericArguments()[0];
                            var keyProp = keyValueType.GetProperty("Key");
                            var valueProp = keyValueType.GetProperty("Value");
                            generic = entries.Cast<object>()
                                .ToDictionary(item => keyProp.GetValue(item, null).ToString(),
                                    item => valueProp.GetValue(item, null));
                        }
                    }
                }

                if (generic != null)
                {
                    var prot = obj as ISDataProtocolAware;
                    if (prot != null)
                    {
                        generic = new SDataResource(generic);
                        ((ISDataProtocolAware) generic).Info = prot.Info;
                    }

                    return generic;
                }
            }

            return null;
        }

        public static IEnumerable<object> AsCollection(object obj)
        {
            if (obj != null && !(obj is string) && !IsDictionary(obj))
            {
                var generic = obj as IEnumerable<object>;
                if (generic != null)
                {
                    return generic;
                }

                var nonGeneric = obj as IEnumerable;
                if (nonGeneric != null)
                {
                    generic = nonGeneric.Cast<object>();

                    var prot = obj as ISDataProtocolAware;
                    if (prot != null)
                    {
                        generic = new SDataCollection<object>(generic);
                        ((ISDataProtocolAware) generic).Info = prot.Info;
                    }
                    else
                    {
                        generic = generic.ToList();
                    }

                    return generic;
                }
            }

            return null;
        }

        public static bool IsDictionary(object obj)
        {
            return obj != null &&
                   (obj is IDictionary ||
                    obj is IDictionary<string, object> ||
                    obj.GetType().GetInterfaces().Any(iface => iface.GetTypeInfo().IsGenericType && iface.GetGenericTypeDefinition() == typeof (IDictionary<,>)));
        }

        public static bool IsCollection(object obj)
        {
            return obj != null &&
                   !(obj is string) &&
                   (obj is IEnumerable ||
                    obj.GetType().GetInterfaces().Any(iface => iface.GetTypeInfo().IsGenericType && iface.GetGenericTypeDefinition() == typeof (IEnumerable<>)));
        }

        public static bool IsObject(object obj)
        {
            return obj != null && !(obj is string) && !(obj is ValueType);
        }

        public static T GetProtocolValue<T>(object obj, SDataProtocolProperty prop)
        {
            Guard.ArgumentNotNull(obj, "obj");

            var prot = obj as ISDataProtocolAware;
            if (prot != null)
            {
                return (T) prot.Info.GetValue(prop);
            }

            var getters = GetProtocolValueCache[obj.GetType()];
            ReflectionUtils.GetDelegate getter;
            return getters.TryGetValue(prop, out getter) ? (T) getter(obj) : default(T);
        }

        public static void SetProtocolValue(object obj, SDataProtocolProperty prop, object value)
        {
            Guard.ArgumentNotNull(obj, "obj");

            var prot = obj as ISDataProtocolAware;
            if (prot != null)
            {
                prot.Info.SetValue(prop, value);
            }

            var setters = SetProtocolValueCache[obj.GetType()];
            ReflectionUtils.SetDelegate setter;
            if (setters.TryGetValue(prop, out setter))
            {
                setter(obj, value);
            }
        }

        private static IDictionary<SDataProtocolProperty, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            var result = new Dictionary<SDataProtocolProperty, ReflectionUtils.GetDelegate>();
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type).Where(info => info.CanRead))
            {
                var getMethod = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
                if (getMethod.IsStatic || !getMethod.IsPublic)
                {
                    continue;
                }
                var attr = propertyInfo.GetCustomAttribute<SDataProtocolPropertyAttribute>();
                if (attr != null && attr.Value != null)
                {
                    result[attr.Value.Value] = ReflectionUtils.GetGetMethod(propertyInfo);
                }
            }
            foreach (var fieldInfo in ReflectionUtils.GetFields(type).Where(info => !info.IsStatic && info.IsPublic))
            {
                var attr = fieldInfo.GetCustomAttribute<SDataProtocolPropertyAttribute>();
                if (attr != null && attr.Value != null)
                {
                    result[attr.Value.Value] = ReflectionUtils.GetGetMethod(fieldInfo);
                }
            }
            return result;
        }

        private static IDictionary<SDataProtocolProperty, ReflectionUtils.SetDelegate> SetterValueFactory(Type type)
        {
            var result = new Dictionary<SDataProtocolProperty, ReflectionUtils.SetDelegate>();
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type).Where(info => info.CanWrite))
            {
                var getMethod = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
                if (getMethod.IsStatic || !getMethod.IsPublic)
                {
                    continue;
                }
                var attr = propertyInfo.GetCustomAttribute<SDataProtocolPropertyAttribute>();
                if (attr != null && attr.Value != null)
                {
                    result[attr.Value.Value] = ReflectionUtils.GetSetMethod(propertyInfo);
                }
            }
            foreach (var fieldInfo in ReflectionUtils.GetFields(type).Where(info => !info.IsInitOnly && !info.IsStatic && info.IsPublic))
            {
                var attr = fieldInfo.GetCustomAttribute<SDataProtocolPropertyAttribute>();
                if (attr != null && attr.Value != null)
                {
                    result[attr.Value.Value] = ReflectionUtils.GetSetMethod(fieldInfo);
                }
            }
            return result;
        }

        #region Nested type: Serializer

        private class Serializer : PocoJsonSerializerStrategy
        {
            private readonly INamingScheme _namingScheme;

            public Serializer(INamingScheme namingScheme)
            {
                _namingScheme = namingScheme;
            }

            public override bool TrySerializeNonPrimitiveObject(object input, out object output)
            {
                if (!IsObject(input))
                {
                    output = input;
                    return true;
                }

                var items = AsCollection(input);
                if (items != null)
                {
                    return TrySerializeCollection(items, out output);
                }

                if (!base.TrySerializeNonPrimitiveObject(input, out output))
                {
                    output = null;
                    return false;
                }

                var dict = (IDictionary<string, object>) output;
                var prot = input as ISDataProtocolAware;
                var info = prot != null ? prot.Info : null;

                foreach (var item in dict.ToList())
                {
                    var value = item.Value;
                    var surrogate = value as XmlMetadataSurrogate;
                    if (surrogate != null)
                    {
                        value = surrogate.Value;
                    }

                    if (item.Key.StartsWith("$"))
                    {
                        if (info == null)
                        {
                            info = new SDataProtocolInfo();
                        }
                        var name = item.Key;
                        var prop = (SDataProtocolProperty) Enum.Parse(typeof (SDataProtocolProperty), char.ToUpperInvariant(name[1]) + name.Substring(2), false);
                        info.SetValue(prop, value);
                        continue;
                    }

                    object result;
                    if (!TrySerializeNonPrimitiveObject(value, out result))
                    {
                        output = null;
                        return false;
                    }

                    if (surrogate != null && result != null)
                    {
                        var itemInfo = ((ISDataProtocolAware) result).Info;
                        itemInfo.XmlLocalName = surrogate.XmlLocalName;
                        itemInfo.XmlNamespace = surrogate.XmlNamespace;
                        itemInfo.XmlIsFlat = surrogate.XmlIsFlat;
                    }

                    dict[item.Key] = result;
                }

                var resource = new SDataResource(dict);

                if (info != null)
                {
                    ((ISDataProtocolAware) resource).Info = info;
                }
                else
                {
                    var type = input.GetType();
#if !NET_2_0
                    var dataAttr = type.GetTypeInfo().GetCustomAttribute<DataContractAttribute>();
                    if (dataAttr != null)
                    {
                        resource.XmlLocalName = dataAttr.Name;
                        resource.XmlNamespace = dataAttr.Namespace;
                    }
                    else
#endif
                    {
                        var xmlAttr = type.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>();
                        if (xmlAttr != null)
                        {
                            resource.XmlLocalName = xmlAttr.TypeName;
                            resource.XmlNamespace = xmlAttr.Namespace;
                        }
                        else
                        {
                            resource.XmlLocalName = type.Name;
                        }
                    }
                }

                output = resource;
                return true;
            }

            private bool TrySerializeCollection(IEnumerable<object> input, out object output)
            {
                var results = new List<object>();
                foreach (var item in input)
                {
                    object result;
                    if (!TrySerializeNonPrimitiveObject(item, out result))
                    {
                        output = null;
                        return false;
                    }
                    results.Add(result);
                }

                output = results.All(result => result is SDataResource)
                    ? new SDataCollection<SDataResource>(results.Cast<SDataResource>())
                    : (object) new SDataCollection<object>(results);

                var prot = input as ISDataProtocolAware;
                if (prot != null)
                {
                    ((ISDataProtocolAware) output).Info = prot.Info;
                }

                return true;
            }

            public override object DeserializeObject(object value, Type type)
            {
                if (value == null)
                {
                    return type.GetTypeInfo().IsValueType && !ReflectionUtils.IsNullableType(type)
                        ? Activator.CreateInstance(type)
                        : null;
                }

                type = Nullable.GetUnderlyingType(type) ?? type;

                if (type.IsInstanceOfType(value))
                {
                    return value;
                }

                if (type == typeof (DateTime) || type == typeof (DateTimeOffset))
                {
                    if (value is DateTime)
                    {
                        return (DateTimeOffset) (DateTime) value;
                    }
                    if (value is DateTimeOffset)
                    {
                        return ((DateTimeOffset) value).DateTime;
                    }
                }
#if NETFX_CORE
                else if (type == typeof (byte) || type == typeof (short) || type == typeof (int) || type == typeof (long) ||
                         type == typeof (float) || type == typeof (double) || type == typeof (decimal) || type == typeof (bool))
#else
                else if (typeof (IConvertible).IsAssignableFrom(type))
#endif
                {
                    return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
                }

                if (!IsObject(value))
                {
                    return base.DeserializeObject(value, type);
                }

                var prot = value as ISDataProtocolAware;

                if (prot != null)
                {
                    var dict = AsDictionary(value);
                    if (dict != null)
                    {
                        foreach (SDataProtocolProperty protocolProp in Enum.GetValues(typeof (SDataProtocolProperty)))
                        {
                            var name = protocolProp.ToString();
                            dict[string.Format("${0}{1}", char.ToLowerInvariant(name[0]), name.Substring(1))] = prot.Info.GetValue(protocolProp);
                        }
                    }
                }

                // SimpleJson only recognizes IList<object>
                var items = AsCollection(value);
                if (items != null && !(value is IList<object>))
                {
                    value = items.ToList();
                }

                var result = base.DeserializeObject(value, type);

                if (prot != null)
                {
                    var resultProt = result as ISDataProtocolAware;
                    if (resultProt == null && items != null)
                    {
                        var itemType = type.GetTypeInfo().IsGenericType ? type.GetGenericArguments()[0] : typeof (object);
                        var colType = typeof (SDataCollection<>).MakeGenericType(itemType);
                        if (type.IsAssignableFrom(colType))
                        {
                            result = Activator.CreateInstance(colType, new[] {result});
                            resultProt = (ISDataProtocolAware) result;
                        }
                    }
                    if (resultProt != null)
                    {
                        resultProt.Info = prot.Info;
                    }
                }

                return result;
            }

            internal override IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
            {
                var result = new Dictionary<string, ReflectionUtils.GetDelegate>();
                foreach (var propertyInfo in GetProperties(type))
                {
                    result[GetName(propertyInfo)] = GetGetter(ReflectionUtils.GetGetMethod(propertyInfo), propertyInfo, propertyInfo.PropertyType);
                }
                foreach (var fieldInfo in GetFields(type))
                {
                    result[GetName(fieldInfo)] = GetGetter(ReflectionUtils.GetGetMethod(fieldInfo), fieldInfo, fieldInfo.FieldType);
                }

                return result;
            }

            private static ReflectionUtils.GetDelegate GetGetter(ReflectionUtils.GetDelegate baseGetter, MemberInfo memberInfo, Type memberType)
            {
                if (memberType != typeof (string) && !memberType.GetTypeInfo().IsValueType)
                {
                    string xmlLocalName = null;
                    string xmlNamespace = null;
                    var xmlIsFlat = false;

                    var elementAttr = memberInfo.GetCustomAttribute<XmlElementAttribute>();
                    if (elementAttr != null)
                    {
                        xmlLocalName = elementAttr.ElementName;
                        xmlNamespace = elementAttr.Namespace;
                        xmlIsFlat = memberType != typeof (string) &&
                                    (typeof (IEnumerable).IsAssignableFrom(memberType) ||
                                     memberType.GetInterfaces().Any(iface => iface.GetTypeInfo().IsGenericType && iface.GetGenericTypeDefinition() == typeof (IEnumerable<>)));
                    }

                    var itemAttr = memberInfo.GetCustomAttribute<XmlArrayItemAttribute>();
                    if (itemAttr != null)
                    {
                        xmlLocalName = itemAttr.ElementName;
                        xmlNamespace = itemAttr.Namespace;
                    }

                    if (xmlLocalName == null)
                    {
                        var itemType = memberType.IsArray
                            ? memberType.GetElementType()
                            : memberType.GetInterfaces()
                                .Where(iface => iface.GetTypeInfo().IsGenericType && iface.GetGenericTypeDefinition() == typeof (IEnumerable<>))
                                .Select(iface => iface.GetGenericArguments()[0])
                                .FirstOrDefault();
                        xmlLocalName = itemType != null && itemType != typeof (object) ? itemType.Name : null;
                        xmlNamespace = null;
                    }

                    if (xmlLocalName != null || xmlNamespace != null || xmlIsFlat)
                    {
                        return source => new XmlMetadataSurrogate
                            {
                                Value = baseGetter(source),
                                XmlLocalName = xmlLocalName,
                                XmlNamespace = xmlNamespace,
                                XmlIsFlat = xmlIsFlat
                            };
                    }
                }

                return baseGetter;
            }

            internal override IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
            {
                var result = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
                foreach (var propertyInfo in GetProperties(type))
                {
                    var setter = ReflectionUtils.GetSetMethod(propertyInfo);
                    result[GetName(propertyInfo)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType, setter);
                }
                foreach (var fieldInfo in GetFields(type))
                {
                    var setter = ReflectionUtils.GetSetMethod(fieldInfo);
                    result[GetName(fieldInfo)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, setter);
                }

                return result;
            }

            private static IEnumerable<PropertyInfo> GetProperties(Type type)
            {
                return ReflectionUtils.GetProperties(type)
                    .Where(item => item.CanRead &&
#if !NET_2_0
                        !item.IsDefined(typeof (IgnoreDataMemberAttribute)) &&
#endif
                        !item.IsDefined(typeof (XmlIgnoreAttribute)))
                    .Where(item =>
                    {
                        var getMethod = ReflectionUtils.GetGetterMethodInfo(item);
                        return !getMethod.IsStatic && getMethod.IsPublic;
                    });
            }

            private static IEnumerable<FieldInfo> GetFields(Type type)
            {
                return ReflectionUtils.GetFields(type)
                    .Where(item => !item.IsInitOnly && !item.IsStatic && item.IsPublic &&
#if !NET_2_0
                        !item.IsDefined(typeof (IgnoreDataMemberAttribute)) &&
#endif
                        !item.IsDefined(typeof (XmlIgnoreAttribute)));
            }

            private string GetName(MemberInfo member)
            {
                var protocolAttr = member.GetCustomAttribute<SDataProtocolPropertyAttribute>();
                if (protocolAttr != null)
                {
                    var name = protocolAttr.Value != null ? protocolAttr.Value.ToString() : member.Name;
                    if (char.IsUpper(name[0]))
                    {
                        name = char.ToLowerInvariant(name[0]) + name.Substring(1);
                    }
                    return "$" + name;
                }

                return _namingScheme.GetName(member);
            }
        }

        #endregion

        #region Nested type: XmlMetadataSurrogate

        private class XmlMetadataSurrogate
        {
            public object Value { get; set; }
            public string XmlLocalName { get; set; }
            public string XmlNamespace { get; set; }
            public bool XmlIsFlat { get; set; }
        }

        #endregion
    }
}