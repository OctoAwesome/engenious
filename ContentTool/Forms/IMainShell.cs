using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ContentTool.Delegates;

namespace ContentTool.Forms
{
    public interface IMainShell
    {
        event ItemActionEventHandler BuildItemClick;
        event ItemActionEventHandler ShowInExplorerItemClick;
        event ItemAddActionEventHandler AddItemClick;
        event ItemActionEventHandler RemoveItemClick;
    }
}
