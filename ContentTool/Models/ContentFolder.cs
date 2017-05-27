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
    public class ContentFolder : ContentItem
    {
        /// <summary>
        /// Path of the content folder
        /// </summary>
        public override string FilePath { get => Path.Combine(Parent.FilePath, Name); }

        /// <summary>
        /// The content of the folder
        /// </summary>
        public List<ContentItem> Content {
            get => content;
            set
            {
                if (value == content) return;
                content = value;
                InternalRaiseChangedEvent(this);
            }
        }
        private List<ContentItem> content;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <param name="parent">Parent item</param>
        public ContentFolder(string name, ContentItem parent) : base(name, parent)
        {
            content = new List<ContentItem>();

        }

        public override ContentItem Deserialize(XElement element)
        {
            Name = element.Element("Name")?.Value;

            foreach(var subElement in element.Element("Contents").Elements())
            {
                if (subElement.Name == "ContentFile")
                    Content.Add(new ContentFile("", this).Deserialize(subElement));
                else if (subElement.Name == "ContentFolder")
                    Content.Add(new ContentFolder("", this).Deserialize(subElement));
            }

            return this;
        }

        public override XElement Serialize()
        {
            XElement element = new XElement("ContentFolder");
            element.Add(new XElement("Name", Name));

            var contentElement = new XElement("Contents");
            foreach (var item in Content)
                contentElement.Add(item.Serialize());
            element.Add(contentElement);

            return element;
        }
    }
}
