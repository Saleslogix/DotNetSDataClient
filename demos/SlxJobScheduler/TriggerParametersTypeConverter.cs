using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using SlxJobScheduler.Model;

namespace SlxJobScheduler
{
    public class TriggerParametersTypeConverter : TypeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var collection = value as ICollection;
            if (collection != null && destinationType == typeof (string))
            {
                return string.Format("({0})", string.Format(collection.Count != 1 ? "{0} items" : "{0} item", collection.Count));
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var items = value as IEnumerable;
            return items != null
                ? new PropertyDescriptorCollection(
                    items.OfType<TriggerParameter>()
                        .Select((item, index) => (PropertyDescriptor) new Descriptor(item, index))
                        .ToArray())
                : TypeDescriptor.GetProperties(value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private class Descriptor : PropertyDescriptor
        {
            private readonly TriggerParameter _item;

            public Descriptor(TriggerParameter item, int index)
                : base(FormatName(item, index), null)
            {
                _item = item;
            }

            public override Type ComponentType
            {
                get { return typeof (IEnumerable<TriggerParameter>); }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return typeof (string); }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            private static string FormatName(TriggerParameter item, int index)
            {
                var name = item.Name;
                return !string.IsNullOrEmpty(name) ? name : string.Format("[{0}]", index);
            }

            public override object GetValue(object component)
            {
                return _item.Value;
            }

            public override void ResetValue(object component)
            {
                SetValue(component, null);
            }

            public override void SetValue(object component, object value)
            {
                _item.Value = value as string;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }
    }
}