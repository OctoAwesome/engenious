using engenious.Helper;

namespace engenious.Graphics
{
    public class BasicEffect : Effect,IModelEffect
    {
        private const string VertexShader =
            @"
#if __VERSION__ >= 300

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
#else
attribute vec3 position;
attribute vec4 color;
attribute vec2 textureCoordinate;
varying vec4 psColor;
varying vec2 psTexCoord;

uniform mat4 World;
uniform mat4 View;
uniform mat4 Proj;
void main(void)
{
   gl_Position = Proj*View*World*vec4(position, 1.0);
   psColor = color;
   psTexCoord = textureCoordinate;
}
#endif
";
        private const string PixelShader =
            @"
#if __VERSION__ >= 300
in vec4 psColor;
in vec2 psTexCoord;
out vec4 outColor;
uniform sampler2D text;
uniform int textEnabled,colorEnabled;
void main(void)
{
   outColor = vec4(1.0,1.0,1.0,1.0);
   if (textEnabled == 1)
     outColor = outColor * texture2D(text,psTexCoord);
   if (colorEnabled == 1)
     outColor = outColor * psColor;
   
}
#else
varying vec4 psColor;
varying vec2 psTexCoord;
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
#endif
";

        public BasicEffect(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            var technique = new EffectTechnique("Basic");

            using (Execute.OnUiContext)
            {
                Shader[] shaders = {
                    new Shader(graphicsDevice,ShaderType.VertexShader, VertexShader),
                    new Shader(graphicsDevice,ShaderType.FragmentShader, PixelShader)
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

