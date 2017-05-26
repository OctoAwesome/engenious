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
        public string Name { get; set; }

        /// <summary>
        /// The path of the content item
        /// </summary>
        public abstract string FilePath { get; }

        /// <summary>
        /// The parent item
        /// </summary>
        public virtual ContentItem Parent { get; set; }

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

    }
}
