using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ContentTool.Models;

namespace ContentTool.Controls
{
    internal class ImporterEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return context?.Instance == null ? base.GetEditStyle(context) : UITypeEditorEditStyle.DropDown;
        }


        private IWindowsFormsEditorService _editorService;

        private ListBox _lb;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            if (!(context?.Instance is ContentFile) || provider == null)
                return value;

            _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            
            _lb = _lb ?? new ListBox();
            _lb.SelectedItem = null;
            _lb.Items.Clear();
            _lb.BackColor = SystemColors.Window;
            _lb.BorderStyle = BorderStyle.None;
            _lb.Dock = DockStyle.Fill;
            _lb.SelectionMode = SelectionMode.One;
            _lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

            var file = (ContentFile) context.Instance;

            var ext = Path.GetExtension(file.Name);
            _lb.Items.AddRange(PipelineHelper.GetImporters(ext).ToArray());
            _lb.MaximumSize = new Size(_lb.MaximumSize.Width,_lb.ItemHeight*_lb.Items.Count);
            _lb.SelectedItem = value;

            _editorService.DropDownControl(_lb);
            return _lb.SelectedItem ?? value;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
