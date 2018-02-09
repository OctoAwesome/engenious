using System;

namespace engenious.Content.Pipeline
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContentProcessorAttribute : Attribute
    {
        public virtual string DisplayName{ get; set; }
    }
}

