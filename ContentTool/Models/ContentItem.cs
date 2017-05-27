using System;
using System.Collections.Generic;
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
        private string name;

        /// <summary>
        /// The path of the content item
        /// </summary>
        public abstract string FilePath { get; }

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
        private ContentItem parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the Item</param>
        /// <param name="parent">Parent item</param>
        public ContentItem(string name, ContentItem parent)
        {
            Name = name;
            Parent = parent;
        }

        /// <summary>
        /// Serializes the item
        /// </summary>
        public abstract ContentItem Deserialize(XElement element);

        /// <summary>
        /// Deserializes the item
        /// </summary>
        public abstract XElement Serialize();

        internal void InternalRaiseChangedEvent(ContentItem changedItem)
        {
            ContentItemChanged?.Invoke(this, changedItem);
            Parent?.InternalRaiseChangedEvent(changedItem);
        }

        public delegate void ContentItemChangedHandler(ContentItem thisItem, ContentItem changedItem);
        public event ContentItemChangedHandler ContentItemChanged;

    }
}
