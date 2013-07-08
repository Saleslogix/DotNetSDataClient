using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using SimpleJson;
using SimpleJson.Reflection;

#if !NET_2_0
using System.Runtime.Serialization;
#endif

namespace Sage.SData.Client.Content
{
    internal static class ContentHelper
    {
        public static object Serialize(object value, INamingScheme namingScheme = null)
        {
            if (namingScheme == null)
            {
                namingScheme = NamingScheme.Default;
            }

            object result;
            new Serializer(namingScheme).TrySerializeNonPrimitiveObject(value, out result);
            return result;
        }

        public static T Deserialize<T>(object value, INamingScheme namingScheme = null)
        {
            if (namingScheme == null)
            {
                namingScheme = NamingScheme.Default;
            }

            return (T) new Serializer(namingScheme).DeserializeObject(value, typeof (T));
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
                if (input == null || input is string || input.GetType().GetTypeInfo().IsValueType)
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
                    if (item.Key.StartsWith("_$_"))
                    {
                        if (info == null)
                        {
                            info = new SDataProtocolInfo();
                        }
                        var prop = (SDataProtocolProperty) Enum.Parse(typeof (SDataProtocolProperty), item.Key.Substring(3), false);
                        info.SetValue(prop, item.Value);
                        continue;
                    }

                    var value = item.Value;
                    var surrogate = value as XmlMetadataSurrogate;
                    if (surrogate != null)
                    {
                        value = surrogate.Value;
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
                if (value == null || value is string || value.GetType().GetTypeInfo().IsValueType)
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
                            dict["_$_" + protocolProp] = prot.Info.GetValue(protocolProp);
                        }
                    }
                }

                // SimpleJson only recognizes IList<object>
                if (!(value is IList<object>))
                {
                    var items = AsCollection(value);
                    if (items != null)
                    {
                        value = items.ToList();
                    }
                }

                var result = base.DeserializeObject(value, type);

                if (prot != null)
                {
                    var resultProt = result as ISDataProtocolAware;
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
                    result[MapMemberName(propertyInfo)] = GetGetter(ReflectionUtils.GetGetMethod(propertyInfo), propertyInfo, propertyInfo.PropertyType);
                }
                foreach (var fieldInfo in GetFields(type))
                {
                    result[MapMemberName(fieldInfo)] = GetGetter(ReflectionUtils.GetGetMethod(fieldInfo), fieldInfo, fieldInfo.FieldType);
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
                    result[MapMemberName(propertyInfo)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType, setter);
                }
                foreach (var fieldInfo in GetFields(type))
                {
                    var setter = ReflectionUtils.GetSetMethod(fieldInfo);
                    result[MapMemberName(fieldInfo)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, setter);
                }

                return result;
            }

            private string MapMemberName(MemberInfo memberInfo)
            {
                var protoAttr = SDataProtocolPropertyAttribute.GetProperty(memberInfo);
                if (protoAttr != null)
                {
                    return "_$_" + protoAttr.Value;
                }

                return _namingScheme.GetName(memberInfo);
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