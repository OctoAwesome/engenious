using System.Windows.Forms;
using ContentTool.Models;
using ContentTool.Models.History;

namespace ContentTool.Viewer
{
    public interface IViewer
    {
        Control GetViewerControl(ContentFile file);
        void Save();
        void Discard();
        IHistory History { get; }
        bool UnsavedChanges { get; }
        ContentFile ContentFile { get; }
    }
}
