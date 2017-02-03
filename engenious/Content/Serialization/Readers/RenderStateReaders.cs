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
            RasterizerState state = new RasterizerState();
            state.CullMode = (CullMode)reader.ReadUInt16();
            state.FillMode = (PolygonMode)reader.ReadUInt16();
            state.MultiSampleAntiAlias = reader.ReadBoolean();
            state.ScissorTestEnable = reader.ReadBoolean();
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
            DepthStencilState state = new DepthStencilState();
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
            BlendState state = new BlendState();
            state.AlphaBlendFunction = (BlendEquationMode)reader.ReadUInt16();
            state.AlphaDestinationBlend = (BlendingFactorDest)reader.ReadUInt16();
            state.AlphaSourceBlend = (BlendingFactorSrc)reader.ReadUInt16();

            state.ColorBlendFunction = (BlendEquationMode)reader.ReadUInt16();
            state.ColorDestinationBlend = (BlendingFactorDest)reader.ReadUInt16();
            state.ColorSourceBlend = (BlendingFactorSrc)reader.ReadUInt16();

            state.BlendFactor = reader.ReadColor();
            return state;
        }

        #endregion
    }
}

