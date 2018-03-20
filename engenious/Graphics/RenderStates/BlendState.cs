using System;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{
    public class BlendState : GraphicsResource
    {
        public static readonly BlendState AlphaBlend;
        public static readonly BlendState Additive;
        public static readonly BlendState NonPremultiplied;
        public static readonly BlendState Opaque;
        private BlendingFactorSrc _colorSourceBlend = BlendingFactorSrc.One;
        private BlendingFactorSrc _alphaSourceBlend = BlendingFactorSrc.One;
        private BlendingFactorDest _colorDestinationBlend = BlendingFactorDest.One;
        private BlendingFactorDest _alphaDestinationBlend = BlendingFactorDest.One;
        private BlendEquationMode _alphaBlendFunction = BlendEquationMode.FuncAdd;
        private BlendEquationMode _colorBlendFunction = BlendEquationMode.FuncAdd;
        private ColorWriteChannels _colorWriteChannels = ColorWriteChannels.None;
        private ColorWriteChannels _colorWriteChannels1 = ColorWriteChannels.None;
        private ColorWriteChannels _colorWriteChannels2 = ColorWriteChannels.None;
        private ColorWriteChannels _colorWriteChannels3 = ColorWriteChannels.None;
        private readonly ColorWriteChannels[] _colorWriteChannelList;

        static BlendState()
        {
            AlphaBlend = new BlendState();
            AlphaBlend.InitPredefined();

            Additive = new BlendState();
            Additive.ColorSourceBlend = BlendingFactorSrc.One;
            Additive.AlphaSourceBlend = BlendingFactorSrc.One;
            Additive.ColorDestinationBlend = BlendingFactorDest.OneMinusSrcColor;//TODO: verify?	
            Additive.AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcColor;
            Additive.InitPredefined();

            NonPremultiplied = new BlendState();
            NonPremultiplied.ColorSourceBlend = BlendingFactorSrc.SrcAlpha;
            NonPremultiplied.AlphaSourceBlend = BlendingFactorSrc.SrcAlpha;
            NonPremultiplied.ColorDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;
            NonPremultiplied.AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;
            NonPremultiplied.InitPredefined();

            Opaque = new BlendState();
            Opaque.ColorSourceBlend = BlendingFactorSrc.One;
            Opaque.AlphaSourceBlend = BlendingFactorSrc.One;
            Opaque.ColorDestinationBlend = BlendingFactorDest.Zero;
            Opaque.AlphaDestinationBlend = BlendingFactorDest.Zero;

            Opaque.InitPredefined();
            //GL.BlendEquationSeparate(
        }

        public BlendState()
        {
            //GL.BlendFunc(BlendingFactorSrc.One,#
            ColorSourceBlend = BlendingFactorSrc.SrcAlpha;
            AlphaSourceBlend = BlendingFactorSrc.SrcAlpha;
            ColorDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;
            AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;

            ColorBlendFunction = BlendEquationMode.FuncAdd;
            AlphaBlendFunction = BlendEquationMode.FuncAdd;

            ColorWriteChannels =
                ColorWriteChannels1 = ColorWriteChannels2 = ColorWriteChannels3 = ColorWriteChannels.All;
            _colorWriteChannelList = new[]
                {_colorWriteChannels, _colorWriteChannels1, _colorWriteChannels2, _colorWriteChannels3};
        }
        private bool _isPredefined;
        internal void InitPredefined()
        {
            _isPredefined = true;
        }

        private void ThrowIfReadOnly()
        {
            if (_isPredefined)
                throw new InvalidOperationException("you are not allowed to change a predefined blend state");
        }

        public BlendingFactorSrc ColorSourceBlend
        {
            get => _colorSourceBlend;
            set
            {
                ThrowIfReadOnly();
                if (_colorSourceBlend == value)
                    return;
                _colorSourceBlend = value;
                if (GraphicsDevice != null)
                    ApplyBlends();
            }
        }

        public BlendingFactorSrc AlphaSourceBlend
        {
            get => _alphaSourceBlend;
            set
            {
                ThrowIfReadOnly();
                if (_alphaSourceBlend == value)
                    return;
                _alphaSourceBlend = value;
                if (GraphicsDevice != null)
                    ApplyBlends();
            }
        }

        public BlendingFactorDest ColorDestinationBlend
        {
            get => _colorDestinationBlend;
            set
            {
                ThrowIfReadOnly();
                if (_colorDestinationBlend == value)
                    return;
                _colorDestinationBlend = value; 
                if (GraphicsDevice != null)
                    ApplyBlends();
            }
        }

        public BlendingFactorDest AlphaDestinationBlend
        {
            get => _alphaDestinationBlend;
            set
            {
                ThrowIfReadOnly();
                if (_alphaDestinationBlend == value)
                    return;
                _alphaDestinationBlend = value;
                if (GraphicsDevice != null)
                    ApplyBlends();
            }
        }

        public BlendEquationMode AlphaBlendFunction
        {
            get => _alphaBlendFunction;
            set
            {
                ThrowIfReadOnly();
                if (_alphaBlendFunction == value)
                    return;
                _alphaBlendFunction = value; 
                if(GraphicsDevice != null)
                    ApplyBlendFuncs();    
            }
        }

        public BlendEquationMode ColorBlendFunction
        {
            get => _colorBlendFunction;
            set
            {
                ThrowIfReadOnly();
                if (_colorBlendFunction == value)
                    return;
                _colorBlendFunction = value; 
                if(GraphicsDevice != null)
                    ApplyBlendFuncs();   
            }
        }



        public ColorWriteChannels ColorWriteChannels
        {
            get => _colorWriteChannels;
            set
            {
                ThrowIfReadOnly();
                if (_colorWriteChannels == value)
                    return;
                _colorWriteChannels = value; 
                if (GraphicsDevice != null)
                    ApplyColorWriteChannels();
            }
        }

        private void ApplyColorWriteChannels()
        {
            using (Execute.OnUiContext)
            {
                GL.ColorMask(
                    (ColorWriteChannels & ColorWriteChannels.Red) != 0,
                    (ColorWriteChannels & ColorWriteChannels.Green) != 0,
                    (ColorWriteChannels & ColorWriteChannels.Blue) != 0,
                    (ColorWriteChannels & ColorWriteChannels.Alpha) != 0);
            }
        }
        private void ApplyColorWriteChannels(uint index)
        {
            using (Execute.OnUiContext)
            {
                var colorWriteChannels = _colorWriteChannelList[index];
                GL.ColorMask(index,
                    (colorWriteChannels & ColorWriteChannels.Red) != 0,
                    (colorWriteChannels & ColorWriteChannels.Green) != 0,
                    (colorWriteChannels & ColorWriteChannels.Blue) != 0,
                    (colorWriteChannels & ColorWriteChannels.Alpha) != 0);
            }
        }

        public ColorWriteChannels ColorWriteChannels1
        {
            get => _colorWriteChannels1;
            set 
            {
                ThrowIfReadOnly();
                if (_colorWriteChannels1 == value)
                    return;
                _colorWriteChannels1 = value;
                if (GraphicsDevice != null)
                    ApplyColorWriteChannels(1);
                
            }
        }

        public ColorWriteChannels ColorWriteChannels2
        {
            get => _colorWriteChannels2;
            set
            {
                ThrowIfReadOnly();
                if (_colorWriteChannels2 == value)
                    return;
                _colorWriteChannels2 = value;
                if (GraphicsDevice != null)
                    ApplyColorWriteChannels(2);
            }
        }

        public ColorWriteChannels ColorWriteChannels3
        {
            get => _colorWriteChannels3;
            set
            {
                ThrowIfReadOnly();
                if (_colorWriteChannels3 == value)
                    return;
                _colorWriteChannels3 = value; 
                if (GraphicsDevice != null)
                    ApplyColorWriteChannels(3);
            }
        }
        public Color BlendFactor
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public int MultiSampleMask
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        private void ApplyBlends()
        {
            using (Execute.OnUiContext)
            {
                GL.BlendFuncSeparate(
                    (OpenTK.Graphics.OpenGL.BlendingFactorSrc) ColorSourceBlend,
                    (OpenTK.Graphics.OpenGL.BlendingFactorDest) ColorDestinationBlend,
                    (OpenTK.Graphics.OpenGL.BlendingFactorSrc) AlphaSourceBlend,
                    (OpenTK.Graphics.OpenGL.BlendingFactorDest) AlphaDestinationBlend);
            }
        }

        private void ApplyBlendFuncs()
        {
            using (Execute.OnUiContext)
            {
                GL.BlendEquationSeparate(
                    (OpenTK.Graphics.OpenGL.BlendEquationMode) ColorBlendFunction,
                    (OpenTK.Graphics.OpenGL.BlendEquationMode) AlphaBlendFunction);
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
                //TODO:(BlendFactor+MultisampleMask)?
                var oldState = GraphicsDevice.BlendState;

                if (oldState == null || oldState.ColorSourceBlend != ColorSourceBlend ||
                    oldState.ColorDestinationBlend != ColorDestinationBlend ||
                    oldState.AlphaSourceBlend != AlphaSourceBlend ||
                    oldState.AlphaDestinationBlend != AlphaDestinationBlend)
                {
                    GL.BlendFuncSeparate(
                        (OpenTK.Graphics.OpenGL.BlendingFactorSrc) ColorSourceBlend,
                        (OpenTK.Graphics.OpenGL.BlendingFactorDest) ColorDestinationBlend,
                        (OpenTK.Graphics.OpenGL.BlendingFactorSrc) AlphaSourceBlend,
                        (OpenTK.Graphics.OpenGL.BlendingFactorDest) AlphaDestinationBlend);
                }

                if (oldState == null || oldState.ColorBlendFunction != ColorBlendFunction || oldState.AlphaBlendFunction != AlphaBlendFunction)
                    GL.BlendEquationSeparate((OpenTK.Graphics.OpenGL.BlendEquationMode) ColorBlendFunction,(OpenTK.Graphics.OpenGL.BlendEquationMode) AlphaBlendFunction);

                if (oldState == null || oldState.ColorWriteChannels != ColorWriteChannels)
                {
                    GL.ColorMask((ColorWriteChannels & ColorWriteChannels.Red) != 0,
                        (ColorWriteChannels & ColorWriteChannels.Green) != 0,
                        (ColorWriteChannels & ColorWriteChannels.Blue) != 0,
                        (ColorWriteChannels & ColorWriteChannels.Alpha) != 0);
                }

                if (oldState == null || oldState.ColorWriteChannels1 != ColorWriteChannels1)
                {
                    GL.ColorMask(1,
                        (ColorWriteChannels1 & ColorWriteChannels.Red) != 0,
                        (ColorWriteChannels1 & ColorWriteChannels.Green) != 0,
                        (ColorWriteChannels1 & ColorWriteChannels.Blue) != 0,
                        (ColorWriteChannels1 & ColorWriteChannels.Alpha) != 0);
                }

                if (oldState == null || oldState.ColorWriteChannels2 != ColorWriteChannels2)
                {
                    GL.ColorMask(2,
                        (ColorWriteChannels2 & ColorWriteChannels.Red) != 0,
                        (ColorWriteChannels2 & ColorWriteChannels.Green) != 0,
                        (ColorWriteChannels2 & ColorWriteChannels.Blue) != 0,
                        (ColorWriteChannels2 & ColorWriteChannels.Alpha) != 0);
                }

                if (oldState == null || oldState.ColorWriteChannels3 != ColorWriteChannels3)
                {
                    GL.ColorMask(3,
                        (ColorWriteChannels3 & ColorWriteChannels.Red) != 0,
                        (ColorWriteChannels3 & ColorWriteChannels.Green) != 0,
                        (ColorWriteChannels3 & ColorWriteChannels.Blue) != 0,
                        (ColorWriteChannels3 & ColorWriteChannels.Alpha) != 0);
                }
            }
        }

        internal void Unbind()
        {
            GraphicsDevice = null;
        }
    }
}

