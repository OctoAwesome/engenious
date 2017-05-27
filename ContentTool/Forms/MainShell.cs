using ContentTool.Models;
using ContentTool.Presenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTool.Forms
{
    public partial class MainShell : Form, IMainShell
    {
        public ContentProject Project {
            get => project;
            set
            {
                if (project == value)
                    return;

                project = value;
                projectTreeView.Project = Project;
            }
        }

        private ContentProject project;

        public MainShell()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs ea)
        {
            base.OnLoad(ea);
            projectTreeView.SelectedContentItemChanged += ProjectTreeView_SelectedContentItemChanged;
            projectTreeView.AddItemClick += (i, t) => AddItemClick?.Invoke(i, t);
            projectTreeView.BuildItemClick += (i) => BuildItemClick?.Invoke(i);
            projectTreeView.RemoveItemClick += (i) => RemoveItemClick?.Invoke(i);
            projectTreeView.ShowInExplorerItemClick += (i) => ShowInExplorerItemClick?.Invoke(i);

            newToolStripMenuItem.Click += (s, e) => NewProjectClick?.Invoke(this, null);
            openToolStripMenuItem.Click += (s, e) => OpenProjectClick?.Invoke(this, null);
            closeProjectToolStripMenuItem.Click += (s, e) => CloseProjectClick?.Invoke(Project);
            saveProjectToolStripMenuItem.Click += (s, e) => SaveProjectClick?.Invoke(Project);
            saveProjectAsToolStripMenuItem.Click += (s, e) => SaveProjectAsClick?.Invoke(Project);

            toolStripButton_new.Click += (s, e) => NewProjectClick?.Invoke(this, null);
            toolStripButton_open.Click += (s, e) => OpenProjectClick?.Invoke(this, null);
            toolStripButton_save.Click += (s, e) => SaveProjectClick?.Invoke(Project);

            toolStripButton_build.Click += (s, e) => BuildItemClick?.Invoke(Project);
            //toolStripButton_clean.Click += (s,e) => Clea

            alwaysShowLogToolStripMenuItem.CheckedChanged += (s, e) => { if (alwaysShowLogToolStripMenuItem.Checked) splitContainer_right.Panel2Collapsed = false; else splitContainer_right.Panel2Collapsed = true; };
        }

        private void ProjectTreeView_SelectedContentItemChanged(ContentItem newItem)
        {
            itemPropertyView.SelectItem(newItem);
        }

        public bool ShowCloseWithoutSavingConfirmation()
        {
            var result = MessageBox.Show($"There are unsaved changes. {Environment.NewLine} Do you want to save the project before closing it?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Cancel)
                return false;
            else if (result == DialogResult.Yes)
            {
                SaveProjectClick?.Invoke(Project);
                return true;
            }
            else
                return true;
        }

        public string ShowOpenDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Engenious Content Project(.ecp)|*.ecp";
            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileName;
            return null;
        }

        public string ShowSaveAsDialog()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Engenious Content Project(.ecp)|*.ecp";
                sfd.FileName = Project.ContentProjectPath;
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                    return sfd.FileName;
                return null;
            }
        }

        public void WriteLog(string text, Color color = default(Color)) => consoleView.Write(text, color);
        public void WriteLineLog(string text, Color color = default(Color)) => consoleView.WriteLine(text, color);
        public void ClearLog() => consoleView.Clear();

        public void ShowLoading()
        {
            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar.Visible = true;
        }

        public void HideLoading() => toolStripProgressBar.Visible = false;

        void IMainShell.Invoke(Delegate d) => Invoke(d);

        public event Delegates.ItemActionEventHandler BuildItemClick;
        public event Delegates.ItemActionEventHandler ShowInExplorerItemClick;
        public event Delegates.ItemAddActionEventHandler AddItemClick;
        public event Delegates.ItemActionEventHandler RemoveItemClick;

        public event EventHandler NewProjectClick;
        public event EventHandler OpenProjectClick;
        public event Delegates.ItemActionEventHandler CloseProjectClick;
        public event Delegates.ItemActionEventHandler SaveProjectClick;
        public event Delegates.ItemActionEventHandler SaveProjectAsClick;
    }
}
