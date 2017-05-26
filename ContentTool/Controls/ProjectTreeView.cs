﻿using System;
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

        public ProjectTreeView()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            treeView.TreeViewNodeSorter = new TreeSorter();
            treeView.AfterSelect += (s,ev) => SelectedContentItemChanged?.Invoke(SelectedItem);
            treeView.NodeMouseClick += (s, ev) => treeView.SelectedNode = ev.Node;
        }

        internal void RecalculateView()
        {
            var projectNode = GetNode(Project);

            treeView.Nodes.Clear();
            treeView.Nodes.Add(projectNode);

            treeView.Sort();

            projectNode.Expand();
        }

        internal TreeNode GetNode(ContentItem item)
        {
            var node = new TreeNode(item.Name) { Tag = item };
            
            if(item is ContentFolder)
            {
                var folder = item as ContentFolder;
                foreach (var child in folder.Content)
                    node.Nodes.Add(GetNode(child));
            }

            node.ContextMenuStrip = GetContextMenu(item);

            return node;
        }

        internal class TreeSorter : IComparer
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

        internal ContextMenuStrip GetContextMenu(ContentItem item)
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
            menu.Items.Add(CreateToolStripMenuItem("Show in Explorer", (s, e) => ShowInExplorerItemClick?.Invoke(SelectedItem)));

            if (!(item is ContentProject))
            {
                menu.Items.Add(new ToolStripSeparator());
                menu.Items.Add(CreateToolStripMenuItem("Remove", (s, e) => RemoveItemClick?.Invoke(SelectedItem)));
            }

            return menu;
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
    }
}