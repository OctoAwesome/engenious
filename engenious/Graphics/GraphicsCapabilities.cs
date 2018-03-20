using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class GraphicsCapabilities
    {
        public GraphicsCapabilities(GraphicsDevice graphicsDevice)
        {
            SupportsTextureFilterAnisotropic = graphicsDevice.Extensions.Contains("GL_EXT_texture_filter_anisotropic");
            float anisotropy = 0;
            if (SupportsTextureFilterAnisotropic)
            {
                GL.GetFloat((GetPName)All.MaxTextureMaxAnisotropyExt,out anisotropy);
            }
            MaxTextureAnisotropy = anisotropy;

            SupportsTextureMaxLevel = true;//TODO other platforms?
        }

        public bool SupportsTextureFilterAnisotropic { get; }
        public float MaxTextureAnisotropy { get; }
        public bool SupportsTextureMaxLevel { get; }
    }
}