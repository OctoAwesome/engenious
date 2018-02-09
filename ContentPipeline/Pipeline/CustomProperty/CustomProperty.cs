using System;

namespace engenious.Pipeline
{
    public class CustomProperty
    {
        public delegate void SetCustomProperty(object value);
        private readonly SetCustomProperty _setter;
        public CustomProperty(string name, object value, Type type, bool readOnly, bool visible,SetCustomProperty setter)
        {
            Name = name;
            Value = value;
            Type = type;
            ReadOnly = readOnly;
            Visible = visible;
            _setter = setter;
        }

        public string Name{get;private set;}
        private object value;
        public object Value{
            get{
                return value;
            }
            internal set{
                this.value = value;
                _setter(value);
            }
            }
        public Type Type{get;private set;}
        public bool ReadOnly{get;private set;}
        public bool Visible{get;private set;}
    }
}

