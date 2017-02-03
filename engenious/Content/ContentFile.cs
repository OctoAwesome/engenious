using System;
using System.IO;
using engenious.Content.Serialization;

namespace engenious.Content
{
    [Serializable]
    internal class ContentFile<T> : ContentFile
    {
        public ContentFile()
            : base(typeof(T).Namespace + "." + typeof(T).Name)
        {
        }
			
    }

    [Serializable]
    internal class ContentFile
    {
        internal ContentFile(string type)
        {
            FileType = type;
        }

        public string FileType{ get; private set; }

        public object Load(ContentManager manager, Stream stream,Type type)
        {
            ContentReader reader = new ContentReader(stream);
            string readName = reader.ReadString();
            IContentTypeReader tp = manager.GetReaderByOutput(type.FullName);
            if (tp == null)
                tp = manager.GetReader(readName);
            return tp.Read(manager, reader);
        }
    }
}

