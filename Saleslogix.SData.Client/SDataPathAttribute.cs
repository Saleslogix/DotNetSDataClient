// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Reflection;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SDataPathAttribute : Attribute
    {
        public static string GetPath(Type type)
        {
            Guard.ArgumentNotNull(type, "type");
            var attr = type.GetTypeInfo().GetCustomAttribute<SDataPathAttribute>();
            return attr != null ? attr.Path : null;
        }

        public static string GetPath(MethodInfo method)
        {
            Guard.ArgumentNotNull(method, "method");
            var attr = method.GetCustomAttribute<SDataPathAttribute>();
            return attr != null ? attr.Path : null;
        }

        private readonly string _path;

        public SDataPathAttribute(string path)
        {
            _path = path;
        }

        public string Path
        {
            get { return _path; }
        }
    }
}