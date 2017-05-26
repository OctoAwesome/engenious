using ContentTool.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentTool.Presenters
{
    class MainShellPresenter
    {
        private IMainShell shell;

        public MainShellPresenter(IMainShell shell)
        {
            this.shell = shell;

            shell.ShowInExplorerItemClick += (i) => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = i.FilePath,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
