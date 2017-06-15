using ContentTool.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        void ShowLog();
        void HideLog();

        void ShowLoading();
        void HideLoading();

        void ShowViewer(Control viewer);
        void HideViewer();

        void ShowAbout();

        bool ShowNotFoundDelete();

        void Refresh();

        string ShowFolderSelectDialog();

        event EventHandler UndoClick;
        event EventHandler RedoClick;

        event ItemActionEventHandler BuildItemClick;
        event ItemActionEventHandler ShowInExplorerItemClick;
        event ItemAddActionEventHandler AddItemClick;
        event ItemActionEventHandler RemoveItemClick;

        event EventHandler RebuildClick;
        event EventHandler CleanClick;

        event ItemActionEventHandler AddExistingFolderClick;
        event ItemActionEventHandler AddNewFolderClick;
        event ItemActionEventHandler AddNewItemClick;
        event ItemActionEventHandler AddExistingItemClick;

        event EventHandler NewProjectClick;
        event EventHandler OpenProjectClick;
        event ItemActionEventHandler CloseProjectClick;
        event ItemActionEventHandler SaveProjectClick;
        event ItemActionEventHandler SaveProjectAsClick;

        event ItemActionEventHandler OnItemSelect;

        event EventHandler OnAboutClick;
        event EventHandler OnHelpClick;
    }
}
