﻿using System;
using System.Reflection;

namespace Sage.SData.Client
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