using engenious;
using engenious.Graphics;
using engenious.UserDefined;

namespace Sample
{
    public class TestShader : simple, IModelEffect
    {
        private Texture _texture;
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;

        public TestShader(GraphicsDevice graphicsDevice) : base(graphicsDevice)
        {
        }

        public Matrix Projection
        {
            get { return _projection; }
            set
            {
                _projection = value; 
                UpdateMatrix();
            }
        }

        public Matrix View
        {
            get { return _view; }
            set
            {
                _view = value;
                UpdateMatrix();
            }
        }

        public Matrix World
        {
            get { return _world; }
            set
            {
                _world = value; 
                UpdateMatrix();
            }
        }

        public Texture Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
            }
        }

        private void UpdateMatrix()
        {
            base.Ambient.Pass1.WorldViewProj = Projection * View * World;
        }
    }
}