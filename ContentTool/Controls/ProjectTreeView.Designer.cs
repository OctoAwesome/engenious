using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ContentTool.Controls
{
    partial class ProjectTreeView
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new Container();
            var resources = new ComponentResourceManager(typeof(ProjectTreeView));
            this.treeView = new TreeView();
            this.imageList = new ImageList(this.components);
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Dock = DockStyle.Fill;
            this.treeView.FullRowSelect = true;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new Size(150, 150);
            this.treeView.TabIndex = 0;
            this.treeView.AfterLabelEdit += new NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.KeyUp += new KeyEventHandler(this.treeView_KeyUp);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = Color.Transparent;
            this.imageList.Images.SetKeyName(0, "project");
            this.imageList.Images.SetKeyName(1, "folder");
            this.imageList.Images.SetKeyName(2, "warning");
            // 
            // ProjectTreeView
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.Name = "ProjectTreeView";
            this.ResumeLayout(false);

        }

        #endregion

        private TreeView treeView;
        private ImageList imageList;
    }
}
