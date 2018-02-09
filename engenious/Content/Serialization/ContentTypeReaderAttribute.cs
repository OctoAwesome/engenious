using System;

namespace engenious.Content.Serialization
{
	[AttributeUsage (AttributeTargets.Class)]
	public class ContentTypeReaderAttribute : Attribute
	{
        public ContentTypeReaderAttribute (Type outputType)
		{
            OutputType = outputType;
		}
        public Type OutputType{get;private set;}
	}
}

