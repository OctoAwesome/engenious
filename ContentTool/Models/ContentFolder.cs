using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Browsable(false)]
        public ContentItemCollection Content {
            get => content;
            set
            {
                if (value == content) return;
                var old = content;
                content = value;
                OnPropertyChanged(old,value);
            }
        }
        protected ContentItemCollection content;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <param name="parent">Parent item</param>
        public ContentFolder(string name, ContentItem parent) : base(name, parent)
        {
            content = new ContentItemCollection();
            content.CollectionChanged += OnCollectionChanged;
            content.PropertyChanged += OnPropertyChanged;
        }

        public override ContentItem Deserialize(XElement element)
        {
            name = element.Element("Name")?.Value;

            if (!Directory.Exists(FilePath))
                Error = ContentErrorType.NotFound;

            supressChangedEvent = true;
            foreach (var subElement in element.Element("Contents").Elements())
            {
                if (subElement.Name == "ContentFile")
                    content.Add(new ContentFile("", this).Deserialize(subElement));
                else if (subElement.Name == "ContentFolder")
                    content.Add(new ContentFolder("", this).Deserialize(subElement));
            }
            supressChangedEvent = false;

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
