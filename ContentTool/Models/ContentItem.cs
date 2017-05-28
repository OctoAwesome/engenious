using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ContentTool.Models
{
    public abstract class ContentItem
    {
        /// <summary>
        /// The name of the content item
        /// </summary>
        public string Name { get => name;
            set
            {
                if (name == value) return;
                name = value;
                InternalRaiseChangedEvent(this);
            }
        }
        protected string name;

        public ContentErrorType Error { get; set; }

        /// <summary>
        /// The path of the content item
        /// </summary>
        public abstract string FilePath { get; }

        /// <summary>
        /// The relative Path of the content item
        /// </summary>
        public virtual string RelativePath => Path.Combine(Parent.RelativePath, Name);

        /// <summary>
        /// The parent item
        /// </summary>
        public virtual ContentItem Parent { get => parent;
            set
            {
                if (value == parent) return;
                parent = value;
                InternalRaiseChangedEvent(this);
            }
        }
        protected ContentItem parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the Item</param>
        /// <param name="parent">Parent item</param>
        public ContentItem(string name, ContentItem parent)
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

        protected void InternalRaiseChangedEvent(ContentItem changedItem)
        {
            ContentItemChanged?.Invoke(this, changedItem);
            Parent?.InternalRaiseChangedEvent(changedItem);
        }

        public delegate void ContentItemChangedHandler(ContentItem thisItem, ContentItem changedItem);
        public event ContentItemChangedHandler ContentItemChanged;

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
