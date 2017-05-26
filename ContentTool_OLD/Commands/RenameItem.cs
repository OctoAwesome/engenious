using System.Collections.Generic;
using System.Windows.Forms;
using ContentTool.old.Items;

namespace ContentTool.old.Commands
{
    public static class RenameItem
    {
        public static void Execute(ContentItem item, Dictionary<ContentItem, TreeNode> treeMap)
        {
            if (item is ContentProject)
                return;

            TreeNode node = null;
            if (treeMap.TryGetValue(item, out node))
            {

                node.BeginEdit();
            }
        }
    }
}