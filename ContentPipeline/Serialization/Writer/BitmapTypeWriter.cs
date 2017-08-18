using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace engenious.Content.Serialization
{
    [ContentTypeWriter]
    public class BitmapTypeWriter : ContentTypeWriter<Bitmap>
    {
        private readonly bool _usePng = true;

        public override void Write(ContentWriter writer, Bitmap bmp)
        {
            if (_usePng)
            {
                writer.Write((byte)1);
                using (var str = new MemoryStream())
                {
                    bmp.Save(str, ImageFormat.Png);

                    writer.Write((int)str.Position);
                    str.Position = 0;
                    writer.Write(str);
                }
            }
            else
            {
                writer.Write((byte)0);
                writer.Write(bmp.Width);
                writer.Write(bmp.Height);
                var data = new int[bmp.Width * bmp.Height];
                var bmpData = bmp.LockBits(new System.Drawing.Rectangle(new System.Drawing.Point(), bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);

                Marshal.Copy(bmpData.Scan0, data, 0, data.Length);

                bmp.UnlockBits(bmpData);
                foreach (int val in data)//TODO: buffer copy?
					writer.Write(val);
            }
        }

        public override string RuntimeReaderName => typeof(Texture2DTypeReader).FullName;
    }
}

