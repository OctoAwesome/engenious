using System;
using System.ComponentModel;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class RasterizerState : GraphicsResource
    {
        public static readonly RasterizerState CullNone;
        public static readonly RasterizerState CullClockwise;
        public static readonly RasterizerState CullCounterClockwise;
        private CullMode _cullMode = CullMode.CounterClockwise;
        private PolygonMode _fillMode = PolygonMode.Fill;
        private bool _scissorTestEnable = false;
        private bool _multiSampleAntiAlias = false;
        private float _slopeScaleDepthBias = 0.0f;
        private float _depthBias = 0.0f;

        static RasterizerState()
        {
            CullNone = new RasterizerState();
            CullNone.CullMode = CullMode.None;
            CullNone.FillMode = PolygonMode.Fill;
            CullNone.InitPredefined();

            CullClockwise = new RasterizerState();
            CullClockwise.CullMode = CullMode.Clockwise;
            CullClockwise.FillMode = PolygonMode.Fill;
            CullClockwise.InitPredefined();

            CullCounterClockwise = new RasterizerState();
            CullCounterClockwise.CullMode = CullMode.CounterClockwise;
            CullCounterClockwise.FillMode = PolygonMode.Fill;
            CullCounterClockwise.InitPredefined();

        }

        public RasterizerState()
        {
            CullMode = CullMode.None;
            FillMode = PolygonMode.Fill;
        }
        private bool _isPredefined;
        internal void InitPredefined()
        {
            _isPredefined = true;
        }

        private void ThrowIfReadOnly()
        {
            if (_isPredefined)
                throw new InvalidOperationException("you are not allowed to change a predefined rasterizer state");
        }

        public CullMode CullMode
        {
            get => _cullMode;
            set
            {
                ThrowIfReadOnly();
                if (_cullMode == value)
                    return;
                _cullMode = value;
                if (GraphicsDevice != null)
                    ApplyCullAndFill();
            }
        }

        public PolygonMode FillMode
        {
            get => _fillMode;
            set
            {
                ThrowIfReadOnly();
                if (_fillMode == value)
                    return;
                _fillMode = value; 
                if (GraphicsDevice != null)
                    ApplyCullAndFill();
            }
        }

        private void ApplyCullAndFill()
        {
            using (Execute.OnUiContext)
            {
                ApplyCullAndFillGl();
            }
        }

        private void ApplyCullAndFillGl()
        {
            if (CullMode == CullMode.None)
            {
                GL.Disable(EnableCap.CullFace);
                GL.PolygonMode(MaterialFace.FrontAndBack, (OpenTK.Graphics.OpenGL.PolygonMode) FillMode);
            }
            else
            {
                GL.Enable(EnableCap.CullFace);
                GL.FrontFace((FrontFaceDirection) CullMode);
                GL.PolygonMode(CullMode == CullMode.Clockwise ? MaterialFace.Back : MaterialFace.Front,
                    (OpenTK.Graphics.OpenGL.PolygonMode) FillMode);
            }
        }
        public bool MultiSampleAntiAlias
        {
            get => _multiSampleAntiAlias;
            set
            {
                ThrowIfReadOnly();
                if (_multiSampleAntiAlias == value)
                    return;
                _multiSampleAntiAlias = value;
                if (GraphicsDevice != null)
                    ApplyMultiSampleAntiAlias();
            }
        }

        private void ApplyMultiSampleAntiAlias()
        {
            using (Execute.OnUiContext)
            {
                if (MultiSampleAntiAlias)
                    GL.Enable(EnableCap.Multisample);
                else
                    GL.Disable(EnableCap.Multisample);
            }
        }

        public bool ScissorTestEnable
        {
            get => _scissorTestEnable;
            set
            {
                ThrowIfReadOnly();
                if (_scissorTestEnable == value)
                    return;
                _scissorTestEnable = value;
                if (GraphicsDevice != null)
                    ApplyScissorTest();
            }
        }

        private void ApplyScissorTest()
        {
            using (Execute.OnUiContext)
            {
                if (ScissorTestEnable)
                    GL.Enable(EnableCap.ScissorTest);
                else
                    GL.Disable(EnableCap.ScissorTest);
            }
        }

        public float SlopeScaleDepthBias
        {
            get => _slopeScaleDepthBias;
            set
            {
                ThrowIfReadOnly();
                if (_slopeScaleDepthBias == value)
                    return;
                _slopeScaleDepthBias = value;
                if (GraphicsDevice != null)
                    ApplyPolygonOffset();
            }
        }

        public float DepthBias
        {
            get => _depthBias;
            set
            {
                ThrowIfReadOnly();
                if (_depthBias == value)
                    return;
                _depthBias = value; 
                if (GraphicsDevice != null)
                    ApplyPolygonOffset();
            }
        }

        private void ApplyPolygonOffset()
        {
            using(Execute.OnUiContext)
                GL.PolygonOffset(_slopeScaleDepthBias, _depthBias);
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
                var oldState = GraphicsDevice.RasterizerState;
                if (oldState == null || oldState.CullMode != CullMode || oldState.FillMode != FillMode)
                    ApplyCullAndFillGl();

                if (oldState == null || oldState.MultiSampleAntiAlias != MultiSampleAntiAlias)
                {
                    if (MultiSampleAntiAlias)
                        GL.Enable(EnableCap.Multisample);
                    else
                        GL.Disable(EnableCap.Multisample);
                }

                if (oldState == null || oldState.ScissorTestEnable != ScissorTestEnable)
                {
                    if (ScissorTestEnable)
                        GL.Enable(EnableCap.ScissorTest);
                    else
                        GL.Disable(EnableCap.ScissorTest);
                }
                if (oldState == null || oldState.SlopeScaleDepthBias != SlopeScaleDepthBias || oldState.DepthBias != DepthBias)
                    GL.PolygonOffset(_slopeScaleDepthBias, _depthBias);
            }
        }

        internal void Unbind()
        {
            GraphicsDevice = null;
        }
    }
}

