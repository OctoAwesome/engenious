using System.Windows.Forms;
using ContentTool.Models;

namespace ContentTool.Viewer
{
    public interface IViewer
    {
        Control GetViewer(ContentFile file);
    }
}
