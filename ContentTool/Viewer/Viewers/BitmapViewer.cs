using System;
using System.Drawing;
using System.Windows.Forms;
using ContentTool.Models;

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

        public Control GetViewer(ContentFile file)
        {
            _img = Image.FromFile(file.FilePath);

            if (_img.Height > Height || _img.Width > Width)
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

            pictureBox.Image = _img;

            return this;
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (Parent == null)
                return;

            if (_img.Height > Height || _img.Width > Width)
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

        }
    }
}
