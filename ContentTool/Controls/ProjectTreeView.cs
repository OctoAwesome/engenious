using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ContentTool.Forms;
using ContentTool.Models;
using static ContentTool.Delegates;
using PropertyValueChangedEventArgs = ContentTool.Observer.PropertyValueChangedEventArgs;

namespace ContentTool.Controls
{
    public partial class ProjectTreeView : UserControl
    {
        public ContentProject Project
        {
            get => _project;
            set
            {
                if (_project == value)
                    return;
                if (_project != null)
                {
                    _project.CollectionChanged -= Project_CollectionChanged;
                    _project.PropertyChanged -= ProjectOnPropertyChanged;
                }

                _project = value;

                if (_project != null)
                {
                    _project.CollectionChanged += Project_CollectionChanged;
                    _project.PropertyChanged += ProjectOnPropertyChanged;
                }
                RecalculateView();
            }
        }

        private void ProjectOnPropertyChanged(object sender, PropertyValueChangedEventArgs args)
        {
            var contentNode = sender as ContentItem;
            var node = GetNodeFromItem(contentNode);

            if (node == null)
                return;
            if (args.PropertyName == "Name")
                node.Text = args.NewValue.ToString();
        }

        private readonly List<Tuple<object, NotifyCollectionChangedEventArgs>> _changes =
            new List<Tuple<object, NotifyCollectionChangedEventArgs>>();

        private void Project_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsRenderingSuspended)
                _changes.Add(new Tuple<object, NotifyCollectionChangedEventArgs>(sender, e));
            else
                ResumeRendering(sender, e);
        }

        public ContentFolder SelectedFolder =>
            (SelectedItem as ContentFolder) ?? (SelectedItem?.Parent as ContentFolder) ?? Project;


        private ContentProject _project;

        public ContentItem SelectedItem => treeView.SelectedNode?.Tag as ContentItem;

        public IMainShell Shell { get; set; }

        public ProjectTreeView()
        {
            InitializeComponent();

            treeView.BeforeExpand += TreeView_BeforeExpand;
        }

        private void TreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            var folder = e.Node.Tag as ContentFolder;
            if (e.Node.Nodes.Count != 1 || folder == null) return;

            if (!e.Node.Nodes[0].Tag.Equals("DummyNode")) return;

            e.Node.Nodes.Clear();

            Shell.ShowLoading();

            var t = new Thread(() =>
            {
                var nodes = new List<TreeNode>(folder.Content.Count);
                foreach (var child in folder.Content)
                {
                    nodes.Add(CreateNode(child, 1, 1));
                }

                Shell.Invoke(new MethodInvoker(() =>
                {
                    foreach (var node in nodes)
                        e.Node.Nodes.Add(node);
                    
                    Shell.HideLoading();

                    e.Node.Expand();
                }));
            });
            t.Start();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            treeView.TreeViewNodeSorter = new TreeSorter();
            treeView.AfterSelect += (s, ev) => SelectedContentItemChanged?.Invoke(SelectedItem);
            treeView.NodeMouseClick += (s, ev) =>
            {
                if (ev.Button == MouseButtons.Right) treeView.SelectedNode = ev.Node;
            };
        }

        private int _suspendCount;
        public bool IsRenderingSuspended => _suspendCount > 0;

        public void SuspendRendering()
        {
            Interlocked.Increment(ref _suspendCount);
        }

        private void ResumeRendering(Tuple<object, NotifyCollectionChangedEventArgs> t, ContentItem topNode = null)
        {
            ResumeRendering(t.Item1, t.Item2, topNode);
        }

        private void ResumeRendering(object sender, NotifyCollectionChangedEventArgs e, ContentItem topNode = null)
        {
            var contentNode = sender as ContentItem;
            if (topNode != null && contentNode != topNode)
                return;
            var node = GetNodeFromItem(contentNode);
            if (node == null)
                return;
            
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (node.IsExpanded)
                    {
                        var nodes = new List<TreeNode>(e.NewItems.Count);
                        foreach (var nI in e.NewItems.OfType<ContentItem>())
                        {
                            nodes.Add(CreateNode(nI));
                        }
                        Invoke(new MethodInvoker(() =>
                        {
                            foreach (var n in nodes)
                            {
                                node.Nodes.Add(n);
                                treeView.SelectedNode = n;
                            }
                        }));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        foreach (var nI in e.OldItems.OfType<ContentItem>())
                        {
                            node.Nodes.Remove(GetNodeFromItem(nI));
                        }
                    }));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    node.Nodes.Clear();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void ResumeRendering()
        {
            lock (this)
            {
                _suspendCount--;
                if (_suspendCount == 0)
                {
                    var topNode = _changes.FirstOrDefault()?.Item1;
                    foreach (var c in _changes)
                        ResumeRendering(c, topNode as ContentItem);
                    _changes.Clear();
                }
            }
        }

        private bool _isRecalculatingView;

        public void RecalculateView()
        {
            if (_isRecalculatingView) return;
            _isRecalculatingView = true;
            treeView.Nodes.Clear();
            treeView.BeginUpdate();

            if (Project == null)
                return;


            var t = new Thread(() =>
            {
                var projectNode = CreateNode(Project);

                Invoke(new MethodInvoker(() =>
                {
                    treeView.Nodes.Add(projectNode);
                    treeView.Sort();
                    projectNode.Expand();

                    treeView.EndUpdate();
                    _isRecalculatingView = false;

                    Refreshed?.Invoke(this, EventArgs.Empty);
                }));
            });

            t.Start();
        }


        protected TreeNode CreateNode(ContentItem item, int maxDepth = 1, int depth = 0)
        {
            var node = new TreeNode(item.Name) {Tag = item};

            var folder = item as ContentFolder;
            if (folder != null)
            {
                if (depth < maxDepth)
                {
                    foreach (var child in folder.Content)
                        node.Nodes.Add(CreateNode(child, maxDepth, depth + 1));
                }
                else if (folder.Content.Count > 0)
                {
                    node.Nodes.Add(new TreeNode("") {Tag = "DummyNode"});
                }
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
                if (a.Tag is ContentFolder && b.Tag is ContentItem)
                    return -1;
                if (a.Tag is ContentItem && b.Tag is ContentFolder)
                    return 1;
                return new CaseInsensitiveComparer().Compare(a.Text, b.Text);
            }
        }

        private ContextMenuStrip _projectContextMenu, _itemContextMenu;

        protected ContextMenuStrip GetContextMenu(ContentItem item)
        {
            if (item == null)
                return null;

            if (item is ContentProject && _projectContextMenu != null)
                return _projectContextMenu;

            if (_itemContextMenu != null && !(item is ContentProject))
                return _itemContextMenu;

            var menu = new ContextMenuStrip();

            var addItem = new ToolStripMenuItem("Add");
            addItem.DropDownItems.Add(CreateToolStripMenuItem("Existing Item",
                (s, e) => AddItemClick?.Invoke(SelectedFolder, AddType.ExistingItem)));
            addItem.DropDownItems.Add(CreateToolStripMenuItem("Existing Folder",
                (s, e) => AddItemClick?.Invoke(SelectedFolder, AddType.ExistingFolder)));
            addItem.DropDownItems.Add(CreateToolStripMenuItem("New Folder",
                (s, e) => AddItemClick?.Invoke(SelectedFolder, AddType.NewFolder)));
            menu.Items.Add(addItem);

            menu.Items.Add(CreateToolStripMenuItem("Build", (s, e) => BuildItemClick?.Invoke(SelectedItem)));
            menu.Items.Add(CreateToolStripMenuItem("Rename", (s, e) => EditItem(SelectedItem)));
            menu.Items.Add(CreateToolStripMenuItem("Show in Explorer",
                (s, e) => ShowInExplorerItemClick?.Invoke(SelectedItem)));

            if (!(item is ContentProject))
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateToolStripMenuItem("Remove", (s, e) => RemoveItemRequest(item)));
                _projectContextMenu = menu;
            }
            else
                _itemContextMenu = menu;

            return menu;
        }

        public void EditItem(ContentItem contentItem)
        {
            GetNodeFromItem(contentItem)?.BeginEdit();
        }

        public void RemoveItemRequest(ContentItem item)
        {
            if (item == null)
                return;

            RemoveItemClick?.Invoke(SelectedItem);
        }

        private TreeNode GetNodeFromItem(ContentItem item)
        {
            if (item == null)
                return null;
            var tmp = new Stack<ContentItem>();
            while (item != null)
            {
                tmp.Push(item);
                item = item.Parent;
            }

            var nodes = treeView.Nodes;
            TreeNode curNode = null;
            while (tmp.Count > 0)
            {
                var contentItem = tmp.Pop();
                curNode = nodes.OfType<TreeNode>().FirstOrDefault(x => (x.Tag as ContentItem) == contentItem);

                if (curNode == null)
                    break;

                nodes = curNode.Nodes;
            }
            return tmp.Count > 0 ? null : curNode;
        }

        protected string GetIconKey(ContentItem item)
        {
            string key = "file";

            if (item is ContentProject)
            {
                key = "project";
            }
            else if (item is ContentFolder)
            {
                key = "folder";
            }
            else
            {
                key = Path.GetExtension(item.FilePath);
                if (!treeView.ImageList.Images.ContainsKey(key))
                {
                    Icon ico = Icon.ExtractAssociatedIcon(Path.GetFullPath(item.FilePath));
                    if (ico != null)
                        Invoke(new MethodInvoker(() => treeView.ImageList.Images.Add(key, ico)));
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

        public event EventHandler Refreshed;

        public event ItemActionEventHandler BuildItemClick;
        public event ItemActionEventHandler ShowInExplorerItemClick;
        public event ItemAddActionEventHandler AddItemClick;
        public event ItemActionEventHandler RemoveItemClick;

        private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node == null)
                return;

            if (!string.IsNullOrWhiteSpace(e.Label))
                ((ContentItem) e.Node.Tag).Name = e.Label;
        }

        private void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                RemoveItemRequest(SelectedItem);
        }
    }
}