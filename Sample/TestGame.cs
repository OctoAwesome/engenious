using System;
using engenious;
using engenious.Audio;
using engenious.Graphics;

namespace Sample
{
    public class TestGame : Game
    {
        private readonly Texture2D _texture;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;

        private RenderTarget2D _target;

        private readonly engenious.UserEffects.simple _effect;
        private SoundEffect _testSoundEffect;
        private SoundEffectInstance _testSound,_testSound2;
        public TestGame()
        {
            _texture = new Texture2D(GraphicsDevice, 512, 512); //Content.Load<Texture2D>("brick");
            _font = Content.Load<SpriteFont>("test");
            _effect = Content.Load<engenious.UserEffects.simple>("simple");
            _spriteBatch = new SpriteBatch(GraphicsDevice);


        }

        public override void LoadContent()
        {
            base.LoadContent();
            _testSoundEffect = Content.Load<SoundEffect>("blub_x");
            _testSound = _testSoundEffect.CreateInstance();
            _testSound2 = _testSoundEffect.CreateInstance();
            _testSound.Play();

            _testSound2.Play();
        }

        protected override void OnResize(object sender, EventArgs e)
        {
            if (GraphicsDevice.Viewport.Width != 0 && GraphicsDevice.Viewport.Height != 0)
            {
                if (_target != null && !_target.IsDisposed)
                    _target.Dispose();
                _target = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width,
                    GraphicsDevice.Viewport.Height, PixelInternalFormat.Rgba8);
            }
            base.OnResize(sender, e);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _texture.BindComputation();
            //_effect.Compute.p1.Compute(_texture.Width, _texture.Height);
            //_effect.Compute.p1.WaitForImageCompletion();
            //_effect.Ambient.Pass1.AmbientColor = Color.AliceBlue;
            
            _spriteBatch.Begin();
            _spriteBatch.Draw(_texture, new Rectangle(0, 0, 512, 512), Color.White);
            _spriteBatch.DrawString(_font, "Taxi.\nTT\nTx\nTe\nTA", new Vector2(), Color.Black);
            _spriteBatch.End();
        }
    }
}