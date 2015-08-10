// Copyright (c) 1997-2014, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Saleslogix.SData.Client.Framework;

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
    public class SDataCollection<T> : IList<T>, IList, ISDataProtocolObject, IChangeTracking, IRevertibleChangeTracking
    {
        private IList<T> _items;
        private IList<T> _snapshot;
        private SDataProtocolInfo _info = new SDataProtocolInfo();

        public SDataCollection()
        {
            _items = new List<T>();
            _snapshot = new List<T>();
        }

        public SDataCollection(int capacity)
        {
            _items = new List<T>(capacity);
            _snapshot = new List<T>(capacity);
        }

        public SDataCollection(IEnumerable<T> collection)
        {
            _items = new List<T>(collection);
            _snapshot = new List<T>(_items);
        }

        public SDataCollection(string xmlLocalName, string xmlNamespace = null)
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

        #region IChangeTracking Members

        public bool IsChanged
        {
            get
            {
                return _items.Count != _snapshot.Count ||
                       _items.Zip(_snapshot, (item, snapshot) => new {item, snapshot})
                           .Any(pair =>
                           {
                               if (Equals(pair.item, pair.snapshot))
                               {
                                   var tracking = pair.item as IChangeTracking;
                                   return tracking != null && tracking.IsChanged;
                               }
                               return true;
                           });
            }
        }

        public object GetChanges()
        {
            var zipped = _items.Zip(_snapshot, (item, snapshot) => new {item, snapshot})
                .Concat(_items.Skip(_snapshot.Count).Select(item => new {item, snapshot = default(T)}))
                .Concat(_snapshot.Skip(_items.Count).Select(snapshot => new {item = default(T), snapshot}))
                .ToList();
            var deleteMissing = !zipped.All(pair => Equals(pair.item, pair.snapshot) || Equals(pair.snapshot, default(T)) || pair.snapshot is ISDataProtocolObject);
            IList<object> changes;
            if (deleteMissing)
            {
                changes = _items
                    .Select(item =>
                    {
                        var tracking = item as IChangeTracking;
                        return (tracking != null ? tracking.GetChanges() : null) ?? item;
                    })
                    .ToList();
            }
            else
            {
                changes = zipped
                    .Select(pair =>
                    {
                        object item = pair.item;
                        if (Equals(item, pair.snapshot))
                        {
                            var tracking = item as IChangeTracking;
                            if (tracking == null)
                            {
                                return null;
                            }
                            item = tracking.GetChanges();
                            if (Equals(item, default(T)))
                            {
                                return null;
                            }
                        }
                        else if (Equals(item, default(T)))
                        {
                            var prot = (ISDataProtocolObject) pair.snapshot;
                            item = new SDataResource
                                {
                                    Key = prot.Info.Key,
                                    IsDeleted = true
                                };
                        }
                        return item;
                    })
                    .Where(item => item != null)
                    .ToList();
            }
            if (changes.Count == 0)
            {
                return null;
            }
            var collection = new SDataCollection<object>(changes);
            ((ISDataProtocolObject) collection).Info = _info;
            collection.DeleteMissing = deleteMissing;
            return collection;
        }

        public void AcceptChanges()
        {
            _snapshot = new List<T>(_items);
            foreach (var value in _items.OfType<IChangeTracking>())
            {
                value.AcceptChanges();
            }
        }

        public void RejectChanges()
        {
            _items = new List<T>(_snapshot);
            foreach (var value in _items.OfType<IRevertibleChangeTracking>())
            {
                value.RejectChanges();
            }
        }

        #endregion

        #region IList Members

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }

        object IList.this[int index]
        {
            get { return ((IList) _items)[index]; }
            set { ((IList) _items)[index] = value; }
        }

        int IList.Add(object value)
        {
            return ((IList) _items).Add(value);
        }

        bool IList.Contains(object value)
        {
            return ((IList) _items).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList) _items).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList) _items).Insert(index, value);
        }

        void IList.Remove(object value)
        {
            ((IList) _items).Remove(value);
        }

        bool IList.IsFixedSize
        {
            get { return ((IList) _items).IsFixedSize; }
        }

        #endregion

        #region ICollection Members

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return _items.IsReadOnly; }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) _items).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection) _items).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection) _items).SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }
}