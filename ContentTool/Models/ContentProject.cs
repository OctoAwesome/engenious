﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ContentTool.Models
{
    public class ContentProject : ContentFolder
    {
        /// <summary>
        /// The path of the ContentFolder
        /// </summary>
        public override string FilePath => filePath;

        /// <summary>
        /// The parent item - always null for projects
        /// </summary>
        public override ContentItem Parent => null;

        /// <summary>
        /// The path to the actual project file
        /// </summary>
        public string ContentProjectPath { get; set; }

        /// <summary>
        /// Directory to save the output to
        /// </summary>
        public string OutputDirectory { get; set; }

        /// <summary>
        /// The configuration of the project
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// References of the project
        /// </summary>
        public List<string> References { get; set; }

        private string filePath;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the project</param>
        /// <param name="contentProjectPath">Path of the actual project file</param>
        /// <param name="folderPath">Path of the project directory</param>
        public ContentProject(string name, string contentProjectPath, string folderPath) : base(name, null)
        {
            Content = new List<ContentItem>();

            ContentProjectPath = contentProjectPath;
            filePath = folderPath;
        }

        public override ContentItem Deserialize(XElement element)
        {
            Name = element.Element("Name")?.Value;
            Configuration = element.Element("Configuration")?.Value;
            OutputDirectory = element.Element("OutputDir")?.Value;

            var refElement = element.Element("References");

            if(refElement != null && refElement.HasElements)
            {
                foreach (var referenceElement in refElement.Elements("Reference"))
                    References.Add(referenceElement.Value);
            }

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
            XElement element = new XElement("Content");

            var refElement = new XElement("References");

            if(References != null)
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
            XElement element = XElement.Load(path);

            ContentProject project = new ContentProject("", path, contentFolderPath);

            project.Deserialize(element);

            return project;
        }
    }
}