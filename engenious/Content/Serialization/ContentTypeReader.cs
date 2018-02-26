using System;

namespace engenious.Content.Serialization
{
	public abstract class ContentTypeReader<T> : IContentTypeReader
	{
	    public abstract T Read (ContentManager manager, ContentReader reader,Type customType = null);

		object IContentTypeReader.Read (ContentManager manager, ContentReader reader,Type customType)
		{
			return Read (manager, reader,customType);
		}
	}
}

