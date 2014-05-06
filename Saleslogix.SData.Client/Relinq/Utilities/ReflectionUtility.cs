// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Linq.Utilities
{
  internal static class ReflectionUtility
  {
    public static MethodInfo GetMethod<T> (Expression<Func<T>> wrappedCall)
    {
      ArgumentUtility.CheckNotNull ("wrappedCall", wrappedCall);

      switch (wrappedCall.Body.NodeType)
      {
        case ExpressionType.Call:
          return ((MethodCallExpression) wrappedCall.Body).Method;
        case ExpressionType.MemberAccess:
          var memberExpression = (MemberExpression) wrappedCall.Body;
          var property = memberExpression.Member as PropertyInfo;
#if NETFX_CORE
          var method = property != null ? property.GetMethod : null;
#else
          var method = property != null ? property.GetGetMethod() : null;
#endif
          if (method != null)
            return method;
          break;
      }

      throw new ArgumentException (string.Format ("Cannot extract a method from the given expression '{0}'.", wrappedCall.Body), "wrappedCall");
    }

    public static MethodInfo GetRuntimeMethodChecked (this Type type, string methodName, Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);

      var methodInfo = type.GetRuntimeMethod (methodName, parameterTypes);
      Assertion.IsNotNull (
          methodInfo,
          "Method '{0} ({1})' was not found on type '{2}'",
          methodName,
          string.Join (", ", parameterTypes.Select (t => t.Name).ToArray()),
          type.FullName);

      return methodInfo;
    }

    public static Type GetMemberReturnType (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);

      var propertyInfo = member as PropertyInfo;
      if (propertyInfo != null)
        return propertyInfo.PropertyType;

      var fieldInfo = member as FieldInfo;
      if (fieldInfo != null)
        return fieldInfo.FieldType;

      var methodInfo = member as MethodInfo;
      if (methodInfo != null)
        return methodInfo.ReturnType;

      throw new ArgumentException ("Argument must be FieldInfo, PropertyInfo, or MethodInfo.", "member");
    }

    public static void CheckTypeIsClosedGenericIEnumerable (Type enumerableType, string argumentName)
    {
      ArgumentUtility.CheckNotNull ("enumerableType", enumerableType);
      ArgumentUtility.CheckNotNullOrEmpty ("argumentName", argumentName);

      Type itemType = TryGetItemTypeOfClosedGenericIEnumerable (enumerableType);
      if (itemType == null)
      {
        var message = string.Format ("Expected a closed generic type implementing IEnumerable<T>, but found '{0}'.", enumerableType);
        throw new ArgumentException (message, argumentName);
      }
    }

    public static Type GetItemTypeOfClosedGenericIEnumerable (Type enumerableType, string argumentName)
    {
      ArgumentUtility.CheckNotNull ("enumerableType", enumerableType);
      ArgumentUtility.CheckNotNullOrEmpty ("argumentName", argumentName);

      Type itemType = TryGetItemTypeOfClosedGenericIEnumerable (enumerableType);
      if (itemType == null)
      {
        var message = string.Format ("Expected a closed generic type implementing IEnumerable<T>, but found '{0}'.", enumerableType);
        throw new ArgumentException (message, argumentName);
      }

      return itemType;
    }

    public static Type TryGetItemTypeOfClosedGenericIEnumerable (Type possibleEnumerableType)
    {
      ArgumentUtility.CheckNotNull ("possibleEnumerableType", possibleEnumerableType);

      var possibleEnumerableTypeInfo = possibleEnumerableType.GetTypeInfo();

      if (possibleEnumerableTypeInfo.IsArray)
        return possibleEnumerableTypeInfo.GetElementType();

      if (!IsIEnumerable (possibleEnumerableTypeInfo))
        return null;

      if (possibleEnumerableTypeInfo.IsGenericTypeDefinition)
        return null;

      if (possibleEnumerableTypeInfo.IsGenericType && possibleEnumerableType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
#if NETFX_CORE
        return possibleEnumerableTypeInfo.GenericTypeArguments[0];
#else
        return possibleEnumerableTypeInfo.GetGenericArguments()[0];
#endif

      var implementedEnumerableInterface =
#if NETFX_CORE
          possibleEnumerableTypeInfo.ImplementedInterfaces.Select (t => t.GetTypeInfo()).FirstOrDefault (IsGenericIEnumerable);
#else
          possibleEnumerableTypeInfo.GetInterfaces().Select (t => t.GetTypeInfo()).FirstOrDefault (IsGenericIEnumerable);
#endif

      if (implementedEnumerableInterface == null)
        return null;

      Assertion.DebugAssert (implementedEnumerableInterface.IsGenericType);
#if NETFX_CORE
      return implementedEnumerableInterface.GenericTypeArguments[0];
#else
      return implementedEnumerableInterface.GetGenericArguments()[0];
#endif
    }
      
#if NETFX_CORE
    private static bool IsIEnumerable (TypeInfo type)
#else
    private static bool IsIEnumerable (Type type)
#endif
    {
      return typeof (IEnumerable).GetTypeInfo().IsAssignableFrom (type);
    }
      
#if NETFX_CORE
    private static bool IsGenericIEnumerable (TypeInfo enumerableType)
#else
    private static bool IsGenericIEnumerable (Type enumerableType)
#endif
    {
      return IsIEnumerable (enumerableType)
             && enumerableType.IsGenericType
             && enumerableType.GetGenericTypeDefinition() == typeof (IEnumerable<>);
    }
  }
}
