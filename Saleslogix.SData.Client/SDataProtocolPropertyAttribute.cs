// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;

namespace Saleslogix.SData.Client
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SDataProtocolPropertyAttribute : Attribute
    {
        private readonly SDataProtocolProperty? _value;

        public SDataProtocolPropertyAttribute()
        {
        }

        public SDataProtocolPropertyAttribute(SDataProtocolProperty value)
        {
            _value = value;
        }

        public SDataProtocolProperty? Value
        {
            get { return _value; }
        }
    }
}