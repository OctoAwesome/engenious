using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ContentTool.old.Items;

namespace ContentTool.old.Dialog
{
    internal class ImporterEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return context?.Instance == null ? base.GetEditStyle(context) : UITypeEditorEditStyle.DropDown;
        }
        private IWindowsFormsEditorService _editorService;
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            if (!(context?.Instance is ContentFile) || provider == null)
                return value;

            _editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            ListBox lb = new ListBox();
            lb.SelectionMode = SelectionMode.One;
            lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

            var file = (ContentFile) context.Instance;

            string ext = System.IO.Path.GetExtension(file.Name);
            lb.Items.AddRange(PipelineHelper.GetImporters(ext).ToArray());
            

            _editorService.DropDownControl(lb);
            if (lb.SelectedItem == null)
                return value;

            return lb.SelectedItem;
        }

        private void OnListBoxSelectedValueChanged(object sender, EventArgs e)
        {
            _editorService.CloseDropDown();
        }
    }
}
