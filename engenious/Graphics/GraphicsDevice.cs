using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace engenious.Graphics
{
    public class GraphicsDevice:IDisposable
    {
        private BlendState blendState;
        private DepthStencilState depthStencilState;
        private RasterizerState rasterizerState;
        private Rectangle scissorRectangle;
        private Viewport viewport;
        private OpenTK.Graphics.IGraphicsContext context;


        /*DebugProc DebugCallbackInstance = DebugCallback;

        static void DebugCallback(DebugSource source, DebugType type, int id,
                                  DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string msg = Marshal.PtrToStringAnsi(message);
            Console.WriteLine("[GL] {0}; {1}; {2}; {3}; {4}",
                source, type, id, severity, msg);
        }*/

        internal Game game;
        internal Dictionary<string, bool> extensions = new Dictionary<string, bool>();
        internal int majorVersion,minorVersion;
        public GraphicsDevice(Game game, OpenTK.Graphics.IGraphicsContext context)
        {
            this.context = context;
            this.game = game;

            majorVersion = GL.GetInteger(GetPName.MajorVersion);
            minorVersion = GL.GetInteger(GetPName.MinorVersion);
            int count;
            GL.GetInteger(GetPName.NumExtensions,out count);
            for (int i = 0; i < count; i++)
            {
                string extension = GL.GetString(StringNameIndexed.Extensions, i);
                extensions.Add(extension, true);
            }
#if DEBUG
            if (extensions.ContainsKey("GL_ARB_debug_output"))
            {
                this.context.ErrorChecking = true;
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
            using (Execute.OnUiThread)
            {
                GL.Clear((OpenTK.Graphics.OpenGL4.ClearBufferMask) mask);
            }
        }
        public void Clear(ClearBufferMask mask, System.Drawing.Color color)
        {
            using (Execute.OnUiThread)
            {
                GL.Clear((OpenTK.Graphics.OpenGL4.ClearBufferMask) mask);
                GL.ClearColor(color);
            }
        }

        internal void CheckError()
        {
            //return;
            #if DEBUG
            return;//TODO:
            var frame = new System.Diagnostics.StackTrace(true).GetFrame(1);
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
                }, true);
            #endif
        }

        public void Clear(Color color)
        {
            using (Execute.OnUiThread)
            {
                GL.Clear(OpenTK.Graphics.OpenGL4.ClearBufferMask.ColorBufferBit | OpenTK.Graphics.OpenGL4.ClearBufferMask.DepthBufferBit);
                GL.ClearColor(color.R, color.G, color.B, color.A);
            }
            CheckError();
        }

        public void Present()
        {
            CheckError();
            context.SwapBuffers();
        }

        public Viewport Viewport
        {
            get
            {
                return viewport;
            }
            set
            {
                if (viewport.Bounds != value.Bounds)
                {
                    viewport = value;
                    using (Execute.OnUiThread)
                    {
                        //GL.Viewport(viewport.X, game.Window.ClientSize.Height - viewport.Y - viewport.Height, viewport.Width, viewport.Height);
                        GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
                        GL.Scissor(scissorRectangle.X, Viewport.Height - scissorRectangle.Bottom, scissorRectangle.Width, scissorRectangle.Height);
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
            using (Execute.OnUiThread)
            {
                VertexBuffer current;
                if (!userBuffers.TryGetValue(vertexDeclaration, out current))
                {
                    current = new VertexBuffer(this, vertexDeclaration, vertexData.Length);
                    userBuffers.Add(vertexDeclaration, current);
                }
                else if (current.VertexCount < vertexData.Length)
                {
                    if (current != null && !current.IsDisposed)
                        current.Dispose();
                    current = new VertexBuffer(this, vertexDeclaration, vertexData.Length);
                    userBuffers[vertexDeclaration] = current;
                }

                current.SetData<T>(vertexData);

                this.VertexBuffer = current;

                DrawPrimitives(primitiveType, vertexOffset, primitiveCount);
            }
            this.VertexBuffer = old;
            CheckError();
        }

        [Obsolete("Do not use this function")]
        public void DrawUserIndexedPrimitives<T>(PrimitiveType primitiveType, T[] vertexData, int vertexOffset, int numVertices, short[] indexData, int indexOffset, int primitiveCount)
        {
            return;
            IVertexType tp = Activator.CreateInstance<T>() as IVertexType;
            if (tp == null)
                throw new ArgumentException("must be a vertexType");
            VertexBuffer old = VertexBuffer;
            using (Execute.OnUiThread)
            {
                VertexBuffer current = new VertexBuffer(this, tp.VertexDeclaration, vertexData.Length);

                this.VertexBuffer = current;

                VertexBuffer.vao.Bind();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                GL.DrawElements(
                    (OpenTK.Graphics.OpenGL4.PrimitiveType) primitiveType,
                    primitiveCount * 3,
                    OpenTK.Graphics.OpenGL4.DrawElementsType.UnsignedShort,
                    indexData);

                this.VertexBuffer = old;

                current.Dispose();
            }
            CheckError();
        }

        public void DrawPrimitives(PrimitiveType primitiveType, int startVertex, int primitiveCount)
        {

            VertexBuffer.EnsureVAO();
            VertexBuffer.vao.Bind();


            GL.DrawArrays((OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType, startVertex, primitiveCount * 3);
            CheckError();
        }

        public void DrawIndexedPrimitives(PrimitiveType primitiveType, int baseVertex, int minVertexIndex, int numVertices, int startIndex, int primitiveCount)
        {
            CheckError();
            VertexBuffer.EnsureVAO();
            if (VertexBuffer.Bind())
            {
                IndexBuffer.Bind();

                GL.DrawElements((OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType, primitiveCount * 3, (OpenTK.Graphics.OpenGL4.DrawElementsType)IndexBuffer.IndexElementSize, IntPtr.Zero);
            }
            CheckError();
        }
        public void DrawInstancedPrimitives(PrimitiveType primitiveType, int baseVertex, int startIndex, int primitiveCount, int instanceCount)
        {
            VertexBuffer.EnsureVAO();
            VertexBuffer.vao.Bind();
            GL.DrawArraysInstancedBaseInstance((OpenTK.Graphics.OpenGL4.PrimitiveType)primitiveType,startIndex,primitiveCount * 3,instanceCount,0);
        }

        public void SetRenderTarget(RenderTarget2D target)
        {
            using (Execute.OnUiThread)
            {
                if (target == null)
                {
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                    Viewport = new Viewport(game.Window.ClientRectangle);
                }
                else
                {
                    target.BindFBO();
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
                return blendState;
            }
            set
            {
                if (blendState != value)
                {
                    
                    blendState = value == null ? BlendState.AlphaBlend : value;
                    using (Execute.OnUiThread)
                    {
                        //TODO:apply more?
                        GL.BlendFuncSeparate(
                            (OpenTK.Graphics.OpenGL4.BlendingFactorSrc) blendState.ColorSourceBlend,
                            (OpenTK.Graphics.OpenGL4.BlendingFactorDest) blendState.ColorDestinationBlend,
                            (OpenTK.Graphics.OpenGL4.BlendingFactorSrc) blendState.AlphaSourceBlend,
                            (OpenTK.Graphics.OpenGL4.BlendingFactorDest) blendState.AlphaDestinationBlend);
                        GL.BlendEquationSeparate(
                            (OpenTK.Graphics.OpenGL4.BlendEquationMode) blendState.ColorBlendFunction,
                            (OpenTK.Graphics.OpenGL4.BlendEquationMode) blendState.AlphaBlendFunction);
                    }
                }
            }
        }

        public DepthStencilState DepthStencilState
        {
            get
            {
                return depthStencilState;
            }
            set
            {
                if (depthStencilState != value)
                {
                    depthStencilState = value == null ? DepthStencilState.Default : value;
                    using (Execute.OnUiThread)
                    {
                        if (depthStencilState.DepthBufferEnable)
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
                return rasterizerState;
            }
            set
            {
                if (rasterizerState != value)
                {
                    rasterizerState = value == null ? RasterizerState.CullClockwise : value;
                    //TODO:apply more
                    using (Execute.OnUiThread)
                    {
                        //GL.FrontFace(FrontFaceDirection.
                        if (rasterizerState.CullMode == CullMode.None)
                            GL.Disable(EnableCap.CullFace);
                        else
                        {
                            GL.Enable(EnableCap.CullFace);
                            GL.FrontFace((FrontFaceDirection) rasterizerState.CullMode);
                        }


                        GL.PolygonMode(MaterialFace.Back, (OpenTK.Graphics.OpenGL4.PolygonMode) rasterizerState.FillMode);

                        if (rasterizerState.MultiSampleAntiAlias)
                            GL.Enable(EnableCap.Multisample);
                        else
                            GL.Disable(EnableCap.Multisample);

                        if (rasterizerState.ScissorTestEnable)
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
                return scissorRectangle;
            }
            set
            {

                if (scissorRectangle != value)
                {
                    scissorRectangle = value;
                    using (Execute.OnUiThread)
                    {
                        GL.Scissor(scissorRectangle.X, Viewport.Height - scissorRectangle.Bottom, scissorRectangle.Width, scissorRectangle.Height);
                    }
                    //GL.Scissor(scissorRectangle.X, scissorRectangle.Y, scissorRectangle.Width, -scissorRectangle.Height);
                }
               

            }
        }

        public VertexBuffer VertexBuffer{ get; set; }

        public IndexBuffer IndexBuffer{ get; set; }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}

