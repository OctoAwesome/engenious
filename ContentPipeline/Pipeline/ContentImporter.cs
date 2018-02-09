using System;

namespace engenious.Content.Pipeline
{
    public abstract class ContentImporter<T> : IContentImporter
    {
        public Type ExportType => _exportType;
        protected static readonly Type _exportType = typeof (T);

        public abstract T Import(string filename, ContentImporterContext context);

        object IContentImporter.Import(string filename, ContentImporterContext context)
        {
            return Import(filename, context);
        }

    }
}

