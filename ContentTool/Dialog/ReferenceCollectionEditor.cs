using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using ContentTool.Items;

namespace ContentTool.Dialog
{
    internal class ReferenceCollectionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return context?.Instance == null ? base.GetEditStyle(context) : UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!(context?.Instance is ContentProject) || provider == null)
                return value;

            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            using (var collectionEditor = new FrmEditReferences())
            {
                collectionEditor.RootDir = System.IO.Path.GetDirectoryName((context.Instance as ContentProject).File);
                collectionEditor.References = value as List<string>;
                if (editorService.ShowDialog(collectionEditor) == System.Windows.Forms.DialogResult.OK)
                    return collectionEditor.References;

                return value;
            }
            //return base.EditValue(context, provider, value);
        }
    }
}
