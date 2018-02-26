using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using engenious.Helper;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class GraphicsDevice : IDisposable
    {
        private BlendState _blendState;
        private DepthStencilState _depthStencilState;
        private RasterizerState _rasterizerState;
        private Rectangle _scissorRectangle;
        private Viewport _viewport;
        private readonly IGraphicsContext _context;


        /*DebugProc DebugCallbackInstance = DebugCallback;

        static void DebugCallback(DebugSource source, DebugType type, int id,
                                  DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = Marshal.PtrToStringAnsi(message);
            Console.WriteLine("[GL] {0}; {1}; {2}; {3}; {4}",
                source, type, id, severity, msg);
        }*/

        internal Game Game;
        internal readonly HashSet<string> Extensions = new HashSet<string>();
        internal string DriverVendor;
        internal Version DriverVersion;
        internal Version GlslVersion;

        
        public GraphicsDevice(Game game, IGraphicsContext context)
        {
            _context = context;
            Game = game;

            int count;
            GL.GetInteger(GetPName.NumExtensions, out count);
            for (var i = 0; i < count; i++)
            {
                var extension = GL.GetString(StringNameIndexed.Extensions, i);
                Extensions.Add(extension);
            }

            ReadOpenGlVersion();
#if DEBUG
            if (Extensions.Contains("GL_ARB_debug_output"))
            {
                _context.ErrorChecking = true;
                //GL.Enable(EnableCap.DebugOutput);
                //GL.Enable(EnableCap.DebugOutputSynchronous);
                //GL.DebugMessageCallback(DebugCallbackInstance, IntPtr.Zero);
            }
#endif

            Capabilities = new GraphicsCapabilities(this);

            Textures = new TextureCollection();
            CheckError();
            //TODO: samplerstate
        }

        private void ReadOpenGlVersion()
        {
            string versionString = null, fullVersion = null;
            try
            {
                fullVersion = GL.GetString(StringName.Version);
                var splt = fullVersion.Split(' ');
                versionString = splt.FirstOrDefault();
                if (versionString == null)
                    return;
                DriverVersion = new Version(versionString);
                DriverVendor = GL.GetString(StringName.Vendor);
                fullVersion = GL.GetString(StringName.ShadingLanguageVersion);
                versionString = fullVersion.Split(' ').FirstOrDefault();
                if (versionString == null)
                    return;

                GlslVersion = new Version(versionString);
            }
            catch (ArgumentException ex)
            {
                if (!Extensions.Contains("VERSION_1_2"))
                {
                    DriverVersion = new Version(1,0);
                    GlslVersion = new Version(1,0);//throw new Exception(string.Join("\r\n\t",Extensions.Keys)+$"\r\ncan't parse version: {versionString} fullversion{fullVersion}", ex);
                    return;
                }

                DriverVersion = new Version(GL.GetInteger(GetPName.MajorVersion), GL.GetInteger(GetPName.MinorVersion));
                if (DriverVersion.Major == 2)
                {
                    GlslVersion = DriverVersion.Minor == 0 ? new Version(1, 10) : new Version(1, 20);
                }
                else if (DriverVersion.Major == 3 && DriverVersion.Minor <= 2)
                {
                    GlslVersion = new Version(1, 30 + 10 * DriverVersion.Minor);
                }
                else if (DriverVersion.Major >= 3)
                {
                    GlslVersion = new Version(DriverVersion.Major, DriverVersion.Minor * 10);
                }
                else
                {
                    GlslVersion = new Version(1, 0);
                }
            }
        }

        public void Clear(ClearBufferMask mask)
        {
            using (Execute.OnUiContext)
            {
                GL.Clear((OpenTK.Graphics.OpenGL.ClearBufferMask) mask);
            }
        }

        public void Clear(ClearBufferMask mask, System.Drawing.Color color)
        {
            using (Execute.OnUiContext)
            {
                GL.ClearColor(color);
                GL.Clear((OpenTK.Graphics.OpenGL.ClearBufferMask) mask);
            }
        }

        internal void CheckError()
        {
            //return;
#if DEBUG
            /*var frame = new System.Diagnostics.StackTrace(true).GetFrame(1);
            ErrorCode code = ErrorCode.InvalidValue;
            ThreadingHelper.BlockOnUIThread(() =>
                {
                    code = GL.GetError();
                    if (code != ErrorCode.NoError)
                    {
                        
                        string filename = frame.GetFileName();
                        int line = frame.GetFileLineNumber();
                        string method = frame.GetMethod().Name;
                        Debug.WriteLine("[GL] " + filename + ":" + method + " - " + line.ToString() + ":" + code.ToString());
                    }
                }, true);*/
#endif
        }

        public void Clear(Color color)
        {
            using (Execute.OnUiContext)
            {
                GL.ClearColor(color.R, color.G, color.B, color.A);
                GL.Clear(OpenTK.Graphics.OpenGL.ClearBufferMask.ColorBufferBit |
                         OpenTK.Graphics.OpenGL.ClearBufferMask.DepthBufferBit);
            }

            CheckError();
        }

        public void Present()
        {
            CheckError();
            _context.SwapBuffers();
        }

        public GraphicsCapabilities Capabilities { get; }
        public Viewport Viewport
        {
            get { return _viewport; }
            set
            {
                if (_viewport.Bounds != value.Bounds)
                {
                    _viewport = value;
                    using (Execute.OnUiContext)
                    {
                        //GL.Viewport(viewport.X, game.Window.ClientSize.Height - viewport.Y - viewport.Height, viewport.Width, viewport.Height);
                        GL.Viewport(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height);
                        GL.Scissor(_scissorRectangle.X, Viewport.Height - _scissorRectangle.Bottom,
                            _scissorRectangle.Width, _scissorRectangle.Height);
                    }
                }
            }
        }

        Dictionary<VertexDeclaration, VertexBuffer> userBuffers = new Dictionary<VertexDeclaration, VertexBuffer>();

        [Obsolete("Do not use this function")]
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset,
            int primitiveCount) where T : struct
        {
            var tp = Activator.CreateInstance<T>() as IVertexType;
            if (tp == null)
                throw new ArgumentException("must be a vertexType");
            DrawUserPrimitives(primitiveType, vertexData, vertexOffset, primitiveCount, tp.VertexDeclaration);
            CheckError();
        }

        [Obsolete("Do not use this function")]
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset,
            int primitiveCount, VertexDeclaration vertexDeclaration) where T : struct
        {
            var old = VertexBuffer;
            using (Execute.OnUiContext)
            {
                VertexBuffer current;
                if (!userBuffers.TryGetValue(vertexDeclaration, out current))
                {
                    current = new VertexBuffer(this, vertexDeclaration, vertexData.Length);
                    userBuffers.Add(vertexDeclaration, current);
                }
                else if (current.VertexCount < vertexData.Length)
                {
                    if (!current.IsDisposed)
                        current.Dispose();
                    current = new VertexBuffer(this, vertexDeclaration, vertexData.Length);
                    userBuffers[vertexDeclaration] = current;
                }

                current.SetData(vertexData);

                VertexBuffer = current;

                DrawPrimitives(primitiveType, vertexOffset, primitiveCount);
            }

            VertexBuffer = old;
            CheckError();
        }

        [Obsolete("Do not use this function")]
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset,
            int numVertices, short[] indexData, int indexOffset, int primitiveCount)
        {
            //TODO:
            /*IVertexType tp = Activator.CreateInstance<T>() as IVertexType;
            if (tp == null)
                throw new ArgumentException("must be a vertexType");
            VertexBuffer old = VertexBuffer;
            using (Execute.OnUiContext)
            {
                VertexBuffer current = new VertexBuffer(this, tp.VertexDeclaration, vertexData.Length);

                VertexBuffer = current;

                VertexBuffer.Vao.Bind();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                GL.DrawElements(
                    (OpenTK.Graphics.OpenGL.PrimitiveType) primitiveType,
                    primitiveCount * 3,
                    OpenTK.Graphics.OpenGL.DrawElementsType.UnsignedShort,
                    indexData);

                VertexBuffer = old;

                current.Dispose();
            }
            CheckError();*/
        }

        public void DrawPrimitives(PrimitiveType primitiveType, int startVertex, int vertexCount)
        {
            VertexBuffer.EnsureVao();
            VertexBuffer.Vao.Bind();


            GL.DrawArrays((OpenTK.Graphics.OpenGL.PrimitiveType) primitiveType, startVertex, vertexCount);
            CheckError();
        }

        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex,
            int numVertices, int startIndex, int primitiveCount)
        {
            CheckError();
            VertexBuffer.EnsureVao();
            if (VertexBuffer.Bind())
            {
                IndexBuffer.Bind();

                GL.DrawElements((OpenTK.Graphics.OpenGL.PrimitiveType) primitiveType, primitiveCount * 3,
                    (OpenTK.Graphics.OpenGL.DrawElementsType) IndexBuffer.IndexElementSize, IntPtr.Zero);
            }

            CheckError();
        }

        public void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex,
            int primitiveCount, int instanceCount)
        {
            VertexBuffer.EnsureVao();
            VertexBuffer.Vao.Bind();
            GL.DrawArraysInstancedBaseInstance((OpenTK.Graphics.OpenGL.PrimitiveType) primitiveType, startIndex,
                primitiveCount * 3, instanceCount, 0);
        }

        public void SetRenderTarget(RenderTarget2D target)
        {
            using (Execute.OnUiContext)
            {
                if (target == null)
                {
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    Viewport = new Viewport(Game.Window.ClientRectangle);
                }
                else
                {
                    target.BindFbo();
                    Viewport = new Viewport(target.Bounds);
                    ScissorRectangle = target.Bounds;
                }
            }

            CheckError();
        }

        public TextureCollection Textures { get; private set; }

        public BlendState BlendState
        {
            get { return _blendState; }
            set
            {
                if (_blendState != value)
                {
                    _blendState?.Unbind();
                    var blendState = value ?? BlendState.AlphaBlend;
                    blendState.Bind(this);
                    _blendState = blendState;
                }
            }
        }

        public DepthStencilState DepthStencilState
        {
            get { return _depthStencilState; }
            set
            {
                if (_depthStencilState != value)
                {
                    _depthStencilState?.Unbind();
                    var depthStencilState = value ?? DepthStencilState.Default;
                    depthStencilState.Bind(this);
                    _depthStencilState = depthStencilState;
                }
            }
        }

        public RasterizerState RasterizerState
        {
            get { return _rasterizerState; }
            set
            {
                if (_rasterizerState != value)
                {
                    _rasterizerState?.Unbind();
                    var rasterizerState = value ?? RasterizerState.CullClockwise;
                    rasterizerState.Bind(this);
                    _rasterizerState = rasterizerState;
                }
            }
        }

        //public SamplerStateCollection SamplerStates
        //{
        //    get;
        //    internal set;
        //}

        public Rectangle ScissorRectangle
        {
            get { return _scissorRectangle; }
            set
            {
                if (_scissorRectangle != value)
                {
                    _scissorRectangle = value;
                    using (Execute.OnUiContext)
                    {
                        GL.Scissor(_scissorRectangle.X, Viewport.Height - _scissorRectangle.Bottom,
                            _scissorRectangle.Width, _scissorRectangle.Height);
                    }

                    //GL.Scissor(scissorRectangle.X, scissorRectangle.Y, scissorRectangle.Width, -scissorRectangle.Height);
                }
            }
        }

        public VertexBuffer VertexBuffer { get; set; }

        public IndexBuffer IndexBuffer { get; set; }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}