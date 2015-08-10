// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using Saleslogix.SData.Client.Framework;

#if !NET_2_0 && !NET_3_5
using System.Dynamic;
using Saleslogix.SData.Client.Utilities;
#endif

namespace Saleslogix.SData.Client
{
    [Serializable]
#if !PCL && !NETFX_CORE && !SILVERLIGHT
    [TypeDescriptionProvider(typeof (SDataResourceTypeDescriptionProvider))]
#endif
    public class SDataResource :
#if !NET_2_0 && !NET_3_5
        DynamicObject,
#endif
        IDictionary<string, object>, IDictionary, ISDataProtocolObject, IChangeTracking, IRevertibleChangeTracking
    {
        private IDictionary<string, object> _values;
        private IDictionary<string, object> _snapshot;
        private SDataProtocolInfo _info = new SDataProtocolInfo();

        public SDataResource()
        {
            _values = new Dictionary<string, object>();
            _snapshot = new Dictionary<string, object>();
        }

        public SDataResource(int capacity)
        {
            _values = new Dictionary<string, object>(capacity);
            _snapshot = new Dictionary<string, object>(capacity);
        }

        public SDataResource(IDictionary<string, object> dictionary)
        {
            _values = new Dictionary<string, object>(dictionary);
            _snapshot = new Dictionary<string, object>(dictionary);
        }

        public SDataResource(string xmlLocalName, string xmlNamespace = null)
            : this()
        {
            XmlLocalName = xmlLocalName;
            XmlNamespace = xmlNamespace;
        }

        SDataProtocolInfo ISDataProtocolObject.Info
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
        public string Key
        {
            get { return _info.Key; }
            set { _info.Key = value; }
        }

        [SDataProtocolProperty]
        public Guid? Uuid
        {
            get { return _info.Uuid; }
            set { _info.Uuid = value; }
        }

        [SDataProtocolProperty]
        public string Lookup
        {
            get { return _info.Lookup; }
            set { _info.Lookup = value; }
        }

        [SDataProtocolProperty]
        public string Descriptor
        {
            get { return _info.Descriptor; }
            set { _info.Descriptor = value; }
        }

        [SDataProtocolProperty]
        public HttpMethod? HttpMethod
        {
            get { return _info.HttpMethod; }
            set { _info.HttpMethod = value; }
        }

        [SDataProtocolProperty]
        public HttpStatusCode? HttpStatus
        {
            get { return _info.HttpStatus; }
            set { _info.HttpStatus = value; }
        }

        [SDataProtocolProperty]
        public string HttpMessage
        {
            get { return _info.HttpMessage; }
            set { _info.HttpMessage = value; }
        }

        [SDataProtocolProperty]
        public string Location
        {
            get { return _info.Location; }
            set { _info.Location = value; }
        }

        [SDataProtocolProperty]
        public string ETag
        {
            get { return _info.ETag; }
            set { _info.ETag = value; }
        }

        [SDataProtocolProperty]
        public string IfMatch
        {
            get { return _info.IfMatch; }
            set { _info.IfMatch = value; }
        }

        [SDataProtocolProperty]
        public bool? IsDeleted
        {
            get { return _info.IsDeleted; }
            set { _info.IsDeleted = value; }
        }

        [SDataProtocolProperty]
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

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Descriptor))
            {
                return Descriptor;
            }
            if (!string.IsNullOrEmpty(Key))
            {
                return Key;
            }
            return base.ToString();
        }

        #region IChangeTracking Members

        public bool IsChanged
        {
            get
            {
                return _values.Count != _snapshot.Count ||
                       _values.Any(pair =>
                       {
                           object snapshot;
                           if (_snapshot.TryGetValue(pair.Key, out snapshot) && Equals(pair.Value, snapshot))
                           {
                               var tracking = pair.Value as IChangeTracking;
                               return tracking != null && tracking.IsChanged;
                           }
                           return true;
                       });
            }
        }

        public object GetChanges()
        {
            var changes = _values
                .Select(pair =>
                {
                    var value = pair.Value;
                    object snapshot;
                    if (_snapshot.TryGetValue(pair.Key, out snapshot) && Equals(value, snapshot))
                    {
                        var tracking = value as IChangeTracking;
                        if (tracking == null)
                        {
                            return null;
                        }
                        value = tracking.GetChanges();
                        if (value == null)
                        {
                            return null;
                        }
                    }
                    return new {pair.Key, value};
                })
                .Where(item => item != null)
                .ToDictionary(item => item.Key, item => item.value);
            if (changes.Count == 0)
            {
                return null;
            }
            var resource = new SDataResource(changes);
            ((ISDataProtocolObject) resource).Info = _info;
            return resource;
        }

        public void AcceptChanges()
        {
            _snapshot = new Dictionary<string, object>(_values);
            foreach (var value in Values.OfType<IChangeTracking>())
            {
                value.AcceptChanges();
            }
        }

        public void RejectChanges()
        {
            _values = new Dictionary<string, object>(_snapshot);
            foreach (var value in Values.OfType<IRevertibleChangeTracking>())
            {
                value.RejectChanges();
            }
        }

        #endregion

        #region IDictionary Members

        public void Add(string key, object value)
        {
            _values.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _values.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _values.TryGetValue(key, out value);
        }

        public object this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        public ICollection<string> Keys
        {
            get { return _values.Keys; }
        }

        public ICollection<object> Values
        {
            get { return _values.Values; }
        }

        void IDictionary.Add(object key, object value)
        {
            ((IDictionary) _values).Add(key, value);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary) _values).Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary) _values).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            ((IDictionary) _values).Remove(key);
        }

        object IDictionary.this[object key]
        {
            get { return ((IDictionary) _values)[key]; }
            set { ((IDictionary) _values)[key] = value; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return ((IDictionary) _values).IsFixedSize; }
        }

        ICollection IDictionary.Keys
        {
            get { return ((IDictionary) _values).Keys; }
        }

        ICollection IDictionary.Values
        {
            get { return ((IDictionary) _values).Values; }
        }

        #endregion

        #region ICollection Members

        public void Add(KeyValuePair<string, object> item)
        {
            _values.Add(item);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _values.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _values.Remove(item);
        }

        public int Count
        {
            get { return _values.Count; }
        }

        public bool IsReadOnly
        {
            get { return _values.IsReadOnly; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) _values).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection) _values).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection) _values).SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        #endregion

#if !NET_2_0 && !NET_3_5
        #region DynamicObject Members

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return Keys;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            Guard.ArgumentNotNull(indexes, "indexes");

            if (indexes.Length == 1)
            {
                var key = indexes[0] as string;
                if (key != null)
                {
                    result = this[key];
                    return true;
                }
            }
            result = null;
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Guard.ArgumentNotNull(binder, "binder");
            return _values.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            Guard.ArgumentNotNull(indexes, "indexes");

            if (indexes.Length == 1)
            {
                var key = indexes[0] as string;
                if (key != null)
                {
                    this[key] = value;
                    return true;
                }
            }
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Guard.ArgumentNotNull(binder, "binder");
            _values[binder.Name] = value;
            return true;
        }

        #endregion
#endif
    }
}