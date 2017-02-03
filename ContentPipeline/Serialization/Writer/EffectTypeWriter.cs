using System.IO;
using engenious.Content.Pipeline;

namespace engenious.Content.Serialization
{
    [ContentTypeWriter]
    public class EffectTypeWriter : ContentTypeWriter<EffectContent>
    {
        #region implemented abstract members of ContentTypeWriter

        public override void Write(ContentWriter writer, EffectContent value)
        {
            writer.Write(value.Techniques.Count);
            foreach (var technique in value.Techniques)
            {
                writer.Write(technique.Name);
                writer.Write(technique.Passes.Count);
                foreach (var pass in technique.Passes)
                {
                    writer.Write(pass.Name);

                    writer.WriteObject(pass.BlendState);
                    writer.WriteObject(pass.DepthStencilState);
                    writer.WriteObject(pass.RasterizerState);
                    writer.Write((byte)pass.Shaders.Count);
                    foreach (var shader in pass.Shaders)
                    {
                        writer.Write((ushort)shader.Key);
                        writer.Write(File.ReadAllText(shader.Value));
                    }

                    writer.Write((byte)pass.Attributes.Count);
                    foreach (var attr in pass.Attributes)
                    {
                        writer.Write((byte)attr.Key);
                        writer.Write(attr.Value);
                    }
                }
            }
        }

        public override string RuntimeReaderName => typeof(EffectTypeReader).FullName;

        #endregion
    }
}

