// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using Saleslogix.SData.Client.Framework;

#if !PCL && !NETFX_CORE && !SILVERLIGHT
using System.ComponentModel;
#endif

namespace Saleslogix.SData.Client
{
    public static class SDataCollection
    {
        public static SDataCollection<T> Create<T>(params T[] items)
        {
            return new SDataCollection<T>(items);
        }

        public static SDataCollection<T> Create<T>(bool jsonIsSimpleArray, params T[] items)
        {
            return new SDataCollection<T>(items)
                {
                    JsonIsSimpleArray = jsonIsSimpleArray
                };
        }

        public static SDataCollection<T> Create<T>(string xmlLocalName, params T[] items)
        {
            return new SDataCollection<T>(items)
                {
                    XmlLocalName = xmlLocalName
                };
        }

        public static SDataCollection<T> Create<T>(string xmlLocalName, string xmlNamespace, params T[] items)
        {
            return new SDataCollection<T>(items)
                {
                    XmlLocalName = xmlLocalName,
                    XmlNamespace = xmlNamespace
                };
        }
    }

    [Serializable]
#if !PCL && !NETFX_CORE && !SILVERLIGHT
    [TypeConverter(typeof (SDataCollectionTypeConverter))]
#endif
    public class SDataCollection<T> : List<T>, ISDataProtocolAware
    {
        private SDataProtocolInfo _info = new SDataProtocolInfo();

        public SDataCollection()
        {
        }

        public SDataCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public SDataCollection(int capacity)
            : base(capacity)
        {
        }

        public SDataCollection(string xmlLocalName, string xmlNamespace = null)
        {
            XmlLocalName = xmlLocalName;
            XmlNamespace = xmlNamespace;
        }

        SDataProtocolInfo ISDataProtocolAware.Info
        {
            get { return _info; }
            set { _info = value; }
        }

        [SDataProtocolProperty]
        public string Id
        {
            get { return _info.Id; }
            set { _info.Id = value; }
        }

        [SDataProtocolProperty]
        public string Title
        {
            get { return _info.Title; }
            set { _info.Title = value; }
        }

        [SDataProtocolProperty]
        public DateTimeOffset? Updated
        {
            get { return _info.Updated; }
            set { _info.Updated = value; }
        }

        [SDataProtocolProperty]
        public int? TotalResults
        {
            get { return _info.TotalResults; }
            set { _info.TotalResults = value; }
        }

        [SDataProtocolProperty]
        public int? StartIndex
        {
            get { return _info.StartIndex; }
            set { _info.StartIndex = value; }
        }

        [SDataProtocolProperty]
        public int? ItemsPerPage
        {
            get { return _info.ItemsPerPage; }
            set { _info.ItemsPerPage = value; }
        }

        [SDataProtocolProperty]
        public Uri Url
        {
            get { return _info.Url; }
            set { _info.Url = value; }
        }

        [SDataProtocolProperty]
        public Diagnoses Diagnoses
        {
            get { return _info.Diagnoses ?? (_info.Diagnoses = new Diagnoses()); }
            set { _info.Diagnoses = value; }
        }

        [SDataProtocolProperty]
        public string Schema
        {
            get { return _info.Schema; }
            set { _info.Schema = value; }
        }

        public IList<SDataLink> Links
        {
            get { return _info.Links; }
            set { _info.Links = value; }
        }

        [SDataProtocolProperty]
        public bool? DeleteMissing
        {
            get { return _info.DeleteMissing; }
            set { _info.DeleteMissing = value; }
        }

        [SDataProtocolProperty]
        public SyncMode? SyncMode
        {
            get { return _info.SyncMode; }
            set { _info.SyncMode = value; }
        }

        [SDataProtocolProperty]
        public Digest SyncDigest
        {
            get { return _info.SyncDigest; }
            set { _info.SyncDigest = value; }
        }

        public string XmlLocalName
        {
            get { return _info.XmlLocalName; }
            set { _info.XmlLocalName = value; }
        }

        public string XmlNamespace
        {
            get { return _info.XmlNamespace; }
            set { _info.XmlNamespace = value; }
        }

        public bool XmlIsFlat
        {
            get { return _info.XmlIsFlat; }
            set { _info.XmlIsFlat = value; }
        }

        public bool JsonIsSimpleArray
        {
            get { return _info.JsonIsSimpleArray; }
            set { _info.JsonIsSimpleArray = value; }
        }
    }
}