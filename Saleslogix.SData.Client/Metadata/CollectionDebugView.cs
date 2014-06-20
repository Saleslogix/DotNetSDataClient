// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Metadata
{
    internal sealed class CollectionDebugView<T>
    {
        private readonly ICollection<T> _collection;

        public CollectionDebugView(ICollection<T> collection)
        {
            Guard.ArgumentNotNull(collection, "collection");
            _collection = collection;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get { return _collection.ToArray(); }
        }
    }
}