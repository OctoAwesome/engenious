using System;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class RenderTarget2D : Texture2D
    {
        private readonly int _fbo;
        private readonly int _depth;

        private static void SetDefaultTextureParameters()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            //if (GL.SupportsExtension ("Version12")) {
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);


            /*} else {
                GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)All.Clamp);
                GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)All.Clamp);
            }*/
        }


        public RenderTarget2D(GraphicsDevice graphicsDevice, int width, int height, PixelInternalFormat surfaceFormat)
            : base(graphicsDevice, width, height,1,surfaceFormat)
        {
            using (Execute.OnUiContext)
            {
                var isDepthTarget = ((int) surfaceFormat >= (int) PixelInternalFormat.DepthComponent16 &&
                                      (int) surfaceFormat <= (int) PixelInternalFormat.DepthComponent32Sgix);
                if (!isDepthTarget)
                {
                    GL.GenRenderbuffers(1, out _depth);

                    GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depth);
                    GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, (RenderbufferStorage) All.DepthComponent32, width, height);

                }

                _fbo = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
                //GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Rgba8, width, height);
                //GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultWidth, width);
                //GL.FramebufferParameter(FramebufferTarget.Framebuffer, FramebufferDefaultParameter.FramebufferDefaultHeight, height);

                if (isDepthTarget)
                {
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, Texture, 0);
                    Bind();
                    SetDefaultTextureParameters();
                }
                else
                {
                    GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, Texture, 0);

                    GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, _depth);
                    GL.DrawBuffers(
                        1,
                        new[] {
                            DrawBuffersEnum.ColorAttachment0
                        });
                }
                ErrorHandling();

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }
        internal void BindFbo()
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
        }

        private void ErrorHandling()
        {
            switch (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer))
            {
                case FramebufferErrorCode.FramebufferComplete:
                    break;// The framebuffer is complete and valid for rendering.
                case FramebufferErrorCode.FramebufferIncompleteAttachment:
                    throw new ArgumentException("FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachment:
                    throw new ArgumentException("FBO: There are no attachments.");
            /* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
                 {
                     throw new ArgumentException("FBO: An object has been attached to more than one attachment point.");
                     break;
                 }*/
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    throw new ArgumentException("FBO: Attachments are of different size. All attachments must have the same width and height.");
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    throw new ArgumentException("FBO: The color attachments have different format. All color attachments must have the same format.");
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    throw new ArgumentException("FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    throw new ArgumentException("FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    throw new ArgumentException("FBO: This particular FBO configuration is not supported by the implementation.");
                default:
                    throw new ArgumentException("FBO: Status unknown. (yes, this is really bad.)");
            }
        }

        public override void Dispose()
        {
            using (Execute.OnUiContext)
            {
                GL.DeleteFramebuffer(_fbo);
            }
            base.Dispose();
        }
    }
}

