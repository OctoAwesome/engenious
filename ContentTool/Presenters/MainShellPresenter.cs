using ContentTool.Builder;
using ContentTool.Forms;
using ContentTool.Models;
using ContentTool.Viewer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTool.Presenters
{
    class MainShellPresenter
    {
        private IMainShell shell;

        private ContentBuilder builder;

        private ViewerManager viewerManager;

        public MainShellPresenter(IMainShell shell)
        {
            this.shell = shell;

            shell.CloseProjectClick += (i) => CloseProject();

            shell.ShowInExplorerItemClick += Shell_ShowInExplorerItemClick;
            shell.SaveProjectClick += (i) => SaveProject();
            shell.OpenProjectClick += (s, e) => { if (CloseProject()) OpenProject(); };
            shell.BuildItemClick += Shell_BuildItemClick;
            shell.OnItemSelect += Shell_OnItemSelect;
            shell.RemoveItemClick += Shell_RemoveItemClick;
            
            shell.UndoClick += ShellOnUndoClick;
            shell.RedoClick += ShellOnRedoClick;
            
            shell.RemoveItemClick += item => (item?.Parent as ContentFolder)?.Content.Remove(item);

            shell.OnAboutClick += (s, e) => shell.ShowAbout();

            shell.AddExistingItemClick += Shell_AddExistingItemClick;
            shell.AddExistingFolderClick += Shell_AddExistingFolderClick;

            viewerManager = new ViewerManager();

        }

        private void Shell_AddExistingFolderClick(ContentItem item)
        {
            var fld = (item as ContentFolder) ?? (item?.Parent as ContentFolder) ?? shell.Project;

            var dest = fld.FilePath;
            var src = shell.ShowFolderSelectDialog();
            if (src == null)
                return;

            shell.SuspendRendering();
            FileHelper.CopyDirectory(src, dest, fld);
            shell.ResumeRendering();
        }

       
        private void Shell_AddExistingItemClick(ContentItem item)
        {
            var fld = (item as ContentFolder) ?? (item?.Parent as ContentFolder) ?? shell.Project;


            string[] files = shell.ShowFileSelectDialog();
            if (files == null)
                return;

            string dir = fld.FilePath;
            shell.SuspendRendering();
            FileHelper.CopyFiles(files, dir, fld);
            shell.ResumeRendering();
        }

        private void Shell_RemoveItemClick(ContentItem item)
        {
            var parent = (ContentFolder) item.Parent;
            parent.Content.Remove(item);
            shell.Refresh();
        }

        private void ShellOnRedoClick(object sender, EventArgs eventArgs)
        {
            var history = shell.Project?.History;
            if (history == null || !history.CanRedo)
                return;
            history.Redo();
        }

        private void ShellOnUndoClick(object sender, EventArgs eventArgs)
        {
            var history = shell.Project?.History;
            if (history == null || !history.CanUndo)
                return;
            history.Undo();
        }

        private void Shell_OnItemSelect(ContentItem item)
        {
            if(item.Error.HasFlag(ContentErrorType.NotFound) && shell.ShowNotFoundDelete())
            {
                ContentFolder p = (ContentFolder)item.Parent;
                p.Content.Remove(item);
                shell.Refresh();
            }

            if (item is ContentFile)
                shell.ShowViewer(viewerManager.GetViewer(item as ContentFile));
            else
                shell.HideViewer();
        }

        private void Shell_BuildItemClick(ContentItem item)
        {
            if (builder == null)
            {
                builder = new ContentBuilder(shell.Project);
                builder.BuildMessage += (a) => shell.Invoke(((MethodInvoker)(() => shell.WriteLineLog(a.Message))));
     
       }
            shell.ShowLog();

            builder.Build(item);
        }

        public bool CloseProject()
        {
            if (shell.Project == null)
                return true;

            if (shell.Project.HasUnsavedChanges)
            {
                if (!shell.ShowCloseWithoutSavingConfirmation())
                    return false;
            }

            shell.Project = null;
            shell.ClearLog();
            return true;
        }

        public async void OpenProject(string path = null)
        {
            if (path == null)
                path = shell.ShowOpenDialog();
            if (path == null)
                return;

            try
            {
                shell.ShowLoading();

                ContentProject proj = ContentProject.Load(path);

                shell.Project = proj;
                shell.WriteLineLog("Opened " + path);


            }
            catch (Exception e)
            {
            }
            finally
            {
                shell.HideLoading();
            }
                
        }

        public void SaveProject(string path = null)
        {
            if (path == null)
                shell.Project.Save();
            else
                shell.Project.Save(path);
        }

        private void Shell_ShowInExplorerItemClick(Models.ContentItem item)
        {
            var path = item.FilePath;
            if (item is ContentFile)
                path = Path.GetDirectoryName(path);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = path,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void Shell_CloseProjectClick(Models.ContentItem item)
        {
            CloseProject();
        }
    }
}
