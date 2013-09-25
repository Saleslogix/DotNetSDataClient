// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace Saleslogix.SData.Client.Linq
{
    internal class SDataQueryable<T> : QueryableBase<T>
    {
        public SDataQueryable(ISDataClient client, string path, INamingScheme namingScheme)
            : base(QueryParser.CreateDefault(), new SDataQueryExecutor(client, path, namingScheme))
        {
        }

        public SDataQueryable(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}