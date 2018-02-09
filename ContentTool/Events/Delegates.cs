using ContentTool.Models;

namespace ContentTool
{
    public class Delegates
    {
        public delegate void ItemActionEventHandler(ContentItem item);

        public delegate void FolderAddActionEventHandler(ContentFolder folder);
    }
}