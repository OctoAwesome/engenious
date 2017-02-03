using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ContentTool.Builder;
using ContentTool.Commands;
using ContentTool.Items;
using engenious.Content.Pipeline;

namespace ContentTool
{
    public partial class FrmMain : Form
    {
        private ContentProject _currentProject;

        private ContentProject CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value;
                if (_builder != null)
                {
                    _builder.BuildStatusChanged -= Builder_BuildStatusChanged;
                    _builder.ItemProgress -= Builder_ItemProgress;
                    _builder.BuildMessage -= Builder_BuildMessage;
                }
                _builder = new ContentBuilder(value);
                _builder.BuildStatusChanged += Builder_BuildStatusChanged;
                _builder.ItemProgress += Builder_ItemProgress;
                _builder.BuildMessage += Builder_BuildMessage;
                PipelineHelper.PreBuilt(_currentProject);
            }
        }

        private ContentBuilder _builder;

        private readonly Dictionary<ContentItem, TreeNode> _treeMap;

        public FrmMain()
        {
            InitializeComponent();

            PipelineHelper.DefaultInit();

            _builder = new ContentBuilder(null);
            _builder.BuildStatusChanged += Builder_BuildStatusChanged;
            _builder.ItemProgress += Builder_ItemProgress;
            _builder.BuildMessage += Builder_BuildMessage;
            _treeMap = new Dictionary<ContentItem, TreeNode>();

            treeContentFiles.NodeMouseClick += TreeContentFilesOnNodeMouseClick;

            newFolderMenuItem.Click += (o, e) => AddNewFolder.Execute(GetSelectedItem(), CurrentProject.File);
            existingFolderMenuItem.Click += (o,e) => AddExistingFolder.Execute(GetSelectedItem(), CurrentProject.File);
            existingItemMenuItem.Click += (o, e) => AddExistingItem.Execute(GetSelectedItem(), CurrentProject.File);
            deleteMenuItem.Click += (o,e) => DeleteItem.Execute(GetSelectedItem());
            deleteToolStripMenuItem.Click += (o,e) => DeleteItem.Execute(GetSelectedItem());
            deleteToolStripMenuItem1.Click += (o, e) => DeleteItem.Execute(GetSelectedItem());
            renameMenuItem.Click += (o, e) => RenameItem.Execute(GetSelectedItem(), _treeMap);
            renameToolStripMenuItem.Click += (o, e) => RenameItem.Execute(GetSelectedItem(), _treeMap);
            renameToolStripMenuItem1.Click += (o, e) => RenameItem.Execute(GetSelectedItem(), _treeMap);
            buildToolStripMenuItem.Click += (o, e) => BuildItem.Execute(GetSelectedItem(), _builder);
            buildToolStripMenuItem1.Click += (o, e) => BuildItem.Execute(GetSelectedItem(), _builder);
            buildToolStripMenuItem2.Click += (o, e) => BuildItem.Execute(GetSelectedItem(), _builder);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            foreach (string file in Environment.GetCommandLineArgs().Skip(1))
            {
                if (Path.GetExtension(file) == ".ecp" && File.Exists(file))
                {
                    OpenFile(file);
                    return;
                }
            }

            if (File.Exists(Program.Configuration.LastFile))
                OpenFile(Program.Configuration.LastFile);
        }

        #region Build Events

        private void Builder_BuildMessage(object sender, BuildMessageEventArgs e)
        {

            Log(Program.MakePathRelative(e.FileName) + " " + e.Message, e.MessageType);

        }

        private void Builder_ItemProgress(object sender, ItemProgressEventArgs e)
        {
            string message = e.Item + " " + (e.BuildStep & (BuildStep.Build | BuildStep.Clean)).ToString().ToLower() + "ing ";

            var type = BuildMessageEventArgs.BuildMessageType.Information;

            if ((e.BuildStep & BuildStep.Abort) == BuildStep.Abort)
            {
                message += "failed!";
                type = BuildMessageEventArgs.BuildMessageType.Error;
            }
            else if ((e.BuildStep & BuildStep.Finished) == BuildStep.Finished)
            {
                message += "finished!";
            }
            Log(message, type);
        }

        private void Builder_BuildStatusChanged(object sender, BuildStep buildStep)
        {
            string message = (buildStep & (BuildStep.Build | BuildStep.Clean)) + " ";
            BuildMessageEventArgs.BuildMessageType type = BuildMessageEventArgs.BuildMessageType.Information;
            if ((buildStep & BuildStep.Abort) == BuildStep.Abort)
            {
                message += "aborted!";
                type = BuildMessageEventArgs.BuildMessageType.Warning;
            }
            else if ((buildStep & BuildStep.Finished) == BuildStep.Finished)
            {
                message += "finished!";
                if (_builder.FailedBuilds != 0)
                {
                    message += " " + _builder.FailedBuilds + " files failed to build!";
                }
            }
            Log(message, type);
        }

        #endregion

        #region Project Events

        private void CurrentProject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ContentItem item = sender as ContentItem;
            if (item == null)
                return;
            if (e.PropertyName == "Name")
            {
                TreeNode node;
                _treeMap.TryGetValue(item, out node);
                if (node == null)
                    return;
                node.Text = item.Name;
            }
        }

        private void CurrentProject_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ContentItem item = (e.NewItems == null || e.NewItems.Count == 0) ? null : (e.NewItems[0] as ContentItem);
            if (item == null)
                item = (e.OldItems == null || e.OldItems.Count == 0) ? null : (e.OldItems[0] as ContentItem);
            TreeNode parentNode;
            if (item == null || !_treeMap.TryGetValue(item.Parent, out parentNode))
                return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var node = new TreeNode(item.Name) { Tag = item };
                        if (
                            item.Parent?.Contents?.FirstOrDefault(
                                x =>
                                    x != item &&
                                    Path.GetFileNameWithoutExtension(item.Name) ==
                                    Path.GetFileNameWithoutExtension(x.Name)) != null)
                        {
                            item.Parent?.Contents.Remove(item);
                            MessageBox.Show("There is already an item with that name in this folder!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        node.SelectedImageKey = node.ImageKey = GetImageKey(item);
                        AddContextMenu(node);

                        _treeMap.Add(item, node);
                        if (parentNode == null)
                            treeContentFiles.Nodes.Add(node);
                        else
                        {
                            parentNode.Nodes.Add(node);
                            parentNode.Expand();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        TreeNode node;
                        if (_treeMap.TryGetValue(item, out node))
                        {
                            if (parentNode == null)
                                treeContentFiles.Nodes.Remove(node);
                            else
                                parentNode.Nodes.Remove(node);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        ContentItem oldItem = (e.OldItems == null || e.OldItems.Count == 0) ? null : (e.OldItems[0] as ContentItem);
                        TreeNode node;
                        _treeMap.TryGetValue(oldItem, out node);
                        if (parentNode == null)
                            treeContentFiles.Nodes.Remove(node);
                        else
                            parentNode.Nodes.Remove(node);

                        node = new TreeNode(item.Name) { Tag = item };
                        node.SelectedImageKey = node.ImageKey = GetImageKey(item);
                        _treeMap.Add(item, node);
                        if (parentNode == null)
                            treeContentFiles.Nodes.Add(node);
                        else
                        {
                            parentNode.Nodes.Add(node);
                            parentNode.Expand();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        treeContentFiles.Nodes[0].Nodes.Clear();
                        _treeMap.Clear();
                        break;
                    }
            }
        }

        #endregion

        #region Tree Events

        private void TreeContentFilesOnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs treeNodeMouseClickEventArgs)
        {
            if (treeNodeMouseClickEventArgs.Button == MouseButtons.Right)
                treeContentFiles.SelectedNode = treeNodeMouseClickEventArgs.Node;
        }

        private void treeContentFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            prpItem.SelectedObject = e.Node.Tag;
            panel_editor.Controls.Clear();
            ContentFile file =  e.Node.Tag as ContentFile;
            if (file == null || file.Importer == null || file.Processor == null)
                return;
            var editorWrap = PipelineHelper.GetContentEditor(Path.GetExtension(file.Name), file.Importer.ExportType, file.Processor.ExportType);
            if (editorWrap == null)
                return;
            var absPath = Path.Combine(Path.GetDirectoryName(CurrentProject.File), file.GetPath());
            var importValue = file.Importer.Import(absPath, new ContentImporterContext());
            var processValue = file.Processor.Process(importValue, absPath,
                new ContentProcessorContext(_builder.UiContext));
            editorWrap.Open(importValue, processValue);
            panel_editor.Controls.Add(editorWrap.Editor.MainControl);
        }

        private void TreeContentFiles_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            ContentItem item = null;
            if (e.Node != null && e.Node.Tag != null)
                item = e.Node.Tag as ContentItem;
            if (item == null)
            {
                e.CancelEdit = true;
                return;
            }
            if (item is ContentProject)
                e.CancelEdit = true;
        }

        private void TreeContentFiles_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.CancelEdit = true;
            //TODO: implement
            /*ContentItem item = null;
            if (e.Node != null && e.Node.Tag != null)
                item = e.Node.Tag as ContentItem;
            if (item == null)
            {
                e.CancelEdit = true;
                return;
            }
            string absoluteFolder = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(currentFile), item.Parent.getPath());
            string absoluteItem = System.IO.Path.Combine(absoluteFolder, item.Name);
            string newAbsItem = System.IO.Path.Combine(absoluteFolder, e.Label);
            if (System.IO.Directory.Exists(absoluteItem))
            {
                System.IO.Directory.Move(absoluteItem, newAbsItem);
                item.Name = e.Label;
            }
            else if (System.IO.File.Exists(absoluteItem))
            {
                System.IO.File.Move(absoluteItem, newAbsItem);

                item.Name = e.Label;
            }
            else
            {
                e.CancelEdit = true;
            }*/

        }

        #endregion

        #region File Menu

        private void FileMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            bool fileOpened = CurrentProject != null;
            importMenuItem.Enabled = false;
            closeMenuItem.Enabled = saveAsMenuItem.Enabled = fileOpened;
            saveMenuItem.Enabled = fileOpened;//TODO änderungen?

        }


        private void SaveAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();

        }

        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Engenious Content Project(.ecp)|*.ecp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(ofd.FileName);
                }
            }
        }

        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            CloseMenuItem_Click(sender, e);

            CreateProject();
        }

        private void ExitMenuItem_Click(object sender, EventArgs args)
        {
            Close();
        }

        #endregion

        #region Build Menu

        private void CleanMenuItem_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";

            _builder.Clean();
        }

        private void RebuildMenuItem_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";
            _builder.Rebuild();
        }

        private void BuildMenuItem_Click(object sender, EventArgs e)
        {
            if (!SaveFirst())
                return;
            txtLog.Text = "";
            _builder.Build();
        }

        private void CancelMenuItem_Click(object sender, EventArgs e)
        {
            _builder.Abort();
        }


        private void BuildMainMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            cleanMenuItem.Enabled = _builder.CanClean;
            rebuildMenuItem.Enabled = File.Exists(CurrentProject.File) && cleanMenuItem.Enabled;
        }

        #endregion

        #region Edit Menu

        private void RedoMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UndoMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Context Events

        private void AddContextMenu(TreeNode node)
        {
            var item = node.Tag;
            if (item is ContentProject)
                node.ContextMenuStrip = contextMenuStrip_project;
            else if (item is ContentFolder)
                node.ContextMenuStrip = contextMenuStrip_folder;
            else if (item is ContentFile)
                node.ContextMenuStrip = contextMenuStrip_file;
        }
        private void ContextMenu_ShowInExplorer(object sender, EventArgs e)
        {
            var contentItem = GetSelectedItem();
            string path = Path.Combine(Path.GetDirectoryName(_currentProject.File), contentItem.GetPath());
            if (Directory.Exists(path))
                Process.Start(path);
            else
                Process.Start(Path.GetDirectoryName(path));
        }

        private void ContextMenu_Close(object sender, EventArgs e)
        {
            CloseFile();
        }



        #endregion


        #region Tree

        /// <summary>
        /// Recalculates the TreeView
        /// </summary>
        private void RecalcTreeView()
        {
            _treeMap.Clear();
            treeContentFiles.Nodes.Clear();

            if (CurrentProject == null)
            {
                buildMainMenuItem.Enabled = false;
                return;
            }
            buildMainMenuItem.Enabled = true;

            var rootNode = new TreeNode(CurrentProject.Name) { Tag = CurrentProject };
            AddContextMenu(rootNode);
            treeContentFiles.Nodes.Add(rootNode);
            _treeMap.Add(CurrentProject, rootNode);
            AddTreeNode(CurrentProject, rootNode);
            rootNode.Expand();
        }

        /// <summary>
        /// Returns the ImageKey for the given ContentItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string GetImageKey(ContentItem item)
        {
            if (item is ContentProject)
                return "project";
            if (item is ContentFolder)
                return "folder";
            if (item is ContentFile)
            {
                string ext = Path.GetExtension(item.Name);
                if (!imgList.Images.ContainsKey(ext))
                {
                    string filePath = Path.Combine(Path.GetDirectoryName(CurrentProject.File), item.GetPath());
                    if (File.Exists(filePath))
                        imgList.Images.Add(ext, Icon.ExtractAssociatedIcon(filePath));
                }
                return ext;
                //return "file";
            }

            return "error";
        }


        /// <summary>
        /// Adds a new Node to the given TreeNode
        /// </summary>
        /// <param name="content"></param>
        /// <param name="parent"></param>
        private void AddTreeNode(ContentFolder content, TreeNode parent)
        {
            parent.Expand();

            foreach (var item in content.Contents)
            {
                var treeNode = new TreeNode(item.Name) { Tag = item };

                treeNode.SelectedImageKey = treeNode.ImageKey = GetImageKey(item);

                parent.Nodes.Add(treeNode);
                _treeMap.Add(item, treeNode);

                AddContextMenu(treeNode);

                if (item is ContentFolder)
                {
                    AddTreeNode((ContentFolder)item, treeNode);
                }

            }
        }

        /// <summary>
        /// Returns the ContentItem of a given TreeNode
        /// </summary>
        /// <param name="node">TreeNode</param>
        /// <returns>ContentItem</returns>
        private ContentItem GetItemFromNode(TreeNode node)
        {
            return node?.Tag as ContentItem;
        }

        /// <summary>
        /// Adds a new Folder to the selected Node
        /// </summary>
        /// <param name="name">Name of Folder</param>
        private void AddFolder(string name)
        {
            ContentItem selectedItem = GetSelectedItem();
            ContentFolder selectedFolder = selectedItem as ContentFolder;

            if (selectedFolder == null)
                selectedFolder = selectedItem.Parent;

            selectedFolder?.Contents.Add(new ContentFolder(name, selectedFolder));
        }

        /// <summary>
        /// Returns the selected Node as a ContentItem
        /// </summary>
        /// <returns>ContentItem</returns>
        private ContentItem GetSelectedItem()
        {
            var node = treeContentFiles.SelectedNode;

            if (node == null)
            {
                if (treeContentFiles.Nodes.Count == 0)
                    return null;
                node = treeContentFiles.Nodes[0];
            }

            return GetItemFromNode(node);
        }

        #endregion

        #region File Operations

        /// <summary>
        /// Closes a File
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool CloseFile(string message = "Do you really want to close?")
        {
            if (_builder.IsBuilding)
            {
                if (MessageBox.Show("Your Project is currently Building." + message, "Close file", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;

                _builder.Abort();
            }

            prpItem.SelectedObject = null;
            CurrentProject = null;
            RecalcTreeView();
            return true;
        }

        /// <summary>
        /// Saves a file with the given filename
        /// </summary>
        /// <param name="file"></param>
        private void SaveFile(string file)
        {
            CurrentProject.Name = file;
            CurrentProject.Save(file);
        }

        /// <summary>
        /// Opens a file with the given filename
        /// </summary>
        /// <param name="file"></param>
        private void OpenFile(string file)
        {
            Program.Configuration.LastFile = file;
            Program.Configuration.Save();

            CurrentProject = ContentProject.Load(file);
            CurrentProject.CollectionChanged += CurrentProject_CollectionChanged;
            CurrentProject.PropertyChanged += CurrentProject_PropertyChanged;
            RecalcTreeView();
        }

        /// <summary>
        /// Creates one Directory at a time
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="directoryName"></param>
        private void MakeDirectory(string relativePath, string directoryName)
        {
            string[] splt = directoryName.Split(new[] { Path.DirectorySeparatorChar }, 2);

            string curPath = Path.Combine(relativePath, splt[0]);
            Directory.CreateDirectory(curPath);
            if (splt.Length > 1)
                MakeDirectory(curPath, splt[1]);
        }

        /// <summary>
        /// Saves the current file
        /// </summary>
        private void Save()
        {
            if (string.IsNullOrEmpty(CurrentProject.File) || !File.Exists(CurrentProject.File))
            {
                SaveAs();
                return;
            }
            SaveFile(CurrentProject.File);
        }

        /// <summary>
        /// Shows the SaveDialog and saves the Project
        /// </summary>
        private void SaveAs()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Engenious Content Project(.ecp)|*.ecp";
                sfd.FileName = CurrentProject.File;
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(sfd.FileName);
                }
            }
        }

        /// <summary>
        /// Checks if an the Project is saved and does so if not
        /// </summary>
        /// <returns></returns>
        private bool SaveFirst()
        {
            if (string.IsNullOrEmpty(CurrentProject.File))
            {
                SaveMenuItem_Click(saveMenuItem, new EventArgs());
            }
            return File.Exists(CurrentProject.File);
        }


        /// <summary>
        /// Creates a new Project
        /// </summary>
        private void CreateProject()
        {
            CurrentProject = new ContentProject();
            CurrentProject.CollectionChanged += CurrentProject_CollectionChanged;
            CurrentProject.PropertyChanged += CurrentProject_PropertyChanged;


            RecalcTreeView();

            if (!SaveFirst())
                CurrentProject = null;
            RecalcTreeView();
        }

        #endregion

        #region Build Process

        /// <summary>
        /// Starts Building
        /// </summary>
        private void StartBuilding()
        {
            buildMenuItem.Enabled = false;
            rebuildMenuItem.Enabled = false;
            cleanMenuItem.Enabled = false;
            cancelMenuItem.Enabled = true;
        }

        /// <summary>
        /// Ends Building
        /// </summary>
        private void EndBuilding()
        {

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(EndBuilding));
                return;
            }
            buildMenuItem.Enabled = true;
            rebuildMenuItem.Enabled = true;
            cleanMenuItem.Enabled = true;
            cancelMenuItem.Enabled = false;
        }

        #endregion

        /// <summary>
        /// Logs a message to the console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        private void Log(string message, BuildMessageEventArgs.BuildMessageType type)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate
                {
                    Log(message, type);
                }));
                return;
            }

            switch (type)
            {

                case BuildMessageEventArgs.BuildMessageType.Warning:
                    txtLog.SelectionColor = Color.Orange;
                    break;
                case BuildMessageEventArgs.BuildMessageType.Error:
                    txtLog.SelectionColor = Color.Red;
                    break;
                default:
                    txtLog.SelectionColor = Color.Black;
                    break;

            }

            txtLog.AppendText(message + "\n");
            txtLog.ScrollToCaret();


        }

    }
}

