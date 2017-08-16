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
        bool IsRenderingSuspended { get; }

        void Invoke(Delegate d);
        void BeginInvoke(Delegate d);

        bool ShowCloseWithoutSavingConfirmation();
        string ShowOpenDialog();
        string ShowSaveAsDialog();

        void WriteLog(string text, Color color = default(Color));
        void WriteLineLog(string text, Color color = default(Color));
        void ClearLog();
        void ShowLog();
        void HideLog();

        void ShowLoading(string title = "Please wait...");
        void HideLoading();

        void ShowViewer(Control viewer);
        void HideViewer();

        void RenameItem(ContentItem item);
        void RemoveItem(ContentItem item);

        void ShowAbout();

        bool ShowNotFoundDelete();

        void Refresh();

        void SuspendRendering();
        void ResumeRendering();

        string ShowFolderSelectDialog();
        string[] ShowFileSelectDialog();

        event EventHandler Refreshed;

        event EventHandler UndoClick;
        event EventHandler RedoClick;

        event ItemActionEventHandler BuildItemClick;
        event ItemActionEventHandler ShowInExplorerItemClick;
        event ItemAddActionEventHandler AddItemClick;
        event ItemActionEventHandler RemoveItemClick;
        event ItemActionEventHandler RenameItemClick;

        event EventHandler OnShellLoad;

        event EventHandler RebuildClick;
        event EventHandler CleanClick;

        event FolderAddActionEventHandler AddExistingFolderClick;
        event FolderAddActionEventHandler AddNewFolderClick;
        event FolderAddActionEventHandler AddExistingItemClick;
        event ItemAddActionEventHandler AddNewItemClick;

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
