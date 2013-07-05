using System.Linq;
using Sage.SData.Client.Atom;
using Sage.SData.Client.Common;
using Sage.SData.Client.Framework;

namespace Sage.SData.Client.Extensions
{
    /// <summary>
    /// Helper class for accessing atom feed and entry SDataSyncExtensions
    /// </summary>
    public static class SDataSyncExtensionHelper
    {
        /// <summary>
        /// Extension method to retrieve SData sync mode
        /// </summary>
        public static SyncMode? GetSDataSyncMode(this AtomFeed feed)
        {
            var context = GetContext(feed, false);
            return context != null ? context.SyncMode : null;
        }

        /// <summary>
        /// Extension method to retrieve SData sync digest
        /// </summary>
        public static Digest GetSDataSyncDigest(this AtomFeed feed)
        {
            var context = GetContext(feed, false);
            return context != null ? context.Digest : null;
        }

        /// <summary>
        /// Extension method to retrieve SData sync state
        /// </summary>
        public static SyncState GetSDataSyncState(this AtomEntry entry)
        {
            var context = GetContext(entry, false);
            return context != null ? context.SyncState : null;
        }

        private static SDataSyncExtensionContext GetContext(IExtensibleSyndicationObject entry, bool createIfMissing)
        {
            Guard.ArgumentNotNull(entry, "entry");
            var extension = entry.Extensions.OfType<SDataSyncExtension>().FirstOrDefault();

            if (extension == null)
            {
                if (!createIfMissing)
                {
                    return null;
                }

                extension = new SDataSyncExtension();
                entry.AddExtension(extension);
            }

            return extension.Context;
        }
    }
}