using System;
using System.Collections.Generic;
using System.Net;
using Sage.SData.Client.Framework;

namespace Sage.SData.Client
{
    [Serializable]
    public class SDataResource : Dictionary<string, object>, ISDataProtocolAware
    {
        private SDataProtocolInfo _info = new SDataProtocolInfo();

        public SDataResource()
        {
        }

        public SDataResource(int capacity)
            : base(capacity)
        {
        }

        public SDataResource(IDictionary<string, object> dictionary)
            : base(dictionary)
        {
        }

        public SDataResource(string xmlLocalName, string xmlNamespace = null)
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

        public string Key
        {
            get { return _info.Key; }
            set { _info.Key = value; }
        }

        public Guid? Uuid
        {
            get { return _info.Uuid; }
            set { _info.Uuid = value; }
        }

        public string Lookup
        {
            get { return _info.Lookup; }
            set { _info.Lookup = value; }
        }

        public string Descriptor
        {
            get { return _info.Descriptor; }
            set { _info.Descriptor = value; }
        }

        public HttpMethod? HttpMethod
        {
            get { return _info.HttpMethod; }
            set { _info.HttpMethod = value; }
        }

        public HttpStatusCode? HttpStatus
        {
            get { return _info.HttpStatus; }
            set { _info.HttpStatus = value; }
        }

        public string HttpMessage
        {
            get { return _info.HttpMessage; }
            set { _info.HttpMessage = value; }
        }

        public string Location
        {
            get { return _info.Location; }
            set { _info.Location = value; }
        }

        public string ETag
        {
            get { return _info.ETag; }
            set { _info.ETag = value; }
        }

        public string IfMatch
        {
            get { return _info.IfMatch; }
            set { _info.IfMatch = value; }
        }

        public bool? IsDeleted
        {
            get { return _info.IsDeleted; }
            set { _info.IsDeleted = value; }
        }

        public SyncState SyncState
        {
            get { return _info.SyncState; }
            set { _info.SyncState = value; }
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
    }
}