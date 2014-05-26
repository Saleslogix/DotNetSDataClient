using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using SlxJobScheduler.Model;

namespace SlxJobScheduler
{
    public class TriggerDescriptionProvider : TypeDescriptionProvider
    {
        private readonly PropertyDescriptorCollection _properties = TypeDescriptor.GetProperties(typeof (Trigger));

        public TriggerDescriptionProvider()
        {
            _properties = new PropertyDescriptorCollection(
                TypeDescriptor.GetProperties(typeof (Trigger))
                    .Cast<PropertyDescriptor>()
                    .Select(prop =>
                    {
                        if (prop.PropertyType == typeof (DateTime?))
                        {
                            return TypeDescriptor.CreateProperty(typeof (Trigger), prop, new EditorAttribute("System.ComponentModel.Design.DateTimeEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof (UITypeEditor)));
                        }
                        if (prop.Name == "Parameters")
                        {
                            return TypeDescriptor.CreateProperty(typeof (Trigger), prop, new TypeConverterAttribute(typeof (TriggerParametersTypeConverter)));
                        }
                        return prop;
                    })
                    .ToArray());
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new Descriptor(_properties);
        }

        private class Descriptor : CustomTypeDescriptor
        {
            private readonly PropertyDescriptorCollection _properties;

            public Descriptor(PropertyDescriptorCollection properties)
            {
                _properties = properties;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return _properties;
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                return _properties;
            }
        }
    }
}