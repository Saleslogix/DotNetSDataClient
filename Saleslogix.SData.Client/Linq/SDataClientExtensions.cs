// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System.Linq;
using Saleslogix.SData.Client.Utilities;

namespace Saleslogix.SData.Client.Linq
{
    public static class SDataClientExtensions
    {
        public static IQueryable<SDataResource> Query(this ISDataClient client, string path)
        {
            Guard.ArgumentNotNull(client, "client");
            return new SDataQueryable<SDataResource>(client, path, client.NamingScheme ?? NamingScheme.Default);
        }

        public static IQueryable<T> Query<T>(this ISDataClient client, string path = null)
        {
            Guard.ArgumentNotNull(client, "client");
            return new SDataQueryable<T>(client, path, client.NamingScheme ?? NamingScheme.Default);
        }
    }
}