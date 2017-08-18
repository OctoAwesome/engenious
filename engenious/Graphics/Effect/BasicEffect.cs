using engenious.Helper;

namespace engenious.Graphics
{
    public class BasicEffect : Effect,IModelEffect
    {
        private const string VertexShader =
            @"
#version 440

in vec3 position;
in vec4 color;
in vec2 textureCoordinate;
out vec4 psColor;
out vec2 psTexCoord;

uniform mat4 World;
uniform mat4 View;
uniform mat4 Proj;
void main(void)
{
   gl_Position = Proj*View*World*vec4(position, 1.0);
   psColor = color;
   psTexCoord = textureCoordinate;
}
";
        private const string PixelShader =
            @"
#version 440
in vec4 psColor;
in vec2 psTexCoord;
uniform sampler2D text;
uniform int textEnabled,colorEnabled;
void main(void)
{
   gl_FragColor = vec4(1.0,1.0,1.0,1.0);
   if (textEnabled == 1)
     gl_FragColor = gl_FragColor * texture2D(text,psTexCoord);
   if (colorEnabled == 1)
     gl_FragColor = gl_FragColor * psColor;
   
}
";

        public BasicEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            var technique = new EffectTechnique("Basic");

            using (Execute.OnUiContext)
            {
                Shader[] shaders = {
                    new Shader(ShaderType.VertexShader, VertexShader),
                    new Shader(ShaderType.FragmentShader, PixelShader)
                };

                foreach (var shader in shaders)
                    shader.Compile();
                var pass = new EffectPass("Basic");
                pass.AttachShaders(shaders);
                pass.BindAttribute(VertexElementUsage.Color, "color");
                pass.BindAttribute(VertexElementUsage.TextureCoordinate, "textureCoordinate");
                pass.BindAttribute(VertexElementUsage.Position, "position");
                pass.Link();

                technique.Passes.Add(pass);
                Techniques.Add(technique);
            }

            CurrentTechnique = technique;
            
            Initialize();

            World = View = Projection = Matrix.Identity;


        }

        #region IEffectMatrices implementation

        private Matrix _world, _view, _projection;

        public Matrix Projection
        {
            get
            {
                return _projection;
            }
            set
            {
                if (_projection != value)
                {
                    _projection = value;
                    Parameters["Proj"].SetValue(value);
                }
            }
        }

        public Matrix View
        {
            get
            {
                return _view;
            }
            set
            {
                if (_view != value)
                {
                    _view = value;
                    Parameters["View"].SetValue(value);
                }
            }
        }

        public Matrix World
        {
            get
            {
                return _world;
            }
            set
            {
                if (_world != value)
                {
                    _world = value;
                    Parameters["World"].SetValue(value);
                }
            }
        }

        public Texture Texture
        {

            set
            {
                GraphicsDevice.Textures[0] = value;
                Parameters["text"].SetValue(0);
            }
        }

        public bool TextureEnabled
        {

            set
            {
                Parameters["textEnabled"].SetValue(value ? 1 : 0);
            }
        }

        public bool VertexColorEnabled
        {

            set
            {
                Parameters["colorEnabled"].SetValue(value ? 1 : 0);
            }
        }

        #endregion
    }
}

