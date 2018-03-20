using System;
using System.Collections.Generic;

namespace engenious.Content.Pipeline
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContentImporterAttribute : Attribute
    {
        private readonly List<string> _fileExtensions;


        public ContentImporterAttribute(params string[] fileExtensions)
        {
            _fileExtensions = new List<string>();
            foreach (string ext in fileExtensions)
                _fileExtensions.Add(ext.ToUpperInvariant());
        }

        public IEnumerable<string> FileExtensions => _fileExtensions;

        public string DisplayName{ get; set; }

        public string DefaultProcessor { get; set; }

        public bool CacheImportedData { get; set; }
    }

}

