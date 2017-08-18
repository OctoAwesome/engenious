using System.IO;
using System.Windows.Forms;
using ContentTool.Models;

namespace ContentTool.Viewer.Viewers
{

    [ViewerInfo(".txt", false)]
    [ViewerInfo(".glsl", false)]
    public partial class TextViewer : UserControl, IViewer
    {
        public TextViewer()
        {
            InitializeComponent();
        }

        public Control GetViewer(ContentFile file)
        {
            richTextBox.Clear();
            richTextBox.Lines = File.ReadAllLines(file.FilePath);
            return this;
        }
    }
}
