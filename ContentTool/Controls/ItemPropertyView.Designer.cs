using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ContentTool.Controls
{
    partial class ItemPropertyView
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
            this.propertyGrid = new PropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = DockStyle.Fill;
            this.propertyGrid.LineColor = SystemColors.ControlDark;
            this.propertyGrid.Location = new Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new Size(150, 150);
            this.propertyGrid.TabIndex = 0;
            // 
            // ItemPropertyView
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.propertyGrid);
            this.Name = "ItemPropertyView";
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyGrid propertyGrid;
    }
}
