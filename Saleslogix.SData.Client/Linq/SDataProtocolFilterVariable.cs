using System;

namespace Saleslogix.SData.Client.Linq
{
    public class SDataProtocolFilterVariable
    {
        public static readonly SDataProtocolFilterVariable<Guid> Uuid = new SDataProtocolFilterVariable<Guid>("uuid");
        public static readonly SDataProtocolFilterVariable<string> Key = new SDataProtocolFilterVariable<string>("key");
        public static readonly SDataProtocolFilterVariable<DateTime> Published = new SDataProtocolFilterVariable<DateTime>("published");
        public static readonly SDataProtocolFilterVariable<DateTime> Updated = new SDataProtocolFilterVariable<DateTime>("updated");
        public static readonly SDataProtocolFilterVariable<string> Title = new SDataProtocolFilterVariable<string>("title");

        private readonly string _name;

        public SDataProtocolFilterVariable(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return "$" + _name;
        }
    }

    public class SDataProtocolFilterVariable<T> : SDataProtocolFilterVariable
    {
        public SDataProtocolFilterVariable(string name)
            : base(name)
        {
        }

// ReSharper disable UnusedParameter.Global
        public static implicit operator SDataProtocolFilterVariable<T>(T value)
// ReSharper restore UnusedParameter.Global
        {
            throw new NotSupportedException();
        }

// ReSharper disable UnusedParameter.Global
        public static implicit operator T(SDataProtocolFilterVariable<T> value)
// ReSharper restore UnusedParameter.Global
        {
            throw new NotSupportedException();
        }
    }
}