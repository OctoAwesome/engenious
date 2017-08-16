using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace ContentTool.old.Items
{
    [Serializable]
    public class ContentFolder : ContentItem
    {
        public ContentFolder(ContentFolder parent = null)
            :base(parent)
        {
            Contents = new ContentItemCollection();
            Contents.CollectionChanged += Contents_CollectionChanged;
            Contents.PropertyChanged += Contents_PropertyChanged;
        }

        public ContentFolder(string name, ContentFolder parent = null)
            :this(parent)
        {
            Name = name;
        }

        private void Contents_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvokePropertyChange(sender, e);
        }

        private void Contents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokeCollectionChange(sender, e);
        }

        /// <summary>
        /// Gets an Element by path relative to the current content folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ContentItem GetElement(string path)
        {
            string trailingPath=null;
            int ind = path.IndexOf("/", StringComparison.Ordinal);
            if (ind != -1)
            {
                trailingPath = path.Substring(ind+1);
                path = path.Substring(0,ind);
            }
            foreach(var c in Contents)
            {
                if (c.Name == path)
                {
                    if (c is ContentFolder && trailingPath != null)
                        return ((ContentFolder)c).GetElement(trailingPath);
                    return c;
                }
            }
            return null;
        }

        [Browsable(false)]
        public ContentItemCollection Contents{ get; set; }

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
                case "Contents":
                    foreach(var child in node.ChildNodes.OfType<XmlElement>())
                    {
                        ContentItem item;
                        switch(child.Name)
                        {
                            case "ContentFolder":
                                item = new ContentFolder("",this);
                                break;
                            case "ContentFile":
                                item = new ContentFile("",this);
                                break;
                            default:
                                continue;
                        }
                        foreach(var inner in child.ChildNodes.OfType<XmlElement>())
                            item.ReadItem(inner);
                        Contents.Add(item);
                    }
                    break;
            }
        }

        public override void WriteItems(XmlWriter writer)
        {
            writer.WriteElementString("Name", Name);

            writer.WriteStartElement("Contents");
            {
                foreach (var item in Contents)
                {
                    writer.WriteStartElement(item.GetType().Name);
                    item.WriteItems(writer);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
        }

        #endregion

        /// <summary>
        /// Returns the complete folder structure up to the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>ContentFolder with folder structure</returns>
        public ContentFolder CreateTreeFolderStructure(string path)
        {
            if (string.IsNullOrEmpty(path))
                return this;
            int dInd = path.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.PathSeparator, Path.VolumeSeparatorChar });
            string remainingPath = null;
            if (dInd != -1)
            {
                remainingPath = path.Substring(dInd + 1);
                path = path.Substring(0, dInd);
            }
            var newFolder = (GetElement(path) as ContentFolder) ?? new ContentFolder(path, this);

            Contents.Add(newFolder);
            if (!string.IsNullOrEmpty(remainingPath))
                return newFolder.CreateTreeFolderStructure(remainingPath);
            return newFolder;

        }

        /// <summary>
        /// Adds a folder structure up to the given folder and copies it if necessary
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public void AddSubFolder(string sourcePath, string destinationPath)
        {
            var fn = new DirectoryInfo(sourcePath).Name;
            ContentFolder f = GetElement(fn) as ContentFolder ?? new ContentFolder(fn,this);
            Contents.Add(f);
            if (sourcePath != destinationPath)
            {
                Directory.CreateDirectory(destinationPath);
            }

            foreach (var dir in Directory.EnumerateDirectories(sourcePath))
            {
                var bar = new DirectoryInfo(dir).Name;
                f.AddSubFolder(dir, Path.Combine(destinationPath, bar));
            }

            foreach (var file in Directory.EnumerateFiles(sourcePath))
            {
                f.AddFile( file, Path.Combine(destinationPath,new FileInfo(file).Name));
            }
        }

        /// <summary>
        /// Adds a file to the given folder and copies it if necessary
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destinationPath"></param>
        public void AddFile(string sourcePath, string destinationPath)
        {
            if (sourcePath != destinationPath)
            {
                File.Copy(sourcePath, destinationPath, true); //TODO ask user
            }
            Contents.Add(new ContentFile(Path.GetFileName(destinationPath), this));
        }
    }
}

