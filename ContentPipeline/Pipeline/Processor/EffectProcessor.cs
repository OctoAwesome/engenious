using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;
using System.Text;
using engenious.Graphics;
using engenious.Pipeline;
using OpenTK.Graphics.OpenGL;

namespace engenious.Content.Pipeline
{
    [ContentProcessor(DisplayName = "Effect Processor")]
    public class EffectProcessor : ContentProcessor<EffectContent, EffectContent,EffectProcessorSettings>
    {
        private static BuildMessageEventArgs.BuildMessageType GetMessageType(string line)
        {
            var splt = line.Split(':');
            var messageType = BuildMessageEventArgs.BuildMessageType.None;
            foreach (var s in splt)
            {
                var trimmed = s.Trim();
                if (trimmed.StartsWith("error", StringComparison.InvariantCultureIgnoreCase))
                {
                    messageType = BuildMessageEventArgs.BuildMessageType.Error;
                }
                else if (trimmed.StartsWith("warning", StringComparison.InvariantCultureIgnoreCase))
                {
                    messageType = BuildMessageEventArgs.BuildMessageType.Warning;
                }
                else if (trimmed.StartsWith("info", StringComparison.InvariantCultureIgnoreCase))
                {
                    messageType = BuildMessageEventArgs.BuildMessageType.Information;
                }
            }

            return messageType;
        }

        private int GetMinGreaterZero(int a, int b)
        {
            if (a < 0)
                return b;
            if (b < 0)
                return a;
            return Math.Min(a, b);
        }
        private void PreprocessMessage(ContentProcessorContext context, string file, string msg, BuildMessageEventArgs.BuildMessageType messageType)
        {
            string[] lines = msg.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                messageType = GetMessageType(lines[i]);
                if (messageType == BuildMessageEventArgs.BuildMessageType.Error || messageType == BuildMessageEventArgs.BuildMessageType.Warning)
                {
                    int sInd = GetMinGreaterZero(lines[i].IndexOf("0:", StringComparison.Ordinal),lines[i].IndexOf("0(", StringComparison.Ordinal));
                    string errorLoc = string.Empty;
                    if (sInd != -1)
                    {
                        lines[i] = lines[i].Substring(sInd + 2);
                        int eInd = GetMinGreaterZero(lines[i].IndexOf(':'),lines[i].IndexOf(')'));
                        if (eInd != -1)
                        {
                            errorLoc = lines[i].Substring(0, eInd);
                            if (errorLoc.IndexOf(',') == -1)
                                errorLoc = errorLoc + ",1";
                            lines[i] = lines[i].Substring(eInd+3).Trim();
                        }
                    }
                    
                    lines[i] = file + "("+errorLoc+"): " + lines[i];
                }
                context.RaiseBuildMessage(file,lines[i],messageType);
            }
        }


        private void GenerateEffectSource(EffectContent input, string @namespace,string name, ContentProcessorContext context)
        {
            using(var csSource = new StringWriter())
            using (var csSourceWriter = new IndentedTextWriter(csSource, "    "))
            {
                input.UserEffectName = "engenious.UserEffects." + @namespace + "." + name;
                csSourceWriter.WriteLine("using engenious.Graphics;");
                csSourceWriter.WriteLine("namespace engenious.UserEffects" + (string.IsNullOrEmpty(@namespace) ? string.Empty : "."+@namespace));
                csSourceWriter.WriteLine("{");
                csSourceWriter.Indent++;
                csSourceWriter.WriteLine($"public class {name} : engenious.Graphics.Effect");
                csSourceWriter.WriteLine("{");
                csSourceWriter.Indent++;
                
                csSourceWriter.WriteLine($"public {name}(GraphicsDevice graphicsDevice)");
                csSourceWriter.Indent++;
                csSourceWriter.WriteLine(": base(graphicsDevice)");
                csSourceWriter.Indent--;
                csSourceWriter.WriteLine("{");
                csSourceWriter.WriteLine("}");
                csSourceWriter.WriteLine("protected override void Initialize ()");
                csSourceWriter.WriteLine("{");
                csSourceWriter.Indent++;
                csSourceWriter.WriteLine("base.Initialize();");
                foreach (var technique in input.Techniques)
                {
                    csSourceWriter.WriteLine($"{technique.Name} = Techniques[\"{technique.Name}\"] as {technique.Name}Impl;");
                }
                csSourceWriter.Indent--;
                csSourceWriter.WriteLine("}");
                foreach (var technique in input.Techniques)
                {
                    csSourceWriter.WriteLine($"public {technique.Name}Impl {technique.Name} {{get; private set;}}");
                }

                foreach (var technique in input.Techniques)
                {
                    GenerateEffectTechniqueSource(technique, csSourceWriter);
                }
                csSourceWriter.Indent--;
                csSourceWriter.WriteLine("}");
                csSourceWriter.Indent--;
                
                csSourceWriter.WriteLine("}");


                context.SourceFiles.Add(new SourceFile((string.IsNullOrEmpty(@namespace) ? string.Empty : @namespace + ".") + name, csSource.ToString()));
            }
        }

        private void GenerateEffectTechniqueSource(EffectTechnique technique,IndentedTextWriter src)
        {
            technique.UserTechniqueName = $"{technique.Name}Impl";
            src.WriteLine($"public class {technique.Name}Impl : engenious.Graphics.EffectTechnique");
            src.WriteLine("{");
            src.Indent++;
            src.WriteLine($"public {technique.Name}Impl(string name)");
            src.Indent++;
            src.WriteLine(": base(name)");
            src.Indent--;
            src.WriteLine("{");
            src.WriteLine("}");
            src.WriteLine("protected override void Initialize()");
            src.WriteLine("{");
            src.Indent++;
            src.WriteLine("base.Initialize();");
            foreach (var pass in technique.Passes)
            {
                src.WriteLine($"{pass.Name} = Passes[\"{pass.Name}\"] as {pass.Name}Impl;");
            }
            src.Indent--;
            src.WriteLine("}");
            foreach (var pass in technique.Passes)
            {
                src.WriteLine($"public {pass.Name}Impl {pass.Name} {{get; private set;}}");
            }
            foreach (var pass in technique.Passes)
            {
                GenerateEffectPassSource(pass,src);
            }
            src.Indent--;
            src.WriteLine("}");
        }

        private void GenerateEffectPassSource(EffectPass pass,IndentedTextWriter src)
        {
            src.WriteLine($"public class {pass.Name}Impl : engenious.Graphics.EffectPass");
            src.WriteLine("{");
            src.Indent++;
            src.WriteLine($"public {pass.Name}Impl(string name)");
            src.Indent++;
            src.WriteLine(": base(name)");
            src.Indent--;
            src.WriteLine("{");
            src.WriteLine("}");

            src.WriteLine("protected override void CacheParameters()");
            src.WriteLine("{");
            src.Indent++;
            src.WriteLine("base.CacheParameters();");
            foreach (var p in pass.Parameters)
            {
                if (p.Type == typeof(EffectPassParameter))
                {
                    src.WriteLine($"{p.Name} = Parameters[\"{p.Name}\"];");
                }
                else
                {
                    src.WriteLine($"_{p.Name}PassParameter = Parameters[\"{p.Name}\"];");
                }
            }
            src.Indent--;
            src.WriteLine("}");
            foreach (var p in pass.Parameters)
            {
                if (p.Type == typeof(EffectPassParameter))
                {
                    src.WriteLine($"public {p.Type.FullName} {p.Name} {{get; private set;}}");
                }
                else
                {
                    src.WriteLine($"private {p.Type.FullName} _{p.Name};");
                    src.WriteLine($"private EffectPassParameter _{p.Name}PassParameter;");
                    src.WriteLine($"public {p.Type.FullName} {p.Name}");
                    src.WriteLine("{");
                    src.Indent++;
                    src.WriteLine("get");
                    src.WriteLine("{");
                    src.Indent++;
                    src.WriteLine($"return _{p.Name};");
                    src.Indent--;
                    src.WriteLine("}");
                    src.WriteLine("set");
                    src.WriteLine("{");
                    src.Indent++;
                    src.WriteLine($"if (_{p.Name} == value) return;");
                    src.WriteLine($"_{p.Name} = value;");
                    src.WriteLine($"_{p.Name}PassParameter.SetValue(value);");
                    src.Indent--;
                    src.WriteLine("}");
                    src.Indent--;
                    src.WriteLine("}");
                }
            }
            src.Indent--;
            
            src.WriteLine("}");
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
                var success = true;
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
                                var tmp = new Shader(context.GraphicsDevice,shader.Key, File.ReadAllText(shader.Value));
                                tmp.Compile();
                                compiledPass.AttachShader(tmp);
                            }
                            catch (Exception ex)
                            {
                                success = false;
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

                if (!success)
                    return null;

                if (input.CreateUserEffect)
                {
                    string rel = context.GetRelativePath(filename);
                    string namespce = Path.GetDirectoryName(rel);
                    namespce = namespce.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
                    GenerateEffectSource(input, namespce,Path.GetFileNameWithoutExtension(rel) , context);
                }

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

