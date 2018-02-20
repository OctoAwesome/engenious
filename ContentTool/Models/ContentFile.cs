using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ContentTool.Controls;
using engenious.Content.Pipeline;

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
        [Browsable(false)]
        public IContentImporter Importer { get; private set; }

        /// <summary>
        /// Processor of the file
        /// </summary>
        [Browsable(false)]
        public IContentProcessor Processor { get; private set; }

        /// <summary>
        /// Name of the importer
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(ImporterNameDropDown))]
        public string ImporterName { get => importerName;
            set
            {
                if (importerName == value)
                    return;
                var old = importerName;
                importerName = value;
                Importer = PipelineHelper.CreateImporter(Path.GetExtension(FilePath), ref importerName);
                OnPropertyChanged(old,value);
            }
        }
        private string importerName;

        /// <summary>
        /// Name of the processor
        /// </summary
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(ProcessorNameDropDown))]
        public string ProcessorName { get => processorName;
            set
            {
                if (value == processorName) return;
                var old = processorName;
                processorName = value;

                if (string.IsNullOrWhiteSpace(processorName))
                    processorName = PipelineHelper.GetProcessor(Name, importerName);

                if (old != processorName && !string.IsNullOrWhiteSpace(processorName) && Importer != null)
                    Processor = PipelineHelper.CreateProcessor(Importer.GetType(), processorName);

                OnPropertyChanged(old,value);
            }
        }

        private string processorName;


        /// <summary>
        /// Settings for the processor
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProcessorSettings Settings
        {
            get
            {
                return Processor?.Settings;
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
            Importer = PipelineHelper.CreateImporter(Path.GetExtension(name), ref importerName);
            if (string.IsNullOrWhiteSpace(processorName))
                processorName = PipelineHelper.GetProcessor(Name, importerName);
            if (!string.IsNullOrWhiteSpace(processorName) && Importer != null)
                Processor = PipelineHelper.CreateProcessor(Importer.GetType(), processorName);
        }

        /// <summary>
        /// Deserialize the given XmlElement
        /// </summary>
        /// <param name="element">XMLElement</param>
        public override ContentItem Deserialize(XElement element)
        {
            SupressChangedEvent = true;
            Name = element.Element("Name")?.Value;

            if (!File.Exists(FilePath))
                Error = ContentErrorType.NotFound;

            importerName = element.Element("Importer")?.Value;
            Importer = PipelineHelper.CreateImporter(Path.GetExtension(FilePath), ref importerName);

            if (Importer == null)
                Error |= ContentErrorType.ImporterError;

            processorName = element.Element("Processor")?.Value;
            if (string.IsNullOrWhiteSpace(processorName))
                processorName = PipelineHelper.GetProcessor(Name, importerName);

            if (!string.IsNullOrWhiteSpace(processorName) && Importer != null)
                Processor = PipelineHelper.CreateProcessor(Importer.GetType(), processorName);

            if (Processor == null)
                Error |= ContentErrorType.ProcessorError;

            if (Settings != null && element.Element("Settings") != null)
            {
                Settings.Read(element.Element("Settings"));
            }

            SupressChangedEvent = false;
            return this;
        }

        /// <summary>
        /// Serialize the object and return a XmlElement
        /// </summary>
        /// <returns></returns>
        public override XElement Serialize()
        {
            var element = new XElement("ContentFile");

            element.Add(new XElement("Name", Name));
            element.Add(new XElement("Processor", ProcessorName));
            element.Add(new XElement("Importer", ImporterName));

            if (Settings != null)
            {
                var settingsElement = new XElement("Settings");
                Settings.Write(settingsElement);
                element.Add(settingsElement);
            }


            return element;
        }
    }
}
