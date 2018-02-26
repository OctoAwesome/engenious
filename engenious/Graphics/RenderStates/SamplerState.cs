using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.CompilerServices;
using engenious.Helper;
using OpenTK.Graphics.OpenGL;

namespace engenious.Graphics
{

    public class SamplerState : GraphicsResource
    {
        
        private const TextureParameterName TextureParameterNameTextureMaxAnisotropy = (TextureParameterName) ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt;

        private const TextureParameterName TextureParameterNameTextureMaxLevel = (TextureParameterName) All.TextureMaxLevel;
        public static readonly SamplerState LinearClamp;
        public static readonly SamplerState LinearWrap;

        static SamplerState()
        {
            LinearClamp = new SamplerState();
            LinearClamp.InitPredefined();
            LinearWrap = new SamplerState();
            LinearWrap.AddressU = LinearWrap.AddressV = LinearWrap.AddressW = TextureWrapMode.Repeat;
            LinearWrap.InitPredefined();
        }

        public SamplerState()
        {
            //TODO: implement completly	
            AddressU = AddressV = AddressW = TextureWrapMode.ClampToEdge;
        }

        private bool _isPredefined;
        internal void InitPredefined()
        {
            _isPredefined = true;
        }

        private void ThrowIfReadOnly()
        {
            if (_isPredefined)
                throw new InvalidOperationException("you are not allowed to change a predefined sampler state");
        }

        public TextureWrapMode AddressU
        {
            get => _addressU;
            set
            {
                ThrowIfReadOnly();
                if (_addressU == value)
                    return;
                _addressU = value;
                if (_boundTexture != null)
                    ApplyAddressU();
            }
        }

        private void ApplyAddressU()
        {
            using (Execute.OnUiContext)
            {
                BeginApply();
                ApplyAddressUGl();
                EndApply();
            }
        }
        private void ApplyAddressUGl()
        {
            GL.TexParameter(_boundTexture.Target, TextureParameterName.TextureWrapS,(int)AddressU);
        }

        public TextureWrapMode AddressV
        {
            get => _addressV;
            set
            {
                ThrowIfReadOnly();
                if (_addressV == value)
                    return;
                _addressV = value; 
                if (_boundTexture != null)
                    ApplyAddressVGl();
            }
        }

        private void ApplyAddressV()
        {
            using (Execute.OnUiContext)
            {
                BeginApply();
                ApplyAddressVGl();
                EndApply();
            }
        }
        private void ApplyAddressVGl()
        {
            GL.TexParameter(_boundTexture.Target, TextureParameterName.TextureWrapT,(int)AddressV);
        }

        public TextureWrapMode AddressW
        {
            get => _addressW;
            set
            {
                ThrowIfReadOnly();
                if (_addressW == value)
                    return;
                _addressW = value;
                if (_boundTexture != null)
                    ApplyAddressW();
            }
        }

        private void ApplyAddressW()
        {
            using (Execute.OnUiContext)
            {
                BeginApply();
                ApplyAddressWGl();
                EndApply();
            }
        }
        private void ApplyAddressWGl()
        {
            GL.TexParameter(_boundTexture.Target, TextureParameterName.TextureWrapR,(int)AddressW);
        }

        public MinFilter MinFilter
        {
            get => _minFilter;
            set
            {
                ThrowIfReadOnly();
                if (_minFilter == value)
                    return;
                _minFilter = value;
                if (_boundTexture != null)
                    ApplyMinFilter();
            }
        }

        private void ApplyMinFilter()
        {
            using (Execute.OnUiContext)
            {
                BeginApply();
                ApplyMinFilterGl();
                EndApply();
            }
        }
        private void ApplyMinFilterGl()
        {
            GL.TexParameter(_boundTexture.Target, TextureParameterName.TextureMinFilter,(int)MinFilter);
        }

        public MagFilter MagFilter
        {
            get => _magFilter;
            set
            {
                ThrowIfReadOnly();
                if (_magFilter == value)
                    return;
                _magFilter = value;
                if (_boundTexture != null)
                    ApplyMagFilter();
            }
        }

        private void ApplyMagFilter()
        {
            using (Execute.OnUiContext)
            {
                BeginApply();
                ApplyMagFilterGl();
                EndApply();
            }
        }

        private void ApplyMagFilterGl()
        {
            GL.TexParameter(_boundTexture.Target, TextureParameterName.TextureMagFilter,(int)MagFilter);
        }


        public int MaxAnisotropy
        {
            get => _maxAnisotropy;
            set
            {
                ThrowIfReadOnly();
                if (_maxAnisotropy == value)
                    return;
                _maxAnisotropy = value;
                if (_boundTexture != null)
                    ApplyMaxAnisotropy();
            }
        }
        private void ApplyMaxAnisotropy()
        {
            if (!GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic)
                return;
            using (Execute.OnUiContext)
            {
                BeginApply();
                ApplyMaxAnisotropyGl();
                EndApply();
            }
        }

        private void ApplyMaxAnisotropyGl()
        {
            GL.TexParameter(_boundTexture.Target, TextureParameterNameTextureMaxAnisotropy,MathHelper.Clamp(MaxAnisotropy, 1.0f, GraphicsDevice.Capabilities.MaxTextureAnisotropy));
        }

        
        public int MaxMipLevel
        {
            get => _maxMipLevel;
            set
            {
                ThrowIfReadOnly();
                if (_maxMipLevel == value)
                    return;
                _maxMipLevel = value;
                if (_boundTexture != null)
                    ApplyMaxMipLevel();
            }
        }

        private void ApplyMaxMipLevel()
        {
            if (!GraphicsDevice.Capabilities.SupportsTextureMaxLevel)
                return;
            using (Execute.OnUiContext)
            {
                BeginApply();

                ApplyMaxMipLevelGl();

                EndApply();
            }
        }

        private void ApplyMaxMipLevelGl()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterNameTextureMaxLevel,MaxMipLevel > 0 ? MaxMipLevel : 1000);
        }

        public float MipMapLevelOfDetailBias
        {
            get => _mipMapLevelOfDetailBias;
            set
            {
                ThrowIfReadOnly();
                if (_mipMapLevelOfDetailBias == value)
                    return;
                _mipMapLevelOfDetailBias = value;
                if (_boundTexture != null)
                    ApplyMipMapLevelOfDetailBias();
            }
        }
        private void ApplyMipMapLevelOfDetailBias()
        {
            using (Execute.OnUiContext)
            {
                BeginApply();

                ApplyMipMapLevelOfDetailBiasGl();

                EndApply();
            }
        }
        private void ApplyMipMapLevelOfDetailBiasGl()
        {
            GL.TexParameter(_boundTexture.Target, TextureParameterName.TextureLodBias, MipMapLevelOfDetailBias);
        }

        private int _maxAnisotropy;
        private int _maxMipLevel;
        private float _mipMapLevelOfDetailBias;

        private GetIndexedPName GetPNameFromTarget(TextureTarget target)
        {
            return PNames[GetIndexFromTarget(target)];
        }

        private int GetIndexFromTarget(TextureTarget target)
        {
            switch (target)
            {
                case TextureTarget.Texture1D:
                    return 0;
                case TextureTarget.Texture2D:
                    return 1;
                case TextureTarget.Texture3D:
                    return 2;
                case TextureTarget.Texture1DArray:
                    return 3;
                case TextureTarget.Texture2DArray:
                    return 4;
                case TextureTarget.TextureRectangle:
                    return 5;
                case TextureTarget.TextureCubeMap:
                    return 6;
                case TextureTarget.TextureCubeMapArray:
                    return 7;
            }

            throw new NotSupportedException();
        }

        private static readonly TextureTarget[] Targets = new[]
        {
            TextureTarget.Texture1D, TextureTarget.Texture2D, TextureTarget.Texture3D, TextureTarget.Texture1DArray,
            TextureTarget.Texture2DArray, TextureTarget.TextureRectangle, TextureTarget.TextureCubeMap,
            TextureTarget.TextureCubeMapArray
        };

        private static readonly GetIndexedPName[] PNames = new[]
        {
            GetIndexedPName.TextureBinding1D, GetIndexedPName.TextureBinding2D, GetIndexedPName.TextureBinding3D,
            GetIndexedPName.TextureBinding1DArray, GetIndexedPName.TextureBinding2DArray,
            GetIndexedPName.TextureBindingRectangle, GetIndexedPName.TextureBindingCubeMap,
            GetIndexedPName.TextureBindingCubeMapArray
        };



        private Texture _boundTexture = null;
        private GetIndexedPName _pName;
        internal void Bind(Texture texture)
        {
            texture.SamplerState?.Unbind();
            GraphicsDevice = texture.GraphicsDevice;
            _boundTexture = texture;
            _pName = GetPNameFromTarget(texture.Target);
            
            Apply();
        }

        private bool _bound;
        private int _previousText;
        private MinFilter _minFilter;
        private MagFilter _magFilter;
        private TextureWrapMode _addressU;
        private TextureWrapMode _addressV;
        private TextureWrapMode _addressW;

        private void BeginApply()
        {
            if (_bound)
                return;
            var index = GetIndexFromTarget(_boundTexture.Target);
            GL.GetInteger(PNames[index], 0, out _previousText);
            _bound = true;
        }

        private void EndApply()
        {
            _bound = false;
            if (_previousText != 0)
            {
                GL.BindTexture(_boundTexture.Target, _previousText);
            }
        }
        private void Apply()
        {
            using (Execute.OnUiContext)
            {
                BeginApply();
                var oldState = _boundTexture.SamplerState;

                if (oldState == null || oldState.MipMapLevelOfDetailBias != MipMapLevelOfDetailBias)
                    ApplyMipMapLevelOfDetailBiasGl();
                
                if (oldState == null ||oldState.AddressU != AddressU)
                    ApplyAddressUGl();
                if (oldState == null ||oldState.AddressV != AddressV)
                    ApplyAddressVGl();
                if (oldState == null ||oldState.AddressW != AddressW)
                    ApplyAddressWGl();

                if (oldState == null ||oldState.MagFilter != MagFilter)
                    ApplyMagFilterGl();
                if (oldState == null ||oldState.MinFilter != MinFilter)
                    ApplyMagFilterGl();
                
                if (GraphicsDevice.Capabilities.SupportsTextureMaxLevel && (oldState == null ||oldState.MaxMipLevel != MaxMipLevel))
                    ApplyMaxMipLevelGl();
                
                if (GraphicsDevice.Capabilities.SupportsTextureFilterAnisotropic && (oldState == null ||oldState.MaxAnisotropy != MaxAnisotropy))
                    ApplyMaxAnisotropyGl();
                
                
                EndApply();
            }
        }

        

        internal void Unbind()
        {
            _boundTexture = null;
        }
    }
}