using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ContentTool.Controls
{
    public partial class ItemPropertyView : UserControl
    {
        public ItemPropertyView()
        {
            InitializeComponent();
        }

        public void SelectItem(object o)
        {
            propertyGrid.SelectedObject = o;
        }
    }
}
