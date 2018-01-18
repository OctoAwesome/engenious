using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using engenious.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace engenious.Content.Pipeline
{
    [ContentProcessor(DisplayName = "Effect Processor")]
    public class EffectProcessor : ContentProcessor<EffectContent, EffectContent,EffectProcessorSettings>
    {
        private void PreprocessMessage(ContentProcessorContext context, string file, string msg, BuildMessageEventArgs.BuildMessageType messageType)
        {
            string[] lines = msg.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("error:", StringComparison.InvariantCultureIgnoreCase))
                {
                    lines[i] = lines[i].Substring("ERROR: 0:".Length);
                    int eInd = lines[i].IndexOf(':');
                    string errorLoc = string.Empty;
                    if (eInd != -1)
                    {
                        errorLoc = "(" + lines[i].Substring(0, eInd) + ")";
                        lines[i] = lines[i].Substring(eInd + 1);
                    }
                    lines[i] = errorLoc + ":ERROR:" + lines[i];
                }
                context.RaiseBuildMessage(file,lines[i],messageType);
            }
        }


        private void GenerateEffectSource(EffectContent input, string name, ContentProcessorContext context)
        {
            input.UserEffectName = "engenious.UserEffects." + name;
            var mainCsSource = new StringBuilder();
            mainCsSource.AppendLine("using engenious.Graphics;");
            mainCsSource.AppendLine("namespace engenious.UserEffects");
            mainCsSource.AppendLine("{");

            mainCsSource.AppendLine($"\tpublic class {name} : engenious.Graphics.Effect");
            mainCsSource.AppendLine("\t{");
            mainCsSource.AppendLine($"\t\tpublic {name}(GraphicsDevice graphicsDevice)");
            mainCsSource.AppendLine("\t\t\t: base(graphicsDevice)");
            mainCsSource.AppendLine("\t\t{");
            mainCsSource.AppendLine("\t\t}");
            mainCsSource.AppendLine("\t\tinternal virtual void Initialize ()");
            mainCsSource.AppendLine("\t\t{");
            mainCsSource.AppendLine("\t\t\tbase.Initialize();");
            foreach (var technique in input.Techniques)
            {
                mainCsSource.AppendLine($"\t\t\t{technique.Name} = Techniques[\"{technique.Name}\"] as {technique.Name}Impl;");
            }
            mainCsSource.AppendLine("\t\t}");
            foreach (var technique in input.Techniques)
            {
                mainCsSource.AppendLine($"\t\tpublic {technique.Name}Impl {technique.Name} {{get; private set;}}");
            }

            foreach (var technique in input.Techniques)
            {
                GenerateEffectTechniqueSource(technique,mainCsSource);
            }
            mainCsSource.AppendLine("\t}");
            mainCsSource.AppendLine("}");



            context.CompiledSourceFiles[name + ".cs"] = mainCsSource.ToString();
        }

        private void GenerateEffectTechniqueSource(EffectTechnique technique,StringBuilder src)
        {
            technique.UserTechniqueName = $"{technique.Name}Impl";
            src.AppendLine($"\t\tpublic class {technique.Name}Impl : engenious.Graphics.EffectTechnique");
            src.AppendLine("\t\t{");
            
            src.AppendLine($"\t\t\tpublic {technique.Name}Impl(string name)");
            src.AppendLine("\t\t\t\t: base(name)");
            src.AppendLine("\t\t\t{");
            src.AppendLine("\t\t\t}");
            foreach (var pass in technique.Passes)
            {
                src.AppendLine($"\t\t\tpublic {pass.Name}Impl {pass.Name} {{get; private set;}}");
            }
            foreach (var pass in technique.Passes)
            {
                GenerateEffectPassSource(pass,src);
            }
            
            src.AppendLine("\t\t}");
        }
        private void GenerateEffectPassSource(EffectPass pass,StringBuilder src)
        {

            src.AppendLine($"\t\t\tpublic class {pass.Name}Impl : engenious.Graphics.EffectPass");
            src.AppendLine("\t\t\t{");

            src.AppendLine($"\t\t\t\tpublic {pass.Name}Impl(string name)");
            src.AppendLine("\t\t\t\t\t: base(name)");
            src.AppendLine("\t\t\t\t{");
            src.AppendLine("\t\t\t\t}");

            src.AppendLine("\t\t\t\tinternal virtual void CacheParameters()");
            src.AppendLine("\t\t\t\t{");
            src.AppendLine("\t\t\t\t\tbase.CacheParameters();");
            foreach (var p in pass.Parameters)
            {
                if (p.Type == typeof(EffectPassParameter))
                {
                    src.AppendLine($"\t\t\t\t\t{p.Name} = Parameters[\"{p.Name}\"];");
                }
                else
                {
                    src.AppendLine($"\t\t\t\t\t_{p.Name}PassParameter = Parameters[\"{p.Name}\"];");
                }
            }
            src.AppendLine("\t\t\t\t}");
            foreach (var p in pass.Parameters)
            {
                if (p.Type == typeof(EffectPassParameter))
                {
                    src.AppendLine($"\t\t\t\tpublic {p.Type.FullName} {p.Name} {{get; private set;}}");
                }
                else
                {
                    src.AppendLine($"\t\t\t\tprivate {p.Type.FullName} _{p.Name};");
                    src.AppendLine($"\t\t\t\tprivate EffectPassParameter _{p.Name}PassParameter;");
                    src.AppendLine($"\t\t\t\tpublic {p.Type.FullName} {p.Name}");
                    src.AppendLine("\t\t\t\t{");
                    src.AppendLine("\t\t\t\t\tget");
                    src.AppendLine("\t\t\t\t\t{");
                    src.AppendLine($"\t\t\t\t\t\treturn _{p.Name};");
                    src.AppendLine("\t\t\t\t\t}");
                    src.AppendLine("\t\t\t\t\tset");
                    src.AppendLine("\t\t\t\t\t{");
                    src.AppendLine($"\t\t\t\t\t\tif (_{p.Name} == value) return;");
                    src.AppendLine($"\t\t\t\t\t\t_{p.Name} = value;");
                    src.AppendLine($"\t\t\t\t\t\t_{p.Name}PassParameter.SetValue(value);");
                    src.AppendLine("\t\t\t\t\t}");
                    src.AppendLine("\t\t\t\t}");
                }
            }
            
            
            src.AppendLine("\t\t\t}");
        }

        private Type getType(ActiveUniformType type)
        {
            Type t;
            switch (type)
            {
                case ActiveUniformType.Bool:
                    t = typeof(bool);
                    break;
                case ActiveUniformType.Double:
                    t = typeof(double);
                    break;
                case ActiveUniformType.Float:
                    t = typeof(float);
                    break;
                case ActiveUniformType.FloatMat4:
                    t = typeof(Matrix);
                    break;
                case ActiveUniformType.FloatVec2:
                    t = typeof(Vector2);
                    break;
                case ActiveUniformType.FloatVec3:
                    t = typeof(Vector3);
                    break;
                case ActiveUniformType.FloatVec4:
                    t = typeof(Vector4);
                    break;
                case ActiveUniformType.Sampler2D:
                    t = typeof(Texture2D);
                    break;
                case ActiveUniformType.Sampler2DArray:
                    t = typeof(Texture2DArray);
                    break;
                case ActiveUniformType.Int:
                    t = typeof(int);
                    break;
                case ActiveUniformType.IntVec2:
                    t = typeof(Point);
                    break;
                case ActiveUniformType.UnsignedInt:
                    t = typeof(uint);
                    break;
                default:
                    t = typeof(EffectPassParameter);
                    break;
            }
            return t;
        }
        public override EffectContent Process(EffectContent input, string filename, ContentProcessorContext context)
        {
            try
            {
                input.CreateUserEffect = settings.CreateUserEffect;
                //Passthrough and verification
                foreach (var technique in input.Techniques)
                {
                    foreach (var pass in technique.Passes)
                    {
                        Graphics.EffectPass compiledPass = new Graphics.EffectPass(pass.Name);

                        foreach (var shader in pass.Shaders)
                        {
                            try
                            {
                                var tmp = new Shader(shader.Key, File.ReadAllText(shader.Value));
                                tmp.Compile();
                                compiledPass.AttachShader(tmp);
                            }
                            catch (Exception ex)
                            {
                                PreprocessMessage(context,shader.Value, ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
                                
                            }
                        }

                        compiledPass.Link();
                        compiledPass.Apply();
                        foreach (var attr in pass.Attributes)
                        {
                            compiledPass.BindAttribute(attr.Key, attr.Value);
                        }
                        compiledPass.CacheParameters();

                        foreach (var p in compiledPass.Parameters)
                        {
                            pass.Parameters.Add(new ParameterInfo(p.Name,getType(p.Type)));
                        }
                    }
                }

                if (input.CreateUserEffect)
                    GenerateEffectSource(input, Path.GetFileNameWithoutExtension(filename), context);
                
                return input;
            }
            catch (Exception ex)
            {
                PreprocessMessage(context,Path.GetFileName(filename), ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }

    }
    [Serializable]
    public class EffectProcessorSettings : ProcessorSettings
    {
        [DefaultValue(true)]
        public bool CreateUserEffect{get;set;}=true;

    }
}

