// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Saleslogix.SData.Client
{
    public class SDataCollectionTypeConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof (string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var collection = value as ICollection;
            if (collection != null && destinationType == typeof (string))
            {
                return string.Format("({0} item{1})", collection.Count, collection.Count != 1 ? "s" : null);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return new PropertyDescriptorCollection(
                ((SDataCollection<SDataResource>) value)
                    .Select((item, i) => (PropertyDescriptor) new ResourceCollectionItemPropertyDescriptor(i))
                    .ToArray());
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        #region Nested type: ResourceCollectionItemPropertyDescriptor

        private class ResourceCollectionItemPropertyDescriptor : PropertyDescriptor
        {
            private readonly int _index;

            public ResourceCollectionItemPropertyDescriptor(int index)
                : base(string.Format("[{0}]", index).TrimEnd(), new Attribute[] {new TypeConverterAttribute(typeof (ExpandableObjectConverter))})
            {
                _index = index;
            }

            #region PropertyDescriptor Members

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return ((SDataCollection<SDataResource>) component)[_index];
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
            }

            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return typeof (SDataCollection<SDataResource>); }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override Type PropertyType
            {
                get { return typeof (SDataResource); }
            }

            #endregion
        }

        #endregion
    }
}