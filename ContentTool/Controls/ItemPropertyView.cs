using System.Windows.Forms;

namespace ContentTool.Controls
{
    public partial class ItemPropertyView : UserControl
    {
        public ItemPropertyView()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            base.Refresh();
            propertyGrid.Update();
            propertyGrid.Refresh();
        }

        public void SelectItem(object o)
        {
            propertyGrid.SelectedObject = o;
        }
    }
}
