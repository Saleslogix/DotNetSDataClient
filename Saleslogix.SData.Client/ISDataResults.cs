// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Net;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client
{
    public interface ISDataResults
    {
        HttpStatusCode StatusCode { get; }
        MediaType? ContentType { get; }
        string ETag { get; }
        string Location { get; }
        DateTime? Expires { get; }
        DateTime? RetryAfter { get; }
        IDictionary<string, string> Form { get; }
        IList<AttachedFile> Files { get; }
    }

    public interface ISDataResults<out T> : ISDataResults
    {
        T Content { get; }
    }
}