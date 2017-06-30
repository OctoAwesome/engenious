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
                
                if (project != null)
                    project.History.HistoryChanged -= HistoryOnHistoryChanged;

                project = value;
                project.History.HistoryChanged += HistoryOnHistoryChanged;
                projectTreeView.Project = Project;
            }
        }

        private void HistoryOnHistoryChanged(object sender, EventArgs eventArgs)
        {
            undoToolStripMenuItem.Enabled = Project?.History?.CanUndo ?? false;
            redoToolStripMenuItem.Enabled = Project?.History?.CanRedo ?? false;
            itemPropertyView.Refresh();
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

            newToolStripMenuItem.Click += (s, e) => NewProjectClick?.Invoke(this, EventArgs.Empty);
            openToolStripMenuItem.Click += (s, e) => OpenProjectClick?.Invoke(this, EventArgs.Empty);
            closeProjectToolStripMenuItem.Click += (s, e) => CloseProjectClick?.Invoke(Project);
            saveProjectToolStripMenuItem.Click += (s, e) => SaveProjectClick?.Invoke(Project);
            saveProjectAsToolStripMenuItem.Click += (s, e) => SaveProjectAsClick?.Invoke(Project);

            toolStripButton_new.Click += (s, e) => NewProjectClick?.Invoke(this, EventArgs.Empty);
            toolStripButton_open.Click += (s, e) => OpenProjectClick?.Invoke(this, EventArgs.Empty);
            toolStripButton_save.Click += (s, e) => SaveProjectClick?.Invoke(Project);

            toolStripButton_build.Click += (s, e) => BuildItemClick?.Invoke(Project);
            toolStripButton_clean.Click += (s, e) => CleanClick?.Invoke(this, EventArgs.Empty);

            buildToolStripMenuItem1.Click += (s, e) => BuildItemClick?.Invoke(Project);
            rebuildToolStripMenuItem.Click += (s, e) => RebuildClick?.Invoke(this, EventArgs.Empty);
            cleanToolStripMenuItem.Click += (s, e) => CleanClick?.Invoke(this, EventArgs.Empty);
            //toolStripButton_clean.Click += (s,e) => Clea

            aboutToolStripMenuItem1.Click += (s, e) => { ShowAbout(); OnAboutClick?.Invoke(this, EventArgs.Empty); };
            helpToolStripMenuItem.Click += (s, e) => OnHelpClick?.Invoke(this, EventArgs.Empty);

            //TODO: removeToolStripMenuItem.Click += (s,e) => RemoveItemClick?.Invoke()
            undoToolStripMenuItem.Click += (s, e) => UndoClick?.Invoke(this, EventArgs.Empty);
            redoToolStripMenuItem.Click += (s, e) => RedoClick?.Invoke(this, EventArgs.Empty);
            editToolStripMenuItem.DropDownOpening += EditToolStripMenuItemOnDropDownOpening;

            projectTreeView.SelectedContentItemChanged += (i) => OnItemSelect?.Invoke(i);

            alwaysShowLogToolStripMenuItem.CheckedChanged += (s, e) => { if (alwaysShowLogToolStripMenuItem.Checked) splitContainer_right.Panel2Collapsed = false; else splitContainer_right.Panel2Collapsed = true; };
        }

        private void EditToolStripMenuItemOnDropDownOpening(object sender, EventArgs eventArgs)
        {
            undoToolStripMenuItem.Enabled = Project?.History?.CanUndo ?? false;
            redoToolStripMenuItem.Enabled = Project?.History?.CanRedo ?? false;
        }

        private void ProjectTreeView_SelectedContentItemChanged(ContentItem newItem) => itemPropertyView.SelectItem(newItem);


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

        public void ShowViewer(Control viewer)
        {
            splitContainer_right.Panel1.Controls.Clear();

            if (viewer == null)
                return;
            viewer.Dock = DockStyle.Fill;
            splitContainer_right.Panel1.Controls.Add(viewer);
        }

        public void HideViewer() => splitContainer_right.Panel1.Controls.Clear();

        public void ShowLog() => splitContainer_right.Panel2Collapsed = false;
        public void HideLog()
        {
            if (alwaysShowLogToolStripMenuItem.Checked == false)
                splitContainer_right.Panel2Collapsed = true;
        }

        public void Refresh() => projectTreeView.RecalculateView();

        public void ShowAbout() => new AboutBox().ShowDialog();

        public bool ShowNotFoundDelete()
            => (MessageBox.Show("This file could not be found. " + Environment.NewLine + "Do you want to remove it from the Project?", "File not found!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes);

        public event Delegates.ItemActionEventHandler BuildItemClick;
        public event Delegates.ItemActionEventHandler ShowInExplorerItemClick;
        public event Delegates.ItemAddActionEventHandler AddItemClick;
        public event Delegates.ItemActionEventHandler RemoveItemClick;

        public event EventHandler NewProjectClick;
        public event EventHandler OpenProjectClick;
        public event Delegates.ItemActionEventHandler CloseProjectClick;
        public event Delegates.ItemActionEventHandler SaveProjectClick;
        public event Delegates.ItemActionEventHandler SaveProjectAsClick;
        public event Delegates.ItemActionEventHandler OnItemSelect;
        public event EventHandler RebuildClick;
        public event EventHandler CleanClick;
        public event Delegates.ItemActionEventHandler AddExistingFolderClick;
        public event Delegates.ItemActionEventHandler AddNewFolderClick;
        public event Delegates.ItemActionEventHandler AddNewItemClick;
        public event Delegates.ItemActionEventHandler AddExistingItemClick;
        public event EventHandler OnAboutClick;
        public event EventHandler OnHelpClick;

        private void MainShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Project != null && Project.HasUnsavedChanges)
            {
                if (!ShowCloseWithoutSavingConfirmation())
                    e.Cancel = true;
            }
        }

        public string ShowFolderSelectDialog()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = Project.FilePath;
            if (fbd.ShowDialog() == DialogResult.OK)
                return fbd.SelectedPath;

            return null;
        }
        
        public event EventHandler UndoClick;
        public event EventHandler RedoClick;
    }
}
