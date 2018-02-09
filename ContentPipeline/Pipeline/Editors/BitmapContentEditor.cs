using engenious.Content.Pipeline;
using System.Drawing;
using System.Windows.Forms;

namespace engenious.Pipeline.Pipeline.Editors
{
    [ContentEditor(".bmp",".png",".jpg" )]
    public class BitmapContentEditor : IContentEditor<Bitmap, TextureContent>
    {
        private readonly PictureBox _pictureBox;
        public BitmapContentEditor()
        {
            _pictureBox=new PictureBox();
            _pictureBox.Dock = DockStyle.Fill;
            
        }

        public Control MainControl => _pictureBox;

        public void Open(Bitmap importerInput, TextureContent processorOutput)
        {
            _pictureBox.Image = importerInput;
            _pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
