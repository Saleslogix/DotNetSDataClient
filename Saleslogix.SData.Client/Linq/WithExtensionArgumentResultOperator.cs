// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace Saleslogix.SData.Client.Linq
{
    internal class WithExtensionArgResultOperator : ResultOperatorBase
    {
        private readonly string _name;
        private readonly string _value;

        public WithExtensionArgResultOperator(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Value
        {
            get { return _value; }
        }

        public override IStreamedData ExecuteInMemory(IStreamedData input)
        {
            return input;
        }

        public override IStreamedDataInfo GetOutputDataInfo(IStreamedDataInfo inputInfo)
        {
            return inputInfo;
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new WithExtensionArgResultOperator(_name, _value);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        public override string ToString()
        {
            return string.Format("WithExtensionArg({0}={1})", _name, _value);
        }
    }
}