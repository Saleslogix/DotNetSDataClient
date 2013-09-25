// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;

namespace Saleslogix.SData.Client.Linq
{
    public class SDataProtocolVariable
    {
        public static readonly SDataProtocolVariable<Guid> Uuid = new SDataProtocolVariable<Guid>("uuid");
        public static readonly SDataProtocolVariable<string> Key = new SDataProtocolVariable<string>("key");
        public static readonly SDataProtocolVariable<DateTimeOffset> Published = new SDataProtocolVariable<DateTimeOffset>("published");
        public static readonly SDataProtocolVariable<DateTimeOffset> Updated = new SDataProtocolVariable<DateTimeOffset>("updated");
        public static readonly SDataProtocolVariable<string> Title = new SDataProtocolVariable<string>("title");

        private readonly string _name;

        public SDataProtocolVariable(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return "$" + _name;
        }
    }

    public class SDataProtocolVariable<T> : SDataProtocolVariable
    {
        public SDataProtocolVariable(string name)
            : base(name)
        {
        }

// ReSharper disable UnusedParameter.Global
        public static implicit operator SDataProtocolVariable<T>(T value)
// ReSharper restore UnusedParameter.Global
        {
            throw new NotSupportedException();
        }

// ReSharper disable UnusedParameter.Global
        public static implicit operator T(SDataProtocolVariable<T> value)
// ReSharper restore UnusedParameter.Global
        {
            throw new NotSupportedException();
        }
    }
}