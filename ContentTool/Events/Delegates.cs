using ContentTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
