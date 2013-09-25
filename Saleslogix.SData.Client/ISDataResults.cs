using System.Collections.Generic;
using System.Net;
using Sage.SData.Client.Framework;

namespace Sage.SData.Client
{
    public interface ISDataResults
    {
        HttpStatusCode StatusCode { get; }
        MediaType? ContentType { get; }
        string ETag { get; }
        string Location { get; }
        IList<AttachedFile> Files { get; }
    }

    public interface ISDataResults<out T> : ISDataResults
    {
        T Content { get; }
    }
}