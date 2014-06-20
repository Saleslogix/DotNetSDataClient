// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace Saleslogix.SData.Client.Linq
{
    internal class WithPrecedenceResultOperator : ResultOperatorBase
    {
        private readonly int _precedence;

        public WithPrecedenceResultOperator(int precedence)
        {
            _precedence = precedence;
        }

        public int Precedence
        {
            get { return _precedence; }
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
            return new WithPrecedenceResultOperator(_precedence);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        public override string ToString()
        {
            return string.Format("WithPrecedence({0})", _precedence);
        }
    }
}