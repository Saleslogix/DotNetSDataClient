// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System.IO;

namespace Saleslogix.SData.Client.Content
{
    public interface IContentHandler
    {
        object ReadFrom(Stream stream);
        void WriteTo(object obj, Stream stream, INamingScheme namingScheme = null);
    }
}