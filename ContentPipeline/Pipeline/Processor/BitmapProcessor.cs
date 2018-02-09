using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using engenious.Content.Serialization;

namespace engenious.Content.Pipeline
{
    [ContentProcessor(DisplayName = "Bitmap Processor")]

    public class BitmapProcessor : ContentProcessor<Bitmap, TextureContent,BitmapProcessorSettings>
    {
        #region implemented abstract members of ContentProcessor
        public override TextureContent Process(Bitmap input, string filename, ContentProcessorContext context)
        {
            var data = input.LockBits(new System.Drawing.Rectangle(0,0,input.Width,input.Height),ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            TextureContent content = new TextureContent(context.GraphicsDevice,!settings.AutoGenerateMipMaps,settings.MipMapCount,data.Scan0,input.Width,input.Height,TextureContentFormat.Png,settings.Format);
            input.UnlockBits(data);
            return content;
        }
        #endregion
    }
    [Serializable]
    public class BitmapProcessorSettings : ProcessorSettings
    {
        [DefaultValue(false)]
        public bool AutoGenerateMipMaps{get;set;}=false;
        [DefaultValue(1)]
        public int MipMapCount{get;set;}=1;
        [DefaultValue(TextureContentFormat.Png)]
        public TextureContentFormat Format{get;set;}=TextureContentFormat.Png;
    }
}

