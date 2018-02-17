using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using engenious.Content.Pipeline;

namespace ContentTool.Controls
{
    public partial class ItemPropertyView : PropertyGrid
    {
        private GridItem _gridItem;
        
        public ItemPropertyView()
        {
            InitializeComponent();
            
            SafeTypeDescriptorWrapper.WorkaroundEvent += WorkaroundEvent;
        }

        private void WorkaroundEvent(object sender, EventArgs eventArgs)
        {
            var prop = (PropertyDescriptor) sender;
            if (typeof(ProcessorSettings).IsAssignableFrom(prop.ComponentType))
                CloseByName(prop.DisplayName);
        }

        public override void Refresh()
        {
            base.Refresh();
            base.Update();
            base.Refresh();
            
        }

        public void SelectItem(object o)
        {
            base.SelectedObject = o;
            base.ExpandAllGridItems();//TODO: perhaps only expand Settings?
        }

        void CloseByName(string name)
        {
            var root = propertyGrid.SelectedGridItem ?? (_gridItem = (GridItem) typeof(PropertyGrid)
                           .GetField("root_grid_item", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(this));

            if (root == null)
                return;
            while (root.Parent != null)
                root = root.Parent;

            CloseGridItem(root, name);
        }

        private void CloseGridItem(GridItem parent, string name)
        {
            foreach (var child in parent.GridItems.OfType<GridItem>())
            {
                if (child.Expanded && child.Label == name)
                    child.Expanded = false;
                CloseGridItem(child,name);
            }
        }

    }
}
