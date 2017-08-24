using System;
using System.IO;
using System.Windows.Forms;
using ContentTool.Models;
using ContentTool.Models.History;

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

        private ContentProject _project;
        private string _oldText;

        public Control GetViewerControl(ContentFile file)
        {
            History = new History();
            ContentFile = file;
            _project = file.Project;
            richTextBox.Clear();
            richTextBox.TextChanged -= RichTextBoxOnTextChanged;
            _oldText = richTextBox.Text = File.ReadAllText(file.FilePath);
            richTextBox.TextChanged += RichTextBoxOnTextChanged;
            return this;
        }

        private void RichTextBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            if (richTextBox.Text == _oldText)
                return;
            History.Push(new HistoryPropertyChange(richTextBox, nameof(richTextBox.Text), _oldText, richTextBox.Text));
            _oldText = richTextBox.Text;
        }

        public void Save()
        {
            richTextBox.SaveFile(ContentFile.FilePath, RichTextBoxStreamType.PlainText);
        }

        public void Discard()
        {
        }

        public IHistory History { get; private set; }

        public bool UnsavedChanges => richTextBox.Text != _oldText;
        public ContentFile ContentFile { get; private set; }
    }
}