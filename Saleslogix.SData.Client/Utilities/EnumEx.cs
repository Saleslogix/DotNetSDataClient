// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace Saleslogix.SData.Client.Utilities
{
    internal static class EnumEx
    {
        public static string Format(object item)
        {
            Guard.ArgumentNotNull(item, "item");

            return GetEnumData(item.GetType()).Format(item);
        }

        public static string Format<T>(T item)
        {
            Guard.ArgumentNotNull(item, "item");

            return EnumData<T>.Format(item);
        }

        public static object Parse(Type type, string value, bool ignoreCase = false)
        {
            Guard.ArgumentNotNull(type, "type");

            return GetEnumData(type).Parse(value, ignoreCase);
        }

        public static T Parse<T>(string value, bool ignoreCase = false)
        {
            return EnumData<T>.Parse(value, ignoreCase);
        }

        private static IEnumData GetEnumData(Type type)
        {
            return (IEnumData) typeof (EnumData<>).MakeGenericType(new[] {type}).GetField("Instance").GetValue(null);
        }

        #region Nested type: IEnumData

        private interface IEnumData
        {
            string Format(object item);
            object Parse(string value, bool ignoreCase);
        }

        #endregion

        #region Nested type: EnumData

        private class EnumData<T> : IEnumData
        {
#pragma warning disable 169
            public static readonly EnumData<T> Instance = new EnumData<T>();
#pragma warning restore 169
            private static readonly IDictionary<T, string> _xmlNames = new Dictionary<T, string>();

            static EnumData()
            {
                foreach (var field in typeof (T).GetFields())
                {
                    if (!field.IsStatic || field.IsDefined(typeof (ObsoleteAttribute)))
                    {
                        continue;
                    }

                    var value = (T) field.GetValue(null);

                    var enumAttr = field.GetCustomAttribute<XmlEnumAttribute>();
                    _xmlNames[value] = (enumAttr != null) ? enumAttr.Name : value.ToString();
                }
            }

            public static string Format(T item)
            {
                string value;

                if (!_xmlNames.TryGetValue(item, out value))
                {
                    value = item.ToString();
                }

                return value;
            }

            public static T Parse(string value, bool ignoreCase)
            {
                foreach (var pair in _xmlNames)
                {
                    if (string.Equals(pair.Value, value, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        return pair.Key;
                    }
                }

                return (T) Enum.Parse(typeof (T), value, ignoreCase);
            }

            #region IEnumData Members

            string IEnumData.Format(object item)
            {
                return Format((T) item);
            }

            object IEnumData.Parse(string value, bool ignoreCase)
            {
                return Parse(value, ignoreCase);
            }

            #endregion
        }

        #endregion
    }
}