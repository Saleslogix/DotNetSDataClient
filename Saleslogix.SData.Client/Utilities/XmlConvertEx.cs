// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace Saleslogix.SData.Client.Utilities
{
    internal static class XmlConvertEx
    {
        private static readonly IDictionary<Type, Func<string, object>> _methods = new Dictionary<Type, Func<string, object>>();

        static XmlConvertEx()
        {
            RegisterMethod(XmlConvert.ToBoolean);
            RegisterMethod(XmlConvert.ToChar);
            RegisterMethod(XmlConvert.ToSByte);
            RegisterMethod(XmlConvert.ToByte);
            RegisterMethod(XmlConvert.ToInt16);
            RegisterMethod(XmlConvert.ToUInt16);
            RegisterMethod(XmlConvert.ToInt32);
            RegisterMethod(XmlConvert.ToUInt32);
            RegisterMethod(XmlConvert.ToInt64);
            RegisterMethod(XmlConvert.ToUInt64);
            RegisterMethod(XmlConvert.ToSingle);
            RegisterMethod(XmlConvert.ToDouble);
            RegisterMethod(XmlConvert.ToDecimal);
#if !PCL && !NETFX_CORE
            RegisterMethod(value => XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.RoundtripKind));
#endif
            RegisterMethod(XmlConvert.ToDateTimeOffset);
            RegisterMethod(XmlConvert.ToTimeSpan);
            RegisterMethod(XmlConvert.ToGuid);
        }

        private static void RegisterMethod<T>(Func<string, T> fromString)
        {
            _methods.Add(typeof (T), str => fromString(str));
        }

        public static T FromString<T>(string value)
        {
            if (value == null)
            {
                return default(T);
            }

            var type = typeof (T);
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.GetTypeInfo().IsEnum)
            {
                return (T) EnumEx.Parse(type, value);
            }

            Func<string, object> method;
            return (T) (_methods.TryGetValue(type, out method)
                            ? method(value)
                            : Convert.ChangeType(value, type, CultureInfo.InvariantCulture));
        }
    }
}