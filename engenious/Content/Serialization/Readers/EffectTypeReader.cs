using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeReader(typeof(Effect))]
    public class EffectTypeReader:ContentTypeReader<Effect>
    {
        private static Dictionary<string, Type> _effectTypes;

        private static HashSet<Assembly> _cachedAssemblies;
        static EffectTypeReader()
        {
            _effectTypes = new Dictionary<string, Type>();
            _cachedAssemblies = new HashSet<Assembly>();
            RecacheAssemblies();
        }

        private static void RecacheAssemblies()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (_cachedAssemblies.Contains(asm))
                    continue;
                _cachedAssemblies.Add(asm);
                if (!asm.IsDynamic)
                {
                    Type[] exportedTypes = null;
                    try
                    {
                        exportedTypes = asm.GetExportedTypes();
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        exportedTypes = e.Types;
                    }

                    if (exportedTypes != null)
                    {
                        foreach (var type in exportedTypes)
                        {
                            try
                            {
                                if (type != null && (typeof(Effect).IsAssignableFrom(type)||typeof(EffectTechnique).IsAssignableFrom(type)||typeof(EffectPass).IsAssignableFrom(type)))
                                {
                                    _effectTypes.Add(type.FullName ?? "", type);
                                }
                            }
                            catch (Exception ex)
                            {
                                
                            }
                        }
                    }
                }
            }
        }
        public override Effect Read(ContentManager manager, ContentReader reader)
        {
            var useCustomType = reader.ReadBoolean();
            bool canUseCustomType = false;
            var effectType = typeof(Effect);
            if (useCustomType)
            {
                try
                {
                    RecacheAssemblies();
                    canUseCustomType = true;
                    var customTypeName = reader.ReadString();
                    effectType = _effectTypes[customTypeName];
                    
                }
                catch (Exception ex)
                {
                    canUseCustomType = false;
                }
                
            }
            Effect effect = null;
            if (canUseCustomType)
                effect = (Effect)Activator.CreateInstance(effectType,manager.GraphicsDevice);
            else
                effect= new Effect(manager.GraphicsDevice);

            var techniqueCount = reader.ReadInt32();
            for (var techniqueIndex = 0; techniqueIndex < techniqueCount; techniqueIndex++)
            {
                EffectTechnique technique = null;
                string techniqueName = reader.ReadString();

                if (useCustomType)
                {
                    try
                    {
                        string customTypeName = effectType.FullName + "+" + reader.ReadString();

                        var techniqueType = _effectTypes[customTypeName];
                        technique = (EffectTechnique) Activator.CreateInstance(techniqueType,techniqueName);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                if (technique == null)
                    technique = new EffectTechnique(techniqueName);
                var passCount = reader.ReadInt32();
                for (var passIndex = 0; passIndex < passCount; passIndex++)
                {
                    var passName = reader.ReadString();
                    EffectPass pass = null;
                    if (useCustomType)
                    {
                        try
                        {
                            string customTypeName = technique.GetType().FullName + "+" + passName + "Impl";

                            var passType = _effectTypes[customTypeName];
                            pass = (EffectPass) Activator.CreateInstance(passType,passName);
                        }
                        catch (Exception ex)
                        {
                        
                        }
                    }
                    if (pass == null)
                        pass = new EffectPass(passName);
                    pass.BlendState = reader.Read<BlendState>(manager);
                    pass.DepthStencilState = reader.Read<DepthStencilState>(manager);
                    pass.RasterizerState = reader.Read<RasterizerState>(manager);
                    int shaderCount = reader.ReadByte();
                    
                    for (var shaderIndex = 0; shaderIndex < shaderCount; shaderIndex++)
                    {
                        var shader = new Shader(manager.GraphicsDevice,(ShaderType)reader.ReadUInt16(), reader.ReadString());
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

            effect.CurrentTechnique = effect.Techniques.FirstOrDefault();
            effect.Initialize();
            return effect;
        }
    }
}

