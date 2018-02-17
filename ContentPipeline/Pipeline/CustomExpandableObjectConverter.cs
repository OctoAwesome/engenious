using System;
using System.ComponentModel;

namespace engenious.Content.Pipeline
{
    public class CustomExpandableObjectConverter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            var props = TypeDescriptor.GetProperties(value, attributes);
            var ret = new PropertyDescriptor[props.Count];
            for (int i = 0; i < props.Count; i++)
            {
                ret[i] = new SafeTypeDescriptorWrapper(props[i]);
            }
            return new PropertyDescriptorCollection(ret);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
    public class SafeTypeDescriptorWrapper : PropertyDescriptor
    {
        public static EventHandler WorkaroundEvent;
        private readonly PropertyDescriptor _baseDescriptor;
        public SafeTypeDescriptorWrapper(PropertyDescriptor baseDescriptor) : base(baseDescriptor)
        {
            _baseDescriptor = baseDescriptor;
        }


        public override bool CanResetValue(object component)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return component != null && _baseDescriptor.CanResetValue(component);
        }

        public override object GetValue(object component)
        {
            // actually can be false not what ReSharper thinks
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return component == null ? null : _baseDescriptor.GetValue(component);
        }

        public override void ResetValue(object component)
        {
            // actually can be false not what ReSharper thinks
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (component == null)
                // ReSharper disable once HeuristicUnreachableCode
                return;

            _baseDescriptor.ResetValue(component);

        }

        public override void SetValue(object component, object value)
        {
            // actually can be false not what ReSharper thinks
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (component == null)
                return;

            _baseDescriptor.SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            try
            {
                // actually can be false not what ReSharper thinks
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                return _baseDescriptor.ShouldSerializeValue(component);
            }catch(ArgumentNullException ex)
            {
                WorkaroundEvent?.Invoke(this,EventArgs.Empty);
                return false;
            }
        }

        public override Type ComponentType => _baseDescriptor.ComponentType;
        public override bool IsReadOnly  => _baseDescriptor.IsReadOnly;
        public override Type PropertyType => _baseDescriptor.PropertyType;
    }
}