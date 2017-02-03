using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace engenious.Content.Serialization
{
    [ContentTypeWriter]
    public class BitmapTypeWriter : ContentTypeWriter<Bitmap>
    {
        private bool usePNG = true;

        public override void Write(ContentWriter writer, Bitmap bmp)
        {
            if (usePNG)
            {
                writer.Write((byte)1);
                using (MemoryStream str = new MemoryStream())
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
                int[] data = new int[bmp.Width * bmp.Height];
                BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(new System.Drawing.Point(), bmp.Size), ImageLockMode.ReadOnly, bmp.PixelFormat);

                Marshal.Copy(bmpData.Scan0, data, 0, data.Length);

                bmp.UnlockBits(bmpData);
                foreach (int val in data)//TODO: buffer copy?
					writer.Write(val);
            }
        }

        public override string RuntimeReaderName => typeof(Texture2DTypeReader).FullName;
    }
}

