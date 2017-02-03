using System;
using System.ComponentModel;

namespace engenious.Pipeline
{
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private readonly object _component;
        private readonly PropertyDescriptor _property;
        public CustomPropertyDescriptor(PropertyDescriptor property,object component,Attribute [] attrs) :base(property.Name, attrs)
        {
            _component = component;
            _property = property;
        }

        #region PropertyDescriptor specific

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType => _property.ComponentType;

        public override object GetValue(object component)
        {
            return _property.GetValue(_component);
        }

        public override string Description => _property.Description;

        public override string Category => _property.Category;

        public override string DisplayName => _property.DisplayName;

        public override bool IsReadOnly => _property.IsReadOnly;

        public override void ResetValue(object component)
        {
            //Have to implement
            _property.ResetValue(_component);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return _property.ShouldSerializeValue(_component);
        }

        public override void SetValue(object component, object value)
        {
            _property.SetValue(_component,value);
        }

        public override Type PropertyType => _property.PropertyType;

        #endregion
    }
}

