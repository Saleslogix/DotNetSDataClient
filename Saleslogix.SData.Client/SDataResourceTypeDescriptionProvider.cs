// Copyright (c) 1997-2013, SalesLogix NA, LLC. All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Saleslogix.SData.Client
{
    public class SDataResourceTypeDescriptionProvider : TypeDescriptionProvider
    {
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ResourceCustomTypeDescriptor((SDataResource) instance);
        }

        #region Nested type: ResourceCustomTypeDescriptor

        private class ResourceCustomTypeDescriptor : CustomTypeDescriptor
        {
            private readonly SDataResource _instance;

            public ResourceCustomTypeDescriptor(SDataResource instance)
            {
                _instance = instance;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = new List<PropertyDescriptor>();

                if (_instance != null)
                {
                    foreach (var pair in _instance)
                    {
                        Type propertyType;
                        Type typeConverterType;
                        if (pair.Value is SDataResource)
                        {
                            propertyType = typeof (SDataResource);
                            typeConverterType = typeof (ExpandableObjectConverter);
                        }
                        else if (pair.Value is SDataCollection<SDataResource>)
                        {
                            propertyType = typeof (SDataCollection<SDataResource>);
                            typeConverterType = typeof (SDataCollectionTypeConverter);
                        }
                        else
                        {
                            propertyType = typeof (string);
                            typeConverterType = null;
                        }
                        properties.Add(new ResourcePropertyDescriptor(pair.Key, propertyType, typeConverterType));
                    }
                }

                return new PropertyDescriptorCollection(properties.ToArray());
            }
        }

        #endregion

        #region Nested type: ResourcePropertyDescriptor

        private class ResourcePropertyDescriptor : PropertyDescriptor
        {
            private readonly Type _type;

            public ResourcePropertyDescriptor(string name, Type type, Type typeConverterType)
                : base(name, typeConverterType != null
                    ? new Attribute[] {new TypeConverterAttribute(typeConverterType)}
                    : null)
            {
                _type = type;
            }

            #region PropertyDescriptor Members

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override object GetValue(object component)
            {
                return ((SDataResource) component)[Name];
            }

            public override void ResetValue(object component)
            {
                SetValue(component, null);
            }

            public override void SetValue(object component, object value)
            {
                ((SDataResource) component)[Name] = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return GetValue(component) != null;
            }

            public override Type ComponentType
            {
                get { return typeof (SDataResource); }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return _type; }
            }

            #endregion
        }

        #endregion
    }
}