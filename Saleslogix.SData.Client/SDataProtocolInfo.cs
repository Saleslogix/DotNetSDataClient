// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Net;
using Saleslogix.SData.Client.Framework;

namespace Saleslogix.SData.Client
{
    [Serializable]
    public class SDataProtocolInfo
    {
        //ATOM: top level everything
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset? Updated { get; set; }

        //OPENSEARCH: top level collections
        public int? TotalResults { get; set; }
        public int? StartIndex { get; set; }
        public int? ItemsPerPage { get; set; }

        //SDATA: everything
        public Uri Url { get; set; }

        //SDATA: top level everything
        public Diagnoses Diagnoses { get; set; }
        public string Schema { get; set; }

        //SDATA: resources
        public string Key { get; set; }
        public Guid? Uuid { get; set; }
        public string Lookup { get; set; }
        public string Descriptor { get; set; }

        //SDATA: top level resources
        public HttpMethod? HttpMethod { get; set; }
        public HttpStatusCode? HttpStatus { get; set; }
        public string HttpMessage { get; set; }
        public string Location { get; set; }
        public string ETag { get; set; }
        public string IfMatch { get; set; }

        //SDATA: nested collections
        public bool? DeleteMissing { get; set; }

        //SDATA: nested collection resources
        public bool? IsDeleted { get; set; }

        //SYNC: top level resources
        public SyncState SyncState { get; set; }

        //SYNC: top level collections
        public SyncMode? SyncMode { get; set; }
        public Digest SyncDigest { get; set; }

        //XML: everything
        public string XmlLocalName { get; set; }
        public string XmlNamespace { get; set; }

        //XML: collections
        public bool XmlIsFlat { get; set; }

        //JSON: collections
        public bool JsonIsSimpleArray { get; set; }

        public object GetValue(SDataProtocolProperty prop)
        {
            switch (prop)
            {
                case SDataProtocolProperty.Id:
                    return Id;
                case SDataProtocolProperty.Title:
                    return Title;
                case SDataProtocolProperty.Updated:
                    return Updated;
                case SDataProtocolProperty.TotalResults:
                    return TotalResults;
                case SDataProtocolProperty.StartIndex:
                    return StartIndex;
                case SDataProtocolProperty.ItemsPerPage:
                    return ItemsPerPage;
                case SDataProtocolProperty.Url:
                    return Url;
                case SDataProtocolProperty.Diagnoses:
                    return Diagnoses;
                case SDataProtocolProperty.Schema:
                    return Schema;
                case SDataProtocolProperty.Key:
                    return Key;
                case SDataProtocolProperty.Uuid:
                    return Uuid;
                case SDataProtocolProperty.Lookup:
                    return Lookup;
                case SDataProtocolProperty.Descriptor:
                    return Descriptor;
                case SDataProtocolProperty.HttpMethod:
                    return HttpMethod;
                case SDataProtocolProperty.HttpStatus:
                    return HttpStatus;
                case SDataProtocolProperty.HttpMessage:
                    return HttpMessage;
                case SDataProtocolProperty.Location:
                    return Location;
                case SDataProtocolProperty.ETag:
                    return ETag;
                case SDataProtocolProperty.IfMatch:
                    return IfMatch;
                case SDataProtocolProperty.DeleteMissing:
                    return DeleteMissing;
                case SDataProtocolProperty.IsDeleted:
                    return IsDeleted;
                case SDataProtocolProperty.SyncState:
                    return SyncState;
                case SDataProtocolProperty.SyncMode:
                    return SyncMode;
                case SDataProtocolProperty.SyncDigest:
                    return SyncDigest;
                default:
                    throw new ArgumentOutOfRangeException("prop");
            }
        }

        public void SetValue(SDataProtocolProperty prop, object value)
        {
            switch (prop)
            {
                case SDataProtocolProperty.Id:
                    Id = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.Title:
                    Title = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.Updated:
                    Updated = (value as DateTimeOffset?) ?? (value != null ? Convert.ToDateTime(value) : (DateTimeOffset?) null);
                    break;
                case SDataProtocolProperty.TotalResults:
                    TotalResults = value != null ? Convert.ToInt32(value) : (int?) null;
                    break;
                case SDataProtocolProperty.StartIndex:
                    StartIndex = value != null ? Convert.ToInt32(value) : (int?) null;
                    break;
                case SDataProtocolProperty.ItemsPerPage:
                    ItemsPerPage = value != null ? Convert.ToInt32(value) : (int?) null;
                    break;
                case SDataProtocolProperty.Url:
                    Url = (Uri) value;
                    break;
                case SDataProtocolProperty.Diagnoses:
                    Diagnoses = (Diagnoses) value;
                    break;
                case SDataProtocolProperty.Schema:
                    Schema = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.Key:
                    Key = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.Uuid:
                    Uuid = (Guid?) value;
                    break;
                case SDataProtocolProperty.Lookup:
                    Lookup = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.Descriptor:
                    Descriptor = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.HttpMethod:
                    HttpMethod = (HttpMethod?) value;
                    break;
                case SDataProtocolProperty.HttpStatus:
                    HttpStatus = (HttpStatusCode?) value;
                    break;
                case SDataProtocolProperty.HttpMessage:
                    HttpMessage = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.Location:
                    Location = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.ETag:
                    ETag = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.IfMatch:
                    IfMatch = Convert.ToString(value);
                    break;
                case SDataProtocolProperty.DeleteMissing:
                    DeleteMissing = value != null ? Convert.ToBoolean(value) : (bool?) null;
                    break;
                case SDataProtocolProperty.IsDeleted:
                    IsDeleted = value != null ? Convert.ToBoolean(value) : (bool?) null;
                    break;
                case SDataProtocolProperty.SyncState:
                    SyncState = (SyncState) value;
                    break;
                case SDataProtocolProperty.SyncMode:
                    SyncMode = (SyncMode?) value;
                    break;
                case SDataProtocolProperty.SyncDigest:
                    SyncDigest = (Digest) value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("prop");
            }
        }
    }
}