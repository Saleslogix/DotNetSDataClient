// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Clauses.StreamedData;

namespace Saleslogix.SData.Client.Linq
{
    internal class ElementAtResultOperator : ChoiceResultOperatorBase
    {
        private readonly int _index;

        public ElementAtResultOperator(int index, bool returnDefaultWhenEmpty)
            : base(returnDefaultWhenEmpty)
        {
            _index = index;
        }

        public int Index
        {
            get { return _index; }
        }

        public override StreamedValue ExecuteInMemory<T>(StreamedSequence input)
        {
            var sequence = input.GetTypedSequence<T>();
            var result = ReturnDefaultWhenEmpty ? sequence.ElementAtOrDefault(_index) : sequence.ElementAt(_index);
            return new StreamedValue(result, (StreamedValueInfo) GetOutputDataInfo(input.DataInfo));
        }

        public override ResultOperatorBase Clone(CloneContext cloneContext)
        {
            return new ElementAtResultOperator(_index, ReturnDefaultWhenEmpty);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        public override string ToString()
        {
            return string.Format(ReturnDefaultWhenEmpty ? "ElementAtOrDefault({0})" : "ElementAt({0})", _index);
        }
    }
}