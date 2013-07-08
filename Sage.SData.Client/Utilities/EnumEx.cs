using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sage.SData.Client.Utilities
{
    internal static class EnumEx
    {
        public static string Format(object item)
        {
            Guard.ArgumentNotNull(item, "item");

            return GetEnumData(item.GetType()).Format(item);
        }

        public static object Parse(Type type, string value)
        {
            Guard.ArgumentNotNull(type, "type");

            return GetEnumData(type).Parse(value);
        }

        public static T Parse<T>(string value)
        {
            return EnumData<T>.Parse(value, false);
        }

        public static T Parse<T>(string value, bool ignoreCase)
        {
            return EnumData<T>.Parse(value, ignoreCase);
        }

        public static T Parse<T>(string value, bool ignoreCase, T defaultValue)
        {
            return EnumData<T>.Parse(value, ignoreCase, defaultValue);
        }

        private static IEnumData GetEnumData(Type type)
        {
            return (IEnumData) typeof (EnumData<>).MakeGenericType(new[] {type}).GetField("Instance").GetValue(null);
        }

        #region Nested type: IEnumData

        private interface IEnumData
        {
            string Format(object item);
            object Parse(string value);
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
                    if (!field.IsStatic || Attribute.IsDefined(field, typeof (ObsoleteAttribute)))
                    {
                        continue;
                    }

                    var value = (T) field.GetValue(null);

                    var enumAttr = (XmlEnumAttribute) Attribute.GetCustomAttribute(field, typeof (XmlEnumAttribute));
                    _xmlNames[value] = (enumAttr != null) ? enumAttr.Name : value.ToString();
                }
            }

            public static T Parse(string value, bool ignoreCase)
            {
                foreach (var pair in _xmlNames)
                {
                    if (string.Equals(pair.Value, value, StringComparison.OrdinalIgnoreCase))
                    {
                        return pair.Key;
                    }
                }

                return (T) Enum.Parse(typeof (T), value, ignoreCase);
            }

            public static T Parse(string value, bool ignoreCase, T defaultValue)
            {
                foreach (var pair in _xmlNames)
                {
                    if (string.Equals(pair.Value, value, StringComparison.OrdinalIgnoreCase))
                    {
                        return pair.Key;
                    }
                }

                try
                {
                    return (T) Enum.Parse(typeof (T), value, ignoreCase);
                }
                catch (FormatException)
                {
                    return defaultValue;
                }
            }

            #region IEnumData Members

            string IEnumData.Format(object item)
            {
                string value;

                if (!_xmlNames.TryGetValue((T) item, out value))
                {
                    value = item.ToString();
                }

                return value;
            }

            object IEnumData.Parse(string value)
            {
                foreach (var pair in _xmlNames)
                {
                    if (pair.Value == value)
                    {
                        return pair.Key;
                    }
                }

                return Enum.Parse(typeof (T), value, false);
            }

            #endregion
        }

        #endregion
    }
}