using System;

namespace engenious.Pipeline
{
    [Serializable]
    public class SourceFile
    {
        public SourceFile()
        {
        }

        public SourceFile(string name,string source)
        {
            Name = name;
            Source = source;
        }
        
        public string Name { get; }
        public string Source { get; }
    }
}