using System.ComponentModel;
using System.IO;
using System.Xml.Linq;

namespace ContentTool.Models
{
    public class ContentFolder : ContentItem
    {
        /// <summary>
        /// Path of the content folder
        /// </summary>
        public override string FilePath
        {
            get => Path.Combine(Parent.FilePath, Name);
        }

        /// <summary>
        /// The content of the folder
        /// </summary>
        [Browsable(false)]
        public ContentItemCollection Content
        {
            get => _content;
            set
            {
                if (value == _content) return;
                var old = _content;
                _content = value;
                OnPropertyChanged(old, value);
            }
        }

        private ContentItemCollection _content;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the element</param>
        /// <param name="parent">Parent item</param>
        public ContentFolder(string name, ContentItem parent) : base(name, parent)
        {
            _content = new ContentItemCollection();
            _content.CollectionChanged += OnCollectionChanged;
            _content.PropertyChanged += OnPropertyChanged;
        }

        public override ContentItem Deserialize(XElement element)
        {
            SupressChangedEvent = true;
            Name = element.Element("Name")?.Value;

            if (!Directory.Exists(FilePath))
                Error = ContentErrorType.NotFound;

            var xElement = element.Element("Contents");
            if (xElement == null)
                return this;

            foreach (var subElement in xElement.Elements())
            {
                if (subElement.Name == "ContentFile")
                    _content.Add(new ContentFile(string.Empty, this).Deserialize(subElement));
                else if (subElement.Name == "ContentFolder")
                    _content.Add(new ContentFolder(string.Empty, this).Deserialize(subElement));
            }
            SupressChangedEvent = false;

            return this;
        }

        public override XElement Serialize()
        {
            var element = new XElement("ContentFolder");
            element.Add(new XElement("Name", Name));

            var contentElement = new XElement("Contents");
            foreach (var item in Content)
                contentElement.Add(item.Serialize());
            element.Add(contentElement);

            return element;
        }
    }
}