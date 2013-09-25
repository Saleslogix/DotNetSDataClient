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
        private readonly IEnumerable<MemberInfo> _memberPath;

        public FetchResultOperator(Expression selector)
            : this(MemberPathBuilder.Build(selector))
        {
        }

        private FetchResultOperator(IEnumerable<MemberInfo> memberPath)
        {
            _memberPath = memberPath;
        }

        public IEnumerable<MemberInfo> MemberPath
        {
            get { return _memberPath; }
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
            return new FetchResultOperator(_memberPath);
        }

        public override void TransformExpressions(Func<Expression, Expression> transformation)
        {
        }

        public override string ToString()
        {
            return string.Format("Fetch({0})", string.Join("/", _memberPath.Select(member => member.Name).ToArray()));
        }
    }
}