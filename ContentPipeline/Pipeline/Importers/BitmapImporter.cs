using System;
using System.Drawing;

namespace engenious.Content.Pipeline
{
    [ContentImporter(".bmp", ".jpg", ".png", DisplayName = "Bitmap Importer", DefaultProcessor = "BitmapProcessor")]
    public class BitmapImporter : ContentImporter<Bitmap>
    {
        public override Bitmap Import(string filename, ContentImporterContext context)
        {
            //if (!System.IO.File.Exists(filename))
            //    return null;
            try
            {
                return new Bitmap(filename);
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename ,  ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }
    }
}

