using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ContentTool.Observer;

namespace ContentTool.Models
{
    public abstract class ContentItem : INotifyPropertyValueChanged,INotifyCollectionChanged
    {
        /// <summary>
        /// The name of the content item
        /// </summary>
        public string Name { get => name;
            set
            {
                if (name == value) return;
                var old = name;
                name = value;
                OnPropertyChanged(name,value);
            }
        }
        protected string name;
        
        protected bool supressChangedEvent = false;

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
        public virtual ContentItem Parent { get => parent;
            set
            {
                if (value == parent) return;
                var old = parent;
                parent = value;
                OnPropertyChanged(old,value);
            }
        }
        protected ContentItem parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the Item</param>
        /// <param name="parent">Parent item</param>
        protected ContentItem(string name, ContentItem parent)
        {
            this.name = name;
            this.parent = parent;
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
            if (supressChangedEvent) return;
            PropertyChanged?.Invoke(sender, args);
        }
        protected virtual void OnPropertyChanged(object sender,object oldValue,object newValue,[CallerMemberName] string propertyName = null)
        {
            if (supressChangedEvent) return;
            OnPropertyChanged(sender, new PropertyValueChangedEventArgs(propertyName,oldValue,newValue));
        }
        protected virtual void OnPropertyChanged(object oldValue,object newValue,[CallerMemberName] string propertyName = null)
        {
            if (supressChangedEvent) return;
            OnPropertyChanged(this,oldValue,newValue,propertyName);
        }
        protected virtual void OnCollectionChanged(object sender,NotifyCollectionChangedEventArgs args)
        {
            if (supressChangedEvent) return;
            CollectionChanged?.Invoke(sender,args);
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
