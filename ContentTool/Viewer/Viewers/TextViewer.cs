using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ContentTool.Models;
using System.IO;

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
