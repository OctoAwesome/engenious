using ContentTool.Forms.Dialogs;
using ContentTool.Models;
using ContentTool.Presenters;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public ContentProject Project
        {
            get => project;
            set
            {
                if (project == value)
                    return;

                if (project != null)
                {
                    project.History.HistoryChanged -= HistoryOnHistoryChanged;
                    project.CollectionChanged -= ProjectOnCollectionChanged;
                }

                project = value;
                if (project != null)
                {
                    project.History.HistoryChanged += HistoryOnHistoryChanged;
                    project.CollectionChanged += ProjectOnCollectionChanged;
                }
                projectTreeView.Project = Project;
            }
        }


        private void HistoryOnHistoryChanged(object sender, EventArgs eventArgs)
        {
            if (IsRenderingSuspended)
                return;
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => HistoryOnHistoryChanged(sender, eventArgs)));
                return;
            }
            undoToolStripMenuItem.Enabled = Project?.History?.CanUndo ?? false;
            redoToolStripMenuItem.Enabled = Project?.History?.CanRedo ?? false;
            itemPropertyView.Refresh();
        }
        private void ProjectOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (!IsRenderingSuspended)
                projectTreeView.RecalculateView();//TODO: use diffs, probably better for keeping the same states(opened subtrees e.g.)
        }

        private ContentProject project;

        private LoadingDialog loadingDialog = new LoadingDialog();
        private Timer loadingTimer = new Timer();

        public MainShell()
        {
            InitializeComponent();
        }

        public event EventHandler OnShellLoad;

        protected override void OnLoad(EventArgs ea)
        {
            base.OnLoad(ea);

            ShowItemButtons(false);

            projectTreeView.Shell = this;
            projectTreeView.SelectedContentItemChanged += ProjectTreeView_SelectedContentItemChanged;
            projectTreeView.AddItemClick += (i, t) => AddItemClick?.Invoke(i, t);
            projectTreeView.BuildItemClick += (i) => BuildItemClick?.Invoke(i);
            projectTreeView.RemoveItemClick += (i) => { RemoveItemClick?.Invoke(i); };
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

            toolStripButton_existingItemAdd.Click += (s, e) => AddExistingItemClick?.Invoke(projectTreeView.SelectedFolder);
            toolStripButton_existingFolderAdd.Click += (s, e) => AddExistingFolderClick?.Invoke(projectTreeView.SelectedFolder);
            toolStripButton_newFolderAdd.Click += (s, e) => AddNewFolderClick?.Invoke(projectTreeView.SelectedFolder);
            //toolStripButton_newItemAdd.Click += (s, e) => AddNewItemClick?.Invoke(projectTreeView.SelectedFolder, baaah);

            aboutToolStripMenuItem1.Click += (s, e) => { OnAboutClick?.Invoke(this, EventArgs.Empty); };
            helpToolStripMenuItem.Click += (s, e) => OnHelpClick?.Invoke(this, EventArgs.Empty);

            newFolderToolStripMenuItem.Click += (s, e) => AddNewFolderClick?.Invoke(projectTreeView.SelectedFolder);
            removeToolStripMenuItem.Click += (s, e) => RemoveItemClick?.Invoke(projectTreeView.SelectedItem);
            renameToolStripMenuItem.Click += (s, e) => RenameItemClick?.Invoke(projectTreeView.SelectedItem);

            undoToolStripMenuItem.Click += (s, e) => UndoClick?.Invoke(this, EventArgs.Empty);
            redoToolStripMenuItem.Click += (s, e) => RedoClick?.Invoke(this, EventArgs.Empty);
            editToolStripMenuItem.DropDownOpening += EditToolStripMenuItemOnDropDownOpening;

            projectTreeView.SelectedContentItemChanged += (i) => OnItemSelect?.Invoke(i);
            projectTreeView.Refreshed += (s, e) => Refreshed?.Invoke(this, e);

            alwaysShowLogToolStripMenuItem.CheckedChanged += (s, e) => { if (alwaysShowLogToolStripMenuItem.Checked) splitContainer_right.Panel2Collapsed = false; else splitContainer_right.Panel2Collapsed = true; };
        }

        public void ShowItemButtons(bool value)
        {
            removeToolStripMenuItem.Enabled = value;
            renameToolStripMenuItem.Enabled = value;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            OnShellLoad?.Invoke(this, EventArgs.Empty);
        }

        private void EditToolStripMenuItemOnDropDownOpening(object sender, EventArgs eventArgs)
        {
            undoToolStripMenuItem.Enabled = Project?.History?.CanUndo ?? false;
            redoToolStripMenuItem.Enabled = Project?.History?.CanRedo ?? false;
        }

        private void ProjectTreeView_SelectedContentItemChanged(ContentItem newItem)
        {
            ShowItemButtons(!(newItem == null || newItem is ContentProject));
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

        public void ShowLoading(string title = "Please wait...")
        {
            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar.Visible = true;

            loadingDialog = new LoadingDialog();
            loadingDialog.Title = title;
            loadingTimer.Tick += (s, e) =>
            {
                loadingTimer.Stop();
                if (loadingDialog.Visible) return;
                this.Enabled = false;
                loadingDialog.Show(this);
            };
            loadingTimer.Interval = 400;
            loadingTimer.Start();

        }

        public void HideLoading()
        {
            toolStripProgressBar.Visible = false;
            loadingTimer.Stop();

            loadingDialog.Close();
            this.Enabled = true;
            this.Focus();
        }

        void IMainShell.Invoke(Delegate d) => Invoke(d);
        void IMainShell.BeginInvoke(Delegate d) => BeginInvoke(d);

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

        public void Refresh()
        {
            if (!IsRenderingSuspended)
                projectTreeView.RecalculateView();
        }

        public void ShowAbout() => new AboutBox().ShowDialog();

        public bool ShowNotFoundDelete()
            => (MessageBox.Show("This file could not be found. " + Environment.NewLine + "Do you want to remove it from the Project?", "File not found!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes);

        public event EventHandler Refreshed;

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
        public event Delegates.FolderAddActionEventHandler AddExistingFolderClick;
        public event Delegates.FolderAddActionEventHandler AddNewFolderClick;
        public event Delegates.FolderAddActionEventHandler AddExistingItemClick;
        public event Delegates.ItemAddActionEventHandler AddNewItemClick;
        public event EventHandler OnAboutClick;
        public event EventHandler OnHelpClick;

        public event EventHandler UndoClick;
        public event EventHandler RedoClick;
        public event Delegates.ItemActionEventHandler RenameItemClick;

        private void MainShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Project != null && Project.HasUnsavedChanges)
            {
                if (!ShowCloseWithoutSavingConfirmation())
                    e.Cancel = true;
            }
        }

        public string ShowFolderSelectDialog()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Project.FilePath;
                if (fbd.ShowDialog() == DialogResult.OK)
                    return fbd.SelectedPath;
            }
            return null;
        }

        public string[] ShowFileSelectDialog()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = Project.FilePath;
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                    return ofd.FileNames;
            }
            return null;
        }

        private int _suspendCount;
        public bool IsRenderingSuspended => _suspendCount > 0;

        public void SuspendRendering()
        {
            System.Threading.Interlocked.Increment(ref _suspendCount);
        }

        public void ResumeRendering()
        {
            lock (this)
            {
                if (_suspendCount == 0)
                {
                    projectTreeView.RecalculateView();
                    return;
                }
                _suspendCount--;
            }
        }

        public void RenameItem(ContentItem item) => projectTreeView.EditItem(item);
        public void RemoveItem(ContentItem item) => projectTreeView.RemoveItemRequest(projectTreeView.SelectedItem);

    }
}
