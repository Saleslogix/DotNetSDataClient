﻿// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

#if PCL || NETFX_CORE
namespace System.ComponentModel
{
    internal class BrowsableAttribute : Attribute
    {
// ReSharper disable UnusedParameter.Local
        public BrowsableAttribute(bool browsable)
// ReSharper restore UnusedParameter.Local
        {
        }
    }
}
#endif

#if PCL || NETFX_CORE || SILVERLIGHT
namespace System
{
    internal interface ICloneable
    {
// ReSharper disable UnusedMember.Global
        object Clone();
// ReSharper restore UnusedMember.Global
    }

    internal class SerializableAttribute : Attribute
    {
    }

    namespace Net
    {
        using Saleslogix.SData.Client.Utilities;

        internal static class WebHeaderCollectionExtensions
        {
            public static void Add(this WebHeaderCollection headers, string header)
            {
                Guard.ArgumentNotNull(header, "header");
                var index = header.IndexOf(':');
                if (index < 0)
                {
                    throw new ArgumentException("Colon missing in web header", "header");
                }
                var name = header.Substring(0, index);
                var value = header.Substring(index + 1);
                headers[name] = value;
            }
        }
    }

    namespace Runtime.Serialization
    {
        internal interface ISerializable
        {
        }
    }

    namespace Text
    {
        internal static class EncodingExtensions
        {
            public static string GetString(this Encoding encoding, byte[] bytes)
            {
                return encoding.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}
#endif

#if NET_2_0 || NET_3_5
namespace System.IO
{
    internal static class StreamExtensions
    {
        public static void CopyTo(this Stream source, Stream destination)
        {
            var buffer = new byte[0x1000];
            int num;
            while ((num = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                destination.Write(buffer, 0, num);
            }
        }
    }
}
#endif

#if NET_2_0 || NET_3_5
namespace System.Linq
{
    using Collections.Generic;
    using Saleslogix.SData.Client.Utilities;

    internal static class EnumerableExtensions
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> func)
        {
            Guard.ArgumentNotNull(first, "first");
            Guard.ArgumentNotNull(first, "second");
            Guard.ArgumentNotNull(first, "func");
            using (var ie1 = first.GetEnumerator())
            using (var ie2 = second.GetEnumerator())
            {
                while (ie1.MoveNext() && ie2.MoveNext())
                {
                    yield return func(ie1.Current, ie2.Current);
                }
            }
        }
    }
}
#endif

namespace System.Reflection
{
    using Collections.Generic;
    using Saleslogix.SData.Client.Utilities;

    internal static class IntrospectionExtensions
    {
        #if !NET48
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
        #endif

        public static Type AsType(this Type type)
        {
            return type;
        }
    }

    internal static class RuntimeReflectionExtensions
    {
        #if !NET48
        public static MethodInfo GetMethodInfo(this Delegate del)
        {
            Guard.ArgumentNotNull(del, "del");
            return del.Method;
        }

        public static IEnumerable<MethodInfo> GetRuntimeMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        }

        public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] parameters)
        {
            return type.GetMethod(name, parameters);
        }

        public static PropertyInfo GetRuntimeProperty(this Type type, string name)
        {
            return type.GetProperty(name);
        }

        public static FieldInfo GetRuntimeField(this Type type, string name)
        {
            return type.GetField(name);
        }
        #endif
    }

    #if !NET48
    internal static class CustomAttributeExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute
        {
            return (T) Attribute.GetCustomAttribute(element, typeof (T));
        }

        public static T GetCustomAttribute<T>(this ParameterInfo element) where T : Attribute
        {
            return (T) Attribute.GetCustomAttribute(element, typeof (T));
        }

        public static bool IsDefined(this MemberInfo element, Type attributeType)
        {
            return Attribute.IsDefined(element, attributeType);
        }
    }
    #endif
}

#if !NETFX_CORE && !SILVERLIGHT
namespace System.Reflection
{
    internal static class MethodInfoExtensions
    {
        public static Delegate CreateDelegate(this MethodInfo method, Type delegateType, object target)
        {
            return Delegate.CreateDelegate(delegateType, target, method);
        }
    }
}
#endif

#if NETFX_CORE
namespace System
{
    using Linq;
    using Reflection;

    internal static class TypeExtensions
    {
        public static Type[] GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }

        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }

        public static bool IsInstanceOfType(this Type type, object obj)
        {
            return obj != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
        }

        public static ConstructorInfo[] GetConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors.ToArray();
        }

        public static FieldInfo[] GetFields(this Type type)
        {
            return type.GetTypeInfo().DeclaredFields.ToArray();
        }

        public static MethodInfo[] GetMethods(this Type type)
        {
            return type.GetTypeInfo().DeclaredMethods.ToArray();
        }

        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            return type.GetTypeInfo().DeclaredConstructors.First(ctor => ctor.GetParameters().Select(parm => parm.ParameterType).SequenceEqual(types));
        }

        public static FieldInfo GetField(this Type type, string fieldName)
        {
            return type.GetRuntimeFields().First(field => field.Name == fieldName);
        }

        public static PropertyInfo GetProperty(this Type type, string propName)
        {
            return type.GetRuntimeProperties().First(prop => prop.Name == propName);
        }

        public static MethodInfo GetMethod(this Type type, string methodName, Type[] types = null)
        {
            return type.GetRuntimeMethods().First(method => method.Name == methodName && (types == null || method.GetParameters().Select(parm => parm.ParameterType).SequenceEqual(types)));
        }
    }

    namespace Reflection
    {
        internal static class AssemblyExtensions
        {
            public static Type[] GetTypes(this Assembly asm)
            {
                return asm.DefinedTypes.Select(type => type.AsType()).ToArray();
            }
        }

        internal static class PropertyInfoExtensions
        {
            public static MethodInfo GetGetMethod(this PropertyInfo prop, bool nonPublic = false)
            {
                var method =  prop.GetMethod;
                return nonPublic || method.IsPublic ? method : null;
            }
        }
    }
}
#endif