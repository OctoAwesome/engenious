using System;
using System.Windows.Forms;
using System.Collections.Generic;
using engenious.Graphics;
using System.Linq;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ContentTool.Builder;
using ContentTool.Commands;
using ContentTool.Items;
using engenious.Content.Pipeline;
using engenious.Pipeline.Pipeline.Editors;

namespace ContentTool
{
    public partial class frmMain : Form
    {
        private ContentProject currentProject;

        private ContentProject CurrentProject
        {
            get { return currentProject; }
            set
            {
                currentProject = value;
                if (builder != null)
                {
                    builder.BuildStatusChanged -= Builder_BuildStatusChanged;
                    builder.ItemProgress -= Builder_ItemProgress;
                    builder.BuildMessage -= Builder_BuildMessage;
                }
                builder = new ContentBuilder(value);
                builder.BuildStatusChanged += Builder_BuildStatusChanged;
                builder.ItemProgress += Builder_ItemProgress;
                builder.BuildMessage += Builder_BuildMessage;
                PipelineHelper.PreBuilt(currentProject);
            }
        }

        private ContentBuilder builder;

        private Dictionary<ContentItem, TreeNode> treeMap;

        public frmMain()
        {
            InitializeComponent();

            PipelineHelper.DefaultInit();

            builder = new ContentBuilder(null);
            builder.BuildStatusChanged += Builder_BuildStatusChanged;
            builder.ItemProgress += Builder_ItemProgress;
            builder.BuildMessage += Builder_BuildMessage;
            treeMap = new Dictionary<ContentItem, TreeNode>();

            treeContentFiles.NodeMouseClick += TreeContentFilesOnNodeMouseClick;

            this.newFolderMenuItem.Click += (o, e) => AddNewFolder.Execute(GetSelectedItem(), CurrentProject.File);
            this.existingFolderMenuItem.Click += (o,e) => AddExistingFolder.Execute(GetSelectedItem(), CurrentProject.File);
            this.existingItemMenuItem.Click += (o, e) => AddExistingItem.Execute(GetSelectedItem(), CurrentProject.File);
            this.deleteMenuItem.Click += (o,e) => DeleteItem.Execute(GetSelectedItem());
            this.deleteToolStripMenuItem.Click += (o,e) => DeleteItem.Execute(GetSelectedItem());
            this.deleteToolStripMenuItem1.Click += (o, e) => DeleteItem.Execute(GetSelectedItem());
            this.renameMenuItem.Click += (o, e) => RenameItem.Execute(GetSelectedItem(), treeMap);
            this.renameToolStripMenuItem.Click += (o, e) => RenameItem.Execute(GetSelectedItem(), treeMap);
            this.renameToolStripMenuItem1.Click += (o, e) => RenameItem.Execute(GetSelectedItem(), treeMap);
            this.buildToolStripMenuItem.Click += (o, e) => BuildItem.Execute(GetSelectedItem(), builder);
            this.buildToolStripMenuItem1.Click += (o, e) => BuildItem.Execute(GetSelectedItem(), builder);
            this.buildToolStripMenuItem2.Click += (o, e) => BuildItem.Execute(GetSelectedItem(), builder);
        }

        void FrmMain_Load(object sender, System.EventArgs e)
        {
            foreach (string file in System.Environment.GetCommandLineArgs().Skip(1))
            {
                if (System.IO.Path.GetExtension(file) == ".ecp" && System.IO.File.Exists(file))
                {
                    OpenFile(file);
                    return;
                }
            }

            if (File.Exists(Program.Configuration.LastFile))
                OpenFile(Program.Configuration.LastFile);
        }

        #region Build Events

        private void Builder_BuildMessage(object sender, engenious.Content.Pipeline.BuildMessageEventArgs e)
        {

            Log(Program.MakePathRelative(e.FileName) + " " + e.Message, e.MessageType);

        }

        void Builder_ItemProgress(object sender, ItemProgressEventArgs e)
        {
            string message = e.Item + " " + (e.BuildStep & (BuildStep.Build | BuildStep.Clean)).ToString().ToLower() + "ing ";

            BuildMessageEventArgs.BuildMessageType type = BuildMessageEventArgs.BuildMessageType.Information;

            if ((e.BuildStep & Builder.BuildStep.Abort) == Builder.BuildStep.Abort)
            {
                message += "failed!";
                type = BuildMessageEventArgs.BuildMessageType.Error;
            }
            else if ((e.BuildStep & Builder.BuildStep.Finished) == Builder.BuildStep.Finished)
            {
                message += "finished!";
            }
            Log(message, type);
        }

        void Builder_BuildStatusChanged(object sender, BuildStep buildStep)
        {
            string message = (buildStep & (BuildStep.Build | BuildStep.Clean)).ToString() + " ";
            BuildMessageEventArgs.BuildMessageType type = BuildMessageEventArgs.BuildMessageType.Information;
            if ((buildStep & Builder.BuildStep.Abort) == Builder.BuildStep.Abort)
            {
                message += "aborted!";
                type = BuildMessageEventArgs.BuildMessageType.Warning;
            }
            else if ((buildStep & Builder.BuildStep.Finished) == Builder.BuildStep.Finished)
            {
                message += "finished!";
                if (builder.FailedBuilds != 0)
                {
                    message += " " + builder.FailedBuilds.ToString() + " files failed to build!";
                }
            }
            Log(message, type);
        }

        #endregion

        #region Project Events

        void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ContentItem item = sender as ContentItem;
            if (item == null)
                return;
            if (e.PropertyName == "Name")
            {
                TreeNode node = null;
                treeMap.TryGetValue(item, out node);
                if (node == null)
                    return;
                node.Text = item.Name;
            }
        }

        void CurrentProject_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ContentItem item = (e.NewItems == null || e.NewItems.Count == 0) ? null : (e.NewItems[0] as ContentItem);
            if (item == null)
                item = (e.OldItems == null || e.OldItems.Count == 0) ? null : (e.OldItems[0] as ContentItem);
            TreeNode parentNode = null;
            if (!treeMap.TryGetValue(item.Parent, out parentNode))
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

                        treeMap.Add(item, node);
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
                        TreeNode node = null;
                        if (treeMap.TryGetValue(item, out node))
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
                        TreeNode node = null;
                        treeMap.TryGetValue(oldItem, out node);
                        if (parentNode == null)
                            treeContentFiles.Nodes.Remove(node);
                        else
                            parentNode.Nodes.Remove(node);

                        node = new TreeNode(item.Name) { Tag = item };
                        node.SelectedImageKey = node.ImageKey = GetImageKey(item);
                        treeMap.Add(item, node);
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
                        treeMap.Clear();
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
            var absPath = Path.Combine(Path.GetDirectoryName(CurrentProject.File), file.getPath());
            var importValue = file.Importer.Import(absPath, new ContentImporterContext());
            var processValue = file.Processor.Process(importValue, absPath,
                new ContentProcessorContext(builder.UiContext));
            editorWrap.Open(importValue, processValue);
            panel_editor.Controls.Add(editorWrap.Editor.MainControl);
        }

        void TreeContentFiles_BeforeLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
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

        void TreeContentFiles_AfterLabelEdit(object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
        {
            e.CancelEdit = true;
            return;
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

        void FileMenuItem_DropDownOpening(object sender, System.EventArgs e)
        {
            bool fileOpened = CurrentProject != null;
            this.importMenuItem.Enabled = false;
            this.closeMenuItem.Enabled = this.saveAsMenuItem.Enabled = fileOpened;
            this.saveMenuItem.Enabled = fileOpened;//TODO änderungen?

        }


        void SaveAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        void SaveMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        void CloseMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();

        }

        void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Engenious Content Project(.ecp)|*.ecp";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenFile(ofd.FileName);
            }
        }

        void NewMenuItem_Click(object sender, EventArgs e)
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

        void CleanMenuItem_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";

            builder.Clean();
        }

        void RebuildMenuItem_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";
            builder.Rebuild();
        }

        void BuildMenuItem_Click(object sender, EventArgs e)
        {
            if (!SaveFirst())
                return;
            txtLog.Text = "";
            builder.Build();
        }

        void CancelMenuItem_Click(object sender, EventArgs e)
        {
            builder.Abort();
        }


        void BuildMainMenuItem_DropDownOpening(object sender, System.EventArgs e)
        {
            cleanMenuItem.Enabled = builder.CanClean;
            rebuildMenuItem.Enabled = System.IO.File.Exists(CurrentProject.File) && cleanMenuItem.Enabled;
        }

        #endregion

        #region Edit Menu

        void RedoMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void UndoMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        
        void NewFolderMenuItem_Click(object sender, EventArgs e)
        {

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
            var ContentItem = GetSelectedItem();
            string path = Path.Combine(Path.GetDirectoryName(currentProject.File), ContentItem.getPath());
            if (System.IO.Directory.Exists(path))
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
            treeMap.Clear();
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
            treeMap.Add(CurrentProject, rootNode);
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
                string ext = System.IO.Path.GetExtension(item.Name);
                if (!imgList.Images.ContainsKey(ext))
                {
                    string filePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(CurrentProject.File), item.getPath());
                    if (System.IO.File.Exists(filePath))
                        imgList.Images.Add(ext, System.Drawing.Icon.ExtractAssociatedIcon(filePath));
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
                treeMap.Add(item, treeNode);

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
                selectedFolder = selectedItem.Parent as ContentFolder;

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
            if (builder.IsBuilding)
            {
                if (MessageBox.Show("Your Project is currently Building." + message, "Close file", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;

                builder.Abort();
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
            string[] splt = directoryName.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, 2);

            string curPath = System.IO.Path.Combine(relativePath, splt[0]);
            System.IO.Directory.CreateDirectory(curPath);
            if (splt.Length > 1)
                MakeDirectory(curPath, splt[1]);
        }

        /// <summary>
        /// Saves the current file
        /// </summary>
        private void Save()
        {
            if (string.IsNullOrEmpty(CurrentProject.File) || !System.IO.File.Exists(CurrentProject.File))
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
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Engenious Content Project(.ecp)|*.ecp";
            sfd.FileName = CurrentProject.File;
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveFile(sfd.FileName);
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
            return System.IO.File.Exists(CurrentProject.File);
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
            this.buildMenuItem.Enabled = false;
            this.rebuildMenuItem.Enabled = false;
            this.cleanMenuItem.Enabled = false;
            this.cancelMenuItem.Enabled = true;
        }

        /// <summary>
        /// Ends Building
        /// </summary>
        private void EndBuilding()
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    EndBuilding();
                }));
                return;
            }
            this.buildMenuItem.Enabled = true;
            this.rebuildMenuItem.Enabled = true;
            this.cleanMenuItem.Enabled = true;
            this.cancelMenuItem.Enabled = false;
        }

        #endregion

        /// <summary>
        /// Logs a message to the console
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        private void Log(string message, BuildMessageEventArgs.BuildMessageType type)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate ()
                {
                    Log(message, type);
                }));
                return;
            }

            switch (type)
            {

                case BuildMessageEventArgs.BuildMessageType.Warning:
                    txtLog.SelectionColor = System.Drawing.Color.Orange;
                    break;
                case BuildMessageEventArgs.BuildMessageType.Error:
                    txtLog.SelectionColor = System.Drawing.Color.Red;
                    break;
                default:
                    txtLog.SelectionColor = System.Drawing.Color.Black;
                    break;

            }

            txtLog.AppendText(message + "\n");
            txtLog.ScrollToCaret();


        }

    }
}

