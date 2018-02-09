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

        public int Progress
        {
            get => progressBar.Value;
            set => progressBar.Value = value;
        }

        public ProgressBarStyle Style
        {
            get => progressBar.Style;
            set => progressBar.Style = value;
        }

        public LoadingDialog()
        {
            InitializeComponent();
        }
    }
}