using System;

namespace engenious.Content.Serialization
{
	public interface IContentTypeReader
	{
		object Read(ContentManager manager, ContentReader reader,Type customType=null);


	}
}

