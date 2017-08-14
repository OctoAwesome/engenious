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
        private Arguments arguments;

        public MainShellPresenter(IMainShell shell, Arguments arguments)
        {
            this.shell = shell;
            this.arguments = arguments;

            shell.CloseProjectClick += (i) => CloseProject();

            shell.ShowInExplorerItemClick += Shell_ShowInExplorerItemClick;
            shell.SaveProjectClick += (i) => SaveProject();
            shell.OpenProjectClick += (s, e) => { if (CloseProject()) OpenProject(); };
            shell.BuildItemClick += Shell_BuildItemClick;
            shell.OnItemSelect += Shell_OnItemSelect;

            shell.UndoClick += ShellOnUndoClick;
            shell.RedoClick += ShellOnRedoClick;

            shell.RenameItemClick += (i) => shell.RenameItem(i);
            shell.RemoveItemClick += Shell_RemoveItemClick;
            shell.OnAboutClick += (s, e) => shell.ShowAbout();

            shell.AddExistingItemClick += Shell_AddExistingItemClick;
            shell.AddExistingFolderClick += Shell_AddExistingFolderClick;
            shell.AddNewFolderClick += Shell_AddNewFolderClick;

            shell.OnShellLoad += Shell_OnShellLoad;

            viewerManager = new ViewerManager();

        }

        private void Shell_RemoveItemClick(ContentItem item)
        {
            var folder = item.Parent as ContentFolder;
            if (folder != null)
                folder.Content.Remove(item);
        }

        private void Shell_OnShellLoad(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(arguments.ContentProject))//TODO perhaps use laodi
                OpenProject(@"D:\Projects\engenious\Sample\Content\Content.ecp");
            else
                OpenProject(arguments.ContentProject);
        }

        private void Shell_AddNewFolderClick(ContentFolder folder)
        {
            if (folder == null)
                return;
            var numbers = folder.Content.Where(t => t.Name.StartsWith("New Folder")).Select(t =>
            {
                if (t.Name == "New Folder")
                    return 1;

                var str = t.Name.Substring("New Folder ".Length);
                if (int.TryParse(str, out var num))
                    return num;
                else
                    return -1;
            }).ToArray();
            var max = numbers.Length == 0 ? -1 : numbers.Max();
            var newFolder = new ContentFolder("New Folder" + (max == -1 ? string.Empty : $" {max + 1}"), folder);
            Directory.CreateDirectory(newFolder.FilePath);
            folder.Content.Add(newFolder);
            shell.RenameItem(newFolder);

        }


        private void Shell_AddExistingFolderClick(ContentFolder fld)
        {
            var dest = fld.FilePath;
            var src = shell.ShowFolderSelectDialog();
            if (src == null)
                return;

            shell.ShowLoading();
            shell.SuspendRendering();

            var t = new Thread(() =>
            {

                FileHelper.CopyDirectory(src, dest, fld);
                shell.Invoke(new MethodInvoker(() =>
                {
                    shell.ResumeRendering();
                    shell.HideLoading();
                }));
            });
            t.Start();

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
            if (item.Error.HasFlag(ContentErrorType.NotFound) && shell.ShowNotFoundDelete())
            {
                ContentFolder p = (ContentFolder)item.Parent;
                p.Content.Remove(item);
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

            shell.ShowLoading("Loading Project...");

            var t = new Thread(() =>
            {
                try
                {
                    ContentProject proj = ContentProject.Load(path);

                    shell.Invoke(new MethodInvoker(() =>
                    {
                        shell.Project = proj;
                        shell.WriteLineLog("Opened " + path);
                    }));


                }
                catch (Exception e)
                {

                }
                finally
                {
                    shell.Invoke(new MethodInvoker(() => shell.HideLoading()));
                }
            });
            t.Start();

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
