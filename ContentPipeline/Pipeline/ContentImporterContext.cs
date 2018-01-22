using System.Collections.Generic;
using engenious.Pipeline;

namespace engenious.Content.Pipeline
{
    public class ContentImporterContext : ContentContext
    {
        public override void Dispose()
        {
        }
        public List<SourceFile> SourceFiles { get; set; }
    }
}

