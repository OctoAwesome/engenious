using engenious.Content.Pipeline;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ContentTool.Models
{
    public class ContentFile : ContentItem
    {
        /// <summary>
        /// Path of the file
        /// </summary>
        public override string FilePath => Path.Combine(Parent.FilePath, Name);

        /// <summary>
        /// Importer of the file
        /// </summary>
        public IContentImporter Importer { get; set; }

        /// <summary>
        /// Processor of the file
        /// </summary>
        public IContentProcessor Processor { get; set; }

        /// <summary>
        /// Name of the importer
        /// </summary>
        public string ImporterName { get; set; }

        /// <summary>
        /// Name of the processor
        /// </summary>
        public string ProcessorName { get; set; }

        /// <summary>
        /// Settings for the processor
        /// </summary>
        public ProcessorSettings Settings
        {
            get
            {
                if (Processor == null)
                    return null;
                return Processor.Settings;
            }
            set
            {
                if (Processor == null)
                    return;
                Processor.Settings = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <param name="parent">Parent item</param>
        public ContentFile(string name, ContentItem parent) : base(name, parent)
        {
        }

        /// <summary>
        /// Deserialize the given XmlElement
        /// </summary>
        /// <param name="element">XMLElement</param>
        public override ContentItem Deserialize(XElement element)
        {
            Name = element.Element("Name")?.Value;
            ProcessorName = element.Element("Processor")?.Value;
            ImporterName = element.Element("Importer")?.Value;

            if(Settings != null && element.Element("Settings") != null)
            {
                string xml = element.Element("Settings")?.ToString();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                Settings.Read(doc.ChildNodes);
            }

            return this;
        }

        /// <summary>
        /// Serialize the object and return a XmlElement
        /// </summary>
        /// <returns></returns>
        public override XElement Serialize()
        {
            XElement element = new XElement("ContentFile");

            element.Add(new XElement("Name", Name));
            element.Add(new XElement("Processor", Processor.GetType().Name));
            element.Add(new XElement("Importer", Importer.GetType().Name));

            if(Settings != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlWriter w = XmlWriter.Create(stream);
                    Settings.Write(w);
                    element.Add(XElement.Load(stream));
                }
            }

            return element;
        }
    }
}
