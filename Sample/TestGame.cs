using System;
using System.Linq;
using System.Threading;
using engenious.Graphics;
using engenious;
using engenious.Audio;
using OpenTK.Graphics.OpenGL4;

namespace Sample
{
    public class TestGame : engenious.Game
    {
        private Texture2D texture;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private RenderTarget2D target;

        private Effect effect;
        private SoundEffect testSoundEffect;
        private SoundEffectInstance testSound,testSound2;
        public TestGame()
        {
            texture = new Texture2D(GraphicsDevice, 512, 512); //Content.Load<Texture2D>("brick");
            font = Content.Load<SpriteFont>("test");
            effect = Content.Load<Effect>("simple");

            spriteBatch = new SpriteBatch(GraphicsDevice);


        }

        public override void LoadContent()
        {
            base.LoadContent();
            testSoundEffect = Content.Load<SoundEffect>("blub");
            testSound = testSoundEffect.CreateInstance();
            testSound2 = testSoundEffect.CreateInstance();
            testSound.Play();
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != 0 && GraphicsDevice.Viewport.Height != 0)
            {
                if (target != null && !target.IsDisposed)
                    target.Dispose();
                target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height, engenious.PixelInternalFormat.Rgba8);
            }
            base.OnResize(sender, e);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            texture.BindComputation(0);
            effect.CurrentTechnique = effect.Techniques["Compute"];
            effect.CurrentTechnique.Passes[0].Compute(texture.Width, texture.Height);
            effect.CurrentTechnique.Passes[0].WaitForImageCompletion();

            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle(0, 0, 512, 512), Color.White);
            spriteBatch.DrawString(font, "Taxi.\nTT\nTx\nTe\nTA", new Vector2(), Color.Black);
            spriteBatch.End();
        }
    }
}