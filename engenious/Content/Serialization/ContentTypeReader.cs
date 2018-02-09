namespace engenious.Content.Serialization
{
	public abstract class ContentTypeReader<T> : IContentTypeReader
	{
	    public abstract T Read (ContentManager manager, ContentReader reader);

		object IContentTypeReader.Read (ContentManager manager, ContentReader reader)
		{
			return Read (manager, reader);
		}
	}
}

