using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using engenious.Graphics;

namespace engenious.Content.Pipeline
{

    [ContentImporter(".glsl", DisplayName = "Effect Importer", DefaultProcessor = "EffectProcessor")]
    public class EffectImporter : ContentImporter<EffectContent>
    {
        #region implemented abstract members of ContentImporter

        private static ShaderType ParseShaderType(string type)
        {
            if (type == "PixelShader" || type == "FragmentShader")
                return ShaderType.FragmentShader;
            if (type == "VertexShader")
                return ShaderType.VertexShader;

            if (type == "GeometryShader")
                return ShaderType.GeometryShader;
            if (type == "TessControlShader")
                return ShaderType.TessControlShader;
            if (type == "TessEvaluationShader")
                return ShaderType.TessEvaluationShader;
            if (type == "ComputeShader")
                return ShaderType.ComputeShader;

            return (ShaderType)(-1);
        }

        private static float ParseColorPart(string value)
        {
            value = value.Trim();
            int tmp;
            if (int.TryParse(value, out tmp))
                return  tmp / 255.0f;
            return float.Parse(value);
        }

        private static Color ParseColor(XmlElement el)
        {
            if (el.HasChildNodes)
            {
                float a = 1.0f, r = 0, g = 0, b = 0;
                foreach (XmlElement e in el.ChildNodes.OfType<XmlElement>())
                {
                    if (e.Name == "A")
                    {
                        a = ParseColorPart(e.InnerText);                           
                    }
                    else if (e.Name == "R")
                    {
                        r = ParseColorPart(e.InnerText);                           
                    }
                    else if (e.Name == "G")
                    {
                        g = ParseColorPart(e.InnerText);                           
                    }
                    else if (e.Name == "B")
                    {
                        b = ParseColorPart(e.InnerText);                           
                    }
                    else
                    {
                        throw new Exception("'" + e.Name + "' is not an option for the Color element");
                    }
                }

                return new Color(r, g, b, a);
            }
            if (string.IsNullOrEmpty(el.InnerText.Trim()))
                throw new Exception("Empty value not allowed for Colors");
            try
            {
                FieldInfo fI = typeof(Color).GetField(el.InnerText.Trim(), BindingFlags.Static);
                var value = fI?.GetValue(null);
                if (value != null) return (Color)value;
            }
            catch
            {
                // ignored
            }
            {
                string value = el.InnerText.Trim();
                int a, r, g, b;
                if (value.Length == 4 || value.Length == 3)
                {
                    int index = 0;
                    if (value.Length == 4)
                        a = Convert.ToInt16(value[index++].ToString(), 16);
                    else
                        a = 0xF;
                    r = Convert.ToInt16(value[index++].ToString(), 16);
                    g = Convert.ToInt16(value[index++].ToString(), 16);
                    b = Convert.ToInt16(value[index].ToString(), 16);

                    a = a << 4 | a;
                    r = r << 4 | r;
                    g = g << 4 | g;
                    b = b << 4 | b;
                }
                else if (value.Length == 6 || value.Length == 8)
                {
                    int index = 0;
                    if (value.Length == 6)
                        a = Convert.ToInt16(value.Substring(index += 2, 2), 16);
                    else
                        a = 0xFF;
                    r = Convert.ToInt16(value.Substring(index += 2, 2), 16);
                    g = Convert.ToInt16(value.Substring(index += 2, 2), 16);
                    b = Convert.ToInt16(value.Substring(index, 2), 16);
                }
                else
                {
                    throw new Exception("Color must either use A/R/G/B Xml Elements or be a Hexadecimal value of Length 3/4/6/8");
                }


                return new Color(r, g, b, a);
            }
        }

        private static BlendState ParseBlendState(XmlElement element)
        {
            if (!element.HasChildNodes || element.Name != "BlendState")
                return null;
            BlendState blendState = new BlendState();
            foreach (XmlElement el in element.ChildNodes.OfType<XmlElement>())
            {
                
                if (el.Name == "AlphaBlendFunction")
                    blendState.AlphaBlendFunction = (BlendEquationMode)Enum.Parse(typeof(BlendEquationMode), el.InnerText);
                else if (el.Name == "AlphaDestinationBlend")
                    blendState.AlphaDestinationBlend = (BlendingFactorDest)Enum.Parse(typeof(BlendingFactorDest), el.InnerText);
                else if (el.Name == "AlphaSourceBlend")
                    blendState.AlphaSourceBlend = (BlendingFactorSrc)Enum.Parse(typeof(BlendingFactorSrc), el.InnerText);
                else if (el.Name == "BlendFactor")
                    blendState.BlendFactor = ParseColor(el);
                else if (el.Name == "ColorBlendFunction")
                    blendState.ColorBlendFunction = (BlendEquationMode)Enum.Parse(typeof(BlendEquationMode), el.InnerText);
                else if (el.Name == "ColorDestinationBlend")
                    blendState.ColorDestinationBlend = (BlendingFactorDest)Enum.Parse(typeof(BlendingFactorDest), el.InnerText);
                else if (el.Name == "ColorSourceBlend")
                    blendState.ColorSourceBlend = (BlendingFactorSrc)Enum.Parse(typeof(BlendingFactorSrc), el.InnerText);
                /*else if (el.Name == "ColorWriteChannels")
                    blendState.ColorWriteChannels = (ColorWriteChannels)Enum.Parse(typeof(ColorWriteChannels), el.InnerText);
                else if (el.Name == "ColorWriteChannels1")
                    blendState.ColorWriteChannels1 = (ColorWriteChannels)Enum.Parse(typeof(ColorWriteChannels), el.InnerText);
                else if (el.Name == "ColorWriteChannels2")
                    blendState.ColorWriteChannels2 = (ColorWriteChannels)Enum.Parse(typeof(ColorWriteChannels), el.InnerText);
                else if (el.Name == "ColorWriteChannels3")
                    blendState.ColorWriteChannels3 = (ColorWriteChannels)Enum.Parse(typeof(ColorWriteChannels), el.InnerText);
                else if (el.Name == "IndependentBlendEnable")
                    blendState.IndependentBlendEnable = bool.Parse(el.InnerText);
                else if (el.Name == "MultiSampleMask")
                    blendState.MultiSampleMask = int.Parse(el.InnerText);*/
                else
                    throw new Exception("'" + el.Name + "' is not an option of the BlendState");

            }
            return blendState;
        }

        private static DepthStencilState ParseDepthStencilState(XmlElement element)
        {
            if (!element.HasChildNodes || element.Name != "DepthStencilState")
                return null;
            DepthStencilState depthStencilState = new DepthStencilState();
            foreach (XmlElement el in element.ChildNodes.OfType<XmlElement>())
            {
                /*if (el.Name == "CounterClockwiseStencilDepthBufferFail")
                    depthStencilState.CounterClockwiseStencilDepthBufferFail = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "CounterClockwiseStencilFail")
                    depthStencilState.CounterClockwiseStencilFail = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "CounterClockwiseStencilFunction")
                    depthStencilState.CounterClockwiseStencilFunction = (CompareFunction)Enum.Parse(typeof(CompareFunction), el.InnerText);
                else if (el.Name == "CounterClockwiseStencilPass")
                    depthStencilState.CounterClockwiseStencilPass = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);*/
                if (el.Name == "DepthBufferEnable")
                    depthStencilState.DepthBufferEnable = bool.Parse(el.InnerText);
                else if (el.Name == "DepthBufferFunction")
                    depthStencilState.DepthBufferFunction = (DepthFunction)Enum.Parse(typeof(DepthFunction), el.InnerText);
                else if (el.Name == "DepthBufferWriteEnable")
                    depthStencilState.DepthBufferWriteEnable = bool.Parse(el.InnerText);
                else if (el.Name == "ReferenceStencil")
                    depthStencilState.ReferenceStencil = int.Parse(el.InnerText);
                else if (el.Name == "DepthBufferFunction")
                    depthStencilState.StencilDepthBufferFail = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "ReferenceStencil")
                    depthStencilState.StencilEnable = bool.Parse(el.InnerText);
                else if (el.Name == "StencilFail")
                    depthStencilState.StencilFail = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);
                else if (el.Name == "StencilFunction")
                    depthStencilState.StencilFunction = (StencilFunction)Enum.Parse(typeof(StencilFunction), el.InnerText);
                else if (el.Name == "DepthBufferFunction")
                    depthStencilState.StencilMask = int.Parse(el.InnerText);
                else if (el.Name == "StencilPass")
                    depthStencilState.StencilPass = (StencilOp)Enum.Parse(typeof(StencilOp), el.InnerText);
                /*else if (el.Name == "DepthBufferFunction")
                    depthStencilState.StencilWriteMask = int.Parse(el.InnerText);
                else if (el.Name == "TwoSidedStencilMode")
                    depthStencilState.TwoSidedStencilMode = bool.Parse(el.InnerText);*/
                else
                    throw new Exception("'" + el.Name + "' is not an option of the DepthStencilState");
            }
            return depthStencilState;
        }

        private static RasterizerState ParseRasterizerState(XmlElement element)
        {
            if (!element.HasChildNodes || element.Name != "RasterizerState")
                return null;
            RasterizerState rasterizerState = new RasterizerState();
            foreach (XmlElement el in element.ChildNodes.OfType<XmlElement>())
            {
                if (el.Name == "CullMode")
                    rasterizerState.CullMode = (CullMode)Enum.Parse(typeof(CullMode), el.InnerText);
                else if (el.Name == "FillMode")
                    rasterizerState.FillMode = (PolygonMode)Enum.Parse(typeof(PolygonMode), el.InnerText);
                else if (el.Name == "MultiSampleAntiAlias")
                    rasterizerState.MultiSampleAntiAlias = bool.Parse(el.InnerText);
                else if (el.Name == "ScissorTestEnable")
                    rasterizerState.ScissorTestEnable = bool.Parse(el.InnerText);
                /*else if (el.Name == "SlopeScaleDepthBias")
                    rasterizerState.SlopeScaleDepthBias = float.Parse(el.InnerText);
                else if (el.Name == "DepthBias")
                    rasterizerState.DepthBias = float.Parse(el.InnerText);
                else if (el.Name == "DepthClipEnable")
                    rasterizerState.DepthClipEnable = bool.Parse(el.InnerText);*/
                else
                    throw new Exception("'" + el.Name + "' is not an option of the RasterizerState");
                
            }
            return rasterizerState;
        }

        public override EffectContent Import(string filename, ContentImporterContext context)
        {
            try
            {
                EffectContent content = new EffectContent();

                XmlDocument doc = new XmlDocument();
                doc.Load(filename);
                XmlNode current = doc.FirstChild;
                while (current != null && current.NodeType != XmlNodeType.Element)
                {
                    current = current.NextSibling;
                }
                if (current == null)
                    throw new XmlException("no xml element found");
                var effectElement = (XmlElement)current;
                foreach (XmlElement technique in effectElement.ChildNodes.OfType<XmlElement>())
                {
                    EffectTechnique info = new EffectTechnique();
                    info.Name = technique.GetAttribute("name");
                    foreach (XmlElement pass in technique.ChildNodes.OfType<XmlElement>())
                    {
                        EffectPass pi = new EffectPass();
                        pi.Name = pass.GetAttribute("name");
                        foreach (XmlElement sh in pass.ChildNodes.OfType<XmlElement>())
                        {
                            if (sh.Name == "Shader")
                            {
                                ShaderType type = ParseShaderType(sh.GetAttribute("type"));
                                if ((int)type == -1)
                                    throw new Exception("Unsupported Shader type detected");
                                string shaderFile = Path.Combine(Path.GetDirectoryName(filename), sh.GetAttribute("filename"));
                                pi.Shaders.Add(type, shaderFile);
                                context.Dependencies.Add(shaderFile);

                            }
                            else if (sh.Name == "BlendState")
                            {
                                pi.BlendState = ParseBlendState(sh);

                            }
                            else if (sh.Name == "DepthStencilState")
                            {
                                pi.DepthStencilState = ParseDepthStencilState(sh);
                            }
                            else if (sh.Name == "RasterizerState")
                            {
                                pi.RasterizerState = ParseRasterizerState(sh);
                            }
                            else if (sh.Name == "Attributes")
                            {
                                foreach (XmlElement attr in sh.ChildNodes.OfType<XmlElement>())
                                {
                                    if (attr.Name == "attribute")
                                    {
                                        VertexElementUsage usage = (VertexElementUsage)Enum.Parse(typeof(VertexElementUsage), attr.InnerText);
                                        string nm = attr.GetAttribute("name");
                                        if (nm.Length < 1)
                                            throw new Exception("Not a valid attribute name'" + nm + "'");
                                        pi.Attributes.Add(usage, nm);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("'" + sh.Name + "' element not recognized");
                            }
                        }
                        info.Passes.Add(pi);
                    }
                    content.Techniques.Add(info);
                }

                return content;
            }
            catch (Exception ex)
            {
                context.RaiseBuildMessage(filename, ex.Message, BuildMessageEventArgs.BuildMessageType.Error);
            }
            return null;
        }

        #endregion
    }

    public class EffectContent
    {
        public EffectContent()
        {
            Techniques = new List<EffectTechnique>();
        }
        public bool CreateUserEffect { get; internal set; }
        public string UserEffectName { get; internal set; }
        public List<EffectTechnique> Techniques{ get; private set; }
    }

    public class EffectTechnique
    {
        public EffectTechnique()
        {
            Passes = new List<EffectPass>();
        }

        public string Name{ get; internal set; }
        public string UserTechniqueName { get; internal set; }
        public List<EffectPass> Passes{ get; private set; }
    }
    public class EffectPass
    {
        public EffectPass()
        {
            Shaders = new Dictionary<ShaderType, string>();
            Attributes = new Dictionary<VertexElementUsage,string>(); 
            Parameters = new List<ParameterInfo>();
        }

        public string Name{ get; internal set; }

        public BlendState BlendState{ get; internal set; }

        public DepthStencilState DepthStencilState{ get; internal set; }

        public RasterizerState RasterizerState{ get; internal set; }

        public Dictionary<ShaderType,string> Shaders{ get; private set; }

        public Dictionary<VertexElementUsage,string> Attributes{ get; private set; }

        public List<ParameterInfo> Parameters { get; }
    }
    }

    public class ParameterInfo
    {
        public ParameterInfo(string name,Type type)
        {
            Name = name;
            Type = type;
        }
        public string Name { get; }
        public Type Type { get; }
}

