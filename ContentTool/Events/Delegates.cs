using ContentTool.Models;

namespace ContentTool
{
    public class Delegates
    {
        public delegate void ItemActionEventHandler(ContentItem item);

        public delegate void FolderAddActionEventHandler(ContentFolder folder);

        public delegate void ItemAddActionEventHandler(ContentFolder folder, AddType addType);

        public enum AddType
        {
            ExistingItem,
            ExistingFolder,
            NewFolder
        }
    }
}
