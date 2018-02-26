using System.Threading;
using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeReader(typeof(RasterizerState))]
    public class RasterizerStateTypeReader : ContentTypeReader<RasterizerState>
    {
        #region implemented abstract members of ContentTypeReader

        public override RasterizerState Read(ContentManager manager, ContentReader reader)
        {
            if (reader.ReadBoolean())
                return null;
            var state = new RasterizerState();
            state.CullMode = (CullMode)reader.ReadUInt16();
            state.FillMode = (PolygonMode)reader.ReadUInt16();
            state.MultiSampleAntiAlias = reader.ReadBoolean();
            state.ScissorTestEnable = reader.ReadBoolean();
            
            state.DepthBias = reader.ReadSingle();
            state.SlopeScaleDepthBias = reader.ReadSingle();
            return state;
        }

        #endregion
    }

    [ContentTypeReader(typeof(DepthStencilState))]
    public class DepthStencilStateTypeReader : ContentTypeReader<DepthStencilState>
    {
        #region implemented abstract members of ContentTypeReader

        public override DepthStencilState Read(ContentManager manager, ContentReader reader)
        {
            if (reader.ReadBoolean())
                return null;
            var state = new DepthStencilState();
            state.DepthBufferEnable = reader.ReadBoolean();
            state.DepthBufferWriteEnable = reader.ReadBoolean();
            state.StencilEnable = reader.ReadBoolean();

            state.ReferenceStencil = reader.ReadInt32();
            state.StencilMask = reader.ReadInt32();

            state.DepthBufferFunction = (DepthFunction)reader.ReadUInt16();
            state.StencilFunction = (StencilFunction)reader.ReadUInt16();
            state.StencilDepthBufferFail = (StencilOp)reader.ReadUInt16();
            state.StencilFail = (StencilOp)reader.ReadUInt16();
            state.StencilPass = (StencilOp)reader.ReadUInt16();

            state.TwoSidedStencilMode = reader.ReadBoolean();
            state.CounterClockwiseStencilFunction = (StencilFunction) reader.ReadUInt16();
            state.CounterClockwiseStencilDepthBufferFail= (StencilOp) reader.ReadUInt16();
            state.CounterClockwiseStencilFail= (StencilOp) reader.ReadUInt16();
            state.CounterClockwiseStencilPass= (StencilOp) reader.ReadUInt16();
            return state;
        }

        #endregion
    }

    [ContentTypeReader(typeof(BlendState))]
    public class BlendStateTypeReader : ContentTypeReader<BlendState>
    {
        #region implemented abstract members of ContentTypeReader

        public override BlendState Read(ContentManager manager, ContentReader reader)
        {
            if (reader.ReadBoolean())
                return null;
            var state = new BlendState();
            state.AlphaBlendFunction = (BlendEquationMode)reader.ReadUInt16();
            state.AlphaDestinationBlend = (BlendingFactorDest)reader.ReadUInt16();
            state.AlphaSourceBlend = (BlendingFactorSrc)reader.ReadUInt16();

            state.ColorBlendFunction = (BlendEquationMode)reader.ReadUInt16();
            state.ColorDestinationBlend = (BlendingFactorDest)reader.ReadUInt16();
            state.ColorSourceBlend = (BlendingFactorSrc)reader.ReadUInt16();

            //state.BlendFactor = reader.ReadColor();
            state.ColorWriteChannels = (ColorWriteChannels) reader.ReadByte();
            state.ColorWriteChannels1 = (ColorWriteChannels) reader.ReadByte();
            state.ColorWriteChannels2 = (ColorWriteChannels) reader.ReadByte();
            state.ColorWriteChannels3 = (ColorWriteChannels) reader.ReadByte();
            return state;
        }

        #endregion
    }
}

