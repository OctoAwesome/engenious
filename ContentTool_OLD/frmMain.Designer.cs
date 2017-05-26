namespace ContentTool.old
{
    
    #region Windows Form Designer generated code
    public partial class FrmMain
    {
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.splitContainer_main = new System.Windows.Forms.SplitContainer();
            this.splitContainer_left = new System.Windows.Forms.SplitContainer();
            this.treeContentFiles = new System.Windows.Forms.TreeView();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.prpItem = new System.Windows.Forms.PropertyGrid();
            this.splitContainer_right = new System.Windows.Forms.SplitContainer();
            this.panel_editor = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newItemMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existingItemMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.existingFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildMainMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.contextMenuStrip_project = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.buildToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_file = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showInExplorerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip_folder = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showInExplorerToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.buildToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).BeginInit();
            this.splitContainer_main.Panel1.SuspendLayout();
            this.splitContainer_main.Panel2.SuspendLayout();
            this.splitContainer_main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_left)).BeginInit();
            this.splitContainer_left.Panel1.SuspendLayout();
            this.splitContainer_left.Panel2.SuspendLayout();
            this.splitContainer_left.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_right)).BeginInit();
            this.splitContainer_right.Panel1.SuspendLayout();
            this.splitContainer_right.Panel2.SuspendLayout();
            this.splitContainer_right.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.contextMenuStrip_project.SuspendLayout();
            this.contextMenuStrip_file.SuspendLayout();
            this.contextMenuStrip_folder.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer_main
            // 
            this.splitContainer_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_main.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer_main.Location = new System.Drawing.Point(0, 24);
            this.splitContainer_main.Name = "splitContainer_main";
            // 
            // splitContainer_main.Panel1
            // 
            this.splitContainer_main.Panel1.Controls.Add(this.splitContainer_left);
            this.splitContainer_main.Panel1.Text = "Panel1";
            // 
            // splitContainer_main.Panel2
            // 
            this.splitContainer_main.Panel2.Controls.Add(this.splitContainer_right);
            this.splitContainer_main.Panel2.Text = "Panel2";
            this.splitContainer_main.Size = new System.Drawing.Size(681, 468);
            this.splitContainer_main.SplitterDistance = 191;
            this.splitContainer_main.TabIndex = 0;
            this.splitContainer_main.TabStop = false;
            this.splitContainer_main.Text = "splitContainer";
            // 
            // splitContainer_left
            // 
            this.splitContainer_left.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_left.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_left.Name = "splitContainer_left";
            this.splitContainer_left.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_left.Panel1
            // 
            this.splitContainer_left.Panel1.Controls.Add(this.treeContentFiles);
            // 
            // splitContainer_left.Panel2
            // 
            this.splitContainer_left.Panel2.Controls.Add(this.prpItem);
            this.splitContainer_left.Size = new System.Drawing.Size(191, 468);
            this.splitContainer_left.SplitterDistance = 281;
            this.splitContainer_left.TabIndex = 0;
            // 
            // treeContentFiles
            // 
            this.treeContentFiles.BackColor = System.Drawing.SystemColors.Window;
            this.treeContentFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeContentFiles.ForeColor = System.Drawing.SystemColors.WindowText;
            this.treeContentFiles.ImageIndex = 0;
            this.treeContentFiles.ImageList = this.imgList;
            this.treeContentFiles.Indent = 19;
            this.treeContentFiles.ItemHeight = 16;
            this.treeContentFiles.LabelEdit = true;
            this.treeContentFiles.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(86)))), ((int)(((byte)(86)))));
            this.treeContentFiles.Location = new System.Drawing.Point(0, 0);
            this.treeContentFiles.Name = "treeContentFiles";
            this.treeContentFiles.SelectedImageIndex = 0;
            this.treeContentFiles.Size = new System.Drawing.Size(191, 281);
            this.treeContentFiles.TabIndex = 1;
            this.treeContentFiles.Text = "treeContentFiles";
            this.treeContentFiles.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeContentFiles_BeforeLabelEdit);
            this.treeContentFiles.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.TreeContentFiles_AfterLabelEdit);
            this.treeContentFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeContentFiles_AfterSelect);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "project.png");
            this.imgList.Images.SetKeyName(1, "folder.png");
            // 
            // prpItem
            // 
            this.prpItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prpItem.Location = new System.Drawing.Point(0, 0);
            this.prpItem.Name = "prpItem";
            this.prpItem.Size = new System.Drawing.Size(191, 183);
            this.prpItem.TabIndex = 0;
            // 
            // splitContainer_right
            // 
            this.splitContainer_right.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer_right.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer_right.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_right.Name = "splitContainer_right";
            this.splitContainer_right.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_right.Panel1
            // 
            this.splitContainer_right.Panel1.Controls.Add(this.panel_editor);
            // 
            // splitContainer_right.Panel2
            // 
            this.splitContainer_right.Panel2.Controls.Add(this.txtLog);
            this.splitContainer_right.Size = new System.Drawing.Size(486, 468);
            this.splitContainer_right.SplitterDistance = 284;
            this.splitContainer_right.TabIndex = 1;
            // 
            // panel_editor
            // 
            this.panel_editor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel_editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_editor.Location = new System.Drawing.Point(0, 0);
            this.panel_editor.Name = "panel_editor";
            this.panel_editor.Padding = new System.Windows.Forms.Padding(10);
            this.panel_editor.Size = new System.Drawing.Size(486, 284);
            this.panel_editor.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.SystemColors.Window;
            this.txtLog.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(486, 180);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // newMenuItem
            // 
            this.newMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.newMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.newMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.newMenuItem.Name = "newMenuItem";
            this.newMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newMenuItem.Size = new System.Drawing.Size(185, 22);
            this.newMenuItem.Text = "New...";
            this.newMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.newMenuItem.Click += new System.EventHandler(this.NewMenuItem_Click);
            // 
            // openMenuItem
            // 
            this.openMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.openMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.openMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.openMenuItem.Name = "openMenuItem";
            this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openMenuItem.Size = new System.Drawing.Size(185, 22);
            this.openMenuItem.Text = "Open...";
            this.openMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.openMenuItem.Click += new System.EventHandler(this.OpenMenuItem_Click);
            // 
            // closeMenuItem
            // 
            this.closeMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.closeMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.closeMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.closeMenuItem.Name = "closeMenuItem";
            this.closeMenuItem.Size = new System.Drawing.Size(185, 22);
            this.closeMenuItem.Text = "Close...";
            this.closeMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.closeMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // importMenuItem
            // 
            this.importMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.importMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.importMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.importMenuItem.Name = "importMenuItem";
            this.importMenuItem.Size = new System.Drawing.Size(185, 22);
            this.importMenuItem.Text = "Import...";
            this.importMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.saveMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.saveMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveMenuItem.Text = "Save";
            this.saveMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.saveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // saveAsMenuItem
            // 
            this.saveAsMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.saveAsMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.saveAsMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.saveAsMenuItem.Name = "saveAsMenuItem";
            this.saveAsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveAsMenuItem.Text = "Save As...";
            this.saveAsMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.saveAsMenuItem.Click += new System.EventHandler(this.SaveAsMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.exitMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.exitMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.exitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.closeMenuItem,
            this.importMenuItem,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.exitMenuItem});
            this.fileMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.fileMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileMenuItem.Text = "&File";
            this.fileMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.fileMenuItem.DropDownOpening += new System.EventHandler(this.FileMenuItem_DropDownOpening);
            // 
            // undoMenuItem
            // 
            this.undoMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.undoMenuItem.Enabled = false;
            this.undoMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.undoMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.undoMenuItem.Name = "undoMenuItem";
            this.undoMenuItem.Size = new System.Drawing.Size(152, 22);
            this.undoMenuItem.Text = "Undo";
            this.undoMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.undoMenuItem.Click += new System.EventHandler(this.UndoMenuItem_Click);
            // 
            // redoMenuItem
            // 
            this.redoMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.redoMenuItem.Enabled = false;
            this.redoMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.redoMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.redoMenuItem.Name = "redoMenuItem";
            this.redoMenuItem.Size = new System.Drawing.Size(152, 22);
            this.redoMenuItem.Text = "Redo";
            this.redoMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.redoMenuItem.Click += new System.EventHandler(this.RedoMenuItem_Click);
            // 
            // newItemMenuItem
            // 
            this.newItemMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.newItemMenuItem.Enabled = false;
            this.newItemMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.newItemMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.newItemMenuItem.Name = "newItemMenuItem";
            this.newItemMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newItemMenuItem.Text = "New Item...";
            this.newItemMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // newFolderMenuItem
            // 
            this.newFolderMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.newFolderMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.newFolderMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.newFolderMenuItem.Name = "newFolderMenuItem";
            this.newFolderMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newFolderMenuItem.Text = "New Folder...";
            this.newFolderMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // existingItemMenuItem
            // 
            this.existingItemMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.existingItemMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.existingItemMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.existingItemMenuItem.Name = "existingItemMenuItem";
            this.existingItemMenuItem.Size = new System.Drawing.Size(152, 22);
            this.existingItemMenuItem.Text = "Existing Item...";
            this.existingItemMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // existingFolderMenuItem
            // 
            this.existingFolderMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.existingFolderMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.existingFolderMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.existingFolderMenuItem.Name = "existingFolderMenuItem";
            this.existingFolderMenuItem.Size = new System.Drawing.Size(152, 22);
            this.existingFolderMenuItem.Text = "Existing Folder...";
            this.existingFolderMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // addMenuItem
            // 
            this.addMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.addMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newItemMenuItem,
            this.newFolderMenuItem,
            this.existingItemMenuItem,
            this.existingFolderMenuItem});
            this.addMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.addMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.addMenuItem.Name = "addMenuItem";
            this.addMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addMenuItem.Text = "Add";
            this.addMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // renameMenuItem
            // 
            this.renameMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.renameMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.renameMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.renameMenuItem.Name = "renameMenuItem";
            this.renameMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.renameMenuItem.Size = new System.Drawing.Size(152, 22);
            this.renameMenuItem.Text = "Rename";
            this.renameMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // deleteMenuItem
            // 
            this.deleteMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.deleteMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.deleteMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.deleteMenuItem.Name = "deleteMenuItem";
            this.deleteMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteMenuItem.Size = new System.Drawing.Size(152, 22);
            this.deleteMenuItem.Text = "Delete";
            this.deleteMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // editMenuItem
            // 
            this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.redoMenuItem,
            this.addMenuItem,
            this.renameMenuItem,
            this.deleteMenuItem});
            this.editMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.editMenuItem.Name = "editMenuItem";
            this.editMenuItem.Size = new System.Drawing.Size(37, 20);
            this.editMenuItem.Text = "Edit";
            // 
            // buildMenuItem
            // 
            this.buildMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.buildMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.buildMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buildMenuItem.Name = "buildMenuItem";
            this.buildMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.buildMenuItem.Size = new System.Drawing.Size(150, 22);
            this.buildMenuItem.Text = "Build";
            this.buildMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.buildMenuItem.Click += new System.EventHandler(this.BuildMenuItem_Click);
            // 
            // rebuildMenuItem
            // 
            this.rebuildMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.rebuildMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.rebuildMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rebuildMenuItem.Name = "rebuildMenuItem";
            this.rebuildMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F8)));
            this.rebuildMenuItem.Size = new System.Drawing.Size(150, 22);
            this.rebuildMenuItem.Text = "Rebuild";
            this.rebuildMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.rebuildMenuItem.Click += new System.EventHandler(this.RebuildMenuItem_Click);
            // 
            // cleanMenuItem
            // 
            this.cleanMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.cleanMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cleanMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cleanMenuItem.Name = "cleanMenuItem";
            this.cleanMenuItem.Size = new System.Drawing.Size(150, 22);
            this.cleanMenuItem.Text = "Clean";
            this.cleanMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.cleanMenuItem.Click += new System.EventHandler(this.CleanMenuItem_Click);
            // 
            // cancelMenuItem
            // 
            this.cancelMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.cancelMenuItem.Enabled = false;
            this.cancelMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.cancelMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cancelMenuItem.Name = "cancelMenuItem";
            this.cancelMenuItem.Size = new System.Drawing.Size(150, 22);
            this.cancelMenuItem.Text = "Cancel Build";
            this.cancelMenuItem.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            this.cancelMenuItem.Click += new System.EventHandler(this.CancelMenuItem_Click);
            // 
            // buildMainMenuItem
            // 
            this.buildMainMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.buildMainMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buildMenuItem,
            this.rebuildMenuItem,
            this.cleanMenuItem,
            this.cancelMenuItem});
            this.buildMainMenuItem.Enabled = false;
            this.buildMainMenuItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.buildMainMenuItem.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buildMainMenuItem.Name = "buildMainMenuItem";
            this.buildMainMenuItem.Size = new System.Drawing.Size(42, 20);
            this.buildMainMenuItem.Text = "Build";
            this.buildMainMenuItem.DropDownOpening += new System.EventHandler(this.BuildMainMenuItem_DropDownOpening);
            // 
            // mainMenu
            // 
            this.mainMenu.DefaultDropDownDirection = System.Windows.Forms.ToolStripDropDownDirection.BelowRight;
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.buildMainMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(681, 24);
            this.mainMenu.TabIndex = 1;
            this.mainMenu.Text = "mainMenu";
            // 
            // contextMenuStrip_project
            // 
            this.contextMenuStrip_project.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showInExplorerToolStripMenuItem,
            this.addToolStripMenuItem1,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem3,
            this.buildToolStripMenuItem2});
            this.contextMenuStrip_project.Name = "contextMenuStrip_project";
            this.contextMenuStrip_project.Size = new System.Drawing.Size(162, 98);
            // 
            // showInExplorerToolStripMenuItem
            // 
            this.showInExplorerToolStripMenuItem.Name = "showInExplorerToolStripMenuItem";
            this.showInExplorerToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.showInExplorerToolStripMenuItem.Text = "Show in Explorer";
            this.showInExplorerToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_ShowInExplorer);
            // 
            // addToolStripMenuItem1
            // 
            this.addToolStripMenuItem1.Name = "addToolStripMenuItem1";
            this.addToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.addToolStripMenuItem1.Text = "Add";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.ContextMenu_Close);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(158, 6);
            // 
            // buildToolStripMenuItem2
            // 
            this.buildToolStripMenuItem2.Name = "buildToolStripMenuItem2";
            this.buildToolStripMenuItem2.Size = new System.Drawing.Size(161, 22);
            this.buildToolStripMenuItem2.Text = "Build";
            // 
            // contextMenuStrip_file
            // 
            this.contextMenuStrip_file.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showInExplorerToolStripMenuItem1,
            this.renameToolStripMenuItem1,
            this.deleteToolStripMenuItem,
            this.toolStripMenuItem2,
            this.buildToolStripMenuItem});
            this.contextMenuStrip_file.Name = "contextMenuStrip_file";
            this.contextMenuStrip_file.Size = new System.Drawing.Size(162, 98);
            // 
            // showInExplorerToolStripMenuItem1
            // 
            this.showInExplorerToolStripMenuItem1.Name = "showInExplorerToolStripMenuItem1";
            this.showInExplorerToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.showInExplorerToolStripMenuItem1.Text = "Show in Explorer";
            this.showInExplorerToolStripMenuItem1.Click += new System.EventHandler(this.ContextMenu_ShowInExplorer);
            // 
            // renameToolStripMenuItem1
            // 
            this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
            this.renameToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.renameToolStripMenuItem1.Text = "Rename";
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(158, 6);
            // 
            // buildToolStripMenuItem
            // 
            this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            this.buildToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.buildToolStripMenuItem.Text = "Build";
            // 
            // contextMenuStrip_folder
            // 
            this.contextMenuStrip_folder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showInExplorerToolStripMenuItem2,
            this.addToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.deleteToolStripMenuItem1,
            this.toolStripMenuItem1,
            this.buildToolStripMenuItem1});
            this.contextMenuStrip_folder.Name = "contextMenuStrip_folder";
            this.contextMenuStrip_folder.Size = new System.Drawing.Size(162, 120);
            // 
            // showInExplorerToolStripMenuItem2
            // 
            this.showInExplorerToolStripMenuItem2.Name = "showInExplorerToolStripMenuItem2";
            this.showInExplorerToolStripMenuItem2.Size = new System.Drawing.Size(161, 22);
            this.showInExplorerToolStripMenuItem2.Text = "Show in Explorer";
            this.showInExplorerToolStripMenuItem2.Click += new System.EventHandler(this.ContextMenu_ShowInExplorer);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.addToolStripMenuItem.Text = "Add";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.deleteToolStripMenuItem1.Text = "Delete";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(158, 6);
            // 
            // buildToolStripMenuItem1
            // 
            this.buildToolStripMenuItem1.Name = "buildToolStripMenuItem1";
            this.buildToolStripMenuItem1.Size = new System.Drawing.Size(161, 22);
            this.buildToolStripMenuItem1.Text = "Build";
            // 
            // FrmMain
            // 
            this.ClientSize = new System.Drawing.Size(681, 492);
            this.Controls.Add(this.splitContainer_main);
            this.Controls.Add(this.mainMenu);
            this.Name = "FrmMain";
            this.Text = "Content Pipeline";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.splitContainer_main.Panel1.ResumeLayout(false);
            this.splitContainer_main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_main)).EndInit();
            this.splitContainer_main.ResumeLayout(false);
            this.splitContainer_left.Panel1.ResumeLayout(false);
            this.splitContainer_left.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_left)).EndInit();
            this.splitContainer_left.ResumeLayout(false);
            this.splitContainer_right.Panel1.ResumeLayout(false);
            this.splitContainer_right.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_right)).EndInit();
            this.splitContainer_right.ResumeLayout(false);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.contextMenuStrip_project.ResumeLayout(false);
            this.contextMenuStrip_file.ResumeLayout(false);
            this.contextMenuStrip_folder.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.SplitContainer splitContainer_main;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ToolStripMenuItem newMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newItemMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFolderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem existingItemMenuItem;
        private System.Windows.Forms.ToolStripMenuItem existingFolderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebuildMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelMenuItem;
        private System.Windows.Forms.ToolStripMenuItem buildMainMenuItem;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.SplitContainer splitContainer_left;
        private System.Windows.Forms.PropertyGrid prpItem;
        private System.Windows.Forms.TreeView treeContentFiles;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_project;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_file;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_folder;
        private System.Windows.Forms.ToolStripMenuItem showInExplorerToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.SplitContainer splitContainer_right;
        private System.Windows.Forms.Panel panel_editor;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem2;
    }
    #endregion
}
