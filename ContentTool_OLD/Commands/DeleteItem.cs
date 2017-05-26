using ContentTool.old.Items;

namespace ContentTool.old.Commands
{
    public static class DeleteItem
    {
        public static void Execute(ContentItem item)
        {
            item.Parent.Contents.Remove(item);
        }
    }
}