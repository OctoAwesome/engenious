using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using ContentTool.Models.History;
using ContentTool.Observer;

namespace ContentTool.Models
{
    public class ContentProject : ContentFolder
    {
        /// <summary>
        /// The path of the ContentFolder
        /// </summary>
        public override string FilePath { get; }

        /// <summary>
        /// The parent item - always null for projects
        /// </summary>
        public override ContentItem Parent => null;

        /// <summary>
        /// The path to the actual project file
        /// </summary>
        public string ContentProjectPath { get; set; }

        public override string RelativePath => string.Empty;

        /// <summary>
        /// Directory to save the output to
        /// </summary>
        public string OutputDirectory
        {
            get => _outputDirectory;
            set
            {
                if (value == _outputDirectory) return;
                var old = _outputDirectory;
                _outputDirectory = value;
                OnPropertyChanged(old, value);
            }
        }

        private string _outputDirectory;

        /// <summary>
        /// The configuration of the project
        /// </summary>
        public string Configuration
        {
            get => _configuration;
            set
            {
                if (value == _configuration) return;
                var old = _configuration;
                _configuration = value;
                OnPropertyChanged(old, value);
            }
        }

        private string _configuration;

        /// <summary>
        /// References of the project
        /// </summary>
        public List<string> References
        {
            get => _references;
            set
            {
                if (value == _references) return;
                var old = _references;
                _references = value;
                if (old == null) SupressChangedEvent = true;
                OnPropertyChanged(old, value);
                SupressChangedEvent = false;
            }
        }

        private List<string> _references;

        /// <summary>
        /// Tells if the project has unsaved changes
        /// </summary>
        [Browsable(false)]
        public bool HasUnsavedChanges { get; private set; }

        [Browsable(false)]
        public History.History History { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="contentProjectPath">Path of the actual project file</param>
        /// <param name="folderPath">Path of the project directory</param>
        public ContentProject(string name, string contentProjectPath, string folderPath) : base(name, null)
        {
            ContentProjectPath = contentProjectPath;
            FilePath = folderPath;

            History = new History.History();


            //ContentItemChanged += (a, b) => HasUnsavedChanges = true;
            PropertyChanged += OnPropertyChangedT;
            CollectionChanged += OnCollectionChangedT;
        }

        private void OnCollectionChangedT(object sender, NotifyCollectionChangedEventArgs args)
        {
            //var col = sender as ContentItemCollection;
            //if (col == null)
            //    throw new NotSupportedException();
            var item = HistoryCollectionChange<ContentItem>.CreateInstance((sender as ContentFolder)?.Content, args);
            if (item == null)
                throw new NotSupportedException();
            History.Push(item);

            //History.Push(new HistoryCollectionChange<ContentItem>(col,args.Action,(IList<ContentItem>)args.OldItems,(IList<ContentItem>)args.NewItems));

            HasUnsavedChanges = true;
        }

        private void OnPropertyChangedT(object o, PropertyValueChangedEventArgs args)
        {
            History.Push(new HistoryPropertyChange(o, args.PropertyName, args.OldValue, args.NewValue));
            HasUnsavedChanges = true;
        }

        public override ContentItem Deserialize(XElement element)
        {
            
            SupressChangedEvent = true;
            Name = element.Element("Name")?.Value ?? "Content";
            _configuration = element.Element("Configuration")?.Value;
            _outputDirectory = element.Element("OutputDir")?.Value;

            var refElement = element.Element("References");

            if (refElement != null && refElement.HasElements)
            {
                foreach (var referenceElement in refElement.Elements("Reference"))
                    _references.Add(referenceElement.Value);
            }
            var xElement = element.Element("Contents");
            if (xElement == null)
                return this;

            foreach (var subElement in xElement.Elements())
            {
                if (subElement.Name == "ContentFile")
                    Content.Add(new ContentFile(string.Empty, this).Deserialize(subElement));
                else if (subElement.Name == "ContentFolder")
                    Content.Add(new ContentFolder(string.Empty, this).Deserialize(subElement));
            }
            SupressChangedEvent = false;

            return this;
        }

        public override XElement Serialize()
        {
            var element = new XElement("Content");

            element.Add(new XElement("Name", Name));

            var refElement = new XElement("References");

            if (References != null)
            {
                foreach (var reference in References)
                    refElement.Add(new XElement("Reference", reference));
            }

            element.Add(new XElement("Configuration", Configuration));
            element.Add(new XElement("OutputDir", OutputDirectory));

            var contentElement = new XElement("Contents");
            foreach (var item in Content)
                contentElement.Add(item.Serialize());
            element.Add(contentElement);

            return element;
        }

        public static ContentProject Load(string path)
        {
            return Load(path, Path.GetDirectoryName(path));
        }

        public static ContentProject Load(string path, string contentFolderPath)
        {
            var element = XElement.Load(path);

            var project = new ContentProject(string.Empty, path, contentFolderPath);

            project.Deserialize(element);

            return project;
        }

        public void Save()
        {
            Save(ContentProjectPath);
        }

        public void Save(string path)
        {
            var xelement = Serialize();
            xelement.Save(path);
            ContentProjectPath = path;
            HasUnsavedChanges = false;
        }
    }
}