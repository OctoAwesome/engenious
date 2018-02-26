using System;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class DepthStencilState : GraphicsResource
    {
        public static readonly DepthStencilState Default;
        public static readonly DepthStencilState DepthRead;
        public static readonly DepthStencilState None;
        private bool _depthBufferEnable;
        private bool _depthBufferWriteEnable;
        private DepthFunction _depthBufferFunction;
        private bool _stencilEnable;
        private StencilFunction _stencilFunction;
        private bool _twoSidedStencilMode;
        private StencilFunction _counterClockwiseStencilFunction;
        private int _stencilMask;
        private StencilOp _stencilDepthBufferFail;
        private StencilOp _stencilFail;
        private StencilOp _stencilPass;
        private int _referenceStencil;
        private StencilOp _counterClockwiseStencilFail;
        private StencilOp _counterClockwiseStencilDepthBufferFail;
        private StencilOp _counterClockwiseStencilPass;

        static DepthStencilState()
        {
            Default = new DepthStencilState();
            Default.DepthBufferEnable = true;
            Default.DepthBufferWriteEnable = true;
            Default.InitPredefined();

            DepthRead = new DepthStencilState();
            DepthRead.DepthBufferEnable = true;
            DepthRead.DepthBufferWriteEnable = false;
            DepthRead.InitPredefined();

            None = new DepthStencilState();
            None.DepthBufferEnable = false;
            None.DepthBufferWriteEnable = false;
            None.InitPredefined();
        }

        public DepthStencilState()
        {
            DepthBufferEnable = true;
            DepthBufferWriteEnable = true;
        }
        private bool _isPredefined;
        internal void InitPredefined()
        {
            _isPredefined = true;
        }

        private void ThrowIfReadOnly()
        {
            if (_isPredefined)
                throw new InvalidOperationException("you are not allowed to change a predefined depth stencil state");
        }

        public bool DepthBufferEnable
        {
            get => _depthBufferEnable;
            set
            {
                ThrowIfReadOnly();
                if (_depthBufferEnable == value)
                    return;
                _depthBufferEnable = value;
                if (GraphicsDevice != null)
                    ApplyDepthBufferEnable();
            }
        }

        private void ApplyDepthBufferEnable()
        {
            using (Execute.OnUiContext)
            {
                if (DepthBufferEnable)
                    GL.Enable(EnableCap.DepthTest);
                else
                    GL.Disable(EnableCap.DepthTest);
            }
        }


        public bool DepthBufferWriteEnable
        {
            get => _depthBufferWriteEnable;
            set
            {
                ThrowIfReadOnly();
                if (_depthBufferWriteEnable == value)
                    return;
                _depthBufferWriteEnable = value;
                if (GraphicsDevice != null)
                    ApplyDepthBufferWriteEnable();
            }
        }

        private void ApplyDepthBufferWriteEnable()
        {
            using (Execute.OnUiContext)
                GL.DepthMask(_depthBufferWriteEnable);
        }


        public DepthFunction DepthBufferFunction
        {
            get => _depthBufferFunction;
            set
            {
                ThrowIfReadOnly();
                if (_depthBufferFunction == value)
                    return;
                _depthBufferFunction = value;
                if (GraphicsDevice != null)
                    ApplyDepthBufferFunction();
            }
        }

        private void ApplyDepthBufferFunction()
        {
            using (Execute.OnUiContext)
                GL.DepthFunc((OpenTK.Graphics.OpenGL.DepthFunction) _depthBufferFunction);
        }

        public bool StencilEnable
        {
            get => _stencilEnable;
            set
            {
                ThrowIfReadOnly();
                if (_stencilEnable == value)
                    return;
                _stencilEnable = value;
                if (GraphicsDevice != null)
                    ApplyStencilEnable();
            }
        }

        private void ApplyStencilEnable()
        {
            using (Execute.OnUiContext)
            {
                if (_stencilEnable)
                    GL.Enable(EnableCap.StencilTest);
                else
                    GL.Enable(EnableCap.StencilTest);
            }
        }

        public StencilFunction StencilFunction
        {
            get => _stencilFunction;
            set
            {
                ThrowIfReadOnly();
                if (_stencilFunction == value)
                    return;
                _stencilFunction = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }

        public bool TwoSidedStencilMode
        {
            get => _twoSidedStencilMode;
            set
            {
                ThrowIfReadOnly();
                if (_twoSidedStencilMode == value)
                    return;
                _twoSidedStencilMode = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }

        private void ApplyTwoSidedStencilMode()
        {
            using (Execute.OnUiContext)
            {
                ApplyTwoSidedStencilModeGl();
            }
        }

        private void ApplyTwoSidedStencilModeGl()
        {
            if (TwoSidedStencilMode)
            {
                GL.StencilFuncSeparate(StencilFace.Front, (OpenTK.Graphics.OpenGL.StencilFunction) StencilFunction,
                    ReferenceStencil, StencilMask);

                GL.StencilFuncSeparate(StencilFace.Back,
                    (OpenTK.Graphics.OpenGL.StencilFunction) CounterClockwiseStencilFunction, ReferenceStencil,
                    StencilMask);

                GL.StencilOpSeparate(StencilFace.Front, (OpenTK.Graphics.OpenGL.StencilOp) StencilFail,
                    (OpenTK.Graphics.OpenGL.StencilOp) StencilDepthBufferFail,
                    (OpenTK.Graphics.OpenGL.StencilOp) StencilPass);

                GL.StencilOpSeparate(StencilFace.Back,
                    (OpenTK.Graphics.OpenGL.StencilOp) CounterClockwiseStencilFail,
                    (OpenTK.Graphics.OpenGL.StencilOp) CounterClockwiseStencilDepthBufferFail,
                    (OpenTK.Graphics.OpenGL.StencilOp) CounterClockwiseStencilPass);
            }
            else
            {
                GL.StencilFunc((OpenTK.Graphics.OpenGL.StencilFunction) StencilFunction, ReferenceStencil,
                    StencilMask);

                GL.StencilOp((OpenTK.Graphics.OpenGL.StencilOp) StencilFail,
                    (OpenTK.Graphics.OpenGL.StencilOp) StencilDepthBufferFail,
                    (OpenTK.Graphics.OpenGL.StencilOp) StencilPass);
            }
        }

        private bool TwoSidedStencilChanged(DepthStencilState oldState)
        {
            if (oldState == null || oldState.TwoSidedStencilMode != TwoSidedStencilMode)
                return true;
            return oldState.StencilFunction != StencilFunction || oldState.ReferenceStencil != ReferenceStencil ||
                   oldState.StencilMask != StencilMask ||
                   oldState.StencilFail != StencilFail ||
                   oldState.StencilDepthBufferFail != StencilDepthBufferFail ||
                   oldState.StencilPass != StencilPass || (TwoSidedStencilMode
                                                           && (oldState.CounterClockwiseStencilFunction !=
                                                               CounterClockwiseStencilFunction ||
                                                               oldState.CounterClockwiseStencilDepthBufferFail !=
                                                               CounterClockwiseStencilDepthBufferFail ||
                                                               oldState.CounterClockwiseStencilFail !=
                                                               CounterClockwiseStencilFail ||
                                                               oldState.CounterClockwiseStencilPass !=
                                                               CounterClockwiseStencilPass));
        }


        public int ReferenceStencil
        {
            get => _referenceStencil;
            set
            {
                ThrowIfReadOnly();
                if (_referenceStencil == value)
                    return;
                _referenceStencil = value;

                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }


        public StencilOp CounterClockwiseStencilPass
        {
            get => _counterClockwiseStencilPass;
            set
            {
                ThrowIfReadOnly();
                if (_counterClockwiseStencilPass == value)
                    return;
                _counterClockwiseStencilPass = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }


        public StencilOp CounterClockwiseStencilDepthBufferFail
        {
            get => _counterClockwiseStencilDepthBufferFail;
            set
            {
                ThrowIfReadOnly();
                if (_counterClockwiseStencilDepthBufferFail == value)
                    return;
                _counterClockwiseStencilDepthBufferFail = value;

                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }


        public StencilOp CounterClockwiseStencilFail
        {
            get => _counterClockwiseStencilFail;
            set
            {
                ThrowIfReadOnly();
                if (_counterClockwiseStencilFail == value)
                    return;
                _counterClockwiseStencilFail = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }

        public StencilFunction CounterClockwiseStencilFunction
        {
            get => _counterClockwiseStencilFunction;
            set
            {
                ThrowIfReadOnly();
                if (_counterClockwiseStencilFunction == value)
                    return;
                _counterClockwiseStencilFunction = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }


        public int StencilMask
        {
            get => _stencilMask;
            set
            {
                ThrowIfReadOnly();
                if (_stencilMask == value)
                    return;
                _stencilMask = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }


        public StencilOp StencilDepthBufferFail
        {
            get => _stencilDepthBufferFail;
            set
            {
                ThrowIfReadOnly();
                if (_stencilDepthBufferFail == value)
                    return;
                _stencilDepthBufferFail = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }


        public StencilOp StencilFail
        {
            get => _stencilFail;
            set
            {
                ThrowIfReadOnly();
                if (_stencilFail == value)
                    return;
                _stencilFail = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }


        public StencilOp StencilPass
        {
            get { return _stencilPass; }
            set
            {
                ThrowIfReadOnly();
                if (_stencilPass == value)
                    return;
                _stencilPass = value;
                if (GraphicsDevice != null)
                    ApplyTwoSidedStencilMode();
            }
        }

        internal void Bind(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            Apply();
        }

        private void Apply()
        {
            using (Execute.OnUiContext)
            {
                var oldState = GraphicsDevice.DepthStencilState;

                if (oldState == null || oldState.DepthBufferEnable != DepthBufferEnable)
                {
                    if (DepthBufferEnable)
                        GL.Enable(EnableCap.DepthTest);
                    else
                        GL.Disable(EnableCap.DepthTest);
                }
                if (oldState == null || oldState.DepthBufferFunction != DepthBufferFunction)
                    GL.DepthFunc((OpenTK.Graphics.OpenGL.DepthFunction) _depthBufferFunction);
                
                if (oldState == null || oldState.DepthBufferWriteEnable != DepthBufferWriteEnable)
                    GL.DepthMask(_depthBufferWriteEnable);

                if (oldState == null || oldState.StencilEnable != StencilEnable)
                {
                    if (_stencilEnable)
                        GL.Enable(EnableCap.StencilTest);
                    else
                        GL.Enable(EnableCap.StencilTest);
                }

                if (TwoSidedStencilChanged(oldState))
                    ApplyTwoSidedStencilModeGl();
            }
        }

        internal void Unbind()
        {
            GraphicsDevice = null;
        }
    }
}