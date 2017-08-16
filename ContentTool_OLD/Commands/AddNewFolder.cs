using System;
using System.IO;
using System.Windows.Forms;
using ContentTool.old.Dialog;
using ContentTool.old.Items;

namespace ContentTool.old.Commands
{
    public static class AddNewFolder
    {
        public static void Execute(ContentItem selectedItem, string currentFile)
        {
            var addFolderForm = new NewFolderDialog();

            var curFolder = selectedItem as ContentFolder ?? selectedItem.Parent;


            if (addFolderForm.ShowDialog() == DialogResult.OK)
            {
                var name = addFolderForm.FileName;
                var itemPath = Path.Combine(Path.GetDirectoryName(currentFile), selectedItem.GetPath());
                var folderPath = Path.Combine(itemPath, name);

                //Check if directory already exists
                if (Directory.Exists(folderPath))
                {
                    MessageBox.Show("Directory already exists!", "Directory exists", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    Directory.CreateDirectory(folderPath);
                    curFolder.AddSubFolder(folderPath, folderPath);
                }
                catch (Exception)
                {
                    MessageBox.Show("An Error occured during the operation.", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}
