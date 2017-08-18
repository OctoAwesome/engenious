using System.Windows.Forms;

namespace ContentTool.Forms.Dialogs
{
    public partial class LoadingDialog : Form
    {
        public string Title
        {
            get => Text;
            set => Text = value;
        }

        public LoadingDialog()
        {
            InitializeComponent();
        }
    }
}
