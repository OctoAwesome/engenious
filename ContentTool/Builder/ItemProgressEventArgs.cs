using ContentTool.Items;

namespace ContentTool.Builder
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

