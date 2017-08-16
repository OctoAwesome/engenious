using System.ComponentModel;

namespace ContentTool.Observer
{
    public class PropertyValueChangedEventArgs : PropertyChangedEventArgs
    {
        public PropertyValueChangedEventArgs(string propertyName,object oldValue,object newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
        public object NewValue { get; }
        public object OldValue { get; }
    }
}