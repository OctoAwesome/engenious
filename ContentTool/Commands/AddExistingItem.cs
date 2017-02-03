using System;
using System.IO;
using System.Windows.Forms;
using ContentTool.Items;

namespace ContentTool.Commands
{
    public static class AddExistingItem
    {
        public static void Execute(ContentItem selectedItem, string currentFile)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "All files|*.*|Image files(.png;.bmp;.jpg)|*.png;*.bmp;*.jpg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    ContentFolder curFolder = selectedItem as ContentFolder ?? selectedItem as ContentFolder;

                    if (curFolder == null)
                        return;

                    foreach (var file in ofd.FileNames)
                    {
                        ContentFolder destFolder = curFolder;
                        string fileName = file;
                        string absolutePath = Path.Combine(Path.GetDirectoryName(currentFile), curFolder.GetPath());
                        if (MakePathRelative(ref fileName, absolutePath))
                        {
                            destFolder = curFolder.CreateTreeFolderStructure(Path.GetDirectoryName(fileName));
                        }
                        destFolder.AddFile(ofd.FileName, Path.Combine(absolutePath, Path.GetFileName(file)));
                    }

                }
            }
        }

        private static bool MakePathRelative(ref string filename, string relativeTo)
        {
            filename = Path.GetFullPath(filename);
            string absoluteFolder = Path.GetFullPath(relativeTo);

            if (!filename.StartsWith(absoluteFolder))
                return false;

            filename = filename.Substring(Math.Min(absoluteFolder.Length + 1, filename.Length));
            return true;
        }
    }
}