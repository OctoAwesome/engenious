using ContentTool.old.Builder;
using ContentTool.old.Items;

namespace ContentTool.old.Commands
{
    public static class BuildItem
    {
        public static void Execute(ContentItem selectedItem, ContentBuilder builder)
        {
            builder.Build(selectedItem);
        }
    }
}
