using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeWriter]
    public class RasterizerStateTypeWriter  : ContentTypeWriter<RasterizerState>
    {
        #region implemented abstract members of ContentTypeWriter

        public override void Write(ContentWriter writer, RasterizerState value)
        {
            if (value == null)
            {
                writer.Write(true);
                return;
            }
            writer.Write(false);
            writer.Write((ushort)value.CullMode);
            writer.Write((ushort)value.FillMode);

            writer.Write(value.MultiSampleAntiAlias);
            writer.Write(value.ScissorTestEnable);
        }

        public override string RuntimeReaderName => typeof(RasterizerStateTypeReader).FullName;

        #endregion
    }

    [ContentTypeWriter]
    public class DepthStencilStateTypeWriter  : ContentTypeWriter<DepthStencilState>
    {
        #region implemented abstract members of ContentTypeWriter

        public override void Write(ContentWriter writer, DepthStencilState value)
        {
            if (value == null)
            {
                writer.Write(true);
                return;
            }
            writer.Write(false);
            writer.Write(value.DepthBufferEnable);
            writer.Write(value.DepthBufferWriteEnable);
            writer.Write(value.StencilEnable);

            writer.Write(value.ReferenceStencil);
            writer.Write(value.StencilMask);

            writer.Write((ushort)value.DepthBufferFunction);
            writer.Write((ushort)value.StencilFunction);
            writer.Write((ushort)value.StencilDepthBufferFail);
            writer.Write((ushort)value.StencilFail);
            writer.Write((ushort)value.StencilPass);
        }

        public override string RuntimeReaderName => typeof(DepthStencilStateTypeReader).FullName;

        #endregion
    }

    [ContentTypeWriter]
    public class BlendStateTypeWriter  : ContentTypeWriter<BlendState>
    {
        #region implemented abstract members of ContentTypeWriter

        public override void Write(ContentWriter writer, BlendState value)
        {
            if (value == null)
            {
                writer.Write(true);
                return;
            }
            writer.Write(false);
            writer.Write((ushort)value.AlphaBlendFunction);
            writer.Write((ushort)value.AlphaDestinationBlend);
            writer.Write((ushort)value.AlphaSourceBlend);

            writer.Write((ushort)value.ColorBlendFunction);
            writer.Write((ushort)value.ColorDestinationBlend);
            writer.Write((ushort)value.ColorSourceBlend);

            writer.Write(value.BlendFactor);
        }

        public override string RuntimeReaderName => typeof(BlendStateTypeReader).FullName;

        #endregion
    }
}

