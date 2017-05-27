using ContentTool.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ContentTool.Delegates;

namespace ContentTool.Forms
{
    public interface IMainShell
    {
        ContentProject Project { get; set; }

        void Invoke(Delegate d);

        bool ShowCloseWithoutSavingConfirmation();
        string ShowOpenDialog();
        string ShowSaveAsDialog();

        void WriteLog(string text, Color color = default(Color));
        void WriteLineLog(string text, Color color = default(Color));
        void ClearLog();

        void ShowLoading();
        void HideLoading();

        event ItemActionEventHandler BuildItemClick;
        event ItemActionEventHandler ShowInExplorerItemClick;
        event ItemAddActionEventHandler AddItemClick;
        event ItemActionEventHandler RemoveItemClick;

        event EventHandler NewProjectClick;
        event EventHandler OpenProjectClick;
        event ItemActionEventHandler CloseProjectClick;
        event ItemActionEventHandler SaveProjectClick;
        event ItemActionEventHandler SaveProjectAsClick;
    }
}
