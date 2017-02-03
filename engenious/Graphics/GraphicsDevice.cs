using System;
using System.Collections.Generic;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace engenious.Graphics
{
    public class GraphicsDevice:IDisposable
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
        internal Dictionary<string, bool> Extensions = new Dictionary<string, bool>();
        internal int MajorVersion,MinorVersion;
        public GraphicsDevice(Game game, IGraphicsContext context)
        {
            _context = context;
            Game = game;

            MajorVersion = GL.GetInteger(GetPName.MajorVersion);
            MinorVersion = GL.GetInteger(GetPName.MinorVersion);
            int count;
            GL.GetInteger(GetPName.NumExtensions,out count);
            for (int i = 0; i < count; i++)
            {
                string extension = GL.GetString(StringNameIndexed.Extensions, i);
                Extensions.Add(extension, true);
            }
#if DEBUG
            if (Extensions.ContainsKey("GL_ARB_debug_output"))
            {
                _context.ErrorChecking = true;
                //GL.Enable(EnableCap.DebugOutput);
                //GL.Enable(EnableCap.DebugOutputSynchronous);
                //GL.DebugMessageCallback(DebugCallbackInstance, IntPtr.Zero);
            }
#endif



            Textures = new TextureCollection();
            CheckError();
            //TODO: samplerstate
        }
        public void Clear(ClearBufferMask mask)
        {
            using (Execute.OnUiContext)
            {
                GL.Clear((OpenTK.Graphics.OpenGL4.ClearBufferMask) mask);
            }
        }
        public void Clear(ClearBufferMask mask, System.Drawing.Color color)
        {
            using (Execute.OnUiContext)
            {
                GL.Clear((OpenTK.Graphics.OpenGL4.ClearBufferMask) mask);
                GL.ClearColor(color);
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
                GL.Clear(OpenTK.Graphics.OpenGL4.ClearBufferMask.ColorBufferBit | OpenTK.Graphics.OpenGL4.ClearBufferMask.DepthBufferBit);
                GL.ClearColor(color.R, color.G, color.B, color.A);
            }
            CheckError();
        }

        public void Present()
        {
            CheckError();
            _context.SwapBuffers();
        }

        public Viewport Viewport
        {
            get
            {
                return _viewport;
            }
            set
            {
                if (_viewport.Bounds != value.Bounds)
                {
                    _viewport = value;
                    using (Execute.OnUiContext)
                    {
                        //GL.Viewport(viewport.X, game.Window.ClientSize.Height - viewport.Y - viewport.Height, viewport.Width, viewport.Height);
                        GL.Viewport(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height);
                        GL.Scissor(_scissorRectangle.X, Viewport.Height - _scissorRectangle.Bottom, _scissorRectangle.Width, _scissorRectangle.Height);
                    }
                }
            }
        }

        Dictionary<VertexDeclaration,VertexBuffer> userBuffers = new Dictionary<VertexDeclaration, VertexBuffer>();

        [Obsolete("Do not use this function")]
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount)where T : struct
        {
            IVertexType tp = Activator.CreateInstance<T>() as IVertexType;
            if (tp == null)
                throw new ArgumentException("must be a vertexType");
            DrawUserPrimitives(primitiveType, vertexData, vertexOffset, primitiveCount, tp.VertexDeclaration);
            CheckError();
        }

        [Obsolete("Do not use this function")]
        public void DrawUserPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int primitiveCount, VertexDeclaration vertexDeclaration)  where T : struct
        {
            VertexBuffer old = VertexBuffer;
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
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount)
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
                    (OpenTK.Graphics.OpenGL4.PrimitiveType) primitiveType,
                    primitiveCount * 3,
                    OpenTK.Graphics.OpenGL4.DrawElementsType.UnsignedShort,
                    indexData);

                VertexBuffer = old;

                current.Dispose();
            }
            CheckError();*/
        }

        public void DrawPrimitives(PrimitiveType primitiveType, int startVertex, int primitiveCount)
        {

            VertexBuffer.EnsureVao();
            VertexBuffer.Vao.Bind();


            GL.DrawArrays((OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType, startVertex, primitiveCount * 3);
            CheckError();
        }

        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
        {
            CheckError();
            VertexBuffer.EnsureVao();
            if (VertexBuffer.Bind())
            {
                IndexBuffer.Bind();

                GL.DrawElements((OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType, primitiveCount * 3, (OpenTK.Graphics.OpenGL4.DrawElementsType)IndexBuffer.IndexElementSize, IntPtr.Zero);
            }
            CheckError();
        }
        public void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int instanceCount)
        {
            VertexBuffer.EnsureVao();
            VertexBuffer.Vao.Bind();
            GL.DrawArraysInstancedBaseInstance((OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType,startIndex,primitiveCount * 3,instanceCount,0);
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

        public TextureCollection Textures{ get; private set; }

        public BlendState BlendState
        {
            get
            {
                return _blendState;
            }
            set
            {
                if (_blendState != value)
                {
                    
                    _blendState = value == null ? BlendState.AlphaBlend : value;
                    using (Execute.OnUiContext)
                    {
                        //TODO:apply more?
                        GL.BlendFuncSeparate(
                            (OpenTK.Graphics.OpenGL4.BlendingFactorSrc) _blendState.ColorSourceBlend,
                            (OpenTK.Graphics.OpenGL4.BlendingFactorDest) _blendState.ColorDestinationBlend,
                            (OpenTK.Graphics.OpenGL4.BlendingFactorSrc) _blendState.AlphaSourceBlend,
                            (OpenTK.Graphics.OpenGL4.BlendingFactorDest) _blendState.AlphaDestinationBlend);
                        GL.BlendEquationSeparate(
                            (OpenTK.Graphics.OpenGL4.BlendEquationMode) _blendState.ColorBlendFunction,
                            (OpenTK.Graphics.OpenGL4.BlendEquationMode) _blendState.AlphaBlendFunction);
                    }
                }
            }
        }

        public DepthStencilState DepthStencilState
        {
            get
            {
                return _depthStencilState;
            }
            set
            {
                if (_depthStencilState != value)
                {
                    _depthStencilState = value == null ? DepthStencilState.Default : value;
                    using (Execute.OnUiContext)
                    {
                        if (_depthStencilState.DepthBufferEnable)
                            GL.Enable(EnableCap.DepthTest);
                        else
                            GL.Disable(EnableCap.DepthTest);
                    }
                    //TODO:apply more
                }
            }
        }

        public RasterizerState RasterizerState
        {
            get
            {
                return _rasterizerState;
            }
            set
            {
                if (_rasterizerState != value)
                {
                    _rasterizerState = value == null ? RasterizerState.CullClockwise : value;
                    //TODO:apply more
                    using (Execute.OnUiContext)
                    {
                        //GL.FrontFace(FrontFaceDirection.
                        if (_rasterizerState.CullMode == CullMode.None)
                            GL.Disable(EnableCap.CullFace);
                        else
                        {
                            GL.Enable(EnableCap.CullFace);
                            GL.FrontFace((FrontFaceDirection) _rasterizerState.CullMode);
                        }


                        GL.PolygonMode(MaterialFace.Back, (OpenTK.Graphics.OpenGL4.PolygonMode) _rasterizerState.FillMode);

                        if (_rasterizerState.MultiSampleAntiAlias)
                            GL.Enable(EnableCap.Multisample);
                        else
                            GL.Disable(EnableCap.Multisample);

                        if (_rasterizerState.ScissorTestEnable)
                            GL.Enable(EnableCap.ScissorTest);
                        else
                            GL.Disable(EnableCap.ScissorTest);
                    }
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
            get
            {
                return _scissorRectangle;
            }
            set
            {

                if (_scissorRectangle != value)
                {
                    _scissorRectangle = value;
                    using (Execute.OnUiContext)
                    {
                        GL.Scissor(_scissorRectangle.X, Viewport.Height - _scissorRectangle.Bottom, _scissorRectangle.Width, _scissorRectangle.Height);
                    }
                    //GL.Scissor(scissorRectangle.X, scissorRectangle.Y, scissorRectangle.Width, -scissorRectangle.Height);
                }
               

            }
        }

        public VertexBuffer VertexBuffer{ get; set; }

        public IndexBuffer IndexBuffer{ get; set; }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

