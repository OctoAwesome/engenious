using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using engenious.Content;
using engenious.Content.Pipeline;
using engenious.Content.Serialization;
using engenious.Graphics;
using engenious.Helper;
using SharpFont;

namespace engenious.Pipeline
{
    [ContentProcessor(DisplayName = "Font Processor")]
    public class SpriteFontProcessor : ContentProcessor<SpriteFontContent, CompiledSpriteFont>
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        #region implemented abstract members of ContentProcessor
        public override CompiledSpriteFont Process(SpriteFontContent input, string filename, ContentProcessorContext context)
        {
            string fontFile;
            if (!FontConfig.Instance.GetFontFile(input.FontName, input.Size, input.Style, out fontFile))
                context.RaiseBuildMessage(filename, $"'{input.FontName}' was not found, using fallback font", BuildMessageEventArgs.BuildMessageType.Warning);

            if (fontFile == null)
            {
                context.RaiseBuildMessage(filename, $"'{input.FontName}' was not found, no fallback font provided", BuildMessageEventArgs.BuildMessageType.Error);
                return null;
            }

            //Initialization

            if (PlatformHelper.RunningPlatform() == Platform.Windows)
            {
                string arch = Environment.Is64BitProcess ? "x64" : "x86";
                string dllPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), arch);
                Console.WriteLine("DllPath: " + dllPath);
                SetDllDirectory(dllPath);
            }

            Library lib = new Library();
            var face = lib.NewFace(fontFile, 0);
            Bitmap dummyBitmap = new Bitmap(1,1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dummyBitmap);
            face.SetCharSize(new Fixed26Dot6(0), new Fixed26Dot6(input.Size), (uint)g.DpiX, (uint)g.DpiY);

            CompiledSpriteFont compiled = new CompiledSpriteFont();
            compiled.Spacing = input.Spacing;
            compiled.DefaultCharacter = input.DefaultCharacter;

            
            var glyphs = new Dictionary<uint, GlyphSlot>();

            var characters = input.CharacterRegions.SelectMany(
                r => r.GetChararcters().Select(c => Tuple.Create(c, face.GetCharIndex(c)))).Where(x=>x.Item2 != 0).ToList();

            var bitmaps = new List<Tuple<char, FTBitmap,int,GlyphMetrics>>();

            compiled.LineSpacing = face.Size.Metrics.Height.Value>>6;
            compiled.BaseLine = face.Size.Metrics.Ascender.Value>>6;
            //Loading Glyphs, Calculate Kernings and Create Bitmaps
            int totalWidth=0,maxWidth=0,maxHeight=0;
            foreach (var l in characters)
            {
                var character = l.Item1;
                var glyphIndex = l.Item2;
                
                //Load Glyphs
                face.LoadGlyph(glyphIndex, LoadFlags.Color, LoadTarget.Normal);
                var glyph = face.Glyph;
                glyph.Tag = character;
                glyphs.Add(character, glyph);

                //Calculate Kernings
                if (input.UseKerning)
                {
                    foreach (var r in characters)
                    {
                        var kerning = face.GetKerning(l.Item2, r.Item2, KerningMode.Default);
                        if (kerning == default(FTVector26Dot6)) continue;
                        compiled.Kernings[l.Item1<<16|r.Item1] = kerning.X.Value>>6;
                    }
                }

                //Create bitmaps
                glyph.OwnBitmap();
                var glyphActual = glyph.GetGlyph();
                glyphActual.ToBitmap(RenderMode.Normal, default(FTVector26Dot6), false);

                var bmg = glyphActual.ToBitmapGlyph();
                if (bmg.Bitmap.Width == 0 || bmg.Bitmap.Rows == 0)
                {
                    totalWidth += 2+1;
                    maxWidth = Math.Max(maxWidth,1+2);
                    maxHeight = Math.Max(maxHeight,1+2);
                    bitmaps.Add(Tuple.Create(character, (FTBitmap)null, glyph.Advance.X.Value>>6,glyph.Metrics));
                }
                else
                {
                    var bmp = bmg.Bitmap;
                    totalWidth += 2+bmp.Width;
                    maxWidth = Math.Max(maxWidth,bmp.Width+2);//TODO: divide by 3?
                    maxHeight = Math.Max(maxHeight,bmp.Rows+2);
                    bitmaps.Add(Tuple.Create(character, bmp, glyph.Advance.X.Value>>6,glyph.Metrics));
                }

            }
            g.Dispose();
            dummyBitmap.Dispose();
            int cellCount = (int)Math.Ceiling(Math.Sqrt(bitmaps.Count));

            var target = new Bitmap(cellCount*maxWidth,cellCount*maxHeight);
            var targetRectangle = new Rectangle(0, 0, target.Width, target.Height);
            var targetData = target.LockBits(new System.Drawing.Rectangle(0, 0, target.Width, target.Height), ImageLockMode.WriteOnly, target.PixelFormat);
            int offsetX = 0,offsetY=0;

            //Create Glyph Atlas
            foreach (var bmpKvp in bitmaps)
            {
                var bmp = bmpKvp.Item2;
                var character = bmpKvp.Item1;

                if (bmp == null)
                {
                    compiled.CharacterMap.Add(character, new FontCharacter(character,targetRectangle,new Rectangle(offsetX,offsetY,1,1),new Vector2(bmpKvp.Item4.HorizontalBearingX.Value >> 6,compiled.BaseLine - (bmpKvp.Item4.HorizontalBearingY.Value>>6)), bmpKvp.Item3));
                    if (offsetX++ > target.Width)
                    {
                        offsetY += maxHeight;
                        offsetX = 0;
                    }
                    continue;
                }
                int width = bmp.Width;
                int height = bmp.Rows;
                if (offsetX+width > target.Width)
                {
                    offsetY += maxHeight;
                    offsetX = 0;
                }
                //TODO divide width by 3?
                compiled.CharacterMap.Add(character, new FontCharacter(character,targetRectangle,new Rectangle(offsetX,offsetY,width,height),new Vector2(bmpKvp.Item4.HorizontalBearingX.Value >> 6,compiled.BaseLine - (bmpKvp.Item4.HorizontalBearingY.Value>>6)), bmpKvp.Item3));

                unsafe{
                    switch(bmp.PixelMode)
                    {
                        case PixelMode.Mono:
                            CopyFTBitmapToAtlas_Mono((uint*)targetData.Scan0+offsetX+offsetY*target.Width,offsetX,offsetY,target.Width,bmp,width,height);//TODO: divide width by 3?
                            break;
                        case PixelMode.Gray:
                            CopyFTBitmapToAtlas_Gray((uint*)targetData.Scan0+offsetX+offsetY*target.Width,offsetX,offsetY,target.Width,bmp,width,height);//TODO: divide width by 3?
                            break;
                        case PixelMode.Lcd:
                            CopyFTBitmapToAtlas_LcdBGR((uint*)targetData.Scan0+offsetX+offsetY*target.Width,offsetX,offsetY,target.Width,bmp,width,height);//TODO: divide width by 3?
                            break;
                        case PixelMode.Bgra:
                            CopyFTBitmapToAtlas_BGRA((uint*)targetData.Scan0+offsetX+offsetY*target.Width,offsetX,offsetY,target.Width,bmp,width,height);//TODO: divide width by 3?
                            break;
                        default:
                            throw new NotImplementedException("Pixel Mode not supported");
                    }
                }
                offsetX += width;//TODO divide by 3?
                bmp.Dispose();
            }
            compiled.Texture = new TextureContent(context.GraphicsDevice,false,1,targetData.Scan0,target.Width,target.Height,TextureContentFormat.Png,TextureContentFormat.Png);
            compiled.Spacing = input.Spacing;
            compiled.DefaultCharacter = input.DefaultCharacter;
            
            target.UnlockBits(targetData);
            
            //Saving files
            //target.Save("test.png",ImageFormat.Png);
            target.Dispose();
            //System.Diagnostics.Process.Start("test.png"); //TODO: Remove later

            return compiled;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyFTBitmapToAtlas_Mono(uint* targetPtr,int offsetX,int offsetY,int targetWidth,FTBitmap bmp,int width,int height)
        {
            var bmpPtr = (byte*)bmp.Buffer;
            int subIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, targetPtr++, subIndex++,bmpPtr++)
                {
                    if ((((*bmpPtr) >> subIndex) & 0x1) != 0)
                        *targetPtr = 0xFFFFFFFF; 
                    if (subIndex == 8)
                        subIndex = 0;
                }
                targetPtr += targetWidth - width;
            }
        }

        #endregion


        #region Copy Implementations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyFTBitmapToAtlas_Gray4(uint* targetPtr,int offsetX,int offsetY,int targetWidth,FTBitmap bmp,int width,int height,int padding)
        {
            //TODO: implement
            var bmpPtr = (byte*)bmp.Buffer;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, targetPtr++, bmpPtr++)
                {
                    byte value = *bmpPtr;
                    *targetPtr = (uint)(value<<24) | 0xFFFFFFu; //value > 0 ? 255<< 24: 0;
                }
                targetPtr += targetWidth - width;
                bmpPtr += padding;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyFTBitmapToAtlas_Gray(uint* targetPtr,int offsetX,int offsetY,int targetWidth,FTBitmap bmp,int width,int height)
        {
            var bmpPtr = (byte*)bmp.Buffer;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, targetPtr++, bmpPtr++)
                {
                    byte value = *bmpPtr;
                    *targetPtr = (uint)(value<<24) | 0xFFFFFFu;
                }
                targetPtr += targetWidth - width;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyFTBitmapToAtlas_LcdRGB(uint* targetPtr,int offsetX,int offsetY,int targetWidth,FTBitmap bmp,int width,int height)
        {
            var bmpPtr = (byte*)bmp.Buffer;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, targetPtr++, bmpPtr+=3)
                {
                    uint value = *(uint*)(bmpPtr) >> 8;
                    *targetPtr = 0xFF000000 | value;
                }
                targetPtr += targetWidth - width;
            }
        }
        //TODO: verify direction
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyFTBitmapToAtlas_LcdBGR(uint* targetPtr,int offsetX,int offsetY,int targetWidth,FTBitmap bmp,int width,int height)
        {
            var bmpPtr = (byte*)bmp.Buffer;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, targetPtr++, bmpPtr+=3)
                {
                    uint value = *(uint*)(bmpPtr);
                    //A | R | G | B
                    *targetPtr = 0xFF000000 | (value >> 24) | (value << 8) & 0xFF0000 | (value >> 8) & 0xFF;
                }
                targetPtr += targetWidth - width;
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void CopyFTBitmapToAtlas_BGRA(uint* targetPtr,int offsetX,int offsetY,int targetWidth,FTBitmap bmp,int width,int height)
        {
            var bmpPtr = (uint*)bmp.Buffer;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, targetPtr++, bmpPtr++)
                {
                    uint value = *bmpPtr;
                    //A | R | G | B
                    *targetPtr = (value << 24) | (value >> 24) | (value << 8) & 0xFF0000 | (value >> 8) & 0xFF;
                }
                targetPtr += targetWidth - width;
            }
        }
        #endregion
    }
}

