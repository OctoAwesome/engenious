using System;
using System.Text;

namespace engenious.Graphics
{
    public class SpriteBatch : GraphicsResource
    {
        public enum SpriteSortMode
        {
            Immediate,
            FrontToBack,
            BackToFront,
            Deffered,
            Texture
        }

        [Flags]
        public enum SpriteEffects
        {
            None,
            FlipHorizontally = 1,
            FlipVertically = 2
        }


        private Matrix _matrix;
        private SpriteSortMode _sortMode;
        private BlendState _blendState;
        private SamplerState _samplerState;
        private DepthStencilState _depthStencilState;
        private RasterizerState _rasterizerState;
        private readonly Effect _effect;
        private readonly SpriteBatcher _batcher;
        private EffectParameter _worldViewProj;

        public SpriteBatch(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            _batcher = new SpriteBatcher(graphicsDevice);

            var effect = new BasicEffect(graphicsDevice);
            effect.TextureEnabled = true;
            effect.VertexColorEnabled = true;
            effect.Parameters["View"].SetValue(Matrix.Identity);
            effect.Parameters["Proj"].SetValue(Matrix.Identity);

            _effect = effect;
        }


        public void Begin(SpriteSortMode sortMode = SpriteSortMode.Deffered, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            _sortMode = sortMode;
            _blendState = blendState;
            _samplerState = samplerState;
            if (depthStencilState == null)
                _depthStencilState = DepthStencilState.None;
            else
                _depthStencilState = depthStencilState;
            _rasterizerState = rasterizerState;
            //TODO: this.effect = effect;
            if (transformMatrix.HasValue)
                _matrix = transformMatrix.Value;
            else
                _matrix = Matrix.Identity;
            _worldViewProj = _effect.Parameters["World"];
            _batcher.Begin(sortMode);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        {
            Draw(texture, destinationRectangle, null, color);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            //TODO
            Draw(texture, new Vector2(destinationRectangle.X, destinationRectangle.Y), sourceRectangle, color, 0f, new Vector2(), new Vector2(destinationRectangle.Width, destinationRectangle.Height), SpriteEffects.None, 0.0f);
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            Draw(texture, position, null, color);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            Draw(texture, position, sourceRectangle, color, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None);//TODO?
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth = 0.0f)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale * texture.Width, scale * texture.Height), effects, layerDepth);//TODO?
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 size, SpriteEffects effects, float layerDepth)
        {
            _batcher.AddBatch(SpriteBatcher.BatchPool.AquireBatch(texture, position, sourceRectangle, color, rotation, origin, size, effects, layerDepth, _sortMode));
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, RectangleF sourceRectangle, Color color)
        {
            //TODO
            Draw(texture, new Vector2(destinationRectangle.X, destinationRectangle.Y), sourceRectangle, color, 0f, new Vector2(), new Vector2(destinationRectangle.Width, destinationRectangle.Height), SpriteEffects.None, 0.0f);
        }

        public void Draw(Texture2D texture, Vector2 position, RectangleF sourceRectangle, Color color)
        {
            Draw(texture, position, sourceRectangle, color, 0, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None);//TODO?
        }

        public void Draw(Texture2D texture, Vector2 position, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth = 0.0f)
        {
            Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale * texture.Width, scale * texture.Height), effects, layerDepth);//TODO?
        }

        public void Draw(Texture2D texture, Vector2 position, RectangleF sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 size, SpriteEffects effects, float layerDepth)
        {
            _batcher.AddBatch(SpriteBatcher.BatchPool.AquireBatch(texture, position, sourceRectangle, color, rotation, origin, size, effects, layerDepth, _sortMode));
        }


        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
        {
            DrawString(spriteFont, text, position, color, 0.0f, new Vector2(), 1.0f);
        }

        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
        {
            DrawString(spriteFont, text, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }

        public void DrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
        {
            DrawString(spriteFont, text.ToString(), position, color, rotation, origin, scale, effects, layerDepth);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color)
        {
            DrawString(spriteFont, text, position, color, 0.0f, new Vector2(), 1.0f);
        }

        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
        {
            DrawString(spriteFont, text, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }


        public void DrawString(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects = SpriteEffects.None, float layerDepth = 0.0f)
        {
            var offset = new Vector2(0.0f, 0.0f);
            for (var i = 0; i < text.Length; i++)
            {

                var c = text[i];
                FontCharacter fontChar;
                if (!spriteFont.CharacterMap.TryGetValue(c, out fontChar))
                {
                    if (c == '\n')
                    {
                        offset.X = 0;
                        offset.Y += spriteFont.LineSpacing;
                        continue;
                    }
                    if (!spriteFont.DefaultCharacter.HasValue || !spriteFont.CharacterMap.TryGetValue(spriteFont.DefaultCharacter.Value, out fontChar))
                    {
                        continue;
                    }
                }
                if (fontChar == null)
                    continue;
                
                Draw(spriteFont.Texture, position + offset + fontChar.Offset, fontChar.TextureRegion, color, rotation, origin - offset, 1.0f, SpriteEffects.None, layerDepth);
                offset.X += fontChar.Advance;
                if (i < text.Length - 1)
                {
                    int kerning;
                    if (spriteFont.Kernings.TryGetValue(SpriteFont.GetKerningKey(c, text[i + 1]), out kerning))
                        offset.X += kerning;
                }
            }
        }

        public void End()
        {
            GraphicsDevice.BlendState = _blendState;
            GraphicsDevice.RasterizerState = _rasterizerState;
            GraphicsDevice.DepthStencilState = _depthStencilState;
            _batcher.SamplerState = _samplerState;

            var projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, -1);

            _worldViewProj.SetValue(projection * _matrix);
            _batcher.End(_effect);


        }

        public override void Dispose()
        {
            _batcher.Dispose();
            base.Dispose();
        }
    }
}

