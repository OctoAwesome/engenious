using System.Linq;
using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeReader(typeof(Effect))]
    public class EffectTypeReader:ContentTypeReader<Effect>
    {
        public override Effect Read(ContentManager manager, ContentReader reader)
        {
            var effect = new Effect(manager.GraphicsDevice);

            var techniqueCount = reader.ReadInt32();
            for (var techniqueIndex = 0; techniqueIndex < techniqueCount; techniqueIndex++)
            {
                var technique = new EffectTechnique(reader.ReadString());
                var passCount = reader.ReadInt32();
                for (var passIndex = 0; passIndex < passCount; passIndex++)
                {
                    var pass = new EffectPass(reader.ReadString());

                    pass.BlendState = reader.Read<BlendState>(manager);
                    pass.DepthStencilState = reader.Read<DepthStencilState>(manager);
                    pass.RasterizerState = reader.Read<RasterizerState>(manager);
                    int shaderCount = reader.ReadByte();
                    
                    for (var shaderIndex = 0; shaderIndex < shaderCount; shaderIndex++)
                    {
                        var shader = new Shader((ShaderType)reader.ReadUInt16(), reader.ReadString());
                        shader.Compile();
                        pass.AttachShader(shader);
                    }

                    int attrCount = reader.ReadByte();
                    for (var attrIndex = 0; attrIndex < attrCount; attrIndex++)//TODO: perhaps needs to be done later?
                    {
                        var usage = (VertexElementUsage)reader.ReadByte();
                        pass.BindAttribute(usage, reader.ReadString());
                    }
                    pass.Link();
                    technique.Passes.Add(pass);
                }
                effect.Techniques.Add(technique);
            }

            effect.CurrentTechnique = effect.Techniques.TechniqueList.FirstOrDefault();
            effect.Initialize();
            return effect;
        }
    }
}

