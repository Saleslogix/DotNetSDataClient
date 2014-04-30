// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace Saleslogix.SData.Client.Linq
{
    internal class SDataQueryable<T> : QueryableBase<T>
    {
        private SDataQueryable(QueryParser queryParser, ISDataClient client, string path, INamingScheme namingScheme)
            : base(queryParser, new SDataQueryExecutor(client, path, queryParser.NodeTypeProvider, namingScheme))
        {
        }

        public SDataQueryable(ISDataClient client, string path, INamingScheme namingScheme)
            : this(QueryParser.CreateDefault(), client, path, namingScheme)
        {
        }

        public SDataQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}