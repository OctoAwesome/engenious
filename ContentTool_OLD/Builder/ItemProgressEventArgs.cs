using ContentTool.old.Items;

namespace ContentTool.old.Builder
{
    public class ItemProgressEventArgs
    {
        public ItemProgressEventArgs(BuildStep buildStep,ContentItem item)
        {
            BuildStep = buildStep;
            Item = item;
        }
        public BuildStep BuildStep{get;private set;}
        public ContentItem Item{get;private set;}
    }
}

