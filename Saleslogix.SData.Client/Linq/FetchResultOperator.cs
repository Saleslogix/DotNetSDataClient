// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.StreamedData;

namespace Saleslogix.SData.Client.Linq
{
    internal class FetchResultOperator : ResultOperatorBase
    {
        private readonly IEnumerable<object> _propertyPath;

        public FetchResultOperator(Expression selector)
            : this(PropertyPathBuilder.Build(selector))
        {
        }

        private FetchResultOperator(IEnumerable<object> propertyPath)
        {
            _propertyPath = propertyPath;
        }

        public IEnumerable<object> PropertyPath
        {
            get { return _propertyPath; }
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
            return new FetchResultOperator(_propertyPath);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        public override string ToString()
        {
            return string.Format("Fetch({0})",
                string.Join("/", _propertyPath.Select(prop =>
                {
                    var member = prop as MemberInfo;
                    return member != null ? member.Name : prop.ToString();
                }).ToArray()));
        }
    }
}