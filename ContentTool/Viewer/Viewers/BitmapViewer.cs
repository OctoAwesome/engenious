using System;
using System.Drawing;
using System.IO;
using System.Linq.Expressions;
using System.Windows.Forms;
using ContentTool.Models;
using ContentTool.Models.History;

namespace ContentTool.Viewer.Viewers
{
    [ViewerInfo(".png", false)]
    [ViewerInfo(".bmp", false)]
    [ViewerInfo(".jpg", false)]
    public partial class BitmapViewer : UserControl, IViewer
    {
        private Image _img;

        public BitmapViewer()
        {
            InitializeComponent();
        }

        public Control GetViewerControl(ContentFile file)
        {
            History = new History();
            ContentFile = file;
            try
            {
                _img = Image.FromFile(file.FilePath);
                if (_img.Height > Height || _img.Width > Width)
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                else
                    pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            catch (FileNotFoundException)
            {
                _img = null;
            }
            pictureBox.Image = _img;

            return this;
        }

        public void Save()
        {
            
        }

        public void Discard()
        {
            
        }

        public IHistory History { get; private set; }

        public bool UnsavedChanges => false;
        public ContentFile ContentFile { get; private set; }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (Parent == null || _img == null)
                return;
            
            if (_img.Height > Height || _img.Width > Width)
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

        }
    }
}
