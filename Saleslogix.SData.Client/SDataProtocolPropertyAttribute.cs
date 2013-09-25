// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Reflection;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SDataProtocolPropertyAttribute : Attribute
    {
        public static SDataProtocolProperty? GetProperty(MemberInfo info)
        {
            var attr = info.GetCustomAttribute<SDataProtocolPropertyAttribute>();
            return attr != null ? attr.Value : (SDataProtocolProperty?) null;
        }

        private readonly SDataProtocolProperty _value;

        public SDataProtocolPropertyAttribute(SDataProtocolProperty value)
        {
            _value = value;
        }

        public SDataProtocolProperty Value
        {
            get { return _value; }
        }
    }
}