using ContentTool.Forms;
using ContentTool.Models;
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

        public MainShellPresenter(IMainShell shell)
        {
            this.shell = shell;

            shell.CloseProjectClick += (i) => CloseProject();

            shell.ShowInExplorerItemClick += Shell_ShowInExplorerItemClick;
            shell.SaveProjectClick += (i) => SaveProject();
            shell.OpenProjectClick += (s, e) => { if (CloseProject()) OpenProject(); };

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
                shell.HideLoading();


            }
            catch(Exception e) { }
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
