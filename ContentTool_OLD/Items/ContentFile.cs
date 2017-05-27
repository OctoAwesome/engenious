using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ContentTool.old.Dialog;
using engenious.Content.Pipeline;
using engenious.Pipeline;

namespace ContentTool.old.Items
{
    [Serializable]
    public class ContentFile : ContentItem
    {
        public ContentFile(ContentFolder parent = null)
            : base(parent)
        {
            
        }

        public ContentFile(string name, ContentFolder parent = null)
            : this(parent)
        {
            Name = name;
            ProcessorName = null;
        }

        #region implemented abstract members of ContentItem


        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var props = base.GetProperties(attributes);
            var processorProps = TypeDescriptor.GetProperties(Processor?.Settings,true);
            var newProps = new PropertyDescriptor[props.Count+processorProps.Count];
            for (int i=0;i<props.Count;i++)
                newProps[i] = props[i];
            for (int i=0;i<processorProps.Count;i++){
                newProps[i+props.Count]=new CustomPropertyDescriptor(processorProps[i],Processor?.Settings,attributes);
            }
            return new PropertyDescriptorCollection(newProps);
        }
        #endregion
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                Importer = PipelineHelper.CreateImporter(Path.GetExtension(value));
            }
        }

        [Browsable(false)]
        public ProcessorSettings Settings
        {
            get{
                if (Processor == null)
                    return null;
                return Processor.Settings;
            }
            set{
                if (Processor == null)
                    return;
                Processor.Settings = value;
            }
        }

        [XmlIgnore]
        private string _processorName;
        [XmlElement(IsNullable = true)]
        [Editor(typeof(ProcessorEditor), typeof(UITypeEditor))]
        public string ProcessorName {
            get {
                return _processorName;
            }
            set
            {
                string old = _processorName;
                _processorName = value;
                if (string.IsNullOrWhiteSpace(_processorName))
                {
                    //_processorName = GetProcessor(Name,_importerName);
                }
                if (_processorName != old && !string.IsNullOrWhiteSpace(_processorName))
                {
                    Processor = PipelineHelper.CreateProcessor(Importer.GetType(), ProcessorName);
                }
            }
        }
        [XmlIgnore]
        private string _importerName;
        [XmlElement(IsNullable = true)]
        [Editor(typeof(ImporterEditor), typeof(UITypeEditor))]
        public string ImporterName
        {
            get
            {
                return _importerName;
            }
            set
            {
                _importerName = value;
                Importer = PipelineHelper.CreateImporter(Path.GetExtension(Name),ref _importerName);
                
            }
        }
        [Browsable(false)]
        [XmlIgnore]
        public IContentImporter Importer{
            get;private set;
        }
        [Browsable(false)]
        [XmlIgnore]
        public IContentProcessor Processor{
            get;private set;
        }

        public override string ToString()
        {
            return Name;
        }

        #region implemented abstract members of ContentItem
        public override void ReadItem(XmlElement node)
        {
            switch (node.Name)
            {
                case "Name":
                    Name = node.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                    break;
                case "Processor":
                    ProcessorName = node.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                    break;
                case "Importer":
                    ImporterName = node.ChildNodes.OfType<XmlText>().FirstOrDefault()?.InnerText;
                    break;
                case "Settings":
                    if (Settings != null)
                    {
                        Settings.Read(node.ChildNodes);
                    }
                    break;
            }
        }

        public override void WriteItems(XmlWriter writer)
        {
            writer.WriteElementString("Name",Name);
            writer.WriteElementString("Processor",ProcessorName);
            writer.WriteElementString("Importer", ImporterName);

            if (Settings != null)
            {
                writer.WriteStartElement("Settings");

                Settings.Write(writer);

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}

