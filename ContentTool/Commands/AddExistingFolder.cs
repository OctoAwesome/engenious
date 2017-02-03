using System.IO;
using System.Windows.Forms;
using ContentTool.Items;

namespace ContentTool.Commands
{
    public static class AddExistingFolder
    {
        public static void Execute(ContentItem selectedItem, string currentFile)
        {
            var fbd = new FolderBrowserDialog {ShowNewFolderButton = true};

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            var curFolder = selectedItem as ContentFolder ?? selectedItem.Parent;

            if (curFolder == null)
                return;

            var fn = new DirectoryInfo(fbd.SelectedPath).Name;
            curFolder.AddSubFolder(fbd.SelectedPath, Path.Combine(Path.GetDirectoryName(currentFile), curFolder.GetPath(), fn));
        }
    }
}