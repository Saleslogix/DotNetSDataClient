using System.Linq;

namespace Saleslogix.SData.Client.Linq
{
    public static class SDataClientExtensions
    {
        public static IQueryable<T> Query<T>(this ISDataClient client, string path = null)
        {
            return new SDataQueryable<T>(client, path, client.NamingScheme ?? NamingScheme.Default);
        }
    }
}