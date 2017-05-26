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

        public delegate void ItemAddActionEventHandler(ContentItem item, AddType addType);

        public enum AddType
        {
            ExistingItem,
            ExistingFolder,
            NewFolder
        }
    }
}
