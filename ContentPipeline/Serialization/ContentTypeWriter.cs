using System;

namespace engenious.Content.Serialization
{
	public abstract class ContentTypeWriter<T> : IContentTypeWriter
	{
	    public abstract void Write (ContentWriter writer, T value);

		void IContentTypeWriter.Write (ContentWriter writer, object value)
		{
			Write (writer, (T)value);
		}

		public abstract string RuntimeReaderName{ get; }

		public Type RuntimeType { 
			get {
				return typeof(T);
			}
		
		}
	}
}

