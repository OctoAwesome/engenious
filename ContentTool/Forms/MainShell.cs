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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            projectTreeView.SelectedContentItemChanged += ProjectTreeView_SelectedContentItemChanged;
            projectTreeView.AddItemClick += (i, t) => AddItemClick?.Invoke(i, t);
            projectTreeView.BuildItemClick += (i) => BuildItemClick?.Invoke(i);
            projectTreeView.RemoveItemClick += (i) => RemoveItemClick?.Invoke(i);
            projectTreeView.ShowInExplorerItemClick += (i) => ShowInExplorerItemClick?.Invoke(i);
        }

        private void ProjectTreeView_SelectedContentItemChanged(ContentItem newItem)
        {
            itemPropertyView.SelectItem(newItem);
        }

        public event Delegates.ItemActionEventHandler BuildItemClick;
        public event Delegates.ItemActionEventHandler ShowInExplorerItemClick;
        public event Delegates.ItemAddActionEventHandler AddItemClick;
        public event Delegates.ItemActionEventHandler RemoveItemClick;
    }
}
