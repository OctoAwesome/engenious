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

namespace ContentTool.Viewer.Viewers
{
    [ViewerInfo(".png", false)]
    [ViewerInfo(".bmp", false)]
    [ViewerInfo(".jpg", false)]
    public partial class BitmapViewer : UserControl, IViewer
    {
        Image img;

        public BitmapViewer()
        {
            InitializeComponent();
        }

        public Control GetViewer(ContentFile file)
        {
            img = Image.FromFile(file.FilePath);

            if (img.Height > this.Height || img.Width > Width)
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

            pictureBox.Image = img;

            return this;
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (Parent == null)
                return;

            if (img.Height > this.Height || img.Width > Width)
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            else
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

        }
    }
}
