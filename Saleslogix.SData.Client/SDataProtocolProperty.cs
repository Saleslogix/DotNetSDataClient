// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

namespace Saleslogix.SData.Client
{
    public enum SDataProtocolProperty
    {
        //ATOM: top level everything
        Id,
        Title,
        Updated,

        //OPENSEARCH: top level collections
        TotalResults,
        StartIndex,
        ItemsPerPage,

        //SDATA: everything
        Url,

        //SDATA: top level everything
        Diagnoses,
        Schema,
        Links,

        //SDATA: resources
        Key,
        Uuid,
        Lookup,
        Descriptor,

        //SDATA: top level resources
        HttpMethod,
        HttpStatus,
        HttpMessage,
        Location,
        ETag,
        IfMatch,

        //SDATA: nested collections
        DeleteMissing,

        //SDATA: nested collection resources
        IsDeleted,

        //SYNC: top level resources
        SyncState,

        //SYNC: top level collections
        SyncMode,
        SyncDigest
    }
}