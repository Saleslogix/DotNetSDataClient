using System;
using System.Collections.Generic;
using Sage.SData.Client.Framework;

namespace Sage.SData.Client
{
    [Serializable]
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

        public string Id
        {
            get { return _info.Id; }
            set { _info.Id = value; }
        }

        public string Title
        {
            get { return _info.Title; }
            set { _info.Title = value; }
        }

        public DateTimeOffset? Updated
        {
            get { return _info.Updated; }
            set { _info.Updated = value; }
        }

        public int? TotalResults
        {
            get { return _info.TotalResults; }
            set { _info.TotalResults = value; }
        }

        public int? StartIndex
        {
            get { return _info.StartIndex; }
            set { _info.StartIndex = value; }
        }

        public int? ItemsPerPage
        {
            get { return _info.ItemsPerPage; }
            set { _info.ItemsPerPage = value; }
        }

        public Uri Url
        {
            get { return _info.Url; }
            set { _info.Url = value; }
        }

        public Diagnoses Diagnoses
        {
            get { return _info.Diagnoses ?? (_info.Diagnoses = new Diagnoses()); }
            set { _info.Diagnoses = value; }
        }

        public string Schema
        {
            get { return _info.Schema; }
            set { _info.Schema = value; }
        }

        public bool? DeleteMissing
        {
            get { return _info.DeleteMissing; }
            set { _info.DeleteMissing = value; }
        }

        public SyncMode? SyncMode
        {
            get { return _info.SyncMode; }
            set { _info.SyncMode = value; }
        }

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
    }
}