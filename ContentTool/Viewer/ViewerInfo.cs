using System;

namespace ContentTool.Viewer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ViewerInfo : Attribute
    {
        public string Extension { get; }
        public bool NeedsCompilation { get; }

        public ViewerInfo(string extension, bool needsCompilation)
        {
            Extension = extension;
            NeedsCompilation = needsCompilation;
        }
    }
}
