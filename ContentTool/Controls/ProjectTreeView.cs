using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTool.Models;
using System.Collections;
using ContentTool.Forms;
using static ContentTool.Delegates;
using System.IO;

namespace ContentTool.Controls
{
    public partial class ProjectTreeView : UserControl
    {
        public ContentProject Project
        {
            get => project;
            set
            {
                if (project == value)
                    return;

                project = value;
                RecalculateView();
            }
        }

        private ContentProject project;

        public ContentItem SelectedItem => treeView.SelectedNode?.Tag as ContentItem;

        public string Pat { get; private set; }

        public ProjectTreeView()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            treeView.TreeViewNodeSorter = new TreeSorter();
            treeView.AfterSelect += (s,ev) => SelectedContentItemChanged?.Invoke(SelectedItem);
            treeView.NodeMouseClick += (s, ev) => { if (ev.Button == MouseButtons.Right) treeView.SelectedNode = ev.Node; };
        }

        public void RecalculateView()
        {
            treeView.Nodes.Clear();

            if (Project == null)
                return;

            //Task.Run(() => {
                var projectNode = GetNode(Project);
                treeView.Nodes.Add(projectNode);
                treeView.Sort();
                projectNode.Expand();
           // });
        }

        protected TreeNode GetNode(ContentItem item)
        {
            var node = new TreeNode(item.Name) { Tag = item };
            
            if(item is ContentFolder)
            {
                var folder = item as ContentFolder;
                foreach (var child in folder.Content)
                    node.Nodes.Add(GetNode(child));
            }

            node.ContextMenuStrip = GetContextMenu(item);

            if (item.Error.HasFlag(ContentErrorType.NotFound))
            {
                node.ImageKey = "warning";
                node.SelectedImageKey = "warning";
            }
            else
            {
                var key = GetIconKey(item);
                node.ImageKey = key;
                node.SelectedImageKey = key;
            }

            return node;
        }

        protected class TreeSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                var a = x as TreeNode;
                var b = y as TreeNode;

                if (x == null || y == null)
                    return 0;

                if (a.Tag is ContentFolder && b.Tag is ContentFolder)
                    return 0;
                else if (a.Tag is ContentFolder && b.Tag is ContentItem)
                    return -1;
                else if (a.Tag is ContentItem && b.Tag is ContentFolder)
                    return 1;
                else
                    return new CaseInsensitiveComparer().Compare(a.Text, b.Text);
            }
        }

        protected ContextMenuStrip GetContextMenu(ContentItem item)
        {
            if (item == null)
                return new ContextMenuStrip();

            var menu = new ContextMenuStrip();

            var addItem = new ToolStripMenuItem("Add");
            addItem.DropDownItems.Add(CreateToolStripMenuItem("Existing Item", (s, e) => AddItemClick?.Invoke(SelectedItem, AddType.ExistingItem)));
            addItem.DropDownItems.Add(CreateToolStripMenuItem("Existing Folder", (s, e) => AddItemClick?.Invoke(SelectedItem, AddType.ExistingFolder)));
            addItem.DropDownItems.Add(CreateToolStripMenuItem("New Folder", (s, e) => AddItemClick?.Invoke(SelectedItem, AddType.NewFolder)));
            menu.Items.Add(addItem);

            menu.Items.Add(CreateToolStripMenuItem("Build", (s, e) => BuildItemClick?.Invoke(SelectedItem)));
            menu.Items.Add(CreateToolStripMenuItem("Rename", (s, e) => treeView.SelectedNode?.BeginEdit()));
            menu.Items.Add(CreateToolStripMenuItem("Show in Explorer", (s, e) => ShowInExplorerItemClick?.Invoke(SelectedItem)));

            if (!(item is ContentProject))
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateToolStripMenuItem("Remove", (s, e) => RemoveItemRequest(item)));
            }

            return menu;
        }

        protected void RemoveItemRequest(ContentItem item)
        {
            if (item == null)
                return;

            if(MessageBox.Show($"Do you really want to remove {item.Name}?", "Remove Item", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                RemoveItemClick?.Invoke(SelectedItem);
        }

        protected string GetIconKey(ContentItem item)
        {
            string key = "file";

            if (item is ContentProject) {
                key = "project";
            }
            else if (item is ContentFolder) {
                key = "folder";
            }
            else
            {
                key = Path.GetExtension(item.FilePath);
                if (!treeView.ImageList.Images.ContainsKey(key))
                {
                    Icon ico = Icon.ExtractAssociatedIcon(Path.GetFullPath(item.FilePath));
                    if (ico != null)
                        treeView.ImageList.Images.Add(key, ico);
                    else
                        key = "file";
                }
            }

            return key;
        }

        private ToolStripMenuItem CreateToolStripMenuItem(string text, EventHandler onClick)
        {
            var item = new ToolStripMenuItem(text);
            item.Click += onClick;
            return item;
        }

        public delegate void SelectedContentItemChangedHandler(ContentItem newItem);
        public event SelectedContentItemChangedHandler SelectedContentItemChanged;

        public event ItemActionEventHandler BuildItemClick;
        public event ItemActionEventHandler ShowInExplorerItemClick;
        public event ItemAddActionEventHandler AddItemClick;
        public event ItemActionEventHandler RemoveItemClick;

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Label))
                ((ContentItem)e.Node.Tag).Name = e.Label;

            e.Node.Text = ((ContentItem)e.Node.Tag).Name;
        }

        private void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveItemRequest(SelectedItem);
        }
    }
}
