using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTool.Viewer
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ViewerInfo : System.Attribute
    {
        public string Extension { get; private set; }
        public bool NeedsCompilation { get; private set; }

        public ViewerInfo(string extension, bool needsCompilation)
        {
            Extension = extension;
            NeedsCompilation = needsCompilation;
        }
    }
}
