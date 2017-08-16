using ContentTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTool.Viewer
{
    public interface IViewer
    {
        Control GetViewer(ContentFile file);
    }
}
