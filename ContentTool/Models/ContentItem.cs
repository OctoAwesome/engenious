using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using ContentTool.Observer;

namespace ContentTool.Models
{
    public abstract class ContentItem : INotifyPropertyValueChanged,INotifyCollectionChanged
    {
        /// <summary>
        /// The name of the content item
        /// </summary>
        public string Name { get => _name;
            set
            {
                if (_name == value) return;
                var old = _name;
                _name = value;
                OnPropertyChanged(_name,value);
            }
        }

        private string _name;
        
        protected bool SupressChangedEvent = false;

        [Browsable(false)]
        public ContentErrorType Error { get; set; }

        /// <summary>
        /// The path of the content item
        /// </summary>
        [Browsable(false)]
        public abstract string FilePath { get; }

        /// <summary>
        /// The relative Path of the content item
        /// </summary>
        [Browsable(false)]
        public virtual string RelativePath => Path.Combine(Parent.RelativePath, Name);

        /// <summary>
        /// The parent item
        /// </summary>
        [Browsable(false)]
        public virtual ContentItem Parent { get => _parent;
            set
            {
                if (value == _parent) return;
                var old = _parent;
                _parent = value;
                OnPropertyChanged(old,value);
            }
        }
        /// <summary>
        /// The content project
        /// </summary>
        [Browsable(false)]
        public virtual ContentProject Project {
            get
            {
                var x = this;
                ContentProject proj = null;
                while ((proj = (x as ContentProject)) == null)
                {
                    x = x?.Parent;
                    if (x == null)
                        break;
                }
                return proj;
            }
        }

        private ContentItem _parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the Item</param>
        /// <param name="parent">Parent item</param>
        protected ContentItem(string name, ContentItem parent)
        {
            _name = name;
            _parent = parent;
        }

        /// <summary>
        /// Serializes the item
        /// </summary>
        public abstract ContentItem Deserialize(XElement element);

        /// <summary>
        /// Deserializes the item
        /// </summary>
        public abstract XElement Serialize();


        protected virtual void OnPropertyChanged(object sender,PropertyValueChangedEventArgs args)
        {
            if (SupressChangedEvent) return;
            PropertyChanged?.Invoke(sender, args);
        }
        protected virtual void OnPropertyChanged(object sender,object oldValue,object newValue,[CallerMemberName] string propertyName = null)
        {
            if (SupressChangedEvent) return;
            OnPropertyChanged(sender, new PropertyValueChangedEventArgs(propertyName,oldValue,newValue));
        }
        protected virtual void OnPropertyChanged(object oldValue,object newValue,[CallerMemberName] string propertyName = null)
        {
            if (SupressChangedEvent) return;
            OnPropertyChanged(this,oldValue,newValue,propertyName);
        }
        protected virtual void OnCollectionChanged(object sender,NotifyCollectionChangedEventArgs args)
        {
            if (SupressChangedEvent) return;
            if (sender is ContentItem)
                CollectionChanged?.Invoke(sender,args);
            else
                CollectionChanged?.Invoke(this, args);
        }
        protected virtual void OnCollectionChanged(object sender,NotifyCollectionChangedAction action,object element)
        {
             OnCollectionChanged(sender,new NotifyCollectionChangedEventArgs(action,element));
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event NotifyPropertyValueChangedHandler PropertyChanged;
    }

    [Flags]
    public enum ContentErrorType
    {
        None = 1,
        NotFound = 2,
        ImporterError = 4,
        ProcessorError = 8,
        Other = 16
    }
}
