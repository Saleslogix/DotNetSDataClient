// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Reflection;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SDataResourceAttribute : Attribute
    {
        public static string GetPath(Type type)
        {
            var attr = type.GetTypeInfo().GetCustomAttribute<SDataResourceAttribute>();
            return attr != null ? attr.Path : null;
        }

        private readonly string _path;

        public SDataResourceAttribute(string path)
        {
            _path = path;
        }

        public string Path
        {
            get { return _path; }
        }
    }
}